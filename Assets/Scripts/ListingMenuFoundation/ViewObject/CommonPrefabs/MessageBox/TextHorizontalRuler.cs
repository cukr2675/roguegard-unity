using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    internal class TextHorizontalRuler
    {
        private readonly RectTransform rulesContent;
        private readonly float lineHeight;

        /// <summary>
        /// 一行ごとの罫線
        /// </summary>
        private readonly CanvasGroup[] rules;

        private readonly List<bool> rulesIsEnabled;

        public TextHorizontalRuler(CanvasGroup horizontalRulePrefab, RectTransform rulesContent, int maxLineCount, float lineHeight, float marginTop)
        {
            this.rulesContent = rulesContent;
            this.lineHeight = lineHeight;

            // 罫線 UI の生成
            rules = new CanvasGroup[maxLineCount];
            rulesIsEnabled = new List<bool>();
            for (int i = 0; i < maxLineCount; i++)
            {
                var y = -lineHeight * (i + 1) - marginTop;
                var rule = Object.Instantiate(horizontalRulePrefab, rulesContent);
                rule.alpha = 0f;
                var ruleTransform = (RectTransform)rule.transform;
                ruleTransform.anchorMin = new Vector2(0f, 1f);
                ruleTransform.anchorMax = new Vector2(1f, 1f);
                ruleTransform.sizeDelta = new Vector2(0f, ruleTransform.sizeDelta.y);
                ruleTransform.anchoredPosition = new Vector2(0f, y);
                rules[i] = rule;
                rulesIsEnabled.Add(false);
            }

            // 初期化
            ClearLines();
        }

        public void AddLine(int lineNumber)
        {
            // テキストに 1 行ずつ対応する罫線表示フラグを生成する
            while (rulesIsEnabled.Count <= lineNumber)
            {
                rulesIsEnabled.Add(false);
            }

            rulesIsEnabled[lineNumber] = true;
        }

        /// <summary>
        /// このインスタンス内の行情報を削除する
        /// </summary>
        public void RemoveLinesRange(int index, int count)
        {
            count = Mathf.Min(rulesIsEnabled.Count - index, count);
            rulesIsEnabled.RemoveRange(index, count);
        }

        /// <summary>
        /// このインスタンス内の行情報をすべて削除し、表示もクリアする
        /// </summary>
        public void ClearLines()
        {
            rulesContent.localPosition = Vector3.zero;
            for (int i = 0; i < rules.Length; i++)
            {
                rules[i].alpha = 0f;
            }
            rulesIsEnabled.Clear();
        }

        /// <summary>
        /// スクロール移動するテキスト表示領域に合わせて罫線の表示位置を切り替えて使いまわす。
        /// これにより最低限の罫線スプライト数で表現できる
        /// </summary>
        public void UpdateLines(float scrollPosition)
        {
            // テキスト表示領域に合う罫線のインデックスを取得する
            var ruleIndexOffset = Mathf.FloorToInt(scrollPosition / lineHeight);

            // テキスト表示領域に合う最初の罫線を適切な位置に移動する
            rulesContent.localPosition = new Vector3(0f, -lineHeight * ruleIndexOffset);

            // 罫線の表示を切り替える
            for (int i = 0; i < rules.Length; i++)
            {
                var rule = rules[i];
                var index = i + ruleIndexOffset;
                if (index < 0) continue;

                rule.alpha = index < rulesIsEnabled.Count && rulesIsEnabled[index] ? 1f : 0f;
            }
        }
    }
}
