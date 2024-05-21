using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// メニュー UI
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _floorTitleText = null;
        [SerializeField] private CanvasGroup _floorTitleGroup = null;
        [SerializeField] private Image _touchMask = null;

        [Space]

        [SerializeField] private MessageController _messageController = null;
        [SerializeField] private CaptionWindow _captionWindow = null;
        [SerializeField] private ModelsMenuView _thumbnailMenu = null;
        [SerializeField] private ModelsMenuView _commandMenu = null;
        [SerializeField] private ModelsMenuView _leftAnchorMenu = null;
        [SerializeField] private ModelsMenuView _rightAnchorMenu = null;
        [SerializeField] private FloorMenuView _floorMenu = null;
        [SerializeField] private LoadingMenuView _loadingMenu = null;
        [SerializeField] private SummaryMenuView _summaryMenu = null;
        [SerializeField] private DetailsMenuView _detailsMenu = null;
        [SerializeField] private OptionsMenuView _optionsMenu = null;
        [SerializeField] private CharacterCreationMenuView _characterCreationMenu = null;
        [SerializeField] private TextEditorMenuView _textEditorMenu = null;
        [SerializeField] private PaintMenuView _paintMenu = null;
        [SerializeField] private ModelsMenuView _talkChoicesMenu = null;
        [SerializeField] private StatsWindow _statsWindow = null;

        [SerializeField] private ScrollModelsMenuView _scrollMenu = null;

        private MainMenu mainMenu;
        private LongDownMenu longDownMenu;
        private ObjsMenu objsMenu;

        private StandardMenuRoot menuManager;

        internal ModelsMenuEventManager EventManager => menuManager.EventManager;

        public bool IsDone => menuManager.IsDone;

        /// <summary>
        /// メッセージがスクロール中 or メニュー操作中は待機
        /// </summary>
        public bool Wait => _messageController.IsScrollingNow || menuManager.Any || EventManager.Wait;

        public bool TalkingWait => _messageController.IsTalkingNow || menuManager.Any || EventManager.Wait;

        internal void Initialize(
            SoundController soundController, RogueSpriteRendererPool rendererPool, bool touchMaskIsEnabled = true)
        {
            var objCommandMenu = new ObjCommandMenu();
            var putInCommandMenu = new PutIntoChestCommandMenu();
            var takeOutCommandMenu = new TakeOutFromChestCommandMenu();

            objsMenu = new ObjsMenu(objCommandMenu, putInCommandMenu, takeOutCommandMenu);
            var skillsMenu = new SkillsMenu();
            var partyMemberMenu = new PartyMemberMenu(objsMenu, objCommandMenu, skillsMenu);
            var partyMenu = new PartyMenu(partyMemberMenu);
            mainMenu = new MainMenu(objsMenu, skillsMenu, partyMenu);
            longDownMenu = new LongDownMenu(objsMenu, objCommandMenu);

            _touchMask.raycastTarget = false;
            _scrollMenu.Initialize();
            _summaryMenu.Initialize();
            _optionsMenu.Initialize();
            _characterCreationMenu.Initialize(rendererPool);
            _textEditorMenu.Initialize();
            _paintMenu.Initialize();
            _loadingMenu.Initialize();
            var scrollSensitivity = 64f;
            SetScrollSensitivity(scrollSensitivity);

            var table = new Dictionary<IKeyword, ModelsMenuView>();
            table.Add(DeviceKw.MenuCaption, _captionWindow);
            table.Add(DeviceKw.MenuThumbnail, _thumbnailMenu);
            table.Add(DeviceKw.MenuScroll, _scrollMenu);
            table.Add(DeviceKw.MenuCommand, _commandMenu);
            table.Add(DeviceKw.MenuLeftAnchor, _leftAnchorMenu);
            table.Add(DeviceKw.MenuRightAnchor, _rightAnchorMenu);
            table.Add(DeviceKw.MenuFloor, _floorMenu);
            table.Add(DeviceKw.MenuLoading, _loadingMenu);
            table.Add(DeviceKw.MenuSummary, _summaryMenu);
            table.Add(DeviceKw.MenuDetails, _detailsMenu);
            table.Add(DeviceKw.MenuOptions, _optionsMenu);
            table.Add(DeviceKw.MenuCharacterCreation, _characterCreationMenu);
            table.Add(DeviceKw.MenuTextEditor, _textEditorMenu);
            table.Add(DeviceKw.MenuPaint, _paintMenu);
            table.Add(DeviceKw.MenuLog, _messageController.LogView);
            table.Add(DeviceKw.MenuTalk, _messageController.TalkView);
            table.Add(DeviceKw.MenuTalkChoices, _talkChoicesMenu);
            menuManager = new StandardMenuRoot(_touchMask, _messageController, _statsWindow, soundController, table);

            _touchMask.gameObject.SetActive(touchMaskIsEnabled);
        }

        public void Open(RogueObj menuSubject)
        {
            EventManager.MenuSubject = menuSubject;
        }

        public void GetInfo(out IModelsMenu putIntoChestMenu, out IModelsMenu takeOutFromChestMenu)
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

        public void OpenMainMenu(RogueObj subject)
        {
            EventManager.Add(DeviceKw.EnqueueSE, obj: DeviceKw.Submit);
            menuManager.OpenInitialMenu(mainMenu, subject, null, RogueMethodArgument.Identity);
        }

        public void OpenGroundMenu(RogueObj subject)
        {
            EventManager.Add(DeviceKw.EnqueueSE, obj: DeviceKw.Submit);
            menuManager.OpenInitialMenu(objsMenu.Ground, subject, null, new(targetObj: subject));
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
                menuManager.OpenInitialMenu(longDownMenu, subject, null, new(targetObj: obj));
                return;
            }
            {
                // オブジェクトが見つからないときはタイルを見る
                view.GetTile(position, out _, out var groundTile, out var buildingTile, out _);
                var topTile = buildingTile ?? groundTile;
                menuManager.OpenInitialMenu(longDownMenu, subject, null, new(other: topTile));
            }
        }

        public void OpenInitialMenu(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg, bool enableTouchMask = true)
        {
            menuManager.OpenInitialMenu(menu, self, user, arg, enableTouchMask);
        }

        public void ResetDone() => menuManager.ResetDone();

        private void ShowFloorTitle(string text)
        {
            _floorTitleText.text = text;
            _floorTitleGroup.alpha = 1f;
        }

        private void BehindFloorTitle()
        {
            _floorTitleGroup.alpha = 0f;
        }

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
    }
}
