using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using SkeletalSprite;
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
        [SerializeField] private CharacterCreationStarsItem _stars = null;
        [SerializeField] private ModelsMenuViewItemButton _raceButton = null;
        [SerializeField] private RectTransform _secondParent = null;
        [SerializeField] private ModelsMenuViewItemButton _headerPrefab = null;
        [SerializeField] private CharacterCreationMenuViewItemButton _itemButtonPrefab = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private CharacterCreationDataBuilder builder;
        private CharacterCreationAddMenu addMenu;
        private CharacterCreationOptionMenu optionMenu;

        private IntrinsicItemController intrinsicItemController;
        private StartingItemItemController startingItemItemController;
        private static readonly HeaderChoice intrinsicHeader = new HeaderChoice("固有能力");
        private static readonly HeaderChoice startingItemHeader = new HeaderChoice("初期アイテム");
        private MenuRogueObjSpriteRenderer spriteRenderer;
        private RaceChoice raceChoice;
        private AppearanceChoice appearanceChoice;
        private AppearanceBuildersMenu appearanceBuildersMenu;
        private readonly List<MonoBehaviour> itemObjects = new List<MonoBehaviour>();
        private static readonly LoadPresetChoice loadPresetChoice = new LoadPresetChoice();
        private static readonly object[] leftAnchorObjs = new object[2];

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
            _nameField.onValueChanged.AddListener(text => builder.Name = text);
        }

        public override void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (!(arg.Other is CharacterCreationDataBuilder builder)) throw new RogueException();
            this.builder = builder;
            if (addMenu == null)
            {
                addMenu = new CharacterCreationAddMenu(RoguegardSettings.CharacterCreationDatabase);
                optionMenu = new CharacterCreationOptionMenu(RoguegardSettings.CharacterCreationDatabase);
            }
            addMenu.Set(builder);
            optionMenu.Set(builder);
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
            var spriteTransform = SkeletalSpriteTransform.Identity;
            KeywordSpriteMotion.Wait.ApplyTo(obj.Main.Sprite.MotionSet, 0, RogueDirection.Down, ref spriteTransform, out _);
            obj.Main.Sprite.SetTo(spriteRenderer, spriteTransform.PoseSource.GetSpritePose(spriteTransform.Direction), spriteTransform.Direction);

            _nameField.text = builder.Name;
            builder.UpdateCost();
            var stars = RoguegardCharacterCreationSettings.GetCharacterStars(builder.Cost);
            _stars.SetStars(stars, builder.CostIsUnknown);

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
                itemButton.SetItem(intrinsicItemController, intrinsic, builder);
                itemObjects.Add(itemButton);
            }
            {
                var itemButton = Instantiate(_itemButtonPrefab, _secondParent);
                SetTransform((RectTransform)itemButton.transform, ref sumHeight, ref odd);
                itemButton.Initialize(this);
                itemButton.SetItem(intrinsicItemController, null, "+ 固有能力を追加");
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
                itemButton.SetItem(startingItemItemController, null, "+ 初期アイテムを追加");
                itemObjects.Add(itemButton);
                if (odd) { sumHeight += ((RectTransform)itemButton.transform).rect.height; }
            }
            _scrollRect.content.SetInsetAndSizeFromParentEdge(
                RectTransform.Edge.Top, 0, _firstParent.rect.height + sumHeight);

            _raceButton.SetItem(ChoicesModelsMenuItemController.Instance, raceChoice);
            _appearanceButton.SetItem(ChoicesModelsMenuItemController.Instance, appearanceChoice);
            MenuController.Show(_canvasGroup, true);

            leftAnchorObjs[0] = loadPresetChoice;
            leftAnchorObjs[1] = models[0];
            var leftAnchor = root.Get(DeviceKw.MenuLeftAnchor);
            leftAnchor.OpenView(ChoicesModelsMenuItemController.Instance, leftAnchorObjs, root, self, user, arg);
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

        private class HeaderChoice : BaseModelsMenuChoice
        {
            public override string Name { get; }

            public HeaderChoice(string name)
            {
                Name = name;
            }

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
            }
        }

        private class RaceChoice : BaseModelsMenuChoice
        {
            public override string Name => null;

            public CharacterCreationMenuView parent;

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                root.OpenMenu(parent.optionMenu, self, null, new(other: parent.builder.Race));
            }
        }

        private class AppearanceChoice : BaseModelsMenuChoice
        {
            public override string Name => null;

            public CharacterCreationMenuView parent;

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                root.OpenMenu(parent.appearanceBuildersMenu, self, null, new(other: parent.builder));
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
                    root.OpenMenu(parent.addMenu, self, null, new(other: typeof(IntrinsicBuilder)));
                }
                else
                {
                    root.OpenMenu(parent.optionMenu, self, user, new(other: (IntrinsicBuilder)model));
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
                    root.OpenMenu(parent.addMenu, self, null, new(other: typeof(StartingItemBuilder)));
                }
                else
                {
                    root.OpenMenu(parent.optionMenu, self, null, new(other: (StartingItemBuilder)model));
                }
            }
        }

        private class LoadPresetChoice : BaseModelsMenuChoice
        {
            public override string Name => ":Load";

            private static LoadPresetMenu nextMenu;

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                if (nextMenu == null) { nextMenu = new(); }
                root.OpenMenu(nextMenu, self, null, new(other: arg.Other));
            }
        }

        private class LoadPresetMenu : BaseScrollModelsMenu<CharacterCreationDataBuilder>
        {
            private static List<CharacterCreationDataBuilder> presets;

            private DialogModelsMenuChoice nextMenu;
            private CharacterCreationDataBuilder model;

            protected override Spanning<CharacterCreationDataBuilder> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (presets == null)
                {
                    presets = new List<CharacterCreationDataBuilder>();
                    for (int i = 0; i < RoguegardSettings.CharacterCreationDatabase.PresetsCount; i++)
                    {
                        presets.Add(RoguegardSettings.CharacterCreationDatabase.LoadPreset(i));
                    }
                }
                return presets;
            }

            protected override string GetItemName(
                CharacterCreationDataBuilder model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return model.ShortName;
            }

            protected override void ItemActivate(
                CharacterCreationDataBuilder model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (nextMenu == null) { nextMenu = new DialogModelsMenuChoice(("ロードする", Load)).AppendExit(); }

                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.AddInt(DeviceKw.StartTalk, 0);
                root.AddObject(DeviceKw.AppendText, "ロードすると 編集中のキャラは消えてしまいますが よろしいですか？");
                root.AddInt(DeviceKw.WaitEndOfTalk, 0);
                root.OpenMenuAsDialog(nextMenu, self, user, arg);

                this.model = model;
            }

            private void Load(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var builder = (CharacterCreationDataBuilder)arg.Other;
                builder.Set(model);
                root.Back();
                root.Back();
            }
        }
    }
}
