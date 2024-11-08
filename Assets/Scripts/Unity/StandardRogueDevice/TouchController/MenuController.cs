using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// メニュー UI
    /// </summary>
    public class MenuController : MMgr
    {
        [SerializeField] private StatsSubView _stats = null;
        [SerializeField] private SummarySubView _summary = null;
        [SerializeField] private TextEditorSubView _textEditor = null;
        [SerializeField] private CharacterCreationSubView _characterCreation = null;
        [SerializeField] private PaintSubView _paint = null;

        public StatsSubView Stats => _stats;

        [Header("Title Only")]
        [SerializeField] private GridSubView _titleMenu = null;
        public static string TitleMenuName => "TitleMenu";

        private MainMenu mainMenu;
        private LongDownMenu longDownMenu;
        private ObjsMenu objsMenu;

        private SoundController soundController;
        internal ListMenuEventManager EventManager { get; private set; }

        /// <summary>
        /// メッセージがアニメーション中 or メニュー操作中は待機
        /// </summary>
        public bool Wait =>
            StandardSubViewTable.MessageBox.MessageBox.IsInProgress || StandardSubViewTable.SpeechBox.MessageBox.IsInProgress ||
            ShowsMenuScreen || EventManager.Wait;

        public bool TalkingWait =>
            StandardSubViewTable.SpeechBox.MessageBox.IsInProgress ||
            ShowsMenuScreen || EventManager.Wait;

        internal void Initialize(
            SoundController soundController, RogueSpriteRendererPool rendererPool, bool touchMaskIsEnabled = true)
        {
            BackOption = SelectOption.Create<MMgr, MArg>("<", (manager, arg) => manager.Back(), "Cancel");

            Initialize();
            var objCommandMenu = new ObjCommandMenu();
            var putInCommandMenu = new PutIntoChestCommandMenu();
            var takeOutCommandMenu = new TakeOutFromChestCommandMenu();

            objsMenu = new ObjsMenu(objCommandMenu, putInCommandMenu, takeOutCommandMenu);
            var skillsMenu = new SkillsMenu();
            var partyMemberMenu = new PartyMemberMenu(objsMenu, objCommandMenu, skillsMenu);
            var partyMenu = new PartyMenu(partyMemberMenu);
            mainMenu = new MainMenu(objsMenu, skillsMenu, partyMenu);
            longDownMenu = new LongDownMenu(objsMenu, objCommandMenu);

            _summary.Initialize();
            _characterCreation.Initialize(rendererPool);
            if (_titleMenu != null) { _titleMenu.Initialize(); }
            var scrollSensitivity = 64f;
            SetScrollSensitivity(scrollSensitivity);

            this.soundController = soundController;
            EventManager = new ListMenuEventManager(new MessageController(StandardSubViewTable), soundController);
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
            var panels = GetComponentsInChildren<TwoLayerPanel>();
            foreach (var panel in panels)
            {
                panel.Background.sprite = sprite;
                panel.Background.color = backgroundColor;
                panel.Foreground.sprite = spriteB;
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
            if (subViewName == RoguegardSubViews.Summary) return _summary;
            if (subViewName == RoguegardSubViews.TextEditor) return _textEditor;
            if (subViewName == RoguegardSubViews.CharacterCreation) return _characterCreation;
            if (subViewName == RoguegardSubViews.Paint) return _paint;
            if (subViewName == TitleMenuName) return _titleMenu;
            return base.GetSubView(subViewName);
        }

        public override void HideAll(bool back = false)
        {
            base.HideAll(back);
            _stats.Hide(back);
            _summary.Hide(back);
            _characterCreation.Hide(back);
            _textEditor.Hide(back);
            _paint.Hide(back);
            if (_titleMenu != null) { _titleMenu.Hide(back); }
        }

        public override string Localize(string text)
        {
            return StandardRogueDeviceUtility.Localize(text);
        }

        public override void PushMenuScreen(
            MenuScreen<MMgr, MArg> menuScreen,
            RogueObj self = null, RogueObj user = null,
            RogueObj targetObj = null,
            int count = default,
            Vector2 vector = default,
            EffectableValue value = null,
            RogueObj tool = null,
            object other = null)
        {
            var arg = new RogueMethodArgument(targetObj, count, vector, value, tool, other);
            PushMenuScreen(menuScreen, new MArg.Builder(self, user, arg).ReadOnly);
        }

        public void PushInitialMenuScreen(
            MenuScreen<MMgr, MArg> menuScreen,
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
            PushInitialMenuScreen(menuScreen, new MArg.Builder(self, user, arg).ReadOnly, enableTouchMask);
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

        public void Play(string value, Object sender)
        {
            if (value == "Submit")
            {
                AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
            else if (value == "Cancel")
            {
                AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
            }
            else if (value == "Sort")
            {
                AddObject(DeviceKw.EnqueueSE, StdKw.Sort);
            }
            else if (value == "PickUp")
            {
                AddObject(DeviceKw.EnqueueSE, MainInfoKw.PickUp);
            }
            else if (value == "StartSpeech")
            {
                soundController.PlayLoop(DeviceKw.StartTalk);
            }
            else if (value == "EndSpeech")
            {
                soundController.SetLastLoop(DeviceKw.StartTalk);
            }
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
    }
}
