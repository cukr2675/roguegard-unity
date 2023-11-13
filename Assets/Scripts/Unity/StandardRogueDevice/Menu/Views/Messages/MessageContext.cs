//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using System.Text;
//using TMPro;
//using Roguegard;

//namespace RoguegardUnity
//{
//    public class MessageContext : MonoBehaviour
//    {
//        [SerializeField] private TMP_Text _textPrefab = null;
//        [SerializeField] private RectTransform _content = null;
//        [SerializeField] private int _maxLineCount = 1;
//        [SerializeField] private int _textSpeed = 2;

//        private TMP_Text text;

//        private StringBuilder stringBuilder;

//        private int textTime;

//        private bool dirty;

//        /// <summary>
//        /// 一行ごとの罫線
//        /// </summary>
//        private CanvasGroup[] rules;

//        private List<bool> rulesIsEnabled;

//        private float lineHeight;

//        private float TextOffset
//        {
//            get => _content.localPosition.y;
//            set => _content.localPosition = new Vector3(0f, value);
//        }

//        public bool IsScrollingNow => TextOffset < CalculateTargetTextOffset();

//        /// <summary>
//        /// 1 秒あたり 4 行送る
//        /// </summary>
//        private const float linePerSecond = 4f;

//        private const float framePerSecond = 60f;

//        private const char breakCharacter = '\n';

//        private void Awake()
//        {
//            // テキスト UI の生成
//            text = Instantiate(_textPrefab, _content);
//            var textTransform = (RectTransform)text.transform;
//            textTransform.SetSiblingIndex(1); // ログウィンドウで戻るボタン向け余白オブジェクトより上に表示させる
//            textTransform.anchorMin = Vector2.zero;
//            textTransform.anchorMax = Vector2.one;
//            textTransform.sizeDelta = Vector2.zero;

//            // サイズ取得用テキストを設定してフォントの縦幅を取得する
//            stringBuilder = new StringBuilder();
//            stringBuilder.Append("_");
//            text.SetText(stringBuilder);
//            text.ForceMeshUpdate(true);
//            lineHeight = text.renderedHeight;
//            if (lineHeight <= 0f) throw new RogueException();

//            // 初期化
//            Clear();
//        }

//        private void MeshUpdate()
//        {
//            // 毎フレーム呼び出すと重いので更新時のみ呼び出す
//            if (!dirty) return;

//            text.SetText(stringBuilder);
//            text.ForceMeshUpdate(true);
//            dirty = false;
//        }

//        private float CalculateTargetTextOffset()
//        {
//            // スクロール目標地点はテキストの最下端が露出する位置
//            MeshUpdate();
//            var targetPosition = lineHeight * (text.textInfo.lineCount - _maxLineCount);
//            return targetPosition;
//        }

//        /// <summary>
//        /// テキストスクロール
//        /// </summary>
//        public void UpdateUI(int deltaTime)
//        {
//            if (IsScrollingNow)
//            {
//                textTime += deltaTime;
//                if (textTime < _textSpeed) return;

//                textTime = 0;

//                text.text +=

//                // 現在のテキスト行数が指定された行数を上回る場合、スクロールさせる。
//                var maxScrollWidth = lineHeight * linePerSecond * deltaTime / framePerSecond;
//                TextOffset += maxScrollWidth;

//                // スクロール終了処理
//                if (!IsScrollingNow) { EndScroll(); }

//                var ruleIndexOffset = Mathf.FloorToInt(TextOffset / lineHeight);
//                _rulesContent.localPosition = new Vector3(0f, -lineHeight * ruleIndexOffset);
//                for (int i = 0; i < rules.Length; i++)
//                {
//                    var rule = rules[i];
//                    var index = i + ruleIndexOffset;
//                    rule.alpha = index < rulesIsEnabled.Count && rulesIsEnabled[index] ? 1f : 0f;
//                }
//            }
//        }
//    }
//}
