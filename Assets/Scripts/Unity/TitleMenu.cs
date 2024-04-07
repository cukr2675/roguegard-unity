using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using TMPro;
using Roguegard;
using Roguegard.Device;
using Roguegard.CharacterCreation;

namespace RoguegardUnity
{
    public class TitleMenu : MonoBehaviour
    {
        /// <summary>
        /// <see cref="Addressables.LoadSceneAsync"/> で読み込んだ <see cref="Object"/> や <see cref="ScriptableObject"/> は
        /// null になってしまうためアドレスを要求する
        /// </summary>
        [SerializeField] private string _nextSceneAddress = null;

        [Space]

        [SerializeField] private TMP_Text _versionText = null;
        [SerializeField] private Image _background = null;
        [SerializeField] private Image _logo = null;
        [SerializeField] private ModelsMenuView _titleMenu = null;
        [SerializeField] private TitleCredit _creditMenu = null;

        [Space]

        [SerializeField] private CreditData[] _credits = null;

        private RogueSpriteRendererPool spriteRendererPool;
        private RogueTilemapRenderer tilemapRendererPrefab;
        private TouchController touchControllerPrefab;
        private SoundTable soundTable;
        private AudioMixer audioMixer;
        private AudioSource seAudioSourcePrefab;
        private AudioSource bgmAudioSourcePrefab;
        private StandardRogueDeviceInspector runtimeInspectorPrefab;

        private MenuController menuController;

        public void Initialize(
            RogueSpriteRendererPool spriteRendererPool,
            MenuController menuControllerPrefab,
            RogueTilemapRenderer tilemapRendererPrefab,
            TouchController touchControllerPrefab,
            SoundTable soundTable,
            AudioMixer audioMixer,
            AudioSource seAudioSourcePrefab,
            AudioSource bgmAudioSourcePrefab,
            StandardRogueDeviceInspector runtimeInspectorPrefab,
            Sprite logo)
        {
            this.spriteRendererPool = spriteRendererPool;
            this.tilemapRendererPrefab = tilemapRendererPrefab;
            this.touchControllerPrefab = touchControllerPrefab;
            this.soundTable = soundTable;
            this.audioMixer = audioMixer;
            this.seAudioSourcePrefab = seAudioSourcePrefab;
            this.bgmAudioSourcePrefab = bgmAudioSourcePrefab;
            this.runtimeInspectorPrefab = runtimeInspectorPrefab;
            DontDestroyOnLoad(spriteRendererPool.gameObject);

            menuController = Instantiate(menuControllerPrefab);
            var soundController = new SoundController();
            soundController.Open(null, seAudioSourcePrefab, soundTable);
            menuController.Initialize(soundController, spriteRendererPool, false);

            _logo.sprite = logo;
            _logo.SetNativeSize();

            _creditMenu.Initialize();
            _versionText.text = Application.version;
            _background.sprite = CoreTiles1.Grass.Icon;
            var backgroundColor = CoreTiles1.Grass.Color;
            backgroundColor.a = .375f;
            _background.color = backgroundColor;
            WindowFrameList.GetWindowFrame(0, out var backgroundA, out var backgroundB);
            var windowColor = ColorPreset.GetColor(0);
            SetBackgroundSprite(backgroundA, backgroundB, windowColor);
            menuController.SetWindowFrame(backgroundA, backgroundB, windowColor);

            menuController.OpenInitialMenu(new MainMenu(this), null, null, RogueMethodArgument.Identity, false);
        }

        private void Update()
        {
            var deltaTime = 1;
            menuController.EventManager.UpdateUI(deltaTime);
        }

        private void SetBackgroundSprite(Sprite sprite, Sprite spriteB, Color backgroundColor)
        {
            var backgrounds = GetComponentsInChildren<MenuWindowBackground>();
            foreach (var background in backgrounds)
            {
                background.ImageA.sprite = sprite;
                background.ImageA.color = backgroundColor;
                background.ImageB.sprite = spriteB;
            }
        }

        private void OpenDevice(StandardRogueDevice device)
        {
            device.GetInfo(out var random);
            RogueRandom.Primary = random;

            device.Open(
                spriteRendererPool, tilemapRendererPrefab, touchControllerPrefab,
                soundTable, audioMixer, seAudioSourcePrefab, bgmAudioSourcePrefab, runtimeInspectorPrefab);
        }

        private class MainMenu : IModelsMenu
        {
            private readonly TitleMenu parent;
            private readonly object[] models;

            public MainMenu(TitleMenu parent)
            {
                this.parent = parent;
                models = new object[]
                {
                    new StartGameChoice(parent),
                    new CreditChoice() { parent = parent },
                };
            }

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                parent._titleMenu.OpenView(ChoiceListPresenter.Instance, models, root, null, null, RogueMethodArgument.Identity);
            }
        }

        private class StartGameChoice : BaseModelsMenuChoice
        {
            public override string Name => ":Play";

            private readonly NextMenu nextMenu;

            public StartGameChoice(TitleMenu parent)
            {
                nextMenu = new NextMenu(parent);
            }

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.OpenMenu(nextMenu, null, null, RogueMethodArgument.Identity);
            }

            private class NextMenu : IModelsMenu
            {
                private readonly TitleMenu parent;
                private readonly NewGameMenu nextMenu;
                private readonly SelectFileMenu selectFileMenu;

                public NextMenu(TitleMenu parent)
                {
                    this.parent = parent;
                    nextMenu = new NewGameMenu(parent);

                    selectFileMenu = new SelectFileMenu(SelectFileMenu.Type.Read, (root, path) =>
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        root.Back();
                        SelectFileMenu.ShowLoading(root);

                        FadeCanvas.FadeWithLoadScene($"{parent._nextSceneAddress}", () => Loaded(path));
                    },
                    (root) =>
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                        var builder = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
                        RogueRandom.Primary = new RogueRandom();
                        var player = builder.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                        root.OpenMenu(nextMenu, player, null, new(other: builder));
                    });
                }

                public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.OpenMenu(selectFileMenu, null, null, RogueMethodArgument.Identity);
                }

                private void Loaded(string path)
                {
                    RogueFile.OpenRead(path, (stream, errorMsg) =>
                    {
                        if (errorMsg != null)
                        {
                            Debug.LogError(errorMsg);
                            return;
                        }

                        var name = RogueFile.GetName(path);
                        var save = new StandardRogueDeviceSave();
                        var device = RogueDevice.LoadGame(save, stream);
                        stream.Close();
                        parent.OpenDevice(device);
                    });
                }
            }

            private class NewGameMenu : IModelsMenu
            {
                private readonly TitleMenu parent;
                private readonly object[] models;

                public NewGameMenu(TitleMenu parent)
                {
                    this.parent = parent;
                    models = new object[]
                    {
                        DialogModelsMenuChoice.CreateExit(
                            ":Done", ":DoneMsg", ":SaveAndStart", SaveAndStart, ":QuitWithoutSaving", null)
                    };
                }

                public void OpenMenu(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
                {
                    var builder = (CharacterCreationDataBuilder)arg.Other;
                    root.Get(DeviceKw.MenuCharacterCreation).OpenView(null, models, root, player, null, new(other: builder));
                }

                private void SaveAndStart(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var builder = (CharacterCreationDataBuilder)arg.Other;
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    FadeCanvas.FadeWithLoadScene($"{parent._nextSceneAddress}", () => Loaded(builder));
                }

                private void Loaded(CharacterCreationDataBuilder builder)
                {
                    var save = new StandardRogueDeviceSave(builder);
                    var device = RogueDevice.NewGame(save);
                    parent.OpenDevice(device);
                }
            }
        }

        private class CreditChoice : BaseModelsMenuChoice
        {
            public override string Name => ":Credit";

            public TitleMenu parent;
            private NextMenu nextMenu;

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (nextMenu == null)
                {
                    nextMenu = new NextMenu() { parent = parent };
                }

                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.OpenMenu(nextMenu, null, null, RogueMethodArgument.Identity);
            }

            private class NextMenu : BaseScrollModelsMenu<CreditData>
            {
                protected override string MenuName => ":Credit";

                public TitleMenu parent;

                protected override Spanning<CreditData> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return parent._credits;
                }

                protected override string GetItemName(CreditData credit, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return credit.Name;
                }

                protected override void ItemActivate(CreditData credit, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    parent._creditMenu.Show(credit, root);
                }
            }
        }
    }
}
