using Lysionium;
using Lysionium.Views;
using OchalikeSprites;
using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private CharacterCreationButtonViewItem _buttonViewItemPrefab = null;

        private CharacterCreationData characterCreationData;
        private CharacterCreationAddScreen addScreen;
        private CharacterCreationOptionScreen optionScreen;

        private IButtonViewItemHandler intrinsicPresenter;
        private IButtonViewItemHandler startingItemPresenter;
        private static readonly ISelectOption<MMgr, MArg> intrinsicHeader
            = SelectOption.Create<MMgr, MArg>("固有能力", delegate { });
        private static readonly ISelectOption<MMgr, MArg> startingItemHeader
            = SelectOption.Create<MMgr, MArg>("初期アイテム", delegate { });
        private MenuRogueObjSpriteRenderer spriteRenderer;
        private ISelectOption<MMgr, MArg> raceSelectOption;
        private ISelectOption<MMgr, MArg> appearanceSelectOption;
        private AppearanceEditingScreen appearanceEditingScreen;
        private readonly List<MonoBehaviour> itemObjects = new();
        private static readonly LoadPresetScreen loadPresetScreen = new();
        private static ISelectOption<MMgr, MArg> LoadPresetSelectOption { get; }
            = SelectOption.Create<MMgr, MArg>(":Load", (manager, arg) => manager.PushScreen(loadPresetScreen, arg));
        private static readonly object[] leftAnchorObjs = new object[2];

        private readonly List<ViewItem> viewItems = new();

        ISelectOption<MMgr, MArg> ICharacterCreationElementsSubview.LoadPresetOption => LoadPresetSelectOption;

        public void Initialize(RogueSpriteRendererPool rendererPool)
        {
            appearanceEditingScreen = new AppearanceEditingScreen();
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
                manager.PushScreen(optionScreen, arg.Self, other: characterCreationData.Race);
            });
            appearanceSelectOption = SelectOption.Create<MMgr, MArg>("", (manager, arg) =>
            {
                manager.PushScreen(appearanceEditingScreen, arg.Self, other: characterCreationData);
            });
            _nameField.onValueChanged.AddListener(text => characterCreationData.Name = text);
        }

        public override void SetListHandler(
            IReadOnlyList<object> list, IViewItemHandler handler, IListuiManager manager, IListuiArg iArg,
            ref ISubviewStateProvider stateProvider)
        {
            var arg = (MArg)iArg;
            characterCreationData = (CharacterCreationData)arg.Arg.Other;
            if (addScreen == null)
            {
                addScreen = new CharacterCreationAddScreen(RoguegardSettings.CharacterCreationDatabase);
                optionScreen = new CharacterCreationOptionScreen(RoguegardSettings.CharacterCreationDatabase);
            }
            addScreen.Set(characterCreationData);
            optionScreen.Set(characterCreationData);
            appearanceEditingScreen.OptionScreen = optionScreen;
            appearanceEditingScreen.AddScreen = addScreen;

            viewItems.Clear();

            if (intrinsicPresenter == null)
            {
                intrinsicPresenter = new ButtonViewItemHandler<Intrinsic, MMgr, MArg>()
                {
                    GetName = (intrinsic, manager, arg) =>
                    {
                        if (intrinsic == null) return "+ 固有能力を追加";
                        else return intrinsic.Name;
                    },
                    Click = (intrinsic, manager, arg) =>
                    {
                        if (intrinsic == null) { manager.PushScreen(addScreen, arg.Self, other: typeof(Intrinsic)); }
                        else { manager.PushScreen(optionScreen, arg.Self, other: intrinsic); }
                    },
                };

                startingItemPresenter = new ButtonViewItemHandler<StartingItem, MMgr, MArg>()
                {
                    GetName = (startingItem, manager, arg) =>
                    {
                        if (startingItem == null) return "+ 固有能力を追加";
                        else return startingItem.Name;
                    },
                    Click = (startingItem, manager, arg) =>
                    {
                        if (startingItem == null) { manager.PushScreen(addScreen, arg.Self, other: typeof(StartingItem)); }
                        else { manager.PushScreen(optionScreen, arg.Self, other: startingItem); }
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
                header.Bind(intrinsicHeader);
                itemObjects.Add(header);
            }
            for (int i = 0; i < characterCreationData.Intrinsics.Count; i++)
            {
                var intrinsic = characterCreationData.Intrinsics[i];
                var itemButton = Instantiate(_buttonViewItemPrefab, _secondParent);
                SetTransform((RectTransform)itemButton.transform, ref sumHeight, ref odd);
                itemButton.Initialize(this);
                itemButton.SetItem(intrinsicPresenter, intrinsic, characterCreationData);
                itemObjects.Add(itemButton);
            }
            {
                var itemButton = Instantiate(_buttonViewItemPrefab, _secondParent);
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
                header.Bind(startingItemHeader);
                itemObjects.Add(header);
            }
            for (int i = 0; i < characterCreationData.StartingItemTable.Count; i++)
            {
                var startingItem = characterCreationData.StartingItemTable[i][0];
                var itemButton = Instantiate(_buttonViewItemPrefab, _secondParent);
                SetTransform((RectTransform)itemButton.transform, ref sumHeight, ref odd);
                itemButton.Initialize(this);
                itemButton.SetItem(startingItemPresenter, startingItem);
                itemObjects.Add(itemButton);
            }
            {
                var itemButton = Instantiate(_buttonViewItemPrefab, _secondParent);
                SetTransform((RectTransform)itemButton.transform, ref sumHeight, ref odd);
                itemButton.Initialize(this);
                itemButton.SetItem(startingItemPresenter, null, "+ 初期アイテムを追加");
                itemObjects.Add(itemButton);
                if (odd) { sumHeight += ((RectTransform)itemButton.transform).rect.height; }
            }
            _scrollRect.content.SetInsetAndSizeFromParentEdge(
                RectTransform.Edge.Top, 0, _firstParent.rect.height + sumHeight);

            _raceButton.Bind(raceSelectOption);
            viewItems.Add(_raceButton);
            _appearanceButton.Bind(appearanceSelectOption);
            viewItems.Add(_appearanceButton);
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

        private class LoadPresetScreen : RogueListuiScreen
        {
            private static List<CharacterCreationData> presets;

            private readonly ScrollMenuViewData<CharacterCreationData, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                if (presets == null)
                {
                    presets = new List<CharacterCreationData>();
                    for (int i = 0; i < RoguegardSettings.CharacterCreationDatabase.PresetsCount; i++)
                    {
                        presets.Add(RoguegardSettings.CharacterCreationDatabase.LoadPreset(i));
                    }
                }

                view.Show(presets, manager, arg)
                    ?
                    .NameFrom(preset => preset.ShortName)

                    .VarOnce(out CharacterCreationData selectedPreset)
                    .VarOnce(
                        out var nextScreen, new ChoicesScreen("ロードすると 編集中のキャラは消えてしまいますが よろしいですか？")
                        .Option("ロードする", (manager, arg) => Load(selectedPreset, manager, arg))
                        .Back())
                    .OnClick((preset, manager, arg) =>
                    {
                        selectedPreset = preset;
                        manager.PushScreen(nextScreen, other: (CharacterCreationData)arg.Arg.Other);
                    })

                    .Build();
            }

            private void Load(CharacterCreationData selectedPreset, MMgr manager, MArg arg)
            {
                var characterCreationData = (CharacterCreationData)arg.Arg.Other;
                characterCreationData.Set(selectedPreset);
                manager.PopScreen(2);
            }
        }
    }
}
