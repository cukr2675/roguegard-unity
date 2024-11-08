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
        public RogueObj Subject { get; private set; }
        public RogueObj World { get; private set; }
        public RogueOptions Options { get; private set; }

        private Transform parent;
        private TouchController touchController;
        private CharacterRenderSystem characterRenderSystem;
        private TilemapRenderSystem tilemapRenderSystem;
        private RogueTicker ticker;
        private GameOverDeviceEventHandler gameOverDeviceEventHandler;
        private DateTimeCallbackEventHandler dateTimeCallbackEventHandler;
        private MenuController menuController;
        private StandardRogueDeviceInspector runtimeInspector;

        private static readonly DummySavePoint dummySavePoint = new DummySavePoint();

        public StandardRogueDeviceEventManager EventManager { get; private set; }

        public bool FastForward => Player.Main.Stats.Nutrition >= 1 && touchController.FastForward;

        internal bool CantSave => LobbyMemberList.GetMemberInfo(Player).SavePoint == dummySavePoint;

        public void Initialize(
            string name,
            RogueSpriteRendererPool spriteRendererPool,
            RogueTilemapRenderer tilemapRendererPrefab,
            TouchController touchControllerPrefab,
            SoundTable soundTable,
            AudioMixer audioMixer,
            AudioSource seAudioSourcePrefab,
            AudioSource bgmAudioSourcePrefab,
            StandardRogueDeviceInspector runtimeInspectorPrefab)
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
            soundController.Open(parent, seAudioSourcePrefab, soundTable);

            // UI表示
            touchController = Object.Instantiate(touchControllerPrefab, parent);
            var autoPlayDeviceEventHandler = new AutoPlayDeviceEventHandler(this, touchController, x => Subject = x);
            touchController.Initialize(
                tilemapGrid.Tilemap, soundController, spriteRendererPool, () => autoPlayDeviceEventHandler.StopAutoPlay());
            touchController.GetInfo(out menuController, out var putIntoChestMenu, out var takeOutFromChestMenu);
            Application.logMessageReceived += OnLogMessageReceived;

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
            dateTimeCallbackEventHandler = new DateTimeCallbackEventHandler();
            var eventHandlers = new IStandardRogueDeviceEventHandler[]
            {
                gameOverDeviceEventHandler,
                dateTimeCallbackEventHandler,
                new SaveDeviceEventHandler(this, touchController),
                new ChangePlayerDeviceEventHandler(this, touchController, ticker, x => Player = Subject = x),
                autoPlayDeviceEventHandler,

                new ChestDeviceEventHandler(this, putIntoChestMenu, takeOutFromChestMenu),
            };
            EventManager = new StandardRogueDeviceEventManager(touchController, characterRenderSystem, eventHandlers);

            runtimeInspector = Object.Instantiate(runtimeInspectorPrefab, parent);
            runtimeInspector.Initialize();

            ticker.enabled = true;
        }

        public void OpenDelay(StandardRogueDeviceData data)
        {
            FadeCanvas.StartCanvasCoroutine(OpenCoroutine(data));
        }

        private IEnumerator OpenCoroutine(StandardRogueDeviceData data)
        {
            // StandardRogueDeviceData を適用
            EventManager.IsOpen = false;
            Player = data.Player;
            Subject = data.Subject;
            World = data.World;
            Options.Set(data.Options);

            // 開発者ツールを初期化
            var rootValue = new RogueObjList();
            rootValue.Add(data.World);
            runtimeInspector.SetRoot(rootValue);

            // 適用後の準備処理
            touchController.OpenWalker(Player);
            touchController.MenuOpen(Subject, Player != Subject);
            ticker.Reset();

            // セーブポイントをキャッシュ
            var memberInfo = LobbyMemberList.GetMemberInfo(Player);
            var playerSavePoint = memberInfo.SavePoint;
            memberInfo.SavePoint = null;

            // 前回セーブからの経過時間でターン経過
            System.Exception exception = null;
            if (data.SaveDateTime != null)
            {
                // ターン経過前に視点をプレイヤーへ移動する
                var tempSubject = Subject;
                if (tempSubject != Player)
                {
                    Subject = Player;
                    touchController.MenuOpen(Player, true);
                }

                var saveDateTime = System.DateTime.Parse(data.SaveDateTime);
                var loadDateTime = System.DateTime.UtcNow;
                var relationalDateTime = loadDateTime - saveDateTime;
                var seconds = (int)relationalDateTime.TotalSeconds;
                var secondsPerTurn = 10;

                var maxTurns = 10000;
                var turns = Mathf.Min(seconds / secondsPerTurn, maxTurns);
                AfterStepTurn(); // セーブポイントから復帰させる

                // ロード画面表示
                var synchronizeMenu = new SynchronizeMenu();
                menuController.PushInitialMenuScreen(synchronizeMenu);

                // ダミーのセーブポイントを設定して入力待機ループを素通りする
                memberInfo.SavePoint = dummySavePoint;
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                var coroutine = TickEnumerator.UpdateTurns(Player, Subject, turns, maxTurns * 1000, false);
                var delayInterval = 250;
                yield return new WaitForSeconds(1f);
                while (!synchronizeMenu.Interrupt)
                {
                    dateTimeCallbackEventHandler.SetInGameTime(saveDateTime + new System.TimeSpan(0, 0, coroutine.Current * secondsPerTurn));
                    try
                    {
                        var result = coroutine.MoveNext();
                        if (!result) break;
                    }
                    catch (System.Exception e)
                    {
                        // 例外を保留して処理を中止
                        exception = e;
                        Debug.LogError(e.StackTrace);
                        break;
                    }
                    if (coroutine.Current % delayInterval == 0)
                    {
                        synchronizeMenu.Progress = (float)coroutine.Current / turns;
                        yield return null;
                    }
                }
                stopwatch.Stop();
                Debug.Log($"Synchronization time: {stopwatch.ElapsedMilliseconds}ms");
                synchronizeMenu.Progress = 1f;
                memberInfo.SavePoint = null;
                dateTimeCallbackEventHandler.UseRealTime();

                // ターン経過後に視点を被写体へ戻す
                if (tempSubject != Player)
                {
                    Subject = tempSubject;
                    touchController.MenuOpen(tempSubject, true);
                }
                EventManager.IsOpen = true;
                UpdateCharacters();
            }

            EventManager.IsOpen = true;
            memberInfo.SavePoint = playerSavePoint;
            LoadSavePoint(Player);

            // 保留した例外を投げなおす
            if (exception != null) throw exception;
        }

        public void Close()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
            Object.Destroy(parent.gameObject);
        }

        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            // メニューを開いているときエラーが発生すると行動できなくなることがある
            // その対策として、エラー発生時はメニューを閉じる
            if (type == LogType.Error || type == LogType.Exception)
            {
                touchController.CloseMenu();
            }
        }

        public void LoadSavePoint(RogueObj obj, bool stack0ToGameOver = false)
        {
            var memberInfo = LobbyMemberList.GetMemberInfo(obj);
            if (memberInfo.SavePoint == dummySavePoint) return;

            if (memberInfo.SavePoint == null)
            {
                if (!stack0ToGameOver || memberInfo.Seat == null) return;

                if (obj.Stack == 0 || !RogueWorldInfo.TryGetWorld(obj, out _))
                {
                    // ゲームオーバー処理
                    gameOverDeviceEventHandler.AfterGameOver(obj);
                }
                return;
            }

            if (obj == Subject)
            {
                UpdateCharacters();
            }

            default(IActiveRogueMethodCaller).LoadSavePoint(obj, 0f, memberInfo.SavePoint);
            memberInfo.SavePoint = null;
        }

        public void AfterStepTurn()
        {
            // セーブ前に復帰してしまわないようにする
            var memberInfo = LobbyMemberList.GetMemberInfo(Player);
            if (memberInfo.SavePoint == null || memberInfo.SavePoint == dummySavePoint)
            {
                // セーブポイントから復帰する
                var worldInfo = RogueWorldInfo.GetByCharacter(Player);
                var lobbyMembers = worldInfo.LobbyMembers.Members;
                for (int i = 0; i < lobbyMembers.Count; i++)
                {
                    LoadSavePoint(lobbyMembers[i], true);
                }
            }

            characterRenderSystem.StartAnimation(Subject);
            EventManager.ResetCalledSynchronizedView();

            // クリックによる移動中に攻撃を受けるなど、入力受付でない状態で割り込まれたとき、移動を中止する。
            if (!touchController.WaitsForInput) { touchController.ClearInput(); }
        }

        public bool UpdateAndGetAllowStepTurn()
        {
            ////////////////////////////////////////////////////////////////////////
            // イベントキュー処理前
            ////////////////////////////////////////////////////////////////////////

            // タイルマップタッチとボタン UI の処理
            var deltaTime = 1;
            var directional = characterRenderSystem.TryGetPositioning(Subject, out var playerPosition, out var playerDirection);
            touchController.EarlyUpdateController(directional, playerPosition, playerDirection, deltaTime);



            ////////////////////////////////////////////////////////////////////////
            // イベントキューが空になるまで、毎 Update 少しずつ処理する
            ////////////////////////////////////////////////////////////////////////

            var workingNow = characterRenderSystem.UpdateCharactersAndGetWorkingNow(Subject, !EventManager.Any, deltaTime, FastForward);
            if (!workingNow && !touchController.TalkingWait)
            {
                if (EventManager.Any)
                {
                    // 進行中の RogueCharacterWork が存在しなければ、次の RogueCharacterWork に移る。
                    EventManager.Dequeue(Player, Subject, FastForward);
                }
                else if (characterRenderSystem.InAnimation)
                {
                    // 次の RogueCharacterWork がなければ、今ターンのアニメーションを終了させる。
                    characterRenderSystem.EndAnimation(Subject, false);
                    touchController.NextTurn(Player, Subject);
                }
            }
            tilemapRenderSystem.Update(Subject, touchController.OpenGrid);



            ////////////////////////////////////////////////////////////////////////
            // イベントキュー処理後
            ////////////////////////////////////////////////////////////////////////

            // playerPosition にはオブジェクトの実位置ではなく表示用スプライトの位置を使う。
            if (characterRenderSystem.TryGetPositioning(Subject, out var position, out _)) { playerPosition = position; }

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
                    var worldInfo = RogueWorldInfo.GetByCharacter(Player);
                    if (menuController.Wait || menuController.TalkingWait || touchController.WaitsForInput || !worldInfo.ChartState.TryUpdate())
                    {
                        // コマンド入力処理を行う
                        touchController.CommandProcessing(Player, Subject, FastForward);
                    }
                }
            }
            return !characterRenderSystem.InAnimation && !touchController.InAnimation;
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
                characterRenderSystem.EndAnimation(Player, true);
            }
            tilemapRenderSystem.Update(Player, false);
        }

        /// <summary>
        /// <see cref="TickEnumerator"/> で行動を停止させるためのクラス
        /// </summary>
        [Objforming.IgnoreRequireRelationalComponent]
        private class DummySavePoint : ISavePointInfo
        {
            public IApplyRogueMethod BeforeSave => throw new System.NotSupportedException();
            public IApplyRogueMethod AfterLoad => throw new System.NotSupportedException();
        }
    }
}
