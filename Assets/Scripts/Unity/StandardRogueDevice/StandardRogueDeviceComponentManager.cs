using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;
using Roguegard;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    internal class StandardRogueDeviceComponentManager
    {
        public RogueObj Player { get; private set; }
        public RogueObj TargetObj { get; private set; }
        public RogueObj World { get; private set; }
        public RogueOptions Options { get; private set; }

        private Transform parent;
        private TouchController touchController;
        private CharacterRenderSystem characterRenderSystem;
        private TilemapRenderSystem tilemapRenderSystem;
        private RogueTicker ticker;
        private GameOverDeviceEventHandler gameOverDeviceEventHandler;

        public StandardRogueDeviceEventManager EventManager { get; private set; }

        public bool NextStay => characterRenderSystem.InAnimation || touchController.InAnimation;
        public bool FastForward => Player.Main.Stats.Nutrition >= 1 && touchController.FastForward;

        public void Initialize(
            string name,
            RogueSpriteRendererPool spriteRendererPool,
            RogueTilemapRenderer tilemapRendererPrefab,
            TouchController touchControllerPrefab,
            SoundTable soundTable,
            AudioMixer audioMixer,
            AudioSource seAudioSourcePrefab,
            AudioSource bgmAudioSourcePrefab)
        {
            parent = new GameObject($"{name} - Parent").transform;

            // キャラクター表示
            characterRenderSystem = new CharacterRenderSystem();
            characterRenderSystem.Open(parent, spriteRendererPool);

            // タイル表示
            tilemapRenderSystem = new TilemapRenderSystem();
            var tilemapGrid = Object.Instantiate(tilemapRendererPrefab, parent);
            tilemapRenderSystem.Open(tilemapGrid);

            // 音声再生
            var soundController = new SoundController();
            soundController.Open(parent, seAudioSourcePrefab, soundTable.ToTable());

            // UI表示
            touchController = Object.Instantiate(touchControllerPrefab, parent);
            touchController.Initialize(tilemapGrid.Tilemap, soundController, spriteRendererPool);
            touchController.GetInfo(out var menuController, out var openChestMenu);

            // Unity の Update 実行用オブジェクト
            var gameObject = new GameObject("Ticker");
            gameObject.transform.SetParent(parent, false);
            ticker = gameObject.AddComponent<RogueTicker>();
            ticker.enabled = false;

            // オプション設定値
            Options = new RogueOptions();
            Options.Initialize(menuController, audioMixer);

            // イベントハンドラ設定
            gameOverDeviceEventHandler = new GameOverDeviceEventHandler(this);
            var eventHandlers = new IStandardRogueDeviceEventHandler[]
            {
                gameOverDeviceEventHandler,
                new SaveDeviceEventHandler(this, touchController),
                new ChangePlayerDeviceEventHandler(this, touchController, ticker, x => Player = x),
                new AutoPlayDeviceEventHandler(touchController, x => TargetObj = x),

                new ChestDeviceEventHandler(this, openChestMenu),
            };
            EventManager = new StandardRogueDeviceEventManager(touchController, characterRenderSystem, eventHandlers);

            ticker.enabled = true;
        }

        public void Open(StandardRogueDeviceData data)
        {
            // StandardRogueDeviceData を適用
            Player = data.Player;
            TargetObj = data.TargetObj;
            World = data.World;
            Options.Set(data.Options);

            // 適用後の準備処理
            touchController.Open(Player);
            ticker.Reset();
            UpdateCharacters();

            // セーブポイント読み込みは UpdateCharacters (キューリセット) より後
            if (data.SavePointInfo != null)
            {
                var result = default(IActiveRogueMethodCaller).LoadSavePoint(Player, 0f, data.SavePointInfo);
                if (!result)
                {
                    Debug.LogError($"セーブポイント {data.SavePointInfo} の読み込みに失敗しました。");
                }
            }

            // 前回セーブからの経過時間でターン経過
            if (data.SaveDateTime != null && LobbyMembers.GetMembersByCharacter(RogueDevice.Primary.Player).Count == 2)
            {
                var loadDateTime = System.DateTime.UtcNow;
                var relationalDateTime = loadDateTime - System.DateTime.Parse(data.SaveDateTime);
                var seconds = (int)relationalDateTime.TotalSeconds;

                var lobbyMember = LobbyMembers.GetMembersByCharacter(RogueDevice.Primary.Player)[1];
                var turns = Mathf.Min(seconds, 10000);
                ticker.UpdateObj(lobbyMember, turns);
            }
        }

        public void Close()
        {
            Object.Destroy(parent.gameObject);
        }

        public void Next()
        {
            characterRenderSystem.StartAnimation(TargetObj);
            EventManager.ResetCalledSynchronizedView();

            // クリックによる移動中に攻撃を受けるなど、入力受付でない状態で割り込まれたとき、移動を中止する。
            if (!touchController.WaitsForInput) { touchController.ClearInput(); }
        }

        public void Update()
        {
            if (!touchController.AutoPlayIsEnabled)
            {
                TargetObj = Player;
                touchController.MenuOpen(TargetObj);
            }

            ////////////////////////////////////////////////////////////////////////
            // イベントキュー処理前
            ////////////////////////////////////////////////////////////////////////

            // タイルマップタッチとボタン UI の処理
            var deltaTime = 1;
            var directional = characterRenderSystem.TryGetPositioning(TargetObj, out var playerPosition, out var playerDirection);
            touchController.EarlyUpdateController(directional, playerPosition, playerDirection, deltaTime);



            ////////////////////////////////////////////////////////////////////////
            // イベントキューが空になるまで、毎 Update 少しずつ処理する
            ////////////////////////////////////////////////////////////////////////

            var workingNow = characterRenderSystem.UpdateCharactersAndGetWorkingNow(TargetObj, !EventManager.Any, deltaTime, FastForward);
            if (!workingNow && !touchController.TalkingWait)
            {
                if (EventManager.Any)
                {
                    // 進行中の RogueCharacterWork が存在しなければ、次の RogueCharacterWork に移る。
                    EventManager.Dequeue(Player, FastForward);
                }
                else if (characterRenderSystem.InAnimation)
                {
                    // 次の RogueCharacterWork がなければ、今ターンのアニメーションを終了させる。
                    characterRenderSystem.EndAnimation(TargetObj);
                    touchController.NextTurn(Player);
                }
            }
            tilemapRenderSystem.Update(TargetObj, touchController.OpenGrid);



            ////////////////////////////////////////////////////////////////////////
            // イベントキュー処理後
            ////////////////////////////////////////////////////////////////////////

            // playerPosition にはオブジェクトの実位置ではなく表示用スプライトの位置を使う。
            if (characterRenderSystem.TryGetPositioning(TargetObj, out var position, out _)) { playerPosition = position; }

            // カメラ・メニュー処理
            touchController.LateUpdateController(Player, playerPosition, deltaTime);

            // キュー完全処理後
            if (!characterRenderSystem.InAnimation)
            {
                if (Player.Stack == 0)
                {
                    // ゲームオーバー処理
                    gameOverDeviceEventHandler.AfterGameOver(Player);
                }
                else
                {
                    // コマンド入力処理を行う
                    touchController.CommandProcessing(Player, FastForward);
                }
            }
        }

        public void UpdateCharacters()
        {
            // 空間移動時に相対位置がわからないように、カメラモードを解除する。
            // ログを削除する。
            touchController.ResetUI();
            touchController.ClearInput();

            // アニメーションをリセットする。
            var deltaTime = 0;
            var fastForward = false;
            EventManager.Clear();
            if (characterRenderSystem.IsInitialized)
            {
                characterRenderSystem.StartAnimation(Player);
                characterRenderSystem.UpdateCharactersAndGetWorkingNow(Player, true, deltaTime, fastForward);
                characterRenderSystem.EndAnimation(Player);
            }
            tilemapRenderSystem.Update(Player, false);
        }
    }
}
