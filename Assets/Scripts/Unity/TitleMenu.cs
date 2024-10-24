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

        [SerializeField] private MenuController _menuController = null;
        [SerializeField] private TMP_Text _versionText = null;

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

        public void Show(
            RogueSpriteRendererPool spriteRendererPool,
            RogueTilemapRenderer tilemapRendererPrefab,
            TouchController touchControllerPrefab,
            SoundTable soundTable,
            AudioMixer audioMixer,
            AudioSource seAudioSourcePrefab,
            AudioSource bgmAudioSourcePrefab,
            StandardRogueDeviceInspector runtimeInspectorPrefab)
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

            var soundController = new SoundController();
            soundController.Open(null, seAudioSourcePrefab, soundTable);
            _menuController.Initialize(soundController, spriteRendererPool, false);

            _menuController.OnError += () =>
            {
                // 例外発生時はメニューを開きなおす（操作不能になる可能性があるため）
                _menuController.PushInitialMenuScreen(new MainScreen(this), enableTouchMask: false);
            };

            _versionText.text = Application.version;
            WindowFrameList.GetWindowFrame(0, out var backgroundA, out var backgroundB);
            var windowColor = ColorPreset.GetColor(0);
            SetBackgroundSprite(backgroundA, backgroundB, windowColor);
            _menuController.SetWindowFrame(backgroundA, backgroundB, windowColor);

            _menuController.PushInitialMenuScreen(new MainScreen(this), enableTouchMask: false);
        }

        private void Update()
        {
            var deltaTime = 1;
            _menuController.EventManager.UpdateUI(deltaTime);

            if (_menuController.IsDone)
            {
                _menuController.ResetDone();
                _menuController.PushInitialMenuScreen(new MainScreen(this), enableTouchMask: false);
            }
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

        private class MainScreen : RogueMenuScreen
        {
            private readonly TitleMenu parent;
            private readonly MainMenuViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view;

            public MainScreen(TitleMenu parent)
            {
                this.parent = parent;

                view = new()
                {
                    PrimaryCommandSubViewName = MenuController.TitleMenuName,
                };
            }

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                view.Show(manager, arg)
                    ?
                    .VariableOnce(out var newGameMenu, new NewGameScreen(parent))
                    .VariableOnce(out var loadScreen, new LoadScreen(parent))

                    .Option(":Play", SelectFileMenuScreen.Load(
                        onSelectFile: (fileInfo, manager, arg) =>
                        {
                            manager.HandleClickBack();
                            manager.PushMenuScreen(loadScreen, other: fileInfo.FullName);
                        },
                        onNewFile: (manager, arg) =>
                        {
                            var builder = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
                            RogueRandom.Primary = new RogueRandom();
                            MessageWorkListener.ClearListeners();
                            MessageWorkListener.AddListener(new DeviceMessageWorkListener());
                            var player = builder.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                            manager.PushMenuScreen(newGameMenu, player, null, other: builder);
                        }))

                    .Option(":Credit", new CreditMenu() { parent = parent })

                    .Build();
            }
        }

        private class NewGameScreen : RogueMenuScreen
        {
            private readonly TitleMenu parent;
            private readonly object[] elms;

            public NewGameScreen(TitleMenu parent)
            {
                this.parent = parent;
                elms = new object[]
                {
                    ChoicesMenuScreen.CreateExit(
                        ":DoneMsg", ":SaveAndStart", SaveAndStart, ":QuitWithoutSaving", null)
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

        private class LoadScreen : RogueMenuScreen
        {
            private readonly TitleMenu parent;
            private readonly FadeOutInViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view;

            public LoadScreen(TitleMenu parent)
            {
                this.parent = parent;

                view = new()
                {
                };
            }

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                view.FadeOut(manager, arg)
                    ?
                    .OnFadeOutCompleted((manager, arg) =>
                    {
                        var path = (string)arg.Arg.Other;

                        var loadSceneOperation = Addressables.LoadSceneAsync(parent._nextSceneAddress, activateOnLoad: true);
                        loadSceneOperation.Completed += _ =>
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
                        };
                    })

                    .Build();
            }
        }

        private class CreditMenu : RogueMenuScreen
        {
            public TitleMenu parent;

            private readonly CreditDetailsScreen nextScreen = new();

            private readonly ScrollViewTemplate<CreditData, RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
                Title = ":Credit",
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                view.Show(parent._credits, manager, arg)
                    ?
                    .ElementNameFrom((credit, manager, arg) =>
                    {
                        return credit.Name;
                    })

                    .OnClickElement((credit, manager, arg) =>
                    {
                        manager.PushMenuScreen(nextScreen, other: credit);
                        //parent._creditMenu.Show(credit, manager);
                    })

                    .Build();
            }
        }

        private class CreditDetailsScreen : RogueMenuScreen
        {
            private readonly URLDialog nextScreen = new();

            private readonly DialogViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
                DialogSubViewName = StandardSubViewTable.WidgetsName,
                BackAnchorSubViewName = StandardSubViewTable.BackAnchorName,
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                var credit = (CreditData)arg.Arg.Other;

                view.Show(credit.Details, manager, arg)
                    ?
                    .OnClickLink((manager, arg, link) =>
                    {
                        manager.PushMenuScreen(nextScreen, other: link);
                    })

                    .Build();
            }

            private class URLDialog : RogueMenuScreen
            {
                private readonly SpeechBoxViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
                {
                };

                public override bool IsIncremental => true;

                public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
                {
                    var url = (string)arg.Arg.Other;
                    view.Show($"{url} へ移動しますか？", manager, arg)
                        ?.Option(":Yes", (manager, arg) =>
                        {
                            var url = (string)arg.Arg.Other;
                            manager.HandleClickBack();
                            Application.OpenURL(url);
                        })
                        .Exit()
                        .Build();
                }

                public override void CloseScreen(RogueMenuManager manager, bool back)
                {
                    view.HideSubViews(manager, back);
                }
            }
        }
    }
}
