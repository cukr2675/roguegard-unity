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

            // �L�����N�^�[�\��
            characterRenderSystem = new CharacterRenderSystem();
            characterRenderSystem.Open(parent, spriteRendererPool);

            // �^�C���\��
            tilemapRenderSystem = new TilemapRenderSystem();
            var tilemapGrid = Object.Instantiate(tilemapRendererPrefab, parent);
            tilemapRenderSystem.Open(tilemapGrid);

            // �����Đ�
            var soundController = new SoundController();
            soundController.Open(parent, seAudioSourcePrefab, soundTable);

            // UI�\��
            touchController = Object.Instantiate(touchControllerPrefab, parent);
            var autoPlayDeviceEventHandler = new AutoPlayDeviceEventHandler(this, touchController, x => Subject = x);
            touchController.Initialize(
                tilemapGrid.Tilemap, soundController, spriteRendererPool, () => autoPlayDeviceEventHandler.StopAutoPlay());
            touchController.GetInfo(out menuController, out var putIntoChestMenu, out var takeOutFromChestMenu);
            Application.logMessageReceived += OnLogMessageReceived;

            // Unity �� Update ���s�p�I�u�W�F�N�g
            var gameObject = new GameObject("Ticker");
            gameObject.transform.SetParent(parent, false);
            ticker = gameObject.AddComponent<RogueTicker>();
            ticker.enabled = false;

            // �I�v�V�����ݒ�l
            Options = new RogueOptions();
            Options.Initialize(menuController, audioMixer);

            // �C�x���g�n���h���ݒ�
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
            // StandardRogueDeviceData ��K�p
            EventManager.IsOpen = false;
            Player = data.Player;
            Subject = data.Subject;
            World = data.World;
            Options.Set(data.Options);

            // �J���҃c�[����������
            var rootValue = new RogueObjList();
            rootValue.Add(data.World);
            runtimeInspector.SetRoot(rootValue);

            // �K�p��̏�������
            touchController.OpenWalker(Player);
            touchController.MenuOpen(Subject, Player != Subject);
            ticker.Reset();

            // �Z�[�u�|�C���g���L���b�V��
            var memberInfo = LobbyMemberList.GetMemberInfo(Player);
            var playerSavePoint = memberInfo.SavePoint;
            memberInfo.SavePoint = null;

            // �O��Z�[�u����̌o�ߎ��ԂŃ^�[���o��
            System.Exception exception = null;
            if (data.SaveDateTime != null)
            {
                // �^�[���o�ߑO�Ɏ��_���v���C���[�ֈړ�����
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
                AfterStepTurn(); // �Z�[�u�|�C���g���畜�A������

                // ���[�h��ʕ\��
                var synchronizeMenu = new SynchronizeMenu();
                menuController.PushInitialMenuScreen(synchronizeMenu);

                // �_�~�[�̃Z�[�u�|�C���g��ݒ肵�ē��͑ҋ@���[�v��f�ʂ肷��
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
                        // ��O��ۗ����ď����𒆎~
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

                // �^�[���o�ߌ�Ɏ��_���ʑ֖̂߂�
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

            // �ۗ�������O�𓊂��Ȃ���
            if (exception != null) throw exception;
        }

        public void Close()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
            Object.Destroy(parent.gameObject);
        }

        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            // ���j���[���J���Ă���Ƃ��G���[����������ƍs���ł��Ȃ��Ȃ邱�Ƃ�����
            // ���̑΍�Ƃ��āA�G���[�������̓��j���[�����
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
                    // �Q�[���I�[�o�[����
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
            // �Z�[�u�O�ɕ��A���Ă��܂�Ȃ��悤�ɂ���
            var memberInfo = LobbyMemberList.GetMemberInfo(Player);
            if (memberInfo.SavePoint == null || memberInfo.SavePoint == dummySavePoint)
            {
                // �Z�[�u�|�C���g���畜�A����
                var worldInfo = RogueWorldInfo.GetByCharacter(Player);
                var lobbyMembers = worldInfo.LobbyMembers.Members;
                for (int i = 0; i < lobbyMembers.Count; i++)
                {
                    LoadSavePoint(lobbyMembers[i], true);
                }
            }

            characterRenderSystem.StartAnimation(Subject);
            EventManager.ResetCalledSynchronizedView();

            // �N���b�N�ɂ��ړ����ɍU�����󂯂�ȂǁA���͎�t�łȂ���ԂŊ��荞�܂ꂽ�Ƃ��A�ړ��𒆎~����B
            if (!touchController.WaitsForInput) { touchController.ClearInput(); }
        }

        public bool UpdateAndGetAllowStepTurn()
        {
            ////////////////////////////////////////////////////////////////////////
            // �C�x���g�L���[�����O
            ////////////////////////////////////////////////////////////////////////

            // �^�C���}�b�v�^�b�`�ƃ{�^�� UI �̏���
            var deltaTime = 1;
            var directional = characterRenderSystem.TryGetPositioning(Subject, out var playerPosition, out var playerDirection);
            touchController.EarlyUpdateController(directional, playerPosition, playerDirection, deltaTime);



            ////////////////////////////////////////////////////////////////////////
            // �C�x���g�L���[����ɂȂ�܂ŁA�� Update ��������������
            ////////////////////////////////////////////////////////////////////////

            var workingNow = characterRenderSystem.UpdateCharactersAndGetWorkingNow(Subject, !EventManager.Any, deltaTime, FastForward);
            if (!workingNow && !touchController.TalkingWait)
            {
                if (EventManager.Any)
                {
                    // �i�s���� RogueCharacterWork �����݂��Ȃ���΁A���� RogueCharacterWork �Ɉڂ�B
                    EventManager.Dequeue(Player, Subject, FastForward);
                }
                else if (characterRenderSystem.InAnimation)
                {
                    // ���� RogueCharacterWork ���Ȃ���΁A���^�[���̃A�j���[�V�������I��������B
                    characterRenderSystem.EndAnimation(Subject, false);
                    touchController.NextTurn(Player, Subject);
                }
            }
            tilemapRenderSystem.Update(Subject, touchController.OpenGrid);



            ////////////////////////////////////////////////////////////////////////
            // �C�x���g�L���[������
            ////////////////////////////////////////////////////////////////////////

            // playerPosition �ɂ̓I�u�W�F�N�g�̎��ʒu�ł͂Ȃ��\���p�X�v���C�g�̈ʒu���g���B
            if (characterRenderSystem.TryGetPositioning(Subject, out var position, out _)) { playerPosition = position; }

            // �J�����E���j���[����
            touchController.LateUpdateController(Player, playerPosition, deltaTime);

            // �L���[���S������
            if (!characterRenderSystem.InAnimation)
            {
                if (Player.Stack == 0)
                {
                    // �Q�[���I�[�o�[����
                    gameOverDeviceEventHandler.AfterGameOver(Player);
                }
                else
                {
                    var worldInfo = RogueWorldInfo.GetByCharacter(Player);
                    if (menuController.Wait || menuController.TalkingWait || touchController.WaitsForInput || !worldInfo.ChartState.TryUpdate())
                    {
                        // �R�}���h���͏������s��
                        touchController.CommandProcessing(Player, Subject, FastForward);
                    }
                }
            }
            return !characterRenderSystem.InAnimation && !touchController.InAnimation;
        }

        public void UpdateCharacters()
        {
            // ��Ԉړ����ɑ��Έʒu���킩��Ȃ��悤�ɁA�J�������[�h����������B
            // ���O���폜����B
            touchController.ResetUI();
            touchController.ClearInput();

            // �A�j���[�V���������Z�b�g����B
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
        /// <see cref="TickEnumerator"/> �ōs�����~�����邽�߂̃N���X
        /// </summary>
        [Objforming.IgnoreRequireRelationalComponent]
        private class DummySavePoint : ISavePointInfo
        {
            public IApplyRogueMethod BeforeSave => throw new System.NotSupportedException();
            public IApplyRogueMethod AfterLoad => throw new System.NotSupportedException();
        }
    }
}
