using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    [ObjectFormer.Formable]
    public class StandardRogueDevice : IRogueDevice
    {
        public string Name => "StandardRogueDevice";

        public string Version => "0.1";

        public string Description => "";

        public RogueObj Player { get; private set; }

        private RogueObj world;

        private IRogueRandom currentRandom;

        private ISavePointInfo savePointInfo;

        public RogueOptions Options { get; }

        [field: System.NonSerialized] public bool CalledSynchronizedView { get; private set; }

        [System.NonSerialized] private Transform parent;

        [System.NonSerialized] private TouchController touchController;
        [System.NonSerialized] private CharacterRenderSystem characterRenderSystem;
        [System.NonSerialized] private TilemapRenderSystem tilemapRenderSystem;
        [System.NonSerialized] private RogueTicker ticker;

        [System.NonSerialized] private MessageWorkQueue messageWorkQueue;
        [System.NonSerialized] private IModelsMenu openChestMenu;

        public bool NextStay => characterRenderSystem.InAnimation || touchController.InAnimation;

        private bool FastForward => Player.Main.Stats.Nutrition >= 1 && touchController.FastForward;

        private StandardRogueDevice() { }

        public StandardRogueDevice(IRogueRandom random, RogueObj player, RogueObj world, ISavePointInfo savePointInfo, RogueOptions options)
        {
            currentRandom = random;
            Player = player;
            this.world = world;
            this.savePointInfo = savePointInfo;
            Options = options;
        }

        private void Reload(StandardRogueDevice device)
        {
            currentRandom = device.currentRandom;
            Player = device.Player;
            world = device.world;
            savePointInfo = device.savePointInfo;
            Options.Set(device.Options);
            Open();
        }

        public void GetInfo(out IRogueRandom random)
        {
            random = currentRandom;
        }

        public void Open(
            RogueSpriteRendererPool spriteRendererPool,
            RogueTilemapRenderer tilemapRendererPrefab,
            TouchController touchControllerPrefab,
            SoundTable soundTable,
            AudioMixer audioMixer,
            AudioSource seAudioSourcePrefab,
            AudioSource bgmAudioSourcePrefab,
            CharacterCreationDatabase characterCreationDatabase)
        {
            if (!Player.TryGet<ViewInfo>(out _))
            {
                Player.SetInfo(new ViewInfo());
            }

            var name = "StandardDevice";
            parent = new GameObject($"{name} - Parent").transform;

            characterRenderSystem = new CharacterRenderSystem();
            characterRenderSystem.Open(parent, spriteRendererPool);

            tilemapRenderSystem = new TilemapRenderSystem();
            var tilemapGrid = Object.Instantiate(tilemapRendererPrefab, parent);
            tilemapRenderSystem.Open(tilemapGrid);

            var soundController = new SoundController();
            soundController.Open(parent, seAudioSourcePrefab, soundTable.ToTable());

            touchController = Object.Instantiate(touchControllerPrefab, parent);
            touchController.Initialize(tilemapGrid.Tilemap, soundController, characterCreationDatabase);
            touchController.GetInfo(out var menuController, out openChestMenu);

            Options.Initialize(menuController, audioMixer);

            messageWorkQueue = new MessageWorkQueue();

            // 更新実行用 GameObject を生成
            var gameObject = new GameObject("Ticker");
            gameObject.transform.SetParent(parent, false);
            ticker = gameObject.AddComponent<RogueTicker>();

            Open();
        }

        private void Open()
        {
            touchController.Open(Player);
            Options.Set(Options);
            ticker.Reset();
            if (currentRandom != null)
            {
                RogueRandom.Primary = currentRandom;
                currentRandom = null;
            }

            UpdateCharacters();

            // セーブポイント読み込みは UpdateCharacters (キューリセット) より後
            if (savePointInfo != null)
            {
                var result = default(IActiveRogueMethodCaller).LoadSavePoint(Player, 0f, savePointInfo);
                if (!result)
                {
                    Debug.LogError($"セーブポイント {savePointInfo} の読み込みに失敗しました。");
                }
                savePointInfo = null;
            }
        }

        public void Close()
        {
            Object.Destroy(parent.gameObject);
        }

        public void Next()
        {
            characterRenderSystem.StartAnimation(Player);

            // 入力受付が始まった時、ターンが経過したとみなして
            // メッセージに区切り線を入れる。
            if (CalledSynchronizedView && !touchController.TalkingWait)
            {
                //touchController.Log(Player, DeviceKw.DemarcateView, null);
            }

            CalledSynchronizedView = false;

            // クリックによる移動中に入力受付でない状態（攻撃を受けるなど）で割り込まれたとき、移動を中断する。
            if (!touchController.WaitsForInput) { touchController.ClearInput(); }
        }

        public void Update()
        {
            ////////////////////////////////////////////////////////////////////////
            // キュー処理前
            ////////////////////////////////////////////////////////////////////////

            // タイルマップタッチとボタン UI の処理
            var deltaTime = 1;
            var directional =
                characterRenderSystem.TryGetPosition(Player, out var playerPosition) &
                characterRenderSystem.TryGetDirection(Player, out var playerDirection);
            touchController.EarlyUpdateController(directional, playerPosition, playerDirection, deltaTime);



            ////////////////////////////////////////////////////////////////////////
            // messageWorkQueue が空になるまで、毎 Update 少しずつ処理する
            ////////////////////////////////////////////////////////////////////////

            var workingNow = characterRenderSystem.UpdateCharactersAndGetWorkingNow(Player, messageWorkQueue.Count == 0, deltaTime, FastForward);
            if (!workingNow && !touchController.TalkingWait)
            {
                if (messageWorkQueue.Count >= 1)
                {
                    // 進行中の RogueCharacterWork が存在しなければ、次の RogueCharacterWork に移る。
                    Dequeue();
                }
                else if (characterRenderSystem.InAnimation)
                {
                    // 次の RogueCharacterWork がなければ、今ターンのアニメーションを終了させる。
                    characterRenderSystem.EndAnimation(Player);
                    touchController.NextTurn(Player);
                }
            }
            tilemapRenderSystem.Update(Player, touchController.OpenGrid);



            ////////////////////////////////////////////////////////////////////////
            // キュー処理後
            ////////////////////////////////////////////////////////////////////////

            // playerPosition にはオブジェクトの実位置ではなく表示用スプライトの位置を使う。
            if (characterRenderSystem.TryGetPosition(Player, out var position)) { playerPosition = position; }

            // カメラ・メニュー処理
            touchController.LateUpdateController(Player, playerPosition, deltaTime);

            // キュー完全処理後
            if (!characterRenderSystem.InAnimation)
            {
                if (Player.Stack == 0)
                {
                    // ゲームオーバー処理
                    AfterGameOver();
                }
                else
                {
                    // コマンド入力処理を行う
                    touchController.CommandProcessing(Player, FastForward);
                }
            }
        }

        private void AfterGameOver()
        {
            var view = Player.Get<ViewInfo>();
            var dungeon = view.Location;

            view.ReadyView(Player.Location);
            view.AddView(Player);
            view.EnqueueState();
            Player.TrySetStack(1);
            SpaceUtility.TryLocate(Player, world);

            var gameOverArg = new RogueMethodArgument(targetObj: dungeon);
            messageWorkQueue.EnqueueMenu(new GameOverMenu(), Player, null, gameOverArg);
            RogueDevice.Add(DeviceKw.EnqueueViewDequeueState, 0);
        }

        public void NewGame(float activationDepth)
        {
            SpaceUtility.TryLocate(Player, world);
            if (!default(IActiveRogueMethodCaller).LocateSavePoint(Player, null, activationDepth, RogueWorld.SavePointInfo, true)) return;
            default(IActiveRogueMethodCaller).LoadSavePoint(Player, activationDepth, RogueWorld.SavePointInfo);
        }

        private void Dequeue()
        {
            while (messageWorkQueue.Count >= 1)
            {
                messageWorkQueue.Dequeue(out var other, out var work, out var integer, out var number, out var stackTrace);
                if (other == DeviceKw.EnqueueWork)
                {
                    characterRenderSystem.Work(work, Player, FastForward);
                    if (!work.Continues) break;
                }
                else if (other == DeviceKw.EnqueueInteger)
                {
                    touchController.Log(integer);
                }
                else if (other == DeviceKw.EnqueueNumber)
                {
                    touchController.Log(number);
                }
                else if (other == DeviceKw.StartTalk)
                {
                    touchController.StartTalk();
                }
                else if (other == DeviceKw.EndTalk)
                {
                    break;
                }
                else if (other == DeviceKw.EnqueueMenu)
                {
                    messageWorkQueue.DequeueMenu(out var menu, out var self, out var user, out var arg);
                    touchController.OpenMenu(Player, menu, self, user, arg);
                    break;
                }
                else if (other == DeviceKw.EnqueueSE || other == DeviceKw.EnqueueSEAndWait)
                {
                    messageWorkQueue.Dequeue(out var seName, out _, out _, out _, out _);
                    touchController.Play((IKeyword)seName, other == DeviceKw.EnqueueSEAndWait);
                    if (other == DeviceKw.EnqueueSEAndWait) break;
                }
                else if (other == DeviceKw.EnqueueWaitSeconds)
                {
                    messageWorkQueue.Dequeue(out _, out _, out _, out var waitSeconds, out _);
                    touchController.StartWait(waitSeconds);
                    break;
                }
                else if (other == DeviceKw.EnqueueViewDequeueState)
                {
                    var view = Player.Get<ViewInfo>();
                    view.DequeueState();
                    view.ReadyView(Player.Location);
                    view.AddView(Player);
                }
                else if (other == DeviceKw.HorizontalRule)
                {
                    touchController.Log(Player, other, stackTrace);
                }
                else
                {
                    other = StandardRogueDeviceUtility.LocalizeMessage(other, Player, messageWorkQueue);
                    touchController.Log(Player, other, stackTrace);
                }
            }
        }

        private void Add(IKeyword keyword, AddType type, int integer = 0, float number = 0f, object obj = null)
        {
            if (keyword == null) throw new System.ArgumentNullException(nameof(keyword));

            if (keyword == DeviceKw.AppendText)
            {
                if (type == AddType.Integer) { messageWorkQueue.EnqueueInteger(integer); }
                else if (type == AddType.Float) { messageWorkQueue.EnqueueNumber(number); }
                else { messageWorkQueue.EnqueueOther(obj); }
                CalledSynchronizedView = true;
                return;
            }
            if (keyword == DeviceKw.EnqueueSE && type == AddType.Object)
            {
                messageWorkQueue.EnqueueOther(DeviceKw.EnqueueSE);
                messageWorkQueue.EnqueueOther(obj);
                CalledSynchronizedView = true;
                return;
            }
            if (keyword == DeviceKw.EnqueueSEAndWait && type == AddType.Object)
            {
                messageWorkQueue.EnqueueOther(DeviceKw.EnqueueSEAndWait);
                messageWorkQueue.EnqueueOther(obj);
                CalledSynchronizedView = true;
                return;
            }
            if (keyword == DeviceKw.EnqueueWaitSeconds && type == AddType.Integer)
            {
                messageWorkQueue.EnqueueOther(DeviceKw.EnqueueWaitSeconds);
                messageWorkQueue.EnqueueNumber(integer);
                CalledSynchronizedView = true;
                return;
            }
            if (keyword == DeviceKw.EnqueueWaitSeconds && type == AddType.Float)
            {
                messageWorkQueue.EnqueueOther(DeviceKw.EnqueueWaitSeconds);
                messageWorkQueue.EnqueueNumber(number);
                CalledSynchronizedView = true;
                return;
            }

            if (keyword == DeviceKw.WaitForInput)
            {
                touchController.WaitsForInput = true;
                CalledSynchronizedView = true;
                return;
            }

            if (keyword == DeviceKw.InsertHideCharacterWork && obj is RogueObj hideObj)
            {
                messageWorkQueue.InsertHideCharacterWork(hideObj);
                return;
            }
            if (keyword == DeviceKw.EnqueueViewDequeueState)
            {
                messageWorkQueue.EnqueueOther(DeviceKw.EnqueueViewDequeueState);
                return;
            }
            if (keyword == StdKw.TakeOutFromChest && obj is RogueObj takeChest)
            {
                var arg = new RogueMethodArgument(targetObj: takeChest, count: 0);
                messageWorkQueue.EnqueueMenu(openChestMenu, Player, null, arg);
                return;
            }
            if (keyword == StdKw.PutIntoChest && obj is RogueObj putChest)
            {
                var arg = new RogueMethodArgument(targetObj: putChest, count: 1);
                messageWorkQueue.EnqueueMenu(openChestMenu, Player, null, arg);
                return;
            }
            if (keyword == DeviceKw.AutoSave)
            {
                var savePointInfo = (ISavePointInfo)obj;
                Save(null, StandardRogueDeviceSave.RootDirectory + "/AutoSave.gard", savePointInfo, true);
                return;
            }
            if (keyword == DeviceKw.SaveGame)
            {
                UpdateCharacters();
                var savePointInfo = (ISavePointInfo)obj;
                touchController.OpenSelectFile(
                    (root, path) => Save(root, path, savePointInfo, false),
                    (root) => StandardRogueDeviceSave.GetNewNumberingPath(
                        RoguegardSettings.DefaultSaveFileName, path => Save(root, path, savePointInfo, false)));
                return;
            }
            if (keyword == DeviceKw.LoadGame)
            {
                OpenLoadInGame();
                return;
            }
            if (keyword == DeviceKw.GameOver)
            {
                AfterGameOver();
                return;
            }
            if (keyword == DeviceKw.ChangePlayer && obj is RogueObj newPlayer)
            {
                // ViewInfo を移動させる
                var view = Player.Get<ViewInfo>();
                Player.RemoveInfo(typeof(ViewInfo));
                newPlayer.SetInfo(view);

                // PlayerLeaderInfo を移動させる
                var playerLeaderInfo = Player.Main.GetPlayerLeaderInfo(Player);
                playerLeaderInfo?.Move(Player, newPlayer);

                // RogueDeviceEffect を移動させる
                var deviceEffect = RogueDeviceEffect.Get(Player);
                deviceEffect.Close(Player);
                RogueDeviceEffect.SetTo(newPlayer);

                Player = newPlayer;
                Open();
                return;
            }

            Debug.LogError($"{keyword.Name} に対応するキーワードが見つかりません。（obj: {obj}）");
        }

        public void AddInt(IKeyword keyword, int value) => Add(keyword, AddType.Integer, integer: value);
        public void AddFloat(IKeyword keyword, float value) => Add(keyword, AddType.Float, number: value);
        public void AddObject(IKeyword keyword, object obj) => Add(keyword, AddType.Object, obj: obj);

        public void AddWork(IKeyword keyword, in RogueCharacterWork work)
        {
            if (keyword == DeviceKw.EnqueueWork)
            {
                if (messageWorkQueue.Count == 0 && work.Continues)
                {
                    // 移動モーションなどを複数のオブジェクトで同時再生する。
                    characterRenderSystem.Work(work, Player, FastForward);
                }
                else
                {
                    // 攻撃モーションなどの再生を待機する。
                    messageWorkQueue.EnqueueWork(work);
                    CalledSynchronizedView = true;
                }
            }
        }

        void IRogueDevice.AddMenu(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            messageWorkQueue.EnqueueMenu(menu, self, user, arg);
        }

        bool IRogueDevice.VisibleAt(RogueObj location, Vector2Int position)
        {
            var view = Player.Get<ViewInfo>();
            if (view.Location != location || view.Location.Space.Tilemap == null) return false;

            view.GetTile(position, out var visible, out _, out _);
            if (visible) return true;

            // ペイントされているオブジェクトがいる位置は見える
            var obj = view.Location.Space.GetColliderObj(position);
            if (obj != null)
            {
                var objStatusEffectState = obj.Main.GetStatusEffectState(obj);
                if (objStatusEffectState.TryGetStatusEffect<PaintStatusEffect>(out _)) return true;
            }

            // 視界範囲外の判定が出ても、更新してもう一度試す
            // 出会いがしらの敵を表示する際に有効
            view.AddView(Player);
            view.GetTile(position, out visible, out _, out _);
            return visible;
        }

        private void Save(IModelsMenuRoot root, string path, ISavePointInfo savePointInfo, bool autoSave)
        {
            currentRandom = RogueRandom.Primary;
            this.savePointInfo = savePointInfo;
            var view = Player.Get<ViewInfo>();
            if (Player.Location != view.Location)
            {
                // 空間移動直後にセーブしたとき、移動前の空間の情報を保存しないよう処理する
                view.ReadyView(Player.Location);
            }

            var stream = RogueFile.Create(path);
            var save = new StandardRogueDeviceSave();
            save.SaveGame(stream, this);
            stream.Save(() =>
            {
                root?.Done();
                stream.Close();
                if (autoSave)
                {
                    RogueDevice.Add(DeviceKw.AppendText, path);
                    RogueDevice.Add(DeviceKw.AppendText, "にオートセーブしました\n");
                }
                else
                {
                    RogueDevice.Add(DeviceKw.AppendText, path);
                    RogueDevice.Add(DeviceKw.AppendText, "にセーブしました\n");
                }
            });
        }

        private void OpenLoadInGame()
        {
            touchController.OpenSelectFile((root, path) =>
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.Done();
                AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                RogueFile.OpenRead(path, stream =>
                {
                    var save = new StandardRogueDeviceSave();
                    var loadDevice = save.LoadGame(stream);
                    stream.Close();
                    Reload(loadDevice);
                });
            });
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
            messageWorkQueue.Clear();
            if (characterRenderSystem.IsInitialized)
            {
                characterRenderSystem.StartAnimation(Player);
                characterRenderSystem.UpdateCharactersAndGetWorkingNow(Player, true, deltaTime, fastForward);
                characterRenderSystem.EndAnimation(Player);
            }
            tilemapRenderSystem.Update(Player, false);
        }

        private enum AddType
        {
            Integer,
            Float,
            Object
        }
    }
}
