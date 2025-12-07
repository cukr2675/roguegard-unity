using Lysionium;
using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

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
        private AudioMixer audioMixer;
        private StandardRogueDeviceInspector runtimeInspectorPrefab;

        public void Show(
            RogueSpriteRendererPool spriteRendererPool,
            RogueTilemapRenderer tilemapRendererPrefab,
            TouchController touchControllerPrefab,
            AudioMixer audioMixer,
            StandardRogueDeviceInspector runtimeInspectorPrefab)
        {
            this.spriteRendererPool = spriteRendererPool;
            this.tilemapRendererPrefab = tilemapRendererPrefab;
            this.touchControllerPrefab = touchControllerPrefab;
            this.audioMixer = audioMixer;
            this.runtimeInspectorPrefab = runtimeInspectorPrefab;

            _versionText.text = Application.version;



            _menuController.Initialize(spriteRendererPool);

            _menuController.OnError += () =>
            {
                // 例外発生時はメニューを開きなおす（操作不能になる可能性があるため）
                _menuController.PushInitialMenuScreen(new MainScreen(this), enableTouchMask: false);
            };

            WindowFrameList.GetWindowFrame(0, out var backgroundA, out var backgroundB);
            var windowColor = ColorPreset.GetColor(0);
            _menuController.SetWindowFrame(backgroundA, backgroundB, windowColor);

            _menuController.PushInitialMenuScreen(new MainScreen(this), enableTouchMask: false);
        }

        protected virtual void Update()
        {
            if (_menuController.IsDone)
            {
                _menuController.ResetDone();
                _menuController.PushInitialMenuScreen(new MainScreen(this), enableTouchMask: false);
            }
        }

        private void OpenDevice(StandardRogueDevice device)
        {
            device.GetInfo(out var random);
            RogueRandom.Primary = random;
            MessageWorkListener.ClearListeners();
            MessageWorkListener.AddListener(new DeviceMessageWorkListener());

            device.Open(
                spriteRendererPool, tilemapRendererPrefab, touchControllerPrefab, audioMixer, runtimeInspectorPrefab);
        }



        /// <summary>
        /// タイトルのメイン画面
        /// </summary>
        private class MainScreen : RogueMenuScreen
        {
            private readonly TitleMenu parent;
            private readonly MainMenuViewData<MMgr, MArg> view;

            public MainScreen(TitleMenu parent)
            {
                this.parent = parent;

                view = new()
                {
                    PrimaryCommandSubviewSelector = m => (m as IMMgr)?.TitleMenu,
                };
            }

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.Show(manager, arg)
                    ?
                    .VarOnce(out var loadFadeOutScreen, new LoadFadeOutScreen(parent))
                    .VarOnce(out var newGameMenu, new NewGameScreen(loadFadeOutScreen))

                    // はじめる
                    .Option(":Play", SelectFileMenuScreen.Load(

                        // はじめから
                        onNewFile: (manager, arg) =>
                        {
                            var characterCreationData = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
                            RogueRandom.Primary = new RogueRandom();
                            MessageWorkListener.ClearListeners();
                            MessageWorkListener.AddListener(new DeviceMessageWorkListener());
                            var player = characterCreationData.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                            manager.PushMenuScreen(newGameMenu, player, null, other: characterCreationData);
                        },

                        // つづきから
                        onSelectFile: (fileInfo, manager, arg) =>
                        {
                            manager.PopMenuScreen();
                            manager.PushMenuScreen(loadFadeOutScreen, other: fileInfo.FullName);
                        }))

                    // クレジット
                    .Option(":Credit", new CreditListScreen() { credits = parent._credits })

                    .Build();
            }
        }



        /// <summary>
        /// ニューゲームのキャラクタークリエイト画面
        /// </summary>
        private class NewGameScreen : RogueMenuScreen
        {
            private readonly ScrollMenuViewData<object, MMgr, MArg> view;

            public NewGameScreen(LoadFadeOutScreen loadFadeOutScreen)
            {
                view = new()
                {
                    ScrollSubviewSelector = m => (m as IMMgr)?.CharacterCreation,
                    BackAnchorList = new(
                        _ => _
                        .Option(null) // プリセット読み込みボタン（OpenScreen で設定）
                        .Option(":Done", ChoicesMenuScreen.SaveBackDialog( // キャラクタークリエイト完了ボタン
                            ":DoneMsg", ":SaveAndStart", (manager, arg) => manager.PushMenuScreen(loadFadeOutScreen, arg),
                            ":QuitWithoutSaving", null))),
                };
            }

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                // プリセット読み込みボタンを設定する
                var characterCreation = RoguegardSubviews.GetCharacterCreation(manager);
                view.BackAnchorList[0] = characterCreation.LoadPresetOption;

                view.Show(System.Array.Empty<object>(), manager, arg)
                    ?
                    .Build();
            }
        }

        /// <summary>
        /// - ロードゲーム
        /// - ニューゲームのキャラクタークリエイト確定
        /// の後に実行するフェードアウトとシーン切り替えの画面
        /// </summary>
        private class LoadFadeOutScreen : RogueMenuScreen
        {
            private readonly TitleMenu parent;
            private readonly FadeOutInViewData<MMgr, MArg> view;

            public LoadFadeOutScreen(TitleMenu parent)
            {
                this.parent = parent;

                view = new()
                {
                };
            }

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.FadeOut(manager, arg)
                    ?
                    .OnFadeOutCompleted((manager, arg) =>
                    {
                        if (arg.Arg.Other is CharacterCreationData characterCreationData)
                        {
                            // クリエイトしたキャラクターで開始
                            var loadSceneOperation = Addressables.LoadSceneAsync(parent._nextSceneAddress, activateOnLoad: true);
                            loadSceneOperation.Completed += _ =>
                            {
                                var save = new StandardRogueDeviceSave(characterCreationData);
                                var device = RogueDevice.NewGame(save);
                                parent.OpenDevice(device);
                            };
                        }
                        else if (arg.Arg.Other is string path)
                        {
                            // セーブデータを読み込んで開始
                            var loadSceneOperation = Addressables.LoadSceneAsync(parent._nextSceneAddress, activateOnLoad: true);
                            loadSceneOperation.Completed += _ =>
                            {
                                StandardRogueDevice device;
                                using (var stream = RogueFile.OpenRead(path))
                                {
                                    var save = new StandardRogueDeviceSave();
                                    device = RogueDevice.LoadGame(save, stream);
                                    stream.Close();
                                }
                                parent.OpenDevice(device);
                            };
                        }
                        else throw new System.InvalidOperationException("Unexpected operation.");
                    })

                    .Build();
            }
        }



        /// <summary>
        /// クレジット一覧画面
        /// </summary>
        private class CreditListScreen : RogueMenuScreen
        {
            public IReadOnlyList<CreditData> credits;

            private readonly ScrollMenuViewData<CreditData, MMgr, MArg> view = new()
            {
                Title = ":Credit",
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.Show(credits, manager, arg)
                    ?
                    .NameFrom(credit => credit.Name)

                    .VarOnce(out var nextScreen, new CreditDetailsScreen())
                    .OnClick((credit, manager, arg) =>
                    {
                        manager.PushMenuScreen(nextScreen, other: credit);
                    })

                    .Build();
            }

            /// <summary>
            /// クレジット詳細画面
            /// </summary>
            private class CreditDetailsScreen : RogueMenuScreen
            {
                private readonly DialogViewData<MMgr, MArg> view = new()
                {
                    DialogSubviewSelector = m => m.Widgets,
                };

                public override void OpenScreen(in MMgr manager, in MArg arg)
                {
                    var credit = (CreditData)arg.Arg.Other;

                    // 文字列にリンクを貼ったものを表示
                    var text = Regex.Replace(credit.Details, @"(https?://[a-zA-Z0-9@:%_\\+\-.~#?&/=]+)", "<color=#8080ff><u><link>$1</link></u></color>");

                    view.Show(text, manager, arg)
                        ?
                        .VarOnce(out var viewWidth, 8000f)
                        .Tail.Append(ContentSizeMetaWidgetOption.Create(viewWidth))

                        .VarOnce(out var nextScreen, new URLDialog())
                        .OnClickLink((link, manager, arg) => manager.PushMenuScreen(nextScreen, other: link))

                        .Build();
                }
            }

            /// <summary>
            /// クレジット詳細の URL クリック時の「{URL} へ移動しますか？」ダイアログ
            /// </summary>
            private class URLDialog : RogueMenuScreen
            {
                private readonly SpeechBoxViewData<MMgr, MArg> view = new()
                {
                };

                public override bool IsIncremental => true;

                public override void OpenScreen(in MMgr manager, in MArg arg)
                {
                    var url = (string)arg.Arg.Other;
                    view.Show($"{url} へ移動しますか？", manager, arg)
                        ?
                        .Option(":Yes", (manager, arg) =>
                        {
                            var url = (string)arg.Arg.Other;
                            manager.PopMenuScreen();
                            Application.OpenURL(url);
                        })

                        .Back()

                        .Build();
                }

                public override void CloseScreenView(MMgr manager, bool back)
                {
                    view.Hide(manager, back);
                }
            }
        }
    }
}
