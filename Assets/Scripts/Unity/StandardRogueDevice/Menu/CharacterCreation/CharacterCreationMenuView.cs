using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using ListingMF;
using SDSSprite;
using Roguegard;
using Roguegard.Device;
using Roguegard.CharacterCreation;

namespace RoguegardUnity
{
    public class CharacterCreationMenuView : ElementsSubView
    {
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private RectTransform _firstParent = null;
        [SerializeField] private RectTransform _appearanceParent = null;
        [SerializeField] private ButtonViewElement _appearanceButton = null;
        [SerializeField] private TMP_InputField _nameField = null;
        [SerializeField] private CharacterCreationStarsItem _stars = null;
        [SerializeField] private ButtonViewElement _raceButton = null;
        [SerializeField] private RectTransform _secondParent = null;
        [SerializeField] private LabelViewElement _headerPrefab = null;
        [SerializeField] private CharacterCreationViewElementButton _elementButtonPrefab = null;

        private CharacterCreationDataBuilder builder;
        private CharacterCreationAddMenu addMenu;
        private CharacterCreationOptionMenu optionMenu;

        private IButtonElementHandler intrinsicPresenter;
        private IButtonElementHandler startingItemPresenter;
        private static readonly ISelectOption intrinsicHeader
            = SelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>("固有能力", delegate { });
        private static readonly ISelectOption startingItemHeader
            = SelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>("初期アイテム", delegate { });
        private MenuRogueObjSpriteRenderer spriteRenderer;
        private ISelectOption raceSelectOption;
        private ISelectOption appearanceSelectOption;
        private AppearanceBuildersMenu appearanceBuildersMenu;
        private readonly List<MonoBehaviour> itemObjects = new List<MonoBehaviour>();
        public static ISelectOption LoadPresetSelectOption { get; }
            = SelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(":Load", new LoadPresetMenu());
        private static readonly object[] leftAnchorObjs = new object[2];

        private readonly List<ViewElement> _blockableViewElements = new();
        protected override IReadOnlyList<ViewElement> BlockableViewElements => _blockableViewElements;

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
            raceSelectOption = SelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>("", (manager, arg) =>
            {
                manager.PushMenuScreen(optionMenu, arg.Self, other: builder.Race);
            });
            appearanceSelectOption = SelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>("", (manager, arg) =>
            {
                manager.PushMenuScreen(appearanceBuildersMenu, arg.Self, other: builder);
            });
            _nameField.onValueChanged.AddListener(text => builder.Name = text);
        }

        public override void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg iArg,
            ref IElementsSubViewStateProvider stateProvider)
        {
            var arg = (ReadOnlyMenuArg)iArg;
            builder = (CharacterCreationDataBuilder)arg.Arg.Other;
            if (addMenu == null)
            {
                addMenu = new CharacterCreationAddMenu(RoguegardSettings.CharacterCreationDatabase);
                optionMenu = new CharacterCreationOptionMenu(RoguegardSettings.CharacterCreationDatabase);
            }
            addMenu.Set(builder);
            optionMenu.Set(builder);
            appearanceBuildersMenu.NextMenu = optionMenu;
            appearanceBuildersMenu.AddMenu = addMenu;

            _blockableViewElements.Clear();

            if (intrinsicPresenter == null)
            {
                intrinsicPresenter = new ButtonElementHandler<IntrinsicBuilder, RogueMenuManager, ReadOnlyMenuArg>()
                {
                    GetName = (element, manager, arg) =>
                    {
                        if (element == null) return "+ 固有能力を追加";
                        else return element.Name;
                    },
                    HandleClick = (element, manager, arg) =>
                    {
                        if (element == null) { manager.PushMenuScreen(addMenu, arg.Self, other: typeof(IntrinsicBuilder)); }
                        else { manager.PushMenuScreen(optionMenu, arg.Self, other: element); }
                    },
                };

                startingItemPresenter = new ButtonElementHandler<StartingItemBuilder, RogueMenuManager, ReadOnlyMenuArg>()
                {
                    GetName = (element, manager, arg) =>
                    {
                        if (element == null) return "+ 固有能力を追加";
                        else return element.Name;
                    },
                    HandleClick = (element, manager, arg) =>
                    {
                        if (element == null) { manager.PushMenuScreen(addMenu, arg.Self, other: typeof(StartingItemBuilder)); }
                        else { manager.PushMenuScreen(optionMenu, arg.Self, other: element); }
                    },
                };
            }

            SetArg(manager, arg);

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
                header.SetElement(SelectOptionHandler.Instance, intrinsicHeader);
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
                header.SetElement(SelectOptionHandler.Instance, startingItemHeader);
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

            _raceButton.SetElement(SelectOptionHandler.Instance, raceSelectOption);
            _blockableViewElements.Add(_raceButton);
            _appearanceButton.SetElement(SelectOptionHandler.Instance, appearanceSelectOption);
            _blockableViewElements.Add(_appearanceButton);
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

        private class LoadPresetMenu : RogueMenuScreen
        {
            private static List<CharacterCreationDataBuilder> presets;

            private CharacterCreationDataBuilder element;

            private readonly ChoicesMenuScreen nextMenu;

            private readonly ScrollViewTemplate<CharacterCreationDataBuilder, RogueMenuManager, ReadOnlyMenuArg> view;

            public LoadPresetMenu()
            {
                nextMenu = new ChoicesMenuScreen("ロードすると 編集中のキャラは消えてしまいますが よろしいですか？")
                    .Option("ロードする", Load)
                    .Back();

                view = new()
                {
                };
            }

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                if (presets == null)
                {
                    presets = new List<CharacterCreationDataBuilder>();
                    for (int i = 0; i < RoguegardSettings.CharacterCreationDatabase.PresetsCount; i++)
                    {
                        presets.Add(RoguegardSettings.CharacterCreationDatabase.LoadPreset(i));
                    }
                }

                view.ShowTemplate(presets, manager, arg)
                    ?
                    .ElementNameFrom((preset, manager, arg) =>
                    {
                        return preset.ShortName;
                    })

                    .OnClickElement((preset, manager, arg) =>
                    {
                        element = preset;
                        manager.PushMenuScreen(nextMenu, other: (CharacterCreationDataBuilder)arg.Arg.Other);
                    })

                    .Build();
            }

            private void Load(RogueMenuManager manager, ReadOnlyMenuArg arg)
            {
                var builder = (CharacterCreationDataBuilder)arg.Arg.Other;
                builder.Set(element);
                manager.Back(2);
            }
        }
    }
}
