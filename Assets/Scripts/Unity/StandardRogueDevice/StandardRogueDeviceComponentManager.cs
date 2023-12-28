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

        private static readonly DummySavePoint dummySavePoint = new DummySavePoint();

        public StandardRogueDeviceEventManager EventManager { get; private set; }

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
                tilemapGrid.Tilemap, soundController, spriteRendererPool, () => UpdateCharacters(), () => autoPlayDeviceEventHandler.StopAutoPlay());
            touchController.GetInfo(out var menuController, out var openChestMenu);

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
            var eventHandlers = new IStandardRogueDeviceEventHandler[]
            {
                gameOverDeviceEventHandler,
                new SaveDeviceEventHandler(this, touchController),
                new ChangePlayerDeviceEventHandler(this, touchController, ticker, x => Player = Subject = x),
                autoPlayDeviceEventHandler,

                new ChestDeviceEventHandler(this, openChestMenu),
            };
            EventManager = new StandardRogueDeviceEventManager(touchController, characterRenderSystem, eventHandlers);

            ticker.enabled = true;
        }

        public void Open(StandardRogueDeviceData data)
        {
            // StandardRogueDeviceData ��K�p
            Player = data.Player;
            Subject = data.Subject;
            World = data.World;
            Options.Set(data.Options);

            // �K�p��̏�������
            touchController.OpenWalker(Player);
            touchController.MenuOpen(Subject, Player != Subject);
            ticker.Reset();

            // �O��Z�[�u����̌o�ߎ��ԂŃ^�[���o��
            if (data.SaveDateTime != null)
            {
                var loadDateTime = System.DateTime.UtcNow;
                var relationalDateTime = loadDateTime - System.DateTime.Parse(data.SaveDateTime);
                var seconds = (int)relationalDateTime.TotalSeconds;
                var secondsPerTurn = 60;

                var maxTurns = 1000;
                var turns = Mathf.Min(seconds / secondsPerTurn, maxTurns);
                AfterStepTurn(); // �Z�[�u�|�C���g���畜�A������

                // �_�~�[�̃Z�[�u�|�C���g��ݒ肵�ē��͑ҋ@���[�v�𔲂���
                LobbyMembers.SetSavePoint(Player, dummySavePoint);
                TickEnumerator.UpdateTurns(Player, turns, maxTurns * 10, false);
                LobbyMembers.SetSavePoint(Player, null);
            }

            // ���Ԍo�ߏ�����Ƀ��Z�b�g
            UpdateCharacters();
        }

        public void Close()
        {
            Object.Destroy(parent.gameObject);
        }

        public void AfterStepTurn()
        {
            // �Z�[�u�|�C���g���畜�A����
            var lobbyMembers = LobbyMembers.GetMembersByCharacter(World.Space.Objs[0]);
            for (int i = 0; i < lobbyMembers.Count; i++)
            {
                var lobbyMember = lobbyMembers[i];
                var savePoint = LobbyMembers.GetSavePoint(lobbyMember);
                if (savePoint == null || savePoint == dummySavePoint) continue;

                default(IActiveRogueMethodCaller).LoadSavePoint(lobbyMember, 0f, savePoint);
                LobbyMembers.SetSavePoint(lobbyMember, null);
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
                    EventManager.Dequeue(Player, FastForward);
                }
                else if (characterRenderSystem.InAnimation)
                {
                    // ���� RogueCharacterWork ���Ȃ���΁A���^�[���̃A�j���[�V�������I��������B
                    characterRenderSystem.EndAnimation(Subject);
                    touchController.NextTurn(Player);
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
                    // �R�}���h���͏������s��
                    touchController.CommandProcessing(Player, Subject, FastForward);
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
                characterRenderSystem.EndAnimation(Player);
            }
            tilemapRenderSystem.Update(Player, false);
        }

        /// <summary>
        /// <see cref="TickEnumerator"/> �ōs�����~�����邽�߂̃N���X
        /// </summary>
        [ObjectFormer.IgnoreRequireRelationalComponent]
        private class DummySavePoint : ISavePointInfo
        {
            public IApplyRogueMethod BeforeSave => throw new System.NotSupportedException();
            public IApplyRogueMethod AfterLoad => throw new System.NotSupportedException();
        }
    }
}
