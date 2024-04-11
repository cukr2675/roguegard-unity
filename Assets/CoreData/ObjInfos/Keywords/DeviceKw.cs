using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class DeviceKw : ScriptableLoader
    {
        private static DeviceKw instance;

        [SerializeField] private KeywordData _afterLoad;
        public static IKeyword AfterLoad => instance._afterLoad;

        [SerializeField] private KeywordData _appendText;
        public static IKeyword AppendText => instance._appendText;

        [SerializeField] private KeywordData _autoSave;
        public static IKeyword AutoSave => instance._autoSave;

        [SerializeField] private KeywordData _cancel;
        public static IKeyword Cancel => instance._cancel;

        [SerializeField] private KeywordData _changePlayer;
        public static IKeyword ChangePlayer => instance._changePlayer;

        [SerializeField] private KeywordData _endTalk;
        public static IKeyword EndTalk => instance._endTalk;

        [SerializeField] private KeywordData _enqueueInteger;
        public static IKeyword EnqueueInteger => instance._enqueueInteger;

        [SerializeField] private KeywordData _enqueueMenu;
        public static IKeyword EnqueueMenu => instance._enqueueMenu;

        [SerializeField] private KeywordData _enqueueNumber;
        public static IKeyword EnqueueNumber => instance._enqueueNumber;

        [SerializeField] private KeywordData _enqueueSE;
        public static IKeyword EnqueueSE => instance._enqueueSE;

        [SerializeField] private KeywordData _enqueueSEAndWait;
        public static IKeyword EnqueueSEAndWait => instance._enqueueSEAndWait;

        [SerializeField] private KeywordData _enqueueViewDequeueState;
        public static IKeyword EnqueueViewDequeueState => instance._enqueueViewDequeueState;

        [SerializeField] private KeywordData _enqueueWaitSeconds;
        public static IKeyword EnqueueWaitSeconds => instance._enqueueWaitSeconds;

        [SerializeField] private KeywordData _enqueueWork;
        public static IKeyword EnqueueWork => instance._enqueueWork;

        [SerializeField] private KeywordData _gameClear;
        public static IKeyword GameClear => instance._gameClear;

        [SerializeField] private KeywordData _gameOver;
        public static IKeyword GameOver => instance._gameOver;

        [SerializeField] private KeywordData _getDateTimeUtc;
        public static IKeyword GetDateTimeUtc => instance._getDateTimeUtc;

        [SerializeField] private KeywordData _horizontalRule;
        public static IKeyword HorizontalRule => instance._horizontalRule;

        [SerializeField] private KeywordData _insertHideCharacterWork;
        public static IKeyword InsertHideCharacterWork => instance._insertHideCharacterWork;

        [SerializeField] private KeywordData _loadGame;
        public static IKeyword LoadGame => instance._loadGame;

        [SerializeField] private KeywordData _menuCaption;
        public static IKeyword MenuCaption => instance._menuCaption;

        [SerializeField] private KeywordData _menuCharacterCreation;
        public static IKeyword MenuCharacterCreation => instance._menuCharacterCreation;

        [SerializeField] private KeywordData _menuCommand;
        public static IKeyword MenuCommand => instance._menuCommand;

        [SerializeField] private KeywordData _menuDetails;
        public static IKeyword MenuDetails => instance._menuDetails;

        [SerializeField] private KeywordData _menuFloor;
        public static IKeyword MenuFloor => instance._menuFloor;

        [SerializeField] private KeywordData _menuLeftAnchor;
        public static IKeyword MenuLeftAnchor => instance._menuLeftAnchor;

        [SerializeField] private KeywordData _menuLoading;
        public static IKeyword MenuLoading => instance._menuLoading;

        [SerializeField] private KeywordData _menuLog;
        public static IKeyword MenuLog => instance._menuLog;

        [SerializeField] private KeywordData _menuOptions;
        public static IKeyword MenuOptions => instance._menuOptions;

        [SerializeField] private KeywordData _menuPaint;
        public static IKeyword MenuPaint => instance._menuPaint;

        [SerializeField] private KeywordData _menuRightAnchor;
        public static IKeyword MenuRightAnchor => instance._menuRightAnchor;

        [SerializeField] private KeywordData _menuScroll;
        public static IKeyword MenuScroll => instance._menuScroll;

        [SerializeField] private KeywordData _menuSummary;
        public static IKeyword MenuSummary => instance._menuSummary;

        [SerializeField] private KeywordData _menuTalk;
        public static IKeyword MenuTalk => instance._menuTalk;

        [SerializeField] private KeywordData _menuTalkChoices;
        public static IKeyword MenuTalkChoices => instance._menuTalkChoices;

        [SerializeField] private KeywordData _menuTextEditor;
        public static IKeyword MenuTextEditor => instance._menuTextEditor;

        [SerializeField] private KeywordData _menuThumbnail;
        public static IKeyword MenuThumbnail => instance._menuThumbnail;

        [SerializeField] private KeywordData _nqueueSEAndWait;
        public static IKeyword nqueueSEAndWait => instance._nqueueSEAndWait;

        [SerializeField] private KeywordData _saveGame;
        public static IKeyword SaveGame => instance._saveGame;

        [SerializeField] private KeywordData _startAutoPlay;
        public static IKeyword StartAutoPlay => instance._startAutoPlay;

        [SerializeField] private KeywordData _startTalk;
        public static IKeyword StartTalk => instance._startTalk;

        [SerializeField] private KeywordData _submit;
        public static IKeyword Submit => instance._submit;

        [SerializeField] private KeywordData _waitEndOfTalk;
        public static IKeyword WaitEndOfTalk => instance._waitEndOfTalk;

        [SerializeField] private KeywordData _waitForInput;
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
