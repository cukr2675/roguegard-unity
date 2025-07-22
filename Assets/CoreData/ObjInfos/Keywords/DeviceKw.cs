using System.Collections;
using UnityEngine;

namespace Roguegard
{
    public class DeviceKw : ScriptableLoader
    {
        private static DeviceKw instance;

        [SerializeField] private KeywordAsset _afterLoad;
        public static IKeyword AfterLoad => instance._afterLoad;

        [SerializeField] private KeywordAsset _appendText;
        public static IKeyword AppendText => instance._appendText;

        [SerializeField] private KeywordAsset _autoSave;
        public static IKeyword AutoSave => instance._autoSave;

        [SerializeField] private KeywordAsset _cancel;
        public static IKeyword Cancel => instance._cancel;

        [SerializeField] private KeywordAsset _changePlayer;
        public static IKeyword ChangePlayer => instance._changePlayer;

        [SerializeField] private KeywordAsset _enqueueInteger;
        public static IKeyword EnqueueInteger => instance._enqueueInteger;

        [SerializeField] private KeywordAsset _enqueueMenu;
        public static IKeyword EnqueueMenu => instance._enqueueMenu;

        [SerializeField] private KeywordAsset _enqueueNumber;
        public static IKeyword EnqueueNumber => instance._enqueueNumber;

        [SerializeField] private KeywordAsset _enqueueSE;
        public static IKeyword EnqueueSE => instance._enqueueSE;

        [SerializeField] private KeywordAsset _enqueueSEAndWait;
        public static IKeyword EnqueueSEAndWait => instance._enqueueSEAndWait;

        [SerializeField] private KeywordAsset _enqueueViewDequeueState;
        public static IKeyword EnqueueViewDequeueState => instance._enqueueViewDequeueState;

        [SerializeField] private KeywordAsset _enqueueWaitSeconds;
        public static IKeyword EnqueueWaitSeconds => instance._enqueueWaitSeconds;

        [SerializeField] private KeywordAsset _enqueueWork;
        public static IKeyword EnqueueWork => instance._enqueueWork;

        [SerializeField] private KeywordAsset _gameClear;
        public static IKeyword GameClear => instance._gameClear;

        [SerializeField] private KeywordAsset _gameOver;
        public static IKeyword GameOver => instance._gameOver;

        [SerializeField] private KeywordAsset _getDateTimeUtc;
        public static IKeyword GetDateTimeUtc => instance._getDateTimeUtc;

        [SerializeField] private KeywordAsset _horizontalRule;
        public static IKeyword HorizontalRule => instance._horizontalRule;

        [SerializeField] private KeywordAsset _insertHideCharacterWork;
        public static IKeyword InsertHideCharacterWork => instance._insertHideCharacterWork;

        [SerializeField] private KeywordAsset _loadGame;
        public static IKeyword LoadGame => instance._loadGame;

        [SerializeField] private KeywordAsset _menuCaption;
        public static IKeyword MenuCaption => instance._menuCaption;

        [SerializeField] private KeywordAsset _menuCharacterCreation;
        public static IKeyword MenuCharacterCreation => instance._menuCharacterCreation;

        [SerializeField] private KeywordAsset _menuCommand;
        public static IKeyword MenuCommand => instance._menuCommand;

        [SerializeField] private KeywordAsset _menuDetails;
        public static IKeyword MenuDetails => instance._menuDetails;

        [SerializeField] private KeywordAsset _menuFloor;
        public static IKeyword MenuFloor => instance._menuFloor;

        [SerializeField] private KeywordAsset _menuLeftAnchor;
        public static IKeyword MenuLeftAnchor => instance._menuLeftAnchor;

        [SerializeField] private KeywordAsset _menuLoading;
        public static IKeyword MenuLoading => instance._menuLoading;

        [SerializeField] private KeywordAsset _menuLog;
        public static IKeyword MenuLog => instance._menuLog;

        [SerializeField] private KeywordAsset _menuOptions;
        public static IKeyword MenuOptions => instance._menuOptions;

        [SerializeField] private KeywordAsset _menuPaint;
        public static IKeyword MenuPaint => instance._menuPaint;

        [SerializeField] private KeywordAsset _menuRightAnchor;
        public static IKeyword MenuRightAnchor => instance._menuRightAnchor;

        [SerializeField] private KeywordAsset _menuScroll;
        public static IKeyword MenuScroll => instance._menuScroll;

        [SerializeField] private KeywordAsset _menuSummary;
        public static IKeyword MenuSummary => instance._menuSummary;

        [SerializeField] private KeywordAsset _menuTalk;
        public static IKeyword MenuTalk => instance._menuTalk;

        [SerializeField] private KeywordAsset _menuTalkSelect;
        public static IKeyword MenuTalkSelect => instance._menuTalkSelect;

        [SerializeField] private KeywordAsset _menuTextEditor;
        public static IKeyword MenuTextEditor => instance._menuTextEditor;

        [SerializeField] private KeywordAsset _menuThumbnail;
        public static IKeyword MenuThumbnail => instance._menuThumbnail;

        [SerializeField] private KeywordAsset _saveGame;
        public static IKeyword SaveGame => instance._saveGame;

        [SerializeField] private KeywordAsset _startAutoPlay;
        public static IKeyword StartAutoPlay => instance._startAutoPlay;

        [SerializeField] private KeywordAsset _startPlaytest;
        public static IKeyword StartPlaytest => instance._startPlaytest;

        [SerializeField] private KeywordAsset _startTalk;
        public static IKeyword StartTalk => instance._startTalk;

        [SerializeField] private KeywordAsset _submit;
        public static IKeyword Submit => instance._submit;

        [SerializeField] private KeywordAsset _waitEndOfTalk;
        public static IKeyword WaitEndOfTalk => instance._waitEndOfTalk;

        [SerializeField] private KeywordAsset _waitForInput;
        public static IKeyword WaitForInput => instance._waitForInput;

        public override IEnumerator LoadAsync()
        {
            instance = this;
            yield break;
        }

        public override void TestLoad()
        {
#if UNITY_EDITOR
            instance = this;
#else
            throw new RogueException("This method is Editor Only.");
#endif
        }
    }
}
