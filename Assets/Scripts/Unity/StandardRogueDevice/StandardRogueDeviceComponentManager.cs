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
            soundController.Open(parent, seAudioSourcePrefab, soundTable.ToTable());

            // UI�\��
            touchController = Object.Instantiate(touchControllerPrefab, parent);
            touchController.Initialize(tilemapGrid.Tilemap, soundController, spriteRendererPool);
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
                new ChangePlayerDeviceEventHandler(this, touchController, ticker, x => Player = x),
                new AutoPlayDeviceEventHandler(touchController, x => Subject = x),

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
            touchController.Open(Player);
            ticker.Reset();
            UpdateCharacters();

            // �Z�[�u�|�C���g�ǂݍ��݂� UpdateCharacters (�L���[���Z�b�g) ����
            if (data.SavePointInfo != null)
            {
                var result = default(IActiveRogueMethodCaller).LoadSavePoint(Player, 0f, data.SavePointInfo);
                if (!result)
                {
                    Debug.LogError($"�Z�[�u�|�C���g {data.SavePointInfo} �̓ǂݍ��݂Ɏ��s���܂����B");
                }
            }

            // �O��Z�[�u����̌o�ߎ��ԂŃ^�[���o��
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

        public void AfterStepTurn()
        {
            characterRenderSystem.StartAnimation(Subject);
            EventManager.ResetCalledSynchronizedView();

            // �N���b�N�ɂ��ړ����ɍU�����󂯂�ȂǁA���͎�t�łȂ���ԂŊ��荞�܂ꂽ�Ƃ��A�ړ��𒆎~����B
            if (!touchController.WaitsForInput) { touchController.ClearInput(); }
        }

        public bool UpdateAndGetAllowStepTurn()
        {
            if (!touchController.AutoPlayIsEnabled)
            {
                Subject = Player;
                touchController.MenuOpen(Subject);
            }

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
                    touchController.CommandProcessing(Player, FastForward);
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
    }
}
