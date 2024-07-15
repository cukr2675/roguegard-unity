using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using SDSSprite;
using Roguegard;
using Roguegard.Device;
using Roguegard.CharacterCreation;

namespace RoguegardUnity
{
    public class CharacterCreationMenuView : ElementsView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private RectTransform _firstParent = null;
        [SerializeField] private RectTransform _appearanceParent = null;
        [SerializeField] private ViewElementButton _appearanceButton = null;
        [SerializeField] private TMP_InputField _nameField = null;
        [SerializeField] private CharacterCreationStarsItem _stars = null;
        [SerializeField] private ViewElementButton _raceButton = null;
        [SerializeField] private RectTransform _secondParent = null;
        [SerializeField] private ViewElementButton _headerPrefab = null;
        [SerializeField] private CharacterCreationViewElementButton _elementButtonPrefab = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private CharacterCreationDataBuilder builder;
        private CharacterCreationAddMenu addMenu;
        private CharacterCreationOptionMenu optionMenu;

        private IntrinsicPresenter intrinsicPresenter;
        private StartingItemPresenter startingItemPresenter;
        private static readonly HeaderSelectOption intrinsicHeader = new HeaderSelectOption("固有能力");
        private static readonly HeaderSelectOption startingItemHeader = new HeaderSelectOption("初期アイテム");
        private MenuRogueObjSpriteRenderer spriteRenderer;
        private RaceSelectOption raceSelectOption;
        private AppearanceSelectOption appearanceSelectOption;
        private AppearanceBuildersMenu appearanceBuildersMenu;
        private readonly List<MonoBehaviour> itemObjects = new List<MonoBehaviour>();
        private static readonly LoadPresetSelectOption loadPresetSelectOption = new LoadPresetSelectOption();
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
            raceSelectOption = new RaceSelectOption() { parent = this };
            appearanceSelectOption = new AppearanceSelectOption() { parent = this };
            _nameField.onValueChanged.AddListener(text => builder.Name = text);
        }

        public override void OpenView<T>(
            IElementPresenter presenter, Spanning<T> list, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
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

            if (intrinsicPresenter == null)
            {
                intrinsicPresenter = new IntrinsicPresenter() { parent = this };
                startingItemPresenter = new StartingItemPresenter() { parent = this };
            }

            SetArg(manager, self, user, arg);

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
                header.SetItem(SelectOptionPresenter.Instance, intrinsicHeader);
                itemObjects.Add(header);
            }
            for (int i = 0; i < builder.Intrinsics.Count; i++)
            {
                var intrinsic = builder.Intrinsics[i];
                var itemButton = Instantiate(_elementButtonPrefab, _secondParent);
                SetTransform((RectTransform)itemButton.transform, ref sumHeight, ref odd);
                itemButton.Initialize(this);
                itemButton.SetItem(intrinsicPresenter, intrinsic, builder);
                itemObjects.Add(itemButton);
            }
            {
                var itemButton = Instantiate(_elementButtonPrefab, _secondParent);
                SetTransform((RectTransform)itemButton.transform, ref sumHeight, ref odd);
                itemButton.Initialize(this);
                itemButton.SetItem(intrinsicPresenter, null, "+ 固有能力を追加");
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
                header.SetItem(SelectOptionPresenter.Instance, startingItemHeader);
                itemObjects.Add(header);
            }
            for (int i = 0; i < builder.StartingItemTable.Count; i++)
            {
                var startingItem = builder.StartingItemTable[i][0];
                var itemButton = Instantiate(_elementButtonPrefab, _secondParent);
                SetTransform((RectTransform)itemButton.transform, ref sumHeight, ref odd);
                itemButton.Initialize(this);
                itemButton.SetItem(startingItemPresenter, startingItem);
                itemObjects.Add(itemButton);
            }
            {
                var itemButton = Instantiate(_elementButtonPrefab, _secondParent);
                SetTransform((RectTransform)itemButton.transform, ref sumHeight, ref odd);
                itemButton.Initialize(this);
                itemButton.SetItem(startingItemPresenter, null, "+ 初期アイテムを追加");
                itemObjects.Add(itemButton);
                if (odd) { sumHeight += ((RectTransform)itemButton.transform).rect.height; }
            }
            _scrollRect.content.SetInsetAndSizeFromParentEdge(
                RectTransform.Edge.Top, 0, _firstParent.rect.height + sumHeight);

            _raceButton.SetItem(SelectOptionPresenter.Instance, raceSelectOption);
            _appearanceButton.SetItem(SelectOptionPresenter.Instance, appearanceSelectOption);
            MenuController.Show(_canvasGroup, true);

            leftAnchorObjs[0] = loadPresetSelectOption;
            leftAnchorObjs[1] = list[0];
            var leftAnchor = manager.GetView(DeviceKw.MenuLeftAnchor);
            leftAnchor.OpenView(SelectOptionPresenter.Instance, leftAnchorObjs, manager, self, user, arg);
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

        private class HeaderSelectOption : BaseListMenuSelectOption
        {
            public override string Name { get; }

            public HeaderSelectOption(string name)
            {
                Name = name;
            }

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
            }
        }

        private class RaceSelectOption : BaseListMenuSelectOption
        {
            public override string Name => null;

            public CharacterCreationMenuView parent;

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                manager.OpenMenu(parent.optionMenu, self, null, new(other: parent.builder.Race));
            }
        }

        private class AppearanceSelectOption : BaseListMenuSelectOption
        {
            public override string Name => null;

            public CharacterCreationMenuView parent;

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                manager.OpenMenu(parent.appearanceBuildersMenu, self, null, new(other: parent.builder));
            }
        }

        private class IntrinsicPresenter : IElementPresenter
        {
            public CharacterCreationMenuView parent;

            public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (element == null) return "+ 固有能力を追加";
                else return ((IntrinsicBuilder)element).Name;
            }

            public void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (element == null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    manager.OpenMenu(parent.addMenu, self, null, new(other: typeof(IntrinsicBuilder)));
                }
                else
                {
                    manager.OpenMenu(parent.optionMenu, self, user, new(other: (IntrinsicBuilder)element));
                }
            }
        }

        private class StartingItemPresenter : IElementPresenter
        {
            public CharacterCreationMenuView parent;

            public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (element == null) return "+ 初期アイテムを追加";
                else return ((StartingItemBuilder)element).Name;
            }

            public void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (element == null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    manager.OpenMenu(parent.addMenu, self, null, new(other: typeof(StartingItemBuilder)));
                }
                else
                {
                    manager.OpenMenu(parent.optionMenu, self, null, new(other: (StartingItemBuilder)element));
                }
            }
        }

        private class LoadPresetSelectOption : BaseListMenuSelectOption
        {
            public override string Name => ":Load";

            private static LoadPresetMenu nextMenu;

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                if (nextMenu == null) { nextMenu = new(); }
                manager.OpenMenu(nextMenu, self, null, new(other: arg.Other));
            }
        }

        private class LoadPresetMenu : BaseScrollListMenu<CharacterCreationDataBuilder>
        {
            private static List<CharacterCreationDataBuilder> presets;

            private DialogListMenuSelectOption nextMenu;
            private CharacterCreationDataBuilder element;

            protected override Spanning<CharacterCreationDataBuilder> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
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
                CharacterCreationDataBuilder element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return element.ShortName;
            }

            protected override void ActivateItem(
                CharacterCreationDataBuilder element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (nextMenu == null) { nextMenu = new DialogListMenuSelectOption(("ロードする", Load)).AppendExit(); }

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.AddInt(DeviceKw.StartTalk, 0);
                manager.AddObject(DeviceKw.AppendText, "ロードすると 編集中のキャラは消えてしまいますが よろしいですか？");
                manager.AddInt(DeviceKw.WaitEndOfTalk, 0);
                manager.OpenMenuAsDialog(nextMenu, self, user, arg);

                this.element = element;
            }

            private void Load(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var builder = (CharacterCreationDataBuilder)arg.Other;
                builder.Set(element);
                manager.Back();
                manager.Back();
            }
        }
    }
}
