using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// メニュー UI
    /// </summary>
    public class MenuController : RogueMenuManager
    {
        //[SerializeField] private TMP_Text _floorTitleText = null;
        //[SerializeField] private CanvasGroup _floorTitleGroup = null;
        //[SerializeField] private Image _touchMask = null;

        [Space]

        //[SerializeField] private MessageController _messageController = null;
        //[SerializeField] private CaptionWindow _captionWindow = null;
        //[SerializeField] private ElementsView _thumbnailMenu = null;
        //[SerializeField] private ElementsView _commandMenu = null;
        //[SerializeField] private ElementsView _leftAnchorMenu = null;
        //[SerializeField] private ElementsView _rightAnchorMenu = null;
        //[SerializeField] private FloorMenuView _floorMenu = null;
        //[SerializeField] private LoadingMenuView _loadingMenu = null;
        //[SerializeField] private SummaryMenuView _summaryMenu = null;
        //[SerializeField] private DetailsMenuView _detailsMenu = null;
        //[SerializeField] private OptionsMenuView _optionsMenu = null;
        [SerializeField] private CharacterCreationMenuView _characterCreationMenu = null;
        public CharacterCreationMenuView CharacterCreation => _characterCreationMenu;
        [SerializeField] private TextEditorMenuView _textEditorMenu = null;
        public TextEditorMenuView TextEditor => _textEditorMenu;
        [SerializeField] private PaintMenuView _paintMenu = null;
        //[SerializeField] private ElementsView _talkSelectMenu = null;
        [SerializeField] private StatsWindow _statsWindow = null;
        public StatsWindow Stats => _statsWindow;

        [Header("Title Only")]
        [SerializeField] private GridSubView _titleMenu = null;
        public static string TitleMenuName => "TitleMenu";

        //[SerializeField] private ScrollMenuView _scrollMenu = null;

        private MainMenu mainMenu;
        private LongDownMenu longDownMenu;
        private ObjsMenu objsMenu;

        //private StandardMenuManager menuManager;

        //internal ListMenuEventManager EventManager => menuManager.EventManager;

        internal ListMenuEventManager EventManager { get; private set; }

        /// <summary>
        /// メッセージがアニメーション中 or メニュー操作中は待機
        /// </summary>
        public bool Wait =>
            StandardSubViewTable.MessageBox.MessageBox.IsInProgress || StandardSubViewTable.SpeechBox.MessageBox.IsInProgress ||
            StackCount >= 1 || EventManager.Wait;

        public bool TalkingWait =>
            StandardSubViewTable.MessageBox.MessageBox.IsInProgress || StandardSubViewTable.SpeechBox.MessageBox.IsInProgress ||
            StackCount >= 1 || EventManager.Wait;

        public override string TextEditorValue => _textEditorMenu.Text;

        internal void Initialize(
            SoundController soundController, RogueSpriteRendererPool rendererPool, bool touchMaskIsEnabled = true)
        {
            base.Initialize();
            var objCommandMenu = new ObjCommandMenu();
            var putInCommandMenu = new PutIntoChestCommandMenu();
            var takeOutCommandMenu = new TakeOutFromChestCommandMenu();

            objsMenu = new ObjsMenu(objCommandMenu, putInCommandMenu, takeOutCommandMenu);
            var skillsMenu = new SkillsMenu();
            var partyMemberMenu = new PartyMemberMenu(objsMenu, objCommandMenu, skillsMenu);
            var partyMenu = new PartyMenu(partyMemberMenu);
            mainMenu = new MainMenu(objsMenu, skillsMenu, partyMenu);
            longDownMenu = new LongDownMenu(objsMenu, objCommandMenu);

            //_touchMask.raycastTarget = false;
            //_scrollMenu.Initialize();
            //_summaryMenu.Initialize();
            //_optionsMenu.Initialize();
            _characterCreationMenu.Initialize(rendererPool);
            _textEditorMenu.Initialize();
            _paintMenu.Initialize();
            //_loadingMenu.Initialize();
            if (_titleMenu != null) { _titleMenu.Initialize(); }
            var scrollSensitivity = 64f;
            SetScrollSensitivity(scrollSensitivity);

            var table = new Dictionary<IKeyword, ElementsView>();
            //table.Add(DeviceKw.MenuCaption, _captionWindow);
            //table.Add(DeviceKw.MenuThumbnail, _thumbnailMenu);
            //table.Add(DeviceKw.MenuScroll, _scrollMenu);
            //table.Add(DeviceKw.MenuCommand, _commandMenu);
            //table.Add(DeviceKw.MenuLeftAnchor, _leftAnchorMenu);
            //table.Add(DeviceKw.MenuRightAnchor, _rightAnchorMenu);
            //table.Add(DeviceKw.MenuFloor, _floorMenu);
            //table.Add(DeviceKw.MenuLoading, _loadingMenu);
            //table.Add(DeviceKw.MenuSummary, _summaryMenu);
            //table.Add(DeviceKw.MenuDetails, _detailsMenu);
            //table.Add(DeviceKw.MenuOptions, _optionsMenu);
            //table.Add(DeviceKw.MenuCharacterCreation, _characterCreationMenu);
            table.Add(DeviceKw.MenuTextEditor, _textEditorMenu);
            table.Add(DeviceKw.MenuPaint, _paintMenu);
            //table.Add(DeviceKw.MenuLog, _messageController.LogView);
            //table.Add(DeviceKw.MenuTalk, _messageController.TalkView);
            //table.Add(DeviceKw.MenuTalkSelect, _talkSelectMenu);
            //menuManager = new StandardMenuManager(_touchMask, _messageController, _statsWindow, soundController, table);

            //_touchMask.gameObject.SetActive(touchMaskIsEnabled);

            EventManager = new ListMenuEventManager(GetComponent<MessageController>(), soundController);
        }

        public void Open(RogueObj menuSubject)
        {
            EventManager.MenuSubject = menuSubject;
        }

        public void GetInfo(out RogueMenuScreen putIntoChestMenu, out RogueMenuScreen takeOutFromChestMenu)
        {
            putIntoChestMenu = objsMenu.PutIntoChest;
            takeOutFromChestMenu = objsMenu.TakeOutFromChest;
        }

        public void SetWindowFrame(Sprite sprite, Sprite spriteB, Color backgroundColor)
        {
            var backgrounds = GetComponentsInChildren<MenuWindowBackground>();
            foreach (var background in backgrounds)
            {
                background.ImageA.sprite = sprite;
                background.ImageA.color = backgroundColor;
                background.ImageB.sprite = spriteB;
            }
        }

        private void SetScrollSensitivity(float value)
        {
            var scrollRects = GetComponentsInChildren<ScrollRect>();
            foreach (var scrollRect in scrollRects)
            {
                scrollRect.scrollSensitivity = value;
            }
        }

        public override IElementsSubView GetSubView(string subViewName)
        {
            if (subViewName == TitleMenuName) return _titleMenu;
            return base.GetSubView(subViewName);
        }

        public override void HideAll(bool back = false)
        {
            base.HideAll(back);
            _textEditorMenu.Show(false);
            _statsWindow.Show(false);
            if (_titleMenu != null) { _titleMenu.Hide(back); }
        }

        public override string Localize(string text)
        {
            return StandardRogueDeviceUtility.Localize(text);
        }

        public override void PushMenuScreen(
            MenuScreen<RogueMenuManager, ReadOnlyMenuArg> menuScreen,
            RogueObj self = null, RogueObj user = null,
            RogueObj targetObj = null,
            int count = default,
            Vector2 vector = default,
            EffectableValue value = null,
            RogueObj tool = null,
            object other = null)
        {
            var arg = new RogueMethodArgument(targetObj, count, vector, value, tool, other);
            PushMenuScreen(menuScreen, new MenuArg(self, user, arg).ReadOnly);
        }

        public void PushInitialMenuScreen(
            MenuScreen<RogueMenuManager, ReadOnlyMenuArg> menuScreen,
            RogueObj self = null, RogueObj user = null,
            RogueObj targetObj = null,
            int count = default,
            Vector2 vector = default,
            EffectableValue value = null,
            RogueObj tool = null,
            object other = null,
            bool enableTouchMask = true)
        {
            var arg = new RogueMethodArgument(targetObj, count, vector, value, tool, other);
            PushInitialMenuScreen(menuScreen, new MenuArg(self, user, arg).ReadOnly, enableTouchMask);
        }

        public void OpenMainMenu(RogueObj subject)
        {
            PushInitialMenuScreen(mainMenu, subject);
        }

        public void OpenGroundMenu(RogueObj subject)
        {
            PushInitialMenuScreen(objsMenu.Ground, subject, targetObj: subject);
        }

        public void OpenLongDownMenu(RogueObj subject, Vector2Int position)
        {
            EventManager.Add(DeviceKw.EnqueueSE, obj: DeviceKw.Submit);

            var view = ViewInfo.Get(subject);
            for (int i = 0; i < view.VisibleObjCount; i++)
            {
                var obj = view.GetVisibleObj(i);
                if (obj == null || obj.Position != position) continue;

                // オブジェクトを長押ししたとき
                PushInitialMenuScreen(longDownMenu, subject, targetObj: obj);
                return;
            }
            {
                // オブジェクトが見つからないときはタイルを見る
                view.GetTile(position, out _, out var groundTile, out var buildingTile, out _);
                var topTile = buildingTile ?? groundTile;
                PushInitialMenuScreen(longDownMenu, subject, other: topTile);
            }
        }

        public void CloseMenu()
        {
            Done();
        }

        public override void OpenCharacterCreationView(IListMenuSelectOption exit, ReadOnlyMenuArg arg)
        {
            _characterCreationMenu.OpenView<object>(SelectOptionHandler.Instance, new[] { exit }, this, arg.Self, arg.User, arg.Arg);
        }

        public override void AddInt(IKeyword keyword, int integer) => EventManager.Add(keyword, integer: integer);
        public override void AddFloat(IKeyword keyword, float number) => EventManager.Add(keyword, number: number);
        public override void AddObject(IKeyword keyword, object obj) => EventManager.Add(keyword, obj: obj);

        public static void Show(CanvasGroup canvasGroup, bool show)
        {
            if (show)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            else
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        public override void OpenTextEditor(string text)
        {
            _textEditorMenu.SetText(text);
            _textEditorMenu.Show(true);
        }
    }
}
