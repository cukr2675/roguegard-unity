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
        [SerializeField] private RectTransform _intrinsicParent = null;
        [SerializeField] private RectTransform _startingItemParent = null;
        [SerializeField] private ModelsMenuViewItemButton _headerPrefab = null;
        [SerializeField] private ModelsMenuViewItemButton _itemButtonPrefab = null;
        [SerializeField] private ModelsMenuViewItemButton _exitButton = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private CharacterCreationDatabase database;
        private CharacterCreationDataBuilder builder;

        private IntrinsicItemController intrinsicItemController;
        private StartingItemItemController startingItemItemController;
        private static readonly HeaderChoice intrinsicHeader = new HeaderChoice() { text = "固有能力" };
        private static readonly HeaderChoice startingItemHeader = new HeaderChoice() { text = "初期アイテム" };
        private readonly List<ModelsMenuViewItemButton> intrinsicItemButtons = new List<ModelsMenuViewItemButton>();
        private readonly List<ModelsMenuViewItemButton> startingItemItemButtons = new List<ModelsMenuViewItemButton>();

        public void Initialize(CharacterCreationDatabase database)
        {
            this.database = database;
            _exitButton.Initialize(this);
            _nameField.onValueChanged.AddListener(text => builder.Name = text);
            _shortNameField.onValueChanged.AddListener(text => builder.ShortName = text);
        }

        public override void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (!(models[0] is CharacterCreationDataBuilder builder)) throw new RogueException();
            this.builder = builder;

            if (intrinsicItemController == null)
            {
                intrinsicItemController = new IntrinsicItemController() { parent = this };
                startingItemItemController = new StartingItemItemController() { parent = this };
            }

            SetArg(root, self, user, arg);
            _nameField.text = builder.Name;
            _shortNameField.text = builder.ShortName;

            foreach (var itemButton in intrinsicItemButtons)
            {
                Destroy(itemButton.gameObject);
            }
            intrinsicItemButtons.Clear();
            foreach (var itemButton in startingItemItemButtons)
            {
                Destroy(itemButton.gameObject);
            }
            startingItemItemButtons.Clear();

            var intrinsicSumHeight = 0f;
            {
                var header = Instantiate(_headerPrefab, _intrinsicParent);
                SetTransform((RectTransform)header.transform, ref intrinsicSumHeight);
                header.Initialize(this);
                header.SetItem(ChoicesModelsMenuItemController.Instance, intrinsicHeader);
            }
            for (int i = 0; i < builder.Intrinsics.Count; i++)
            {
                var intrinsic = builder.Intrinsics[i];
                var itemButton = Instantiate(_itemButtonPrefab, _intrinsicParent);
                SetTransform((RectTransform)itemButton.transform, ref intrinsicSumHeight);
                itemButton.Initialize(this);
                itemButton.SetItem(intrinsicItemController, intrinsic);
                intrinsicItemButtons.Add(itemButton);
            }
            {
                var itemButton = Instantiate(_itemButtonPrefab, _intrinsicParent);
                SetTransform((RectTransform)itemButton.transform, ref intrinsicSumHeight);
                itemButton.Initialize(this);
                itemButton.SetItem(intrinsicItemController, null);
                intrinsicItemButtons.Add(itemButton);
            }

            var startingItemSumHeight = 0f;
            {
                var header = Instantiate(_headerPrefab, _startingItemParent);
                SetTransform((RectTransform)header.transform, ref startingItemSumHeight);
                header.Initialize(this);
                header.SetItem(ChoicesModelsMenuItemController.Instance, startingItemHeader);
            }
            for (int i = 0; i < builder.StartingItemTable.Count; i++)
            {
                var startingItem = builder.StartingItemTable[i][0];
                var itemButton = Instantiate(_itemButtonPrefab, _startingItemParent);
                SetTransform((RectTransform)itemButton.transform, ref startingItemSumHeight);
                itemButton.Initialize(this);
                itemButton.SetItem(startingItemItemController, startingItem);
                startingItemItemButtons.Add(itemButton);
            }
            {
                var itemButton = Instantiate(_itemButtonPrefab, _startingItemParent);
                SetTransform((RectTransform)itemButton.transform, ref startingItemSumHeight);
                itemButton.Initialize(this);
                itemButton.SetItem(startingItemItemController, null);
                startingItemItemButtons.Add(itemButton);
            }
            _scrollRect.content.SetInsetAndSizeFromParentEdge(
                RectTransform.Edge.Top, 0, _firstParent.rect.height + Mathf.Max(intrinsicSumHeight, startingItemSumHeight));

            _exitButton.SetItem(ChoicesModelsMenuItemController.Instance, ExitModelsMenuChoice.Instance);
            MenuController.Show(_canvasGroup, true);
        }

        private static void SetTransform(RectTransform itemTransform, ref float sumHeight)
        {
            itemTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, sumHeight, itemTransform.rect.height);
            sumHeight += itemTransform.rect.height;
        }

        public override float GetPosition()
        {
            return 0f;
        }

        public override void SetPosition(float position)
        {
        }

        private class ExitChoice : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "<";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                throw new System.NotImplementedException();
            }
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

        private class AppearanceChoice : IModelsMenu, IModelsMenuChoice
        {
            public CharacterCreationMenuView parent;
            private static readonly OptionItemController optionItemController = new OptionItemController();

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return null;
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                //var openArg = new RogueMethodArgument(other: (AppearanceBuilder)model);
                //root.OpenMenu(this, self, user, openArg, arg);
            }

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(optionItemController, parent.database.IntrinsicOptions, root, self, user, arg);
                scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
            }

            private class OptionItemController : IModelsMenuItemController
            {
                public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return ((IIntrinsicOption)model).Name;
                }

                public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    var builder = (IntrinsicBuilder)arg.Other;
                    builder.Option = (IIntrinsicOption)model;
                    root.Back();
                }
            }
        }

        private class IntrinsicItemController : IModelsMenu, IModelsMenuItemController
        {
            public CharacterCreationMenuView parent;
            private static readonly OptionItemController optionItemController = new OptionItemController();

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(optionItemController, parent.database.IntrinsicOptions, root, self, user, arg);
                scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
            }

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
                    //parent.
                }
                else
                {
                    var openArg = new RogueMethodArgument(other: (IntrinsicBuilder)model);
                    root.OpenMenu(this, self, user, openArg, arg);
                }
            }

            private class OptionItemController : IModelsMenuItemController
            {
                public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return ((IIntrinsicOption)model).Name;
                }

                public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    var builder = (IntrinsicBuilder)arg.Other;
                    builder.Option = (IIntrinsicOption)model;
                    root.Back();
                }
            }
        }

        private class StartingItemItemController : IModelsMenu, IModelsMenuItemController
        {
            public CharacterCreationMenuView parent;
            private readonly List<object> models = new List<object>();
            private static readonly OptionItemController optionItemController = new OptionItemController();

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                models.Clear();
                var spaceObjs = self.Space.Objs;
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    var obj = spaceObjs[i];
                    if (obj.Main.InfoSet is CharacterCreationInfoSet infoSet &&
                        infoSet.Data is IStartingItemOption option)
                    {
                        models.Add(option);
                    }
                }

                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(optionItemController, models, root, self, user, arg);
                scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
            }

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
                    //parent.
                }
                else
                {
                    var openArg = new RogueMethodArgument(other: (StartingItemBuilder)model);
                    root.OpenMenu(this, self, user, openArg, arg);
                }
            }

            private class OptionItemController : IModelsMenuItemController
            {
                public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return ((IStartingItemOption)model).Name;
                }

                public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    var builder = ((StartingItemBuilder)arg.Other);
                    builder.Option = (IStartingItemOption)model;
                    root.Back();
                }
            }
        }
    }
}
