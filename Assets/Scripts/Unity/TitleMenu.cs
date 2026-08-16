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
                _menuController.PushInitialScreen(new MainScreen(this), enableTouchMask: false);
            };

            WindowFrameList.GetWindowFrame(0, out var backgroundA, out var backgroundB);
            var windowColor = ColorPreset.GetColor(0);
            _menuController.SetWindowFrame(backgroundA, backgroundB, windowColor);

            _menuController.PushInitialScreen(new MainScreen(this), enableTouchMask: false);
        }

        protected virtual void Update()
        {
            if (_menuController.IsDone)
            {
                _menuController.ResetDone();
                _menuController.PushInitialScreen(new MainScreen(this), enableTouchMask: false);
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
        private class MainScreen : RogueListuiScreen
        {
            private readonly MainMenuViewData<MMgr> view;

            public MainScreen(TitleMenu parent)
            {
                view = new()
                {
                    PrimaryCommandSubviewSelector = m => m.TitleMenu,
                };

                OnOpenScreen += (manager) =>
                {
                    view.Show(manager)
                    ?
                    .VarOnce(out var loadFadeOutScreen, new LoadFadeOutScreen(parent))
                    .VarOnce(out var newGameScreen, new NewGameScreen(loadFadeOutScreen))

                    // はじめる
                    .Option(":Play", FileSelectionScreen.Load(

                        // はじめから
                        onNewFile: (manager) =>
                        {
                            var characterCreationData = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
                            RogueRandom.Primary = new RogueRandom();
                            MessageWorkListener.ClearListeners();
                            MessageWorkListener.AddListener(new DeviceMessageWorkListener());
                            var player = characterCreationData.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                            manager.PushScreen(newGameScreen, player, null, other: characterCreationData);
                        },

                        // つづきから
                        onSelectFile: (fileInfo, manager) =>
                        {
                            manager.PopScreen();
                            manager.PushScreen(loadFadeOutScreen, other: fileInfo.FullName);
                        }), () => Arg)

                    // クレジット
                    .Option(":Credit", new CreditListScreen(parent._credits), () => Arg)

                    .Build();
                };
            }
        }



        /// <summary>
        /// ニューゲームのキャラクタークリエイト画面
        /// </summary>
        private class NewGameScreen : RogueListuiScreen
        {
            private readonly CharacterCreationViewData view = new()
            {
            };

            public NewGameScreen(LoadFadeOutScreen loadFadeOutScreen)
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(manager)
                    ?
                    .Init(() =>
                    {
                        view.BackAnchorList = new(
                            _ => _
                            .Option(manager.CharacterCreation.LoadPresetOption, () => Arg) // プリセット読み込みボタン
                            .Option(":Done", ChoicesScreen.SaveBackDialog( // キャラクタークリエイト完了ボタン
                                ":DoneMsg", ":SaveAndStart", m => m.PushScreen(loadFadeOutScreen, Arg),
                                ":QuitWithoutSaving", null), () => Arg));
                    })
                    .Build();
                };
            }
        }

        /// <summary>
        /// - ロードゲーム
        /// - ニューゲームのキャラクタークリエイト確定
        /// の後に実行するフェードアウトとシーン切り替えの画面
        /// </summary>
        private class LoadFadeOutScreen : RogueListuiScreen
        {
            private readonly FadeOutInViewData<MMgr> view;

            public LoadFadeOutScreen(TitleMenu parent)
            {
                view = new()
                {
                };

                OnOpenScreen += (manager) =>
                {
                    view.FadeOut(manager)
                    ?
                    .OnFadeOutCompleted((manager) =>
                    {
                        if (Arg.Arg.Other is CharacterCreationData characterCreationData)
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
                        else if (Arg.Arg.Other is string path)
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
                };
            }
        }



        /// <summary>
        /// クレジット一覧画面
        /// </summary>
        private class CreditListScreen : RogueListuiScreen
        {
            private readonly ScrollMenuViewData<CreditData, MMgr> view = new()
            {
                Title = ":Credit",
            };

            public CreditListScreen(IReadOnlyList<CreditData> credits)
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(credits, manager)
                    ?
                    .NameFrom(credit => credit.Name)

                    .VarOnce(out var nextScreen, new CreditDetailsScreen())
                    .OnClick((credit, m) => m.PushScreen(nextScreen, other: credit))

                    .Build();
                };
            }

            /// <summary>
            /// クレジット詳細画面
            /// </summary>
            private class CreditDetailsScreen : RogueListuiScreen
            {
                private readonly DialogViewData<MMgr> view = new()
                {
                    DialogSubviewSelector = m => m.Widgets,
                };

                public CreditDetailsScreen()
                {
                    OnOpenScreen += (manager) =>
                    {
                        var credit = (CreditData)Arg.Arg.Other;

                        // 文字列にリンクを貼ったものを表示
                        var text = Regex.Replace(credit.Details, @"(https?://[a-zA-Z0-9@:%_\\+\-.~#?&/=]+)", "<color=#8080ff><u><link>$1</link></u></color>");

                        view.Show(text, manager)
                        ?
                        .VarOnce(out var viewWidth, 8000f)
                        .Tail.Append(ContentSizeMetaWidgetOption.Create(viewWidth))

                        .VarOnce(out var nextScreen, new URLDialog())
                        .OnClickLink((link, manager, _) => manager.PushScreen(nextScreen, other: link))

                        .Build();
                    };
                }
            }

            /// <summary>
            /// クレジット詳細の URL クリック時の「{URL} へ移動しますか？」ダイアログ
            /// </summary>
            private class URLDialog : RogueListuiScreen
            {
                private readonly SpeechBoxViewData<MMgr> view = new()
                {
                };

                public URLDialog()
                {
                    OnOpenScreen += (manager) =>
                    {
                        var url = (string)Arg.Arg.Other;
                        view.Show($"{url} へ移動しますか？", manager)
                        ?
                        .Option(":Yes", (manager) =>
                        {
                            var url = (string)Arg.Arg.Other;
                            manager.PopScreen();
                            Application.OpenURL(url);
                        })

                        .Back()

                        .Build();
                    };

                    OnCloseScreenView += (manager, back) =>
                    {
                        view.Hide(manager, back);
                    };
                }
            }
        }
    }
}
