using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using UnityEngine.UI;
using TMPro;
using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// メニュー UI
    /// </summary>
    public class MenuController : MonoBehaviour, IModelsMenuRoot
    {
        [SerializeField] private TMP_Text _floorTitleText = null;
        [SerializeField] private CanvasGroup _floorTitleGroup = null;
        [SerializeField] private Image _touchMask = null;

        [Space]

        [SerializeField] private MessageController _messageController = null;
        [SerializeField] private CaptionWindow _captionWindow = null;
        [SerializeField] private ModelsMenuView _thumbnailMenu = null;
        [SerializeField] private ModelsMenuView _commandMenu = null;
        [SerializeField] private ModelsMenuView _floorMenu = null;
        [SerializeField] private SummaryMenuView _summaryMenu = null;
        [SerializeField] private DetailsMenuView _detailsMenu = null;
        [SerializeField] private OptionsMenuView _optionsMenu = null;
        [SerializeField] private CharacterCreationMenuView _characterCreationMenu = null;
        [SerializeField] private ModelsMenuView _talkChoicesMenu = null;
        [SerializeField] private StatsWindow _statsWindow = null;

        [SerializeField] private ScrollModelsMenuView _scrollMenu = null;

        private SoundController soundController;
        private WaitTimer waitTimer;

        private Dictionary<IKeyword, ModelsMenuView> table;

        private MainMenu mainMenu;
        private LongDownMenu longDownMenu;
        private ObjsMenu objsMenu;
        private SelectFileMenu selectFileMenu;

        private RogueObj player;

        private Stack<StackItem> stack;

        private bool currentMenuIsDialog;

        public bool IsDone { get; private set; }

        /// <summary>
        /// メッセージがスクロール中 or メニュー操作中は待機
        /// </summary>
        public bool Wait => _messageController.IsScrollingNow || stack.Count >= 1 || soundController.Wait || waitTimer.Wait;

        public bool TalkingWait => _messageController.IsTalkingNow || stack.Count >= 1 || soundController.Wait || waitTimer.Wait;

        public StatsWindow Stats => _statsWindow;

        internal void Initialize(SoundController soundController, RogueSpriteRendererPool rendererPool)
        {
            this.soundController = soundController;
            waitTimer = new WaitTimer();

            var objCommandMenu = new ObjCommandMenu();
            var putInCommandMenu = new PutIntoChestCommandMenu();
            var takeOutCommandMenu = new TakeOutFromChestCommandMenu();

            objsMenu = new ObjsMenu(_captionWindow, objCommandMenu, putInCommandMenu, takeOutCommandMenu);
            var skillsMenu = new SkillsMenu(_captionWindow);
            var partyMemberMenu = new PartyMemberMenu(_captionWindow, objsMenu, objCommandMenu, skillsMenu);
            var partyMenu = new PartyMenu(_captionWindow, partyMemberMenu);
            mainMenu = new MainMenu(_captionWindow, objsMenu, skillsMenu, partyMenu);
            longDownMenu = new LongDownMenu(objsMenu, objCommandMenu);
            selectFileMenu = new SelectFileMenu(_scrollMenu);

            stack = new Stack<StackItem>();

            _touchMask.raycastTarget = false;

            _scrollMenu.Initialize();
            _summaryMenu.Initialize();
            _detailsMenu.Initialize();
            _optionsMenu.Initialize();
            _characterCreationMenu.Initialize(rendererPool);
            var scrollSensitivity = 64f;
            SetScrollSensitivity(scrollSensitivity);

            table = new Dictionary<IKeyword, ModelsMenuView>();
            table.Add(DeviceKw.MenuThumbnail, _thumbnailMenu);
            table.Add(DeviceKw.MenuScroll, _scrollMenu);
            table.Add(DeviceKw.MenuCommand, _commandMenu);
            table.Add(DeviceKw.MenuFloor, _floorMenu);
            table.Add(DeviceKw.MenuSummary, _summaryMenu);
            table.Add(DeviceKw.MenuDetails, _detailsMenu);
            table.Add(DeviceKw.MenuOptions, _optionsMenu);
            table.Add(DeviceKw.MenuCharacterCreation, _characterCreationMenu);
            table.Add(DeviceKw.MenuLog, _messageController.LogView);
            table.Add(DeviceKw.MenuTalk, _messageController.TalkView);
            table.Add(DeviceKw.MenuTalkChoices, _talkChoicesMenu);
        }

        public void Open(RogueObj player)
        {
            this.player = player;
            waitTimer.Reset();
        }

        public void GetInfo(out IModelsMenu openChestMenu)
        {
            openChestMenu = objsMenu.OpenChest;
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

        public void CloseMessage()
        {
            _messageController.ShowMessage(false);
        }

        public void OpenMainMenu()
        {
            // メニューを一から開いたときスタックをリセットする。
            stack.Clear();

            OpenMenu(mainMenu, player, null, RogueMethodArgument.Identity, RogueMethodArgument.Identity);
            _touchMask.raycastTarget = true;

            Add(DeviceKw.EnqueueSE, AddType.Object, obj: DeviceKw.Submit);
        }

        public void OpenGroundMenu()
        {
            // メニューを一から開いたときスタックをリセットする。
            stack.Clear();

            var arg = new RogueMethodArgument(targetObj: player, count: 1);
            OpenMenu(objsMenu.Ground, player, null, arg, RogueMethodArgument.Identity);
            _touchMask.raycastTarget = true;

            Add(DeviceKw.EnqueueSE, AddType.Object, obj: DeviceKw.Submit);
        }

        public void OpenLongDownMenu(Vector2Int position)
        {
            // メニューを一から開いたときスタックをリセットする。
            stack.Clear();

            _touchMask.raycastTarget = true;

            var view = player.Get<ViewInfo>();
            for (int i = 0; i < view.VisibleObjCount; i++)
            {
                var obj = view.GetVisibleObj(i);
                if (obj == null || obj.Position != position) continue;

                // オブジェクトを長押ししたとき
                var arg = new RogueMethodArgument(targetObj: obj);
                OpenMenu(longDownMenu, player, null, arg, RogueMethodArgument.Identity);
                Add(DeviceKw.EnqueueSE, AddType.Object, obj: DeviceKw.Submit);
                return;
            }
            {
                // オブジェクトが見つからないときはタイルを見る
                view.GetTile(position, out _, out var tile, out _);
                var arg = new RogueMethodArgument(other: tile);
                OpenMenu(longDownMenu, player, null, arg, RogueMethodArgument.Identity);
                Add(DeviceKw.EnqueueSE, AddType.Object, obj: DeviceKw.Submit);
            }
        }

        public void OpenSelectFile(SelectFileMenu.SelectCallback selectCallback, SelectFileMenu.AddCallback addCallback = null)
        {
            selectFileMenu.SetCallback(selectCallback, addCallback);
            OpenMenu(selectFileMenu, null, null, RogueMethodArgument.Identity);
        }

        public IModelsMenuView Get(IKeyword keyword)
        {
            return table.TryGetValue(keyword, out var menuView) ? menuView : null;
        }

        public void OpenMenu(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg, in RogueMethodArgument backArg)
        {
            _messageController.ShowMessage(false);

            HideAll();
            _statsWindow.Show(false);
            menu.OpenMenu(this, self, user, arg);

            if (stack.TryPeek(out var peek))
            {
                peek.Arg = backArg;
            }
            currentMenuIsDialog = false;

            var stackItem = new StackItem();
            stackItem.Set(menu, self, user, arg);
            stack.Push(stackItem);
        }

        public void OpenMenu(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            _messageController.ShowMessage(false);

            HideAll();
            _statsWindow.Show(false);
            menu.OpenMenu(this, self, user, arg);

            stack.Clear();
            currentMenuIsDialog = false;

            var stackItem = new StackItem();
            stackItem.Set(menu, self, user, arg);
            stack.Push(stackItem);
        }

        public void OpenMenuAsDialog(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg, in RogueMethodArgument backArg)
        {
            _messageController.ShowMessage(false);

            menu?.OpenMenu(this, self, user, arg);
            if (!currentMenuIsDialog)
            {
                if (stack.TryPeek(out var peek))
                {
                    peek.Arg = backArg;
                }
                currentMenuIsDialog = true;
            }
        }

        public void Done()
        {
            stack.Clear();
            HideAll();
            _statsWindow.Show(false);
            IsDone = true;
            currentMenuIsDialog = false;
            _touchMask.raycastTarget = false;
        }

        public void Back()
        {
            HideAll();
            _statsWindow.Show(false);
            if (currentMenuIsDialog)
            {
                currentMenuIsDialog = false;
            }
            else
            {
                stack.Pop();
            }
            if (stack.TryPeek(out var stackItem))
            {
                stackItem.Menu.OpenMenu(this, stackItem.Self, stackItem.User, stackItem.Arg);
            }
            else
            {
                UnityEngine.Debug.LogWarning("?");
                Done();
            }
        }

        public void ResetDone()
        {
            IsDone = false;
        }

        public void StartTalk()
        {
            _messageController.StartTalk();
        }

        public void WaitEndOfTalk()
        {
            _messageController.WaitEndOfTalk();
        }

        public void Append(RogueObj player, object obj, StackTrace stackTrace)
        {
            _messageController.Append(player, obj, stackTrace);
        }

        public void AppendInteger(int integer)
        {
            _messageController.AppendInteger(integer);
        }

        public void AppendNumber(float number)
        {
            _messageController.AppendNumber(number);
        }

        public void ClearText()
        {
            _messageController.ClearText();
        }

        public void UpdateUI(int deltaTime)
        {
            _messageController.UpdateUI(soundController, deltaTime);
            waitTimer.UpdateTimer(deltaTime);
        }

        public void ShowFloorTitle(string text)
        {
            _floorTitleText.text = text;
            _floorTitleGroup.alpha = 1f;
        }

        public void BehindFloorTitle()
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

        private void HideAll()
        {
            foreach (var item in table)
            {
                Show(item.Value.CanvasGroup, false);
            }
            _captionWindow.Show(false);
        }

        public void Play(IKeyword keyword, bool wait)
        {
            soundController.Play(keyword, wait);
        }

        public void StartWait(float seconds)
        {
            waitTimer.Start(seconds);
        }

        private void Add(IKeyword keyword, AddType type, int integer = 0, float number = 0f, object obj = null)
        {
            if (keyword == null) throw new System.ArgumentNullException(nameof(keyword));

            if (keyword == DeviceKw.AppendText)
            {
                if (type == AddType.Integer) { AppendInteger(integer); }
                else if (type == AddType.Float) { AppendNumber(number); }
                else if (obj is string text) { Append(player, StandardRogueDeviceUtility.Localize(text), null); }
                else { Append(player, obj, null); }
                return;
            }
            if (keyword == DeviceKw.StartTalk)
            {
                StartTalk();
                return;
            }
            if (keyword == DeviceKw.WaitEndOfTalk)
            {
                WaitEndOfTalk();
                return;
            }
            if (keyword == DeviceKw.EnqueueSE && type == AddType.Object)
            {
                soundController.Play((IKeyword)obj, keyword == DeviceKw.EnqueueSEAndWait);
                return;
            }

            UnityEngine.Debug.LogError($"{keyword.Name} に対応するキーワードが見つかりません。（obj: {obj}）");
        }

        void IModelsMenuRoot.AddInt(IKeyword keyword, int integer) => Add(keyword, AddType.Integer, integer: integer);
        void IModelsMenuRoot.AddFloat(IKeyword keyword, float number) => Add(keyword, AddType.Float, number: number);
        void IModelsMenuRoot.AddObject(IKeyword keyword, object obj) => Add(keyword, AddType.Object, obj: obj);

        void IModelsMenuRoot.AddWork(IKeyword keyword, in RogueCharacterWork work)
        {
            throw new System.NotSupportedException();
        }

        private enum AddType
        {
            Integer,
            Float,
            Object
        }

        private class StackItem
        {
            public IModelsMenu Menu { get; private set; }

            public RogueObj Self { get; private set; }

            public RogueObj User { get; private set; }

            public RogueMethodArgument Arg { get; set; }

            public void Set(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                Menu = menu;
                Self = self;
                User = user;
                Arg = arg;
            }
        }
    }
}
