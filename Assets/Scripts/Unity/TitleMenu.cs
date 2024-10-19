using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using TMPro;
using ListingMF;
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
        [SerializeField] private GridSubView _titleMenu = null;
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

            _titleMenu.Initialize();

            menuController.PushInitialMenuScreen(new MainMenu(this), enableTouchMask: false);
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
            MessageWorkListener.ClearListeners();
            MessageWorkListener.AddListener(new DeviceMessageWorkListener());

            device.Open(
                spriteRendererPool, tilemapRendererPrefab, touchControllerPrefab,
                soundTable, audioMixer, seAudioSourcePrefab, bgmAudioSourcePrefab, runtimeInspectorPrefab);
        }

        private class MainMenu : RogueMenuScreen
        {
            private readonly TitleMenu parent;
            private readonly object[] elms;

            private IElementsSubViewStateProvider subViewStateProvider;

            public MainMenu(TitleMenu parent)
            {
                this.parent = parent;
                var nextMenu = new PlayMenu(parent);
                var creditMenu = new CreditMenu() { parent = parent };
                elms = new object[]
                {
                    ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(":Play", (manager, arg) =>
                    {
                        manager.PushMenuScreen(nextMenu.selectFileMenu);
                    }),
                    ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(":Credit", (manager, arg) =>
                    {
                        manager.PushMenuScreen(creditMenu);
                    }),
                };
            }

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                parent._titleMenu.Show(elms, SelectOptionHandler.Instance, manager, arg, ref subViewStateProvider);
            }
        }

        private class PlayMenu : RogueMenuScreen
        {
            private readonly TitleMenu parent;
            private readonly NewGameMenu nextMenu;
            public readonly SelectFileMenu selectFileMenu;

            public PlayMenu(TitleMenu parent)
            {
                this.parent = parent;
                nextMenu = new NewGameMenu(parent);

                selectFileMenu = new SelectFileMenu(SelectFileMenu.Type.Read, (root, path) =>
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.HandleClickBack();
                    SelectFileMenu.ShowLoading(root);

                    FadeCanvas.FadeWithLoadScene($"{parent._nextSceneAddress}", () => Loaded(path));
                },
                (root) =>
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    var builder = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
                    RogueRandom.Primary = new RogueRandom();
                    MessageWorkListener.ClearListeners();
                    MessageWorkListener.AddListener(new DeviceMessageWorkListener());
                    var player = builder.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                    root.PushMenuScreen(nextMenu, player, null, other: builder);
                });
            }

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                manager.PushMenuScreen(selectFileMenu, arg);
            }

            private void Loaded(string path)
            {
                StandardRogueDevice device;
                using (var stream = RogueFile.OpenRead(path))
                {
                    var name = RogueFile.GetName(path);
                    var save = new StandardRogueDeviceSave();
                    device = RogueDevice.LoadGame(save, stream);
                    stream.Close();
                }
                parent.OpenDevice(device);
            }
        }

        private class NewGameMenu : RogueMenuScreen
        {
            private readonly TitleMenu parent;
            private readonly object[] elms;

            public NewGameMenu(TitleMenu parent)
            {
                this.parent = parent;
                elms = new object[]
                {
                        DialogListMenuSelectOption.CreateExit(
                            ":Done", ":DoneMsg", ":SaveAndStart", SaveAndStart, ":QuitWithoutSaving", null)
                };
            }

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                var builder = (CharacterCreationDataBuilder)arg.Arg.Other;
                var parent = (MenuController)manager;
                parent.CharacterCreation.OpenView<object>(null, elms, manager, arg.Self, null, new(other: builder));
            }

            private void SaveAndStart(IListMenuManager manager, ReadOnlyMenuArg arg)
            {
                var builder = (CharacterCreationDataBuilder)arg.Arg.Other;
                //manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                FadeCanvas.FadeWithLoadScene($"{parent._nextSceneAddress}", () => Loaded(builder));
            }

            private void Loaded(CharacterCreationDataBuilder builder)
            {
                var save = new StandardRogueDeviceSave(builder);
                var device = RogueDevice.NewGame(save);
                parent.OpenDevice(device);
            }
        }

        private class CreditMenu : RogueMenuScreen
        {
            public TitleMenu parent;

            private readonly ScrollViewTemplate<CreditData, RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
                Title = ":Credit",
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                view.Show(parent._credits, manager, arg)
                    ?.ElementNameGetter((credit, manager, arg) =>
                    {
                        return credit.Name;
                    })
                    .OnClickElement((credit, manager, arg) =>
                    {
                        parent._creditMenu.Show(credit, manager);
                    })
                    .Build();
            }
        }
    }
}
