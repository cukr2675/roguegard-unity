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
        [SerializeField] private ScrollModelsMenuView _scrollMenu = null;
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

        private IModelsMenuChoice[] titleMenuModels;

        private SelectFileMenu selectFileMenu;

        private BackChoice backChoice;

        public void Initialize(
            RogueSpriteRendererPool spriteRendererPool,
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

            _logo.sprite = logo;
            _logo.SetNativeSize();

            _scrollMenu.Initialize();
            _creditMenu.Initialize();
            _versionText.text = Application.version;
            _background.sprite = CoreTiles1.Grass.Sprite;
            var backgroundColor = CoreTiles1.Grass.Color;
            backgroundColor.a = .375f;
            _background.color = backgroundColor;
            WindowFrameList.GetWindowFrame(0, out var backgroundA, out var backgroundB);
            var windowColor = ColorPreset.GetColor(0);
            SetBackgroundSprite(backgroundA, backgroundB, windowColor);

            titleMenuModels = new IModelsMenuChoice[]
            {
                new StartGameChoice() { parent = this },
                new CreditChoice() { parent = this },
            };

            selectFileMenu = new SelectFileMenu(_scrollMenu);
            backChoice = new BackChoice() { parent = this };

            _titleMenu.OpenView(ChoicesModelsMenuItemController.Instance, titleMenuModels, null, null, null, RogueMethodArgument.Identity);
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

        private class StartGameChoice : IModelsMenuChoice
        {
            public TitleMenu parent;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "はじめる";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                parent.selectFileMenu.Open((root, path) =>
                {
                    FadeCanvas.FadeWithLoadScene($"{parent._nextSceneAddress}", () => Loaded(path));
                },
                (root) =>
                {
                    FadeCanvas.FadeWithLoadScene($"{parent._nextSceneAddress}", () => Loaded());
                });
                parent._scrollMenu.ShowExitButton(parent.backChoice);
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

        private class CreditChoice : IModelsMenuChoice
        {
            public TitleMenu parent;
            private ItemController itemController;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "クレジット";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (itemController == null)
                {
                    itemController = new ItemController() { parent = parent };
                }

                parent._scrollMenu.OpenView(itemController, parent._credits, null, null, null, RogueMethodArgument.Identity);
                parent._scrollMenu.ShowExitButton(parent.backChoice);
            }

            private class ItemController : IModelsMenuItemController
            {
                public TitleMenu parent;

                public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return ((CreditData)model).Name;
                }

                public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var credit = (CreditData)model;
                    parent._creditMenu.Show(credit);
                }
            }
        }

        private class BackChoice : IModelsMenuChoice
        {
            public TitleMenu parent;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ExitModelsMenuChoice.Instance.GetName(root, self, user, arg);
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                MenuController.Show(parent._scrollMenu.CanvasGroup, false);
                MenuController.Show(parent._creditMenu.CanvasGroup, false);
            }
        }
    }
}
