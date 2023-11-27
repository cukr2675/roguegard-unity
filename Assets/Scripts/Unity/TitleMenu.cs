using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using TMPro;
using Roguegard;
using Roguegard.Device;

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
        private CharacterCreationDatabase characterCreationDatabase;

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
            CharacterCreationDatabase characterCreationDatabase,
            Sprite logo)
        {
            this.spriteRendererPool = spriteRendererPool;
            this.tilemapRendererPrefab = tilemapRendererPrefab;
            this.touchControllerPrefab = touchControllerPrefab;
            this.soundTable = soundTable;
            this.audioMixer = audioMixer;
            this.seAudioSourcePrefab = seAudioSourcePrefab;
            this.bgmAudioSourcePrefab = bgmAudioSourcePrefab;
            this.characterCreationDatabase = characterCreationDatabase;
            DontDestroyOnLoad(spriteRendererPool.gameObject);

            menuController = Instantiate(menuControllerPrefab);
            var soundController = new SoundController();
            soundController.Open(null, seAudioSourcePrefab, soundTable.ToTable());
            menuController.Initialize(soundController, characterCreationDatabase, spriteRendererPool);

            _logo.sprite = logo;
            _logo.SetNativeSize();

            _creditMenu.Initialize();
            _versionText.text = Application.version;
            _background.sprite = CoreTiles1.Grass.Sprite;
            var backgroundColor = CoreTiles1.Grass.Color;
            backgroundColor.a = .375f;
            _background.color = backgroundColor;
            WindowFrameList.GetWindowFrame(0, out var backgroundA, out var backgroundB);
            var windowColor = ColorPreset.GetColor(0);
            SetBackgroundSprite(backgroundA, backgroundB, windowColor);
            menuController.SetWindowFrame(backgroundA, backgroundB, windowColor);

            menuController.OpenMenu(new MainMenu(this), null, null, RogueMethodArgument.Identity);
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
                soundTable, audioMixer, seAudioSourcePrefab, bgmAudioSourcePrefab, characterCreationDatabase);
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
                parent._titleMenu.OpenView(ChoicesModelsMenuItemController.Instance, models, root, null, null, RogueMethodArgument.Identity);
            }
        }

        private class StartGameChoice : IModelsMenuChoice
        {
            private readonly NextMenu nextMenu;

            public StartGameChoice(TitleMenu parent)
            {
                nextMenu = new NextMenu() { parent = parent };
            }

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "はじめる";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.OpenMenu(nextMenu, null, null, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
            }

            private class NextMenu : IModelsMenu
            {
                public TitleMenu parent;

                public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    parent.menuController.OpenSelectFile((root, path) =>
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        FadeCanvas.FadeWithLoadScene($"{parent._nextSceneAddress}", () => Loaded(path));
                    },
                    (root) =>
                    {
                        root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        FadeCanvas.FadeWithLoadScene($"{parent._nextSceneAddress}", () => Loaded());
                    });
                }

                private void Loaded(string path)
                {
                    RogueFile.OpenRead(path, stream =>
                    {
                        var name = RogueFile.GetName(path);
                        var save = new StandardRogueDeviceSave();
                        var device = RogueDevice.LoadGame(save, stream, name);
                        stream.Close();
                        parent.OpenDevice(device);
                    });
                }

                private void Loaded()
                {
                    var save = new StandardRogueDeviceSave();
                    var device = RogueDevice.NewGame(save);
                    parent.OpenDevice(device);
                }
            }
        }

        private class CreditChoice : IModelsMenuChoice
        {
            public TitleMenu parent;
            private NextMenu nextMenu;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "クレジット";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (nextMenu == null)
                {
                    nextMenu = new NextMenu() { parent = parent };
                }

                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.OpenMenu(nextMenu, null, null, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
            }

            private class NextMenu : IModelsMenu, IModelsMenuItemController
            {
                public TitleMenu parent;

                public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                    scroll.OpenView(this, parent._credits, root, null, null, RogueMethodArgument.Identity);
                    scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
                }

                public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return ((CreditData)model).Name;
                }

                public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    var credit = (CreditData)model;
                    parent._creditMenu.Show(credit, root);
                }
            }
        }

        //private class BackChoice : IModelsMenuChoice
        //{
        //    public TitleMenu parent;

        //    public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        return ExitModelsMenuChoice.Instance.GetName(root, self, user, arg);
        //    }

        //    public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        MenuController.Show(parent._scrollMenu.CanvasGroup, false);
        //        MenuController.Show(parent._creditMenu.CanvasGroup, false);
        //    }
        //}
    }
}
