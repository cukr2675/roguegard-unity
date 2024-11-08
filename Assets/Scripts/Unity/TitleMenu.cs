using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;
using UnityEngine.Audio;
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

            var soundController = new SoundController();
            soundController.Open(null, seAudioSourcePrefab, soundTable);

            _versionText.text = Application.version;



            _menuController.Initialize(soundController, spriteRendererPool, false);

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

        private void Update()
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
                spriteRendererPool, tilemapRendererPrefab, touchControllerPrefab,
                soundTable, audioMixer, seAudioSourcePrefab, bgmAudioSourcePrefab, runtimeInspectorPrefab);
        }



        /// <summary>
        /// タイトルのメイン画面
        /// </summary>
        private class MainScreen : RogueMenuScreen
        {
            private readonly TitleMenu parent;
            private readonly MainMenuViewTemplate<MMgr, MArg> view;

            public MainScreen(TitleMenu parent)
            {
                this.parent = parent;

                view = new()
                {
                    PrimaryCommandSubViewName = MenuController.TitleMenuName,
                };
            }

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate(manager, arg)
                    ?
                    .VariableOnce(out var loadFadeOutScreen, new LoadFadeOutScreen(parent))
                    .VariableOnce(out var newGameMenu, new NewGameScreen(loadFadeOutScreen))

                    // はじめる
                    .Option(":Play", SelectFileMenuScreen.Load(

                        // はじめから
                        onNewFile: (manager, arg) =>
                        {
                            var builder = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
                            RogueRandom.Primary = new RogueRandom();
                            MessageWorkListener.ClearListeners();
                            MessageWorkListener.AddListener(new DeviceMessageWorkListener());
                            var player = builder.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
                            manager.PushMenuScreen(newGameMenu, player, null, other: builder);
                        },

                        // つづきから
                        onSelectFile: (fileInfo, manager, arg) =>
                        {
                            manager.Back();
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
            private readonly ScrollViewTemplate<object, MMgr, MArg> view;

            public NewGameScreen(LoadFadeOutScreen loadFadeOutScreen)
            {
                view = new()
                {
                    ScrollSubViewName = RoguegardSubViews.CharacterCreation,
                    BackAnchorList = new()
                    {
                        // プリセット読み込みボタン（OpenScreen で設定）
                        null,

                        // キャラクタークリエイト完了ボタン
                        SelectOption.Create<MMgr, MArg>(
                            ":Done", ChoicesMenuScreen.SaveBackDialog(":DoneMsg", ":SaveAndStart", loadFadeOutScreen, ":QuitWithoutSaving", null))
                    },
                };
            }

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                // プリセット読み込みボタンを設定する
                var characterCreation = RoguegardSubViews.GetCharacterCreation(manager);
                view.BackAnchorList[0] = characterCreation.LoadPresetOption;

                view.ShowTemplate(System.Array.Empty<object>(), manager, arg)
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
            private readonly FadeOutInViewTemplate<MMgr, MArg> view;

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
                        if (arg.Arg.Other is CharacterCreationDataBuilder builder)
                        {
                            // クリエイトしたキャラクターで開始
                            var loadSceneOperation = Addressables.LoadSceneAsync(parent._nextSceneAddress, activateOnLoad: true);
                            loadSceneOperation.Completed += _ =>
                            {
                                var save = new StandardRogueDeviceSave(builder);
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
                        else throw new RogueException("Unexpected operation.");
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

            private readonly ScrollViewTemplate<CreditData, MMgr, MArg> view = new()
            {
                Title = ":Credit",
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate(credits, manager, arg)
                    ?
                    .VariableOnce(out var nextScreen, new CreditDetailsScreen())

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

            /// <summary>
            /// クレジット詳細画面
            /// </summary>
            private class CreditDetailsScreen : RogueMenuScreen
            {
                private readonly DialogViewTemplate<MMgr, MArg> view = new()
                {
                    DialogSubViewName = StandardSubViewTable.WidgetsName,
                };

                public override void OpenScreen(in MMgr manager, in MArg arg)
                {
                    var credit = (CreditData)arg.Arg.Other;

                    // 文字列にリンクを貼ったものを表示
                    var text = Regex.Replace(credit.Details, @"(https?://\S+)", "<color=#8080ff><u><link>$1</link></u></color>");

                    view.ShowTemplate(text, manager, arg)
                        ?
                        .VariableOnce(out var nextScreen, new URLDialog())

                        .OnClickLink((link, manager, arg) =>
                        {
                            manager.PushMenuScreen(nextScreen, other: link);
                        })

                        .Build();
                }
            }

            /// <summary>
            /// クレジット詳細の URL クリック時の「{URL} へ移動しますか？」ダイアログ
            /// </summary>
            private class URLDialog : RogueMenuScreen
            {
                private readonly SpeechBoxViewTemplate<MMgr, MArg> view = new()
                {
                };

                public override bool IsIncremental => true;

                public override void OpenScreen(in MMgr manager, in MArg arg)
                {
                    var url = (string)arg.Arg.Other;
                    view.ShowTemplate($"{url} へ移動しますか？", manager, arg)
                        ?
                        .Option(":Yes", (manager, arg) =>
                        {
                            var url = (string)arg.Arg.Other;
                            manager.Back();
                            Application.OpenURL(url);
                        })

                        .Back()

                        .Build();
                }

                public override void CloseScreen(MMgr manager, bool back)
                {
                    view.HideTemplate(manager, back);
                }
            }
        }
    }
}
