using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using UnityEngine.Events;
using TMPro;

namespace ListingMF
{
    internal class TextTypingEffect
    {
        private readonly TMP_Text text;
        private readonly int maxLineCount;
        private readonly string pageTurnHiddenLinkID;
        private readonly string eofHiddenLinkID;
        private readonly UnityEvent<string> onReachHiddenLink;
        private readonly TextHiddenLinkManager hiddenLinkManager;
        private readonly StringBuilder stringBuilder;

        private bool isDirty;

        public bool IsInProgress => text.maxVisibleCharacters < text.textInfo.characterCount - (text.text.EndsWith(breakCharacter) ? 1 : 0);

        public float LineHeight { get; }

        public int LineCount => text.textInfo.lineCount;
        public int VisibleLineNumber => text.textInfo.characterInfo[text.maxVisibleCharacters].lineNumber;

        private const char breakCharacter = '\n';

        /// <param name="pageTurnHiddenLinkID">テキストが最大行数から下にはみ出たとき発行されるリンクID名</param>
        /// <param name="eofHiddenLinkID">テキストの終端の表示が完了したとき発行されるリンクID</param>
        public TextTypingEffect(
            TMP_Text text, int maxLineCount, string pageTurnHiddenLinkID, string eofHiddenLinkID, UnityEvent<string> onReachHiddenLink)
        {
            if (text.lineSpacing != 0f) { Debug.LogWarning($"{nameof(text.lineSpacing)} != 0 はサポートされていません。"); }

            this.text = text;
            this.maxLineCount = maxLineCount;
            this.pageTurnHiddenLinkID = pageTurnHiddenLinkID;
            this.eofHiddenLinkID = eofHiddenLinkID;
            this.onReachHiddenLink = onReachHiddenLink;
            hiddenLinkManager = new TextHiddenLinkManager();

            // サイズ取得用テキストを設定して一行あたりの幅をサンプリングする
            const int samplingLineCount = 100;
            stringBuilder = new StringBuilder("_\n".Length * samplingLineCount);
            for (int i = 0; i < samplingLineCount; i++)
            {
                stringBuilder.Append("_\n");
            }
            text.SetText(stringBuilder);
            text.ForceMeshUpdate(true);
            LineHeight = text.renderedHeight / samplingLineCount;
            if (LineHeight <= 0f) throw new System.InvalidOperationException();

            // 初期化
            Clear();
        }

        public Vector2 GetCurrentCharacterPosition(int deltaCharacterIndex, float normalizedX)
        {
            if (text.maxVisibleCharacters == 0) return Vector2.zero;

            var info = text.textInfo.characterInfo[text.maxVisibleCharacters - 1 + deltaCharacterIndex];
            var left = (info.topLeft.x + info.bottomLeft.x) / 2f;
            var right = (info.topRight.x + info.bottomRight.x) / 2f;
            var y = -(info.lineNumber - 1) * LineHeight;
            return new Vector2(Mathf.LerpUnclamped(left, right, normalizedX), y);
        }

        public void Append(string text)
        {
            stringBuilder.Append(text);
            isDirty = true;
        }

        public void Append(int integer)
        {
            stringBuilder.Append(integer);
            isDirty = true;
        }

        public void Append(float number)
        {
            stringBuilder.Append(number);
            isDirty = true;
        }

        public void AppendEndBreakCharacter()
        {
            if (stringBuilder[stringBuilder.Length - 1] != breakCharacter)
            {
                stringBuilder.Append(breakCharacter);
                isDirty = true;
            }
        }

        private void Remove(int startIndex, int length)
        {
            stringBuilder.Remove(startIndex, length);
            isDirty = true;
        }

        public void Clear()
        {
            stringBuilder.Clear();
            isDirty = true;
            MeshUpdate();
        }

        public void MeshUpdate()
        {
            // 毎フレーム呼び出すと重いので更新時のみ呼び出す
            if (!isDirty) return;

            text.SetText(stringBuilder);
            text.ForceMeshUpdate(true);
            hiddenLinkManager.UpdateLinks(text);
            isDirty = false;
        }

        public void SeekToStartOfText()
        {
            text.maxVisibleCharacters = 0;
        }

        public void SeekToEndOfText()
        {
            text.maxVisibleCharacters = text.textInfo.characterCount;
        }

        /// <summary>
        /// 1行ごとに表示
        /// </summary>
        public void UpdateUI(int linePosition)
        {
            if (!IsInProgress) return;

            // 1フレーム内で1行すべて表示する
            var overLineIndex = maxLineCount + linePosition;
            int maxVisibleCharacters;
            if (overLineIndex < text.textInfo.lineCount)
            {
                maxVisibleCharacters = text.textInfo.lineInfo[overLineIndex].lastVisibleCharacterIndex + 1;
            }
            else
            {
                maxVisibleCharacters = text.textInfo.characterCount;
                if (text.text.EndsWith(breakCharacter)) { maxVisibleCharacters--; }
            }

            // リンクを検知
            while (hiddenLinkManager.ForwardDetect(maxVisibleCharacters, out var hiddenLinkID, out var linkCharacterIndex))
            {
                text.maxVisibleCharacters = linkCharacterIndex;
                onReachHiddenLink.Invoke(hiddenLinkID);
            }

            // 次の行の終わりまで表示する
            text.maxVisibleCharacters = maxVisibleCharacters;

            // 設定された文字列の終端を検知
            if (!IsInProgress)
            {
                // コンテキストをすべて表示し終えたら終端リンクIDを発行
                onReachHiddenLink.Invoke(eofHiddenLinkID);
                return;
            }

            onReachHiddenLink.Invoke(pageTurnHiddenLinkID);
        }

        /// <summary>
        /// タイピングエフェクト再生
        /// </summary>
        public void UpdateUI(int deltaVisibleCharacters, int linePosition)
        {
            if (!IsInProgress) return;

            // リンクを検知
            if (hiddenLinkManager.ForwardDetect(text.maxVisibleCharacters + deltaVisibleCharacters, out var hiddenLinkID, out var linkCharacterIndex))
            {
                text.maxVisibleCharacters = linkCharacterIndex;
                onReachHiddenLink.Invoke(hiddenLinkID);
                return;
            }

            // タイピングエフェクト
            text.maxVisibleCharacters += deltaVisibleCharacters;

            // 設定された文字列の終端を検知
            if (!IsInProgress && linePosition >= text.textInfo.lineCount - maxLineCount)
            {
                // コンテキストをすべて表示し終えたら終端リンクIDを発行
                onReachHiddenLink.Invoke(eofHiddenLinkID);
                return;
            }

            // テキストが下端からはみ出したことを検知
            var overLineIndex = maxLineCount + linePosition;
            if (overLineIndex < text.textInfo.lineCount)
            {
                var firstOverVisibleCharacterIndex = text.textInfo.lineInfo[overLineIndex].firstVisibleCharacterIndex;
                if (text.maxVisibleCharacters >= firstOverVisibleCharacterIndex + 1)
                {
                    // はみ出したらはみ出した分をいったん消して改ページを発行
                    text.maxVisibleCharacters = firstOverVisibleCharacterIndex;
                    onReachHiddenLink.Invoke(pageTurnHiddenLinkID);
                }
            }
        }

        /// <summary>
        /// 手前側にはみ出したテキストを削除し、削除した行数を取得する
        /// </summary>
        public int TrimBeforeVisibleLine()
        {
            // はみ出しているテキストのうち改行以前を削除する
            // 改行以降を削除すると自動改行に影響が出て、はみ出していないテキストが変わってしまうことがある

            var stringIndex = text.textInfo.characterInfo[text.maxVisibleCharacters - 1].index;

            // テキストが改行で終わっている場合は1行だけ無視する
            if (stringIndex >= 1 && text.text[stringIndex - 1] == breakCharacter) { stringIndex--; }

            // 最後から maxLineCount 番目の改行コードの位置を取得する
            for (int i = 0; i < maxLineCount - 1; i++)
            {
                if (stringIndex <= 0) break;

                stringIndex = text.text.LastIndexOf(breakCharacter, stringIndex - 1);
            }

            // 削除できる部分が見つからない場合、何もしない
            if (stringIndex <= 0) return 0;

            // テキストを削除する（改行コードも含めて削除）
            MeshUpdate(); // テキストの行数を更新する
            var beforeLineCount = text.textInfo.lineCount; // 削除前に取得
            Remove(0, stringIndex + 1);
            MeshUpdate(); // テキストの行数を更新する
            var removedLineCount = beforeLineCount - text.textInfo.lineCount; // 削除した行数を取得

            return removedLineCount;
        }

        /// <summary>
        /// 指定のリンクIDの最初の出現位置から手前を削除し、一部でも削除した行数を取得する
        /// </summary>
        public int TrimBeforeFirstLinkID(string hiddenLinkID)
        {
            if (!hiddenLinkManager.TryGetFirstHiddenLinkCharacterIndex(text.maxVisibleCharacters, hiddenLinkID, out var linkCharacterIndex)) return 0;

            var linkCharacterInfo = text.textInfo.characterInfo[linkCharacterIndex];
            var linkStringIndex = linkCharacterInfo.index;

            // テキストを削除する
            MeshUpdate(); // テキストの行数を更新する
            var beforeLineCount = text.textInfo.lineCount; // 削除前に取得
            Remove(0, linkStringIndex);
            MeshUpdate(); // テキストの行数を更新する
            var removedLineCount = beforeLineCount - text.textInfo.lineCount; // 削除した行数を取得

            // リンクが始端または終端でない位置にある場合、一部を削除した行として加算
            var linkCharacterLineInfo = text.textInfo.lineInfo[linkCharacterInfo.lineNumber];
            if (linkCharacterLineInfo.firstVisibleCharacterIndex != linkCharacterIndex ||
                linkCharacterLineInfo.lastVisibleCharacterIndex != linkCharacterIndex)
            {
                removedLineCount++;
            }

            return removedLineCount;
        }

        /// <summary>
        /// 現在の表示範囲のテキストを削除する
        /// </summary>
        public void TrimBeforeVisibleCharacter()
        {
            var visibleStringIndex = text.textInfo.characterInfo[text.maxVisibleCharacters].index;
            Remove(0, visibleStringIndex);
            MeshUpdate();
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
