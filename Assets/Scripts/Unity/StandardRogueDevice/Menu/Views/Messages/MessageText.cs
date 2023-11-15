using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using UnityEngine.UI;
using TMPro;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class MessageText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textPrefab = null;
        [SerializeField] private CanvasGroup _horizontalRulePrefab = null;
        [SerializeField] private RectTransform _content = null;
        [SerializeField] private RectTransform _rulesContent = null;
        [SerializeField] private int _maxLineCount = 1;
        [Space]
        [SerializeField] private bool _isTalk = false;
        [SerializeField] private Image _horizontalArrow = null;
        [SerializeField] private Image _verticalArrow = null;
        [SerializeField] private int _arrowBlinkInterval = 30;

        private TMP_Text text;

        private StringBuilder stringBuilder;

        private bool dirty;

        /// <summary>
        /// 一行ごとの罫線
        /// </summary>
        private CanvasGroup[] rules;

        private List<bool> rulesIsEnabled;

        private float lineHeight;

        private int flushCount;

        private int arrowBlinkTimer;

        private float TextOffset
        {
            get => _content.localPosition.y;
            set => _content.localPosition = new Vector3(0f, value);
        }

        public bool IsScrollingNow => TextOffset < CalculateTargetTextOffset();

        public bool IsTalkingNow => text.maxVisibleCharacters < text.textInfo.characterCount || _horizontalArrow.enabled || _verticalArrow.enabled;

        public bool IsTypewritingNow { get; private set; }

        public bool WaitsInput => _horizontalArrow.enabled || _verticalArrow.enabled;

        /// <summary>
        /// 1 秒あたり 4 行送る
        /// </summary>
        private const float linePerSecond = 4f;

        private const float framePerSecond = 60f;

        private const char breakCharacter = '\n';

        private void Awake()
        {
            // テキスト UI の生成
            text = Instantiate(_textPrefab, _content);
            var textTransform = (RectTransform)text.transform;
            textTransform.SetSiblingIndex(1); // ログウィンドウで戻るボタン向け余白オブジェクトより上に表示させる
            textTransform.anchorMin = Vector2.zero;
            textTransform.anchorMax = Vector2.one;
            textTransform.sizeDelta = Vector2.zero;

            // サイズ取得用テキストを設定してフォントの縦幅を取得する
            stringBuilder = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                stringBuilder.Append("_\n");
            }
            text.SetText(stringBuilder);
            text.ForceMeshUpdate(true);
            lineHeight = text.renderedHeight / 100f * (1f + text.lineSpacing / 100f);
            if (lineHeight <= 0f) throw new RogueException();

            // 罫線 UI の生成
            rules = new CanvasGroup[_maxLineCount];
            rulesIsEnabled = new List<bool>();
            for (int i = 0; i < _maxLineCount; i++)
            {
                var y = -lineHeight * (i + 1) + text.renderedHeight * text.lineSpacing / 200f;
                var rule = Instantiate(_horizontalRulePrefab, _rulesContent);
                rule.alpha = 0f;
                var ruleTransform = (RectTransform)rule.transform;
                ruleTransform.anchorMin = Vector2.up;
                ruleTransform.anchorMax = Vector2.one;
                ruleTransform.sizeDelta = new Vector2(0f, ruleTransform.sizeDelta.y);
                ruleTransform.anchoredPosition = new Vector2(0f, y);
                rules[i] = rule;
                rulesIsEnabled.Add(false);
            }

            // 初期化
            Clear();
        }

        public void Append(string text)
        {
            stringBuilder.Append(text);
            dirty = true;
        }

        public void Append(int integer)
        {
            stringBuilder.Append(integer);
            dirty = true;
        }

        public void Append(float number)
        {
            stringBuilder.Append(number);
            dirty = true;
        }

        private void Append(char text)
        {
            stringBuilder.Append(text);
            dirty = true;
        }

        private void Remove(int startIndex, int length)
        {
            stringBuilder.Remove(startIndex, length);
            dirty = true;
        }

        public void AppendHorizontalRule()
        {
            // 一文字も設定されていないときは何もしない
            if (stringBuilder.Length == 0) return;

            if (stringBuilder[stringBuilder.Length - 1] != breakCharacter)
            {
                // 罫線は必ず改行コードの直後に挿入することとする
                // 改行コードで終了していなければ改行を追加する
                Append(breakCharacter);
            }

            // テキストに 1 行ずつ対応する罫線表示フラグを生成する
            MeshUpdate();
            var lineCount = text.textInfo.lineCount;
            while (rulesIsEnabled.Count < lineCount)
            {
                rulesIsEnabled.Add(false);
            }

            // 現在のテキストでの最終行に罫線を追加する
            var ruleIndex = rulesIsEnabled.Count - 1;
            rulesIsEnabled[ruleIndex] = true;

            // テキストの最下端でなければ即座に表示させる
            if (ruleIndex < rules.Length - 1)
            {
                var rule = rules[ruleIndex];
                rule.alpha = 1f;
            }
        }

        public void Clear()
        {
            if (_isTalk)
            {
                text.maxVisibleCharacters = 0;
                flushCount = 0;
            }

            stringBuilder.Clear();
            TextOffset = 0f;
            _rulesContent.localPosition = Vector3.zero;
            for (int i = 0; i < rules.Length; i++)
            {
                rules[i].alpha = 0f;
            }
            rulesIsEnabled.Clear();
            dirty = true;
            MeshUpdate();
        }

        private void MeshUpdate()
        {
            // 毎フレーム呼び出すと重いので更新時のみ呼び出す
            if (!dirty) return;

            if (_isTalk)
            {
                while (stringBuilder.Length >= 1)
                {
                    var lastChar = stringBuilder[stringBuilder.Length - 1];
                    if (lastChar != '\n' && lastChar != '\t' && lastChar != '\v') break;

                    // 改行や文字送りで終了していたら取り除く
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                }
            }

            text.SetText(stringBuilder);
            text.ForceMeshUpdate(true);
            dirty = false;
        }

        private float CalculateTargetTextOffset()
        {
            // スクロール目標地点はテキストの最下端が露出する位置
            MeshUpdate();
            if (_isTalk) return lineHeight * _maxLineCount * flushCount;

            var targetPosition = lineHeight * (text.textInfo.lineCount - _maxLineCount);
            return targetPosition;
        }

        /// <summary>
        /// 文字送り入力
        /// </summary>
        public void Input()
        {
            if (_horizontalArrow.enabled)
            {
                _horizontalArrow.enabled = false;
                text.maxVisibleCharacters++;
            }
            if (_verticalArrow.enabled)
            {
                if (text.maxVisibleCharacters < text.textInfo.characterCount)
                {
                    // テキストに続きがあったらスクロールして続行
                    text.maxVisibleCharacters++;
                    flushCount++;
                    var color = _verticalArrow.color;
                    color.a = 0f;
                    _verticalArrow.color = color;
                }
                else
                {
                    // なかったら終了
                    _verticalArrow.enabled = false;
                }
            }
        }

        /// <summary>
        /// テキストスクロール
        /// </summary>
        public void UpdateUI(int deltaTime)
        {
            if (_isTalk)
            {
                if (_horizontalArrow.enabled)
                {
                    arrowBlinkTimer = (arrowBlinkTimer + deltaTime) % (_arrowBlinkInterval * 2);
                    var color = _horizontalArrow.color;
                    color.a = arrowBlinkTimer >= _arrowBlinkInterval ? 1f : 0f;
                    _horizontalArrow.color = color;
                    return;
                }
                if (_verticalArrow.enabled && !IsScrollingNow)
                {
                    arrowBlinkTimer = (arrowBlinkTimer + deltaTime) % (_arrowBlinkInterval * 2);
                    var color = _verticalArrow.color;
                    color.a = arrowBlinkTimer >= _arrowBlinkInterval ? 1f : 0f;
                    _verticalArrow.color = color;
                    return;
                }

                if (!IsScrollingNow && text.maxVisibleCharacters < text.textInfo.characterCount)
                {
                    var nextChar = text.textInfo.characterInfo[text.maxVisibleCharacters].character;
                    if (nextChar == '\t')
                    {
                        var x = text.textInfo.characterInfo[text.maxVisibleCharacters].topRight.x;
                        var y = -(text.textInfo.characterInfo[text.maxVisibleCharacters].lineNumber - 1) * lineHeight;
                        _horizontalArrow.rectTransform.localPosition = new Vector3(x, y);
                        _horizontalArrow.enabled = true;
                        IsTypewritingNow = false;
                        return;
                    }
                    if (nextChar == '\v')
                    {
                        _verticalArrow.enabled = true;
                        IsTypewritingNow = false;
                        return;
                    }
                    if (deltaTime >= 2) throw new RogueException();

                    text.maxVisibleCharacters += deltaTime;
                    IsTypewritingNow = true;
                    if (text.maxVisibleCharacters >= text.textInfo.characterCount)
                    {
                        // コンテキストをすべて表示し終えたら最後の文字送りを表示
                        _verticalArrow.enabled = true;
                        IsTypewritingNow = false;
                        return;
                    }

                    var overLineIndex = _maxLineCount * (1 + flushCount);
                    if (text.textInfo.lineCount >= overLineIndex + 1)
                    {
                        var firstOverVisibleCharacterIndex = text.textInfo.lineInfo[overLineIndex].firstVisibleCharacterIndex;
                        if (text.maxVisibleCharacters >= firstOverVisibleCharacterIndex)
                        {
                            // はみ出したらはみ出した分をいったん消してスクロール
                            text.maxVisibleCharacters = firstOverVisibleCharacterIndex - 1;
                            flushCount++;
                            IsTypewritingNow = false;
                        }
                    }
                    return;
                }
            }

            if (IsScrollingNow)
            {
                // 現在のテキスト行数が指定された行数を上回る場合、スクロールさせる。
                var maxScrollWidth = lineHeight * linePerSecond * deltaTime / framePerSecond;
                TextOffset += maxScrollWidth;

                // スクロール終了処理
                if (!IsScrollingNow) { EndScroll(); }

                var ruleIndexOffset = Mathf.FloorToInt(TextOffset / lineHeight);
                _rulesContent.localPosition = new Vector3(0f, -lineHeight * ruleIndexOffset);
                for (int i = 0; i < rules.Length; i++)
                {
                    var rule = rules[i];
                    var index = i + ruleIndexOffset;
                    rule.alpha = index < rulesIsEnabled.Count && rulesIsEnabled[index] ? 1f : 0f;
                }
            }
        }

        /// <summary>
        /// はみ出したテキストを削除する。
        /// </summary>
        public void EndScroll()
        {
            if (_isTalk)
            {
                if (_verticalArrow.enabled)
                {
                    _verticalArrow.enabled = false;
                    var verticalTabIndex = stringBuilder.ToString().IndexOf('\v');
                    stringBuilder.Remove(0, verticalTabIndex + 1);
                    dirty = true;
                    text.maxVisibleCharacters = 0;
                    flushCount = 0;
                    TextOffset = CalculateTargetTextOffset();
                }
                return;
            }

            MeshUpdate();
            var textInfo = text.textInfo;
            var beforeLineCount = textInfo.lineCount;
            if (beforeLineCount <= _maxLineCount) return; // はみ出すだけのテキスト量が無ければ何もしない

            // はみ出しているテキストのうち改行以前を削除する
            // 改行以降を削除すると自動改行に影響が出て、はみ出していないテキストが変わってしまう可能性がある

            // 最後から _maxLineCount 番目の改行コードの位置を取得する
            var characterIndex = text.text.Length;
            for (int i = 0; i < _maxLineCount + 1; i++)
            {
                if (characterIndex <= 0) break;

                characterIndex = text.text.LastIndexOf(breakCharacter, characterIndex - 1);
            }

            // 削除できる部分が見つからない場合、何もしない
            if (characterIndex <= 0) return;

            // テキストを削除する（改行コードも含めて削除）
            Remove(0, characterIndex + 1);

            // 罫線もテキストと同じように削除
            MeshUpdate();
            var removedLineCount = beforeLineCount - textInfo.lineCount;
            for (int i = 0; i < removedLineCount; i++)
            {
                if (rulesIsEnabled.Count >= 1) { rulesIsEnabled.RemoveAt(0); }
            }

            // スクロール終了
            TextOffset = CalculateTargetTextOffset();

        }

        /// <summary>
        /// 罫線を文字の一つとする方法
        /// </summary>
        private void SetUpHorizontalRule()
        {
            var faceInfo = text.font.faceInfo;
            faceInfo.lineHeight = 66; // 罫線の有無で行がずれない値に調整する

            const int hrCharacter = -1; // 罫線にする文字情報を指定
            var character = text.font.characterTable[hrCharacter];
            var metrics = character.glyph.metrics;
            metrics.width = 1e+10f; // 罫線の横幅はこれと文字数で調整する
            metrics.height = 1e+9f;
            character.glyph.metrics = metrics;
            character.scale = 1e-8f; // glyphTable[].scale とは別物なので注意
        }
    }
}
