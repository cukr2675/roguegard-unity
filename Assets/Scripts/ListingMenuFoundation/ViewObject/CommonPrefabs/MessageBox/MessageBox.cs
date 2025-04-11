using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using TMPro;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Message Box")]
    public class MessageBox : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private CanvasGroup _horizontalRulePrefab = null;
        [SerializeField] private RectTransform _content = null;
        [SerializeField] private RectTransform _rulesContent = null;
        [SerializeField, Tooltip("1ページあたりのテキスト表示行数")] private int _maxLineCount = 1;

        [Header("Display")]

        [Tooltip("テキスト表示アニメーション")]
        [SerializeField] private VisibleMode _visibleMode = VisibleMode.Static;

        [Tooltip("VisibleMode == Typing: テキスト表示速度　一秒あたりの文字数")]
        [SerializeField] private float _characterPerSecond = 60f;
        public float CharacterPerSecond { get => _characterPerSecond; set => _characterPerSecond = value; }

        [Tooltip("VisibleMode == Typing: ページ区切りのリンクID")]
        [SerializeField] private string _pageBreakHiddenLinkId = "PageBreak";

        [Tooltip("罫線のリンクID")]
        [SerializeField] private string _horizontalRuleHiddenLinkId = "HorizontalRule";

        [Header("Animation")]

        [Tooltip("0 (スクロール開始位置) ～ 1 (スクロール目標位置)\n*初期値は 1 推奨*")]
        [SerializeField] private float _normalizedLineOffset = 1f;
        public float NormalizedLineOffset
        {
            get => _normalizedLineOffset;
            set => _normalizedLineOffset = value;
        }

        [Tooltip("テキストが下端からはみ出したとき発行されるリンクID")]
        [SerializeField] private string _hiddenLinkIdOnPageOver = "PageBreak";
        public string HiddenLinkIdOnPageOver
        {
            get => _hiddenLinkIdOnPageOver;
            set => _hiddenLinkIdOnPageOver = value;
        }

        [Tooltip("テキストの終端に到達したとき発行されるリンクID")]
        [SerializeField] private string _hiddenLinkIdOnEof = "EOF";
        public string HiddenLinkIdOnEof
        {
            get => _hiddenLinkIdOnEof;
            set => _hiddenLinkIdOnEof = value;
        }

        [Space]

        [Tooltip("テキストを含まない<link>タグまで表示が到達したとき発火するイベント\n（上のプロパティから発行されるものも含む）")]
        [SerializeField]
        private ReachHiddenLinkEvent _onReachHiddenLink = null;
        public ReachHiddenLinkEvent OnReachHiddenLink => _onReachHiddenLink;

        private TextTypingEffect textTypingEffect;
        private TextHorizontalRuler textRuleEffect;
        private Animator animator;

        private float characterCount;
        private int linePosition;
        private float startLineOffset;
        private bool _isScrollingNow;

        private float ScrollPosition
        {
            get => _content.localPosition.y;
            set => _content.localPosition = new Vector3(0f, value);
        }

        public bool IsInProgress => !textTypingEffect.IsEof;
        public bool IsTypingNow => enabled && _characterPerSecond > 0f && !textTypingEffect.IsEof && !_isScrollingNow;
        public bool IsScrollingNow => _isScrollingNow;

        private void Awake()
        {
            textTypingEffect = new TextTypingEffect(_text, _maxLineCount, _hiddenLinkIdOnPageOver, _hiddenLinkIdOnEof, _onReachHiddenLink);
            textRuleEffect = new TextHorizontalRuler(_horizontalRulePrefab, _rulesContent, _maxLineCount, textTypingEffect.LineHeight, _text.margin.y);
            SetTextTransform(_text.transform);
            SetTextTransform(_rulesContent);

            // 初期化
            Clear();

            _onReachHiddenLink.AddListener(hiddenLinkId =>
            {
                if (hiddenLinkId == _horizontalRuleHiddenLinkId) { InsertHorizontalRule(); }
            });

            void SetTextTransform(Transform transform)
            {
                var textTransform = (RectTransform)transform;
                textTransform.anchorMin = new Vector2(0f, 0.5f);
                textTransform.anchorMax = new Vector2(1f, 0.5f);
                textTransform.sizeDelta = new Vector2(0f, textTypingEffect.LineHeight * _maxLineCount);
                textTransform.anchoredPosition = Vector2.zero;
            }
        }

        private void Update()
        {
            if (!_isScrollingNow)
            {
                startLineOffset = CalculateTargetTextOffset();

                if (_visibleMode == VisibleMode.Static)
                {
                    textTypingEffect.UpdateUI(linePosition);
                }
                else
                {
                    characterCount += _characterPerSecond * Time.deltaTime;
                    var deltaVisibleCharacters = Mathf.FloorToInt(characterCount);
                    characterCount -= deltaVisibleCharacters;

                    textTypingEffect.UpdateUI(deltaVisibleCharacters, linePosition);
                }
            }
        }

        private void LateUpdate()
        {
            ScrollPosition = Mathf.LerpUnclamped(startLineOffset, CalculateTargetTextOffset(), _normalizedLineOffset);
            textRuleEffect.UpdateLines(ScrollPosition);
        }

        public Vector2 GetCurrentCharacterPosition(int deltaCharacterIndex, float normalizedX)
            => textTypingEffect.GetCurrentCharacterPosition(deltaCharacterIndex, normalizedX);

        public void Append(string text)
        {
            textTypingEffect.Append(text);
        }

        public void Append(int integer)
        {
            textTypingEffect.Append(integer);
        }

        public void Append(float number)
        {
            textTypingEffect.Append(number);
        }

        private void InsertHorizontalRule()
        {
            // 一文字も設定されていないときは何もしない
            if (textTypingEffect.LineCount == 0) return;

            // テキストに 1 行ずつ対応する罫線表示フラグを生成する
            textTypingEffect.MeshUpdate();
            textRuleEffect.AddLine(textTypingEffect.VisibleLineNumber);
        }

        public void Clear()
        {
            textTypingEffect.SeekToStartOfText();
            textTypingEffect.Clear();
            textRuleEffect.ClearLines();
            linePosition = 0;
            ScrollPosition = 0f;
        }

        private float CalculateTargetTextOffset()
        {
            // スクロール目標地点はテキストの最下端が露出する位置
            textTypingEffect.MeshUpdate();
            return textTypingEffect.LineHeight * linePosition;
        }

        public void StartScrollAndAddLinePosition(int lines)
        {
            linePosition += lines;
            _isScrollingNow = true;
        }

        /// <summary>
        /// <see cref="VisibleMode.Static"/> の場合は += 1 、 <see cref="VisibleMode.Typing"/> の場合は += <see cref="_maxLineCount"/>
        /// </summary>
        public void StartScrollAndAddLinePositionAuto(int multiplier)
        {
            if (_visibleMode == VisibleMode.Static) { linePosition += 1 * multiplier; }
            else { linePosition += _maxLineCount * multiplier; }
            _isScrollingNow = true;
        }

        public void EndScroll()
        {
            _isScrollingNow = false;
        }

        /// <summary>
        /// 行スクロールアニメーション完了時にアニメーターから呼び出すメソッド
        /// </summary>
        public void EndScrollAndTrimBeforeAuto()
        {
            _isScrollingNow = false;

            // 表示に必要なくなったテキストを削除する
            int removedLineCount;
            if (_visibleMode == VisibleMode.Static)
            {
                // タイピングエフェクトが無効の場合

                // 全テキストが表示完了していなければ何もしない
                if (!textTypingEffect.IsEof) return;

                // 表示に必要なくなったテキストを行単位で削除する
                removedLineCount = textTypingEffect.TrimBeforeVisibleLine();
                if (removedLineCount == 0) return;

                textTypingEffect.SeekToEndOfText();
            }
            else
            {
                // タイピングエフェクトが有効の場合

                // リンクIDで削除する
                removedLineCount = textTypingEffect.TrimBeforeFirstLinkId(_pageBreakHiddenLinkId);
                if (removedLineCount == 0) return;

                // リンクIDの代わりに現在表示位置で削除することもできるが、色変更タグなども削除されてしまう
                //textTypingEffect.TrimBeforeVisibleCharacter();

                textTypingEffect.SeekToStartOfText();
            }

            // テキストを削除した行数ぶん罫線も削除
            textRuleEffect.RemoveLinesRange(0, removedLineCount);

            // テキストを削除したぶん行表示位置を移動
            linePosition = 0;
        }

        public void InvokeReachHiddenLink(string hiddenLinkId)
        {
            _onReachHiddenLink.Invoke(hiddenLinkId);
        }

        /// <summary>
        /// 指定の名前のトリガーが存在するとき、そのトリガーを有効にする。
        /// アニメーターと関係のない HiddenLink を使用したいとき使う
        /// </summary>
        public void SetAnimatorTriggerIfExists(string name)
        {
            if (animator == null && !TryGetComponent(out animator)) return;

            foreach (var parameters in animator.parameters)
            {
                if (parameters.name == name)
                {
                    animator.SetTrigger(name);
                    break;
                }
            }
        }

        private enum VisibleMode
        {
            Static,
            Typing
        }

        [System.Serializable] public class ReachHiddenLinkEvent : UnityEvent<string> { }
    }
}
