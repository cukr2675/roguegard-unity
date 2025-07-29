using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using Lysionium;
using OchalikeSprites;
using Roguegard;
using Roguegard.Device;
using Roguegard.CharacterCreation;

namespace RoguegardUnity
{
    public class CharacterCreationSubview : Subview, ICharacterCreationElementsSubview
    {
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private RectTransform _firstParent = null;
        [SerializeField] private RectTransform _appearanceParent = null;
        [SerializeField] private ButtonViewItem _appearanceButton = null;
        [SerializeField] private TMP_InputField _nameField = null;
        [SerializeField] private CharacterCreationStarsItem _stars = null;
        [SerializeField] private ButtonViewItem _raceButton = null;
        [SerializeField] private RectTransform _secondParent = null;
        [SerializeField] private LabelViewItem _headerPrefab = null;
        [SerializeField] private CharacterCreationViewElementButton _elementButtonPrefab = null;

        private CharacterCreationData characterCreationData;
        private CharacterCreationAddMenu addMenu;
        private CharacterCreationOptionMenu optionMenu;

        private IButtonViewItemHandler intrinsicPresenter;
        private IButtonViewItemHandler startingItemPresenter;
        private static readonly ISelectOption intrinsicHeader
            = SelectOption.Create<MMgr, MArg>("固有能力", delegate { });
        private static readonly ISelectOption startingItemHeader
            = SelectOption.Create<MMgr, MArg>("初期アイテム", delegate { });
        private MenuRogueObjSpriteRenderer spriteRenderer;
        private ISelectOption raceSelectOption;
        private ISelectOption appearanceSelectOption;
        private AppearanceEditingMenu appearanceEditingMenu;
        private readonly List<MonoBehaviour> itemObjects = new List<MonoBehaviour>();
        private static ISelectOption LoadPresetSelectOption { get; }
            = SelectOption.Create<MMgr, MArg>(":Load", new LoadPresetMenu());
        private static readonly object[] leftAnchorObjs = new object[2];

        private readonly List<ViewItem> viewElements = new();

        ISelectOption ICharacterCreationElementsSubview.LoadPresetOption => LoadPresetSelectOption;

        public void Initialize(RogueSpriteRendererPool rendererPool)
        {
            appearanceEditingMenu = new AppearanceEditingMenu();
            spriteRenderer = rendererPool.GetMenuRogueSpriteRenderer(_appearanceParent);
            var spriteRendererTransform = spriteRenderer.GetComponent<RectTransform>();
            spriteRendererTransform.anchorMin = spriteRendererTransform.anchorMax = new Vector2(.5f, 0f);
            spriteRendererTransform.sizeDelta = Vector2.zero;
            spriteRendererTransform.localPosition = Vector3.zero;
            spriteRendererTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0f, 0f);
            spriteRendererTransform.localScale = Vector3.one * 4f;
            _raceButton.Initialize(this);
            _appearanceButton.Initialize(this);
            raceSelectOption = SelectOption.Create<MMgr, MArg>("", (manager, arg) =>
            {
                manager.PushMenuScreen(optionMenu, arg.Self, other: characterCreationData.Race);
            });
            appearanceSelectOption = SelectOption.Create<MMgr, MArg>("", (manager, arg) =>
            {
                manager.PushMenuScreen(appearanceEditingMenu, arg.Self, other: characterCreationData);
            });
            _nameField.onValueChanged.AddListener(text => characterCreationData.Name = text);
        }

        public override void SetParameters(
            IReadOnlyList<object> list, IViewItemHandler handler, IListMenuManager manager, IListMenuArg iArg,
            ref ISubviewStateProvider stateProvider)
        {
            var arg = (MArg)iArg;
            characterCreationData = (CharacterCreationData)arg.Arg.Other;
            if (addMenu == null)
            {
                addMenu = new CharacterCreationAddMenu(RoguegardSettings.CharacterCreationDatabase);
                optionMenu = new CharacterCreationOptionMenu(RoguegardSettings.CharacterCreationDatabase);
            }
            addMenu.Set(characterCreationData);
            optionMenu.Set(characterCreationData);
            appearanceEditingMenu.NextMenu = optionMenu;
            appearanceEditingMenu.AddMenu = addMenu;

            viewElements.Clear();

            if (intrinsicPresenter == null)
            {
                intrinsicPresenter = new ButtonViewItemHandler<Intrinsic, MMgr, MArg>()
                {
                    GetName = (element, manager, arg) =>
                    {
                        if (element == null) return "+ 固有能力を追加";
                        else return element.Name;
                    },
                    HandleClick = (element, manager, arg) =>
                    {
                        if (element == null) { manager.PushMenuScreen(addMenu, arg.Self, other: typeof(Intrinsic)); }
                        else { manager.PushMenuScreen(optionMenu, arg.Self, other: element); }
                    },
                };

                startingItemPresenter = new ButtonViewItemHandler<StartingItem, MMgr, MArg>()
                {
                    GetName = (element, manager, arg) =>
                    {
                        if (element == null) return "+ 固有能力を追加";
                        else return element.Name;
                    },
                    HandleClick = (element, manager, arg) =>
                    {
                        if (element == null) { manager.PushMenuScreen(addMenu, arg.Self, other: typeof(StartingItem)); }
                        else { manager.PushMenuScreen(optionMenu, arg.Self, other: element); }
                    },
                };
            }

            SetArg(manager, arg);

            var random = new RogueRandom(0);
            var obj = new CharacterCreationData(characterCreationData).CreateObj(null, Vector2Int.zero, random);
            obj.Main.Sprite.Update(obj);
            var spriteTransform = OchalikeSpriteTransform.Identity;
            KeywordSpriteMotion.Wait.ApplyTo(obj.Main.Sprite.MotionSet, 0, RogueDirection.Down, ref spriteTransform, out _);
            obj.Main.Sprite.SetTo(spriteRenderer, spriteTransform.PoseSource.GetSpritePose(spriteTransform.Direction), spriteTransform.Direction);

            _nameField.text = characterCreationData.Name;
            characterCreationData.UpdateCost();
            var stars = RoguegardCharacterCreationSettings.GetCharacterStars(characterCreationData.Cost);
            _stars.SetStars(stars, characterCreationData.CostIsUnknown);

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
                header.Bind(intrinsicHeader, SelectOptionViewItemHandler.Instance);
                itemObjects.Add(header);
            }
            for (int i = 0; i < characterCreationData.Intrinsics.Count; i++)
            {
                var intrinsic = characterCreationData.Intrinsics[i];
                var itemButton = Instantiate(_elementButtonPrefab, _secondParent);
                SetTransform((RectTransform)itemButton.transform, ref sumHeight, ref odd);
                itemButton.Initialize(this);
                itemButton.SetItem(intrinsicPresenter, intrinsic, characterCreationData);
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
                header.Bind(startingItemHeader, SelectOptionViewItemHandler.Instance);
                itemObjects.Add(header);
            }
            for (int i = 0; i < characterCreationData.StartingItemTable.Count; i++)
            {
                var startingItem = characterCreationData.StartingItemTable[i][0];
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

            _raceButton.Bind(raceSelectOption, SelectOptionViewItemHandler.Instance);
            viewElements.Add(_raceButton);
            _appearanceButton.Bind(appearanceSelectOption, SelectOptionViewItemHandler.Instance);
            viewElements.Add(_appearanceButton);
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
            private static List<CharacterCreationData> presets;

            private CharacterCreationData element;

            private readonly ChoicesMenuScreen nextMenu;

            private readonly ScrollViewTemplate<CharacterCreationData, MMgr, MArg> view;

            public LoadPresetMenu()
            {
                nextMenu = new ChoicesMenuScreen("ロードすると 編集中のキャラは消えてしまいますが よろしいですか？")
                    .Option("ロードする", Load)
                    .Back();

                view = new()
                {
                };
            }

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                if (presets == null)
                {
                    presets = new List<CharacterCreationData>();
                    for (int i = 0; i < RoguegardSettings.CharacterCreationDatabase.PresetsCount; i++)
                    {
                        presets.Add(RoguegardSettings.CharacterCreationDatabase.LoadPreset(i));
                    }
                }

                view.ShowTemplate(presets, manager, arg)
                    ?
                    .NameFrom((preset, manager, arg) =>
                    {
                        return preset.ShortName;
                    })

                    .OnClick((preset, manager, arg) =>
                    {
                        element = preset;
                        manager.PushMenuScreen(nextMenu, other: (CharacterCreationData)arg.Arg.Other);
                    })

                    .Build();
            }

            private void Load(MMgr manager, MArg arg)
            {
                var characterCreationData = (CharacterCreationData)arg.Arg.Other;
                characterCreationData.Set(element);
                manager.PopMenuScreen(2);
            }
        }
    }
}
