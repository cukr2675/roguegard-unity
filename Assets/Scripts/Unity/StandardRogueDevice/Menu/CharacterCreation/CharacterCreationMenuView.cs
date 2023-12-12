using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using Roguegard;
using Roguegard.Device;
using Roguegard.CharacterCreation;

namespace RoguegardUnity
{
    public class CharacterCreationMenuView : ModelsMenuView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private RectTransform _firstParent = null;
        [SerializeField] private RectTransform _appearanceParent = null;
        [SerializeField] private ModelsMenuViewItemButton _appearanceButton = null;
        [SerializeField] private TMP_InputField _nameField = null;
        [SerializeField] private TMP_InputField _shortNameField = null;
        [SerializeField] private ModelsMenuViewItemButton _raceButton = null;
        [SerializeField] private RectTransform _secondParent = null;
        [SerializeField] private ModelsMenuViewItemButton _headerPrefab = null;
        [SerializeField] private ModelsMenuViewItemButton _itemButtonPrefab = null;
        [SerializeField] private ModelsMenuViewItemButton _presetButton = null;
        [SerializeField] private ModelsMenuViewItemButton _exitButton = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private CharacterCreationDataBuilder builder;
        private CharacterCreationAddMenu addMenu;
        private CharacterCreationOptionMenu optionMenu;

        private IntrinsicItemController intrinsicItemController;
        private StartingItemItemController startingItemItemController;
        private static readonly HeaderChoice intrinsicHeader = new HeaderChoice() { text = "固有能力" };
        private static readonly HeaderChoice startingItemHeader = new HeaderChoice() { text = "初期アイテム" };
        private MenuRogueObjSpriteRenderer spriteRenderer;
        private RaceChoice raceChoice;
        private AppearanceChoice appearanceChoice;
        private AppearanceBuildersMenu appearanceBuildersMenu;
        private readonly List<ModelsMenuViewItemButton> itemObjects = new List<ModelsMenuViewItemButton>();
        private static readonly LoadPresetChoice loadPresetChoice = new LoadPresetChoice();

        public void Initialize(RogueSpriteRendererPool rendererPool)
        {
            appearanceBuildersMenu = new AppearanceBuildersMenu();
            spriteRenderer = rendererPool.GetMenuRogueSpriteRenderer(_appearanceParent);
            var spriteRendererTransform = spriteRenderer.GetComponent<RectTransform>();
            spriteRendererTransform.anchorMin = spriteRendererTransform.anchorMax = new Vector2(.5f, 0f);
            spriteRendererTransform.sizeDelta = Vector2.zero;
            spriteRendererTransform.localPosition = Vector3.zero;
            spriteRendererTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0f, 0f);
            spriteRendererTransform.localScale = Vector3.one * 4f;
            _raceButton.Initialize(this);
            _appearanceButton.Initialize(this);
            raceChoice = new RaceChoice() { parent = this };
            appearanceChoice = new AppearanceChoice() { parent = this };
            _presetButton.Initialize(this);
            _exitButton.Initialize(this);
            _nameField.onValueChanged.AddListener(text => builder.Name = text);
            _shortNameField.onValueChanged.AddListener(text => builder.ShortName = text);
        }

        public override void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (!(arg.Other is CharacterCreationDataBuilder builder)) throw new RogueException();
            this.builder = builder;
            addMenu = new CharacterCreationAddMenu(builder, RoguegardSettings.CharacterCreationDatabase);
            optionMenu = new CharacterCreationOptionMenu(builder, RoguegardSettings.CharacterCreationDatabase);
            appearanceBuildersMenu.NextMenu = optionMenu;
            appearanceBuildersMenu.AddMenu = addMenu;

            if (intrinsicItemController == null)
            {
                intrinsicItemController = new IntrinsicItemController() { parent = this };
                startingItemItemController = new StartingItemItemController() { parent = this };
            }

            SetArg(root, self, user, arg);

            var random = new RogueRandom(0);
            var obj = new CharacterCreationDataBuilder(builder).CreateObj(null, Vector2Int.zero, random);
            obj.Main.Sprite.Update(obj);
            var spriteTransform = RogueObjSpriteTransform.Identity;
            KeywordBoneMotion.Wait.ApplyTo(obj.Main.Sprite.MotionSet, 0, RogueDirection.Down, ref spriteTransform, out _);
            obj.Main.Sprite.SetTo(spriteRenderer, spriteTransform.PoseSource.GetBonePose(spriteTransform.Direction), spriteTransform.Direction);

            _nameField.text = builder.Name;
            _shortNameField.text = builder.ShortName;

            foreach (var itemObject in itemObjects)
            {
                Destroy(itemObject.gameObject);
            }
            itemObjects.Clear();

            var sumHeight = 0f;
            var odd = false;
            {
                var header = Instantiate(_headerPrefab, _secondParent);
                SetTransform((RectTransform)header.transform, ref sumHeight, ref odd);
                sumHeight += ((RectTransform)header.transform).rect.height;
                odd = false;
                header.Initialize(this);
                header.SetItem(ChoicesModelsMenuItemController.Instance, intrinsicHeader);
                itemObjects.Add(header);
            }
            for (int i = 0; i < builder.Intrinsics.Count; i++)
            {
                var intrinsic = builder.Intrinsics[i];
                var itemButton = Instantiate(_itemButtonPrefab, _secondParent);
                SetTransform((RectTransform)itemButton.transform, ref sumHeight, ref odd);
                itemButton.Initialize(this);
                itemButton.SetItem(intrinsicItemController, intrinsic);
                itemObjects.Add(itemButton);
            }
            {
                var itemButton = Instantiate(_itemButtonPrefab, _secondParent);
                SetTransform((RectTransform)itemButton.transform, ref sumHeight, ref odd);
                itemButton.Initialize(this);
                itemButton.SetItem(intrinsicItemController, null);
                itemObjects.Add(itemButton);
                if (odd) { sumHeight += ((RectTransform)itemButton.transform).rect.height; }
            }

            odd = false;
            {
                var header = Instantiate(_headerPrefab, _secondParent);
                SetTransform((RectTransform)header.transform, ref sumHeight, ref odd);
                sumHeight += ((RectTransform)header.transform).rect.height;
                odd = false;
                header.Initialize(this);
                header.SetItem(ChoicesModelsMenuItemController.Instance, startingItemHeader);
                itemObjects.Add(header);
            }
            for (int i = 0; i < builder.StartingItemTable.Count; i++)
            {
                var startingItem = builder.StartingItemTable[i][0];
                var itemButton = Instantiate(_itemButtonPrefab, _secondParent);
                SetTransform((RectTransform)itemButton.transform, ref sumHeight, ref odd);
                itemButton.Initialize(this);
                itemButton.SetItem(startingItemItemController, startingItem);
                itemObjects.Add(itemButton);
            }
            {
                var itemButton = Instantiate(_itemButtonPrefab, _secondParent);
                SetTransform((RectTransform)itemButton.transform, ref sumHeight, ref odd);
                itemButton.Initialize(this);
                itemButton.SetItem(startingItemItemController, null);
                itemObjects.Add(itemButton);
                if (odd) { sumHeight += ((RectTransform)itemButton.transform).rect.height; }
            }
            _scrollRect.content.SetInsetAndSizeFromParentEdge(
                RectTransform.Edge.Top, 0, _firstParent.rect.height + sumHeight);

            _raceButton.SetItem(ChoicesModelsMenuItemController.Instance, raceChoice);
            _appearanceButton.SetItem(ChoicesModelsMenuItemController.Instance, appearanceChoice);
            _exitButton.SetItem(ChoicesModelsMenuItemController.Instance, models[0]);
            _presetButton.SetItem(ChoicesModelsMenuItemController.Instance, loadPresetChoice);
            MenuController.Show(_canvasGroup, true);
        }

        private static void SetTransform(RectTransform itemTransform, ref float sumHeight, ref bool odd)
        {
            var parentRect = ((RectTransform)itemTransform.parent).rect;
            itemTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, sumHeight, itemTransform.rect.height);
            itemTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, odd ? parentRect.width / 2f : 0f, parentRect.width / 2f);
            if (odd)
            {
                sumHeight += itemTransform.rect.height;
                odd = false;
            }
            else
            {
                odd = true;
            }
        }

        public override float GetPosition()
        {
            return 0f;
        }

        public override void SetPosition(float position)
        {
        }

        private class HeaderChoice : IModelsMenuChoice
        {
            public string text;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return text;
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
            }
        }

        private class RaceChoice : IModelsMenuChoice
        {
            public CharacterCreationMenuView parent;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return null;
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                root.OpenMenu(parent.optionMenu, self, null, new(other: parent.builder.Race), arg);
            }
        }

        private class AppearanceChoice : IModelsMenuChoice
        {
            public CharacterCreationMenuView parent;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return null;
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                root.OpenMenu(parent.appearanceBuildersMenu, self, null, new(other: parent.builder), arg);
            }
        }

        private class IntrinsicItemController : IModelsMenuItemController
        {
            public CharacterCreationMenuView parent;

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null) return "+ 固有能力を追加";
                else return ((IntrinsicBuilder)model).Name;
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.OpenMenu(parent.addMenu, self, null, new(other: typeof(IntrinsicBuilder)), arg);
                }
                else
                {
                    root.OpenMenu(parent.optionMenu, self, user, new(other: (IntrinsicBuilder)model), arg);
                }
            }
        }

        private class StartingItemItemController : IModelsMenuItemController
        {
            public CharacterCreationMenuView parent;

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null) return "+ 初期アイテムを追加";
                else return ((StartingItemBuilder)model).Name;
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.OpenMenu(parent.addMenu, self, null, new(other: typeof(StartingItemBuilder)), arg);
                }
                else
                {
                    root.OpenMenu(parent.optionMenu, self, null, new(other: (StartingItemBuilder)model), arg);
                }
            }
        }

        private class LoadPresetChoice : IModelsMenuChoice
        {
            private static readonly LoadPresetMenu nextMenu = new LoadPresetMenu();

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "ロード";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                root.OpenMenu(nextMenu, self, null, new(other: arg.Other), arg);
            }
        }

        private class LoadPresetMenu : IModelsMenu, IModelsMenuItemController
        {
            private static List<object> presets;

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (presets == null)
                {
                    presets = new List<object>();
                    for (int i = 0; i < RoguegardSettings.CharacterCreationDatabase.PresetsCount; i++)
                    {
                        presets.Add(RoguegardSettings.CharacterCreationDatabase.LoadPreset(i));
                    }
                }

                var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(this, presets, root, self, null, new(other: arg.Other));
                scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
            }

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ((CharacterCreationDataBuilder)model).ShortName;
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var builder = (CharacterCreationDataBuilder)arg.Other;
                builder.Set((CharacterCreationDataBuilder)model);
                root.Back();
            }
        }
    }
}
