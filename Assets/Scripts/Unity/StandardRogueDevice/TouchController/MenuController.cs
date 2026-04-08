using Lysionium;
using Lysionium.Views;
using Roguegard;
using Roguegard.Device;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace RoguegardUnity
{
    /// <summary>
    /// メニュー UI
    /// </summary>
    public class MenuController : MMgr
    {
        [SerializeField] private WebOtherAudioPlayHandler _audioPlayHandler = null;
        [SerializeField] private StatsSubview _stats = null;
        [SerializeField] private FaceSubview _face = null;
        public override IListHandlerSubview Face => _face;
        [SerializeField] private SummarySubview _summary = null;
        public override ISummaryElementsSubview Summary => _summary;
        [SerializeField] private TextEditorSubview _textEditor = null;
        public override ITextEditorElementsSubview TextEditor => _textEditor;
        [SerializeField] private CharacterCreationSubview _characterCreation = null;
        public override ICharacterCreationElementsSubview CharacterCreation => _characterCreation;
        [SerializeField] private PaintSubview _paint = null;
        public override IPaintElementsSubview Paint => _paint;
        [SerializeField] private DopesheetSubview _dopesheet = null;
        public override IListHandlerSubview Dopesheet => _dopesheet;

        public StatsSubview Stats => _stats;

        [Header("Title Only")]
        [SerializeField] private GridSubview _titleMenu = null;
        public static string TitleMenuName => "TitleMenu";
        public override IListHandlerSubview TitleMenu => _titleMenu;

        private MainMenu mainMenu;
        private LongDownMenu longDownMenu;
        private ObjsMenu objsMenu;

        public event System.Action OnDone;

        internal RogueListuiEventManager EventManager { get; private set; }

        public bool IsDone { get; private set; }

        /// <summary>
        /// メッセージがアニメーション中 or メニュー操作中は待機
        /// </summary>
        public bool Wait =>
            (PeekScreenOrDefault() != null) || EventManager.Wait || MessageBox.IsInProgress || SpeechBox.IsInProgress;

        public bool TalkingWait =>
            (PeekScreenOrDefault() != null) || EventManager.Wait || SpeechBox.IsInProgress;

        protected override bool HasManagerLock =>
            base.HasManagerLock || _stats.HasManagerLock || _face.HasManagerLock || _summary.HasManagerLock || _textEditor.HasManagerLock ||
            _characterCreation.HasManagerLock || _paint.HasManagerLock || _dopesheet.HasManagerLock || (_titleMenu != null && _titleMenu.HasManagerLock);

        internal void Initialize(RogueSpriteRendererPool rendererPool)
        {
            BackOption = SelectOption.Create<MMgrBase, MArg>("<", (manager, arg) => manager.PopScreen(), "Cancel click:Cancel");

            CommonInit();
            var objCommandMenuScreen = new ObjCommandMenuScreen();
            var putInCommandMenuScreen = new PutIntoContainerCommandMenuScreen();
            var takeOutCommandMenuScreen = new TakeOutOfContainerCommandMenuScreen();

            objsMenu = new ObjsMenu(objCommandMenuScreen, putInCommandMenuScreen, takeOutCommandMenuScreen);
            var skillsMenu = new SkillsMenu();
            var partyMemberMenu = new PartyMemberMenu(objsMenu, objCommandMenuScreen, skillsMenu);
            var partyMenu = new PartyMenu(partyMemberMenu);
            mainMenu = new MainMenu(objsMenu, skillsMenu, partyMenu);
            longDownMenu = new LongDownMenu(objsMenu, objCommandMenuScreen);

            _face.Initialize(rendererPool);
            _summary.Initialize();
            _characterCreation.Initialize(rendererPool);
            _dopesheet.Initialize();
            if (_titleMenu != null) { _titleMenu.CommonInit(); }

            {
                ISubviewStateProvider _ = null;
                MessageBox.SetText("", this, null, ref _);
                LongMessage.SetText("", this, null, ref _);
            }
            EventManager = new RogueListuiEventManager(new MessageController(MessageBox, LongMessage), _audioPlayHandler);
        }

        public void Open(RogueObj menuSubject)
        {
            EventManager.MenuSubject = menuSubject;
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

        protected override void BlockAll()
        {
            base.BlockAll();
            _stats.SetInteractable(false);
            _face.SetInteractable(false);
            _summary.SetInteractable(false);
            _textEditor.SetInteractable(false);
            _characterCreation.SetInteractable(false);
            _paint.SetInteractable(false);
            _dopesheet.SetInteractable(false);
            if (_titleMenu != null) { _titleMenu.SetInteractable(false); }
        }

        public override void HideAll(bool back = false)
        {
            base.HideAll(back);
            _stats.Hide(back);
            _face.Hide(back);
            _summary.Hide(back);
            _textEditor.Hide(back);
            _characterCreation.Hide(back);
            _paint.Hide(back);
            _dopesheet.Hide(back);
            if (_titleMenu != null) { _titleMenu.Hide(back); }
        }

        public override string Localize(string text)
        {
            return base.Localize(StandardRogueDeviceUtility.Localize(text));
        }

        public override void PushScreen(
            IListuiScreen<MMgrBase, MArg> screen,
            RogueObj self = null, RogueObj user = null,
            RogueObj targetObj = null,
            int count = default,
            Vector2 vector = default,
            EffectableValue value = null,
            RogueObj tool = null,
            object other = null)
        {
            var arg = new RogueMethodArgument(targetObj, count, vector, value, tool, other);
            PushScreen(screen, new MArg.Builder(self, user, arg).ReadOnly);
        }

        public void PushInitialScreen(
            IListuiScreen<MMgrBase, MArg> screen,
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
            PushInitialScreen(screen, new MArg.Builder(self, user, arg).ReadOnly, enableTouchMask);
        }

        /// <summary>
        /// メニュー画面をすべて閉じる
        /// </summary>
        public override void Done()
        {
            Clear();
            IsDone = true;
            OnDone?.Invoke();
        }

        public override void ResetDone()
        {
            IsDone = false;
        }

        public void OpenMainMenu(RogueObj subject)
        {
            PushInitialScreen(mainMenu, subject);
        }

        public void OpenGroundMenu(RogueObj subject)
        {
            PushInitialScreen(objsMenu.Ground, subject, targetObj: subject);
        }

        public void OpenLongDownMenu(RogueObj subject, Vector2Int position)
        {
            var view = ViewInfo.Get(subject);
            for (int i = 0; i < view.VisibleObjCount; i++)
            {
                var obj = view.GetVisibleObj(i);
                if (obj == null || obj.Position != position) continue;

                // オブジェクトを長押ししたとき
                PushInitialScreen(longDownMenu, subject, targetObj: obj);
                return;
            }
            {
                // オブジェクトが見つからないときはタイルを見る
                view.GetTile(position, out _, out var groundTile, out var buildingTile, out _);
                var topTile = buildingTile ?? groundTile;
                PushInitialScreen(longDownMenu, subject, other: topTile);
            }
        }

        [SuppressMessage("Style", "IDE0060", Justification = "UnityEvent で使用")]
        public void Play(string value, object sender)
        {
            if (value == "Select")
            {
                EventManager.Add(DeviceKw.EnqueueSE, obj: DeviceKw.Submit);
            }
            else if (value == "SelectOutOfRange")
            {
                EventManager.Add(DeviceKw.EnqueueSE, obj: DeviceKw.Cancel);
            }
            else if (value == "StartSpeech")
            {
                _audioPlayHandler.PlayLoop(DeviceKw.StartTalk.Name);
            }
            else if (value == "EndSpeech")
            {
                _audioPlayHandler.SetLastLoop(DeviceKw.StartTalk.Name);
            }
        }

        public override void AddInt(IKeyword keyword, int integer) => EventManager.Add(keyword, integer: integer);
        public override void AddFloat(IKeyword keyword, float number) => EventManager.Add(keyword, number: number);
        public override void AddObject(IKeyword keyword, object obj) => EventManager.Add(keyword, obj: obj);
    }
}
