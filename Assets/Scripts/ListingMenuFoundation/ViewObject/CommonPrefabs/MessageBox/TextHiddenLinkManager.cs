using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace ListingMF
{
    internal class TextHiddenLinkManager
    {
        private readonly List<Item> items = new();

        private int nextItemIndex;

        public void UpdateLinks(TMP_Text text)
        {
            items.Clear();
            var value = text.text; // WebGL ビルドで text プロパティを複数回参照すると不正な値を取得してしまうためキャッシュ必須
            for (int i = 0; i < text.textInfo.linkCount; i++)
            {
                var linkInfo = text.textInfo.linkInfo[i];
                if (linkInfo.linkTextLength != 0) continue; // 文字数がゼロでない（'Hidden' でない）リンクタグは無視
                if (linkInfo.linkTextfirstCharacterIndex < text.maxVisibleCharacters) continue; // 表示済みテキスト内のリンクタグは無視

                var hiddenLinkId = value.Substring(linkInfo.linkIdFirstCharacterIndex, linkInfo.linkIdLength);

                // <link="PageBreak"></link><color="red">example</color> のようなテキストだと、
                // PageBreak の linkInfo.linkTextfirstCharacterIndex は赤字テキストの 'e' のインデックス (上の例だと38) となってしまう。
                // （Substring(38) だと example</color> となり色が抜けてしまう）
                //
                // 確実に </link> 直後のインデックス (上の例だと25) を取得したいので自前で計算する。
                // （Substring(25) は <color="red">example</color> となり赤字が維持される）
                //
                // <link="exam" attr="add"  ></link> のような最短でない形式はサポートしない
                var endLinkStringIndex = linkInfo.linkIdFirstCharacterIndex + linkInfo.linkIdLength + "\"></link>".Length;

                var item = new Item(hiddenLinkId, linkInfo.linkTextfirstCharacterIndex, endLinkStringIndex);
                items.Add(item);
            }
            nextItemIndex = 0;
        }

        public bool ForwardDetect(int endCharacterIndex, out string hiddenLinkId, out int nextVisibleCharacters)
        {
            if (nextItemIndex >= items.Count)
            {
                hiddenLinkId = null;
                nextVisibleCharacters = default;
                return false;
            }

            var nextItem = items[nextItemIndex];
            if (endCharacterIndex < nextItem.NextVisibleCharacters)
            {
                hiddenLinkId = null;
                nextVisibleCharacters = default;
                return false;
            }

            nextItemIndex++;

            hiddenLinkId = nextItem.HiddenLinkId;
            nextVisibleCharacters = nextItem.NextVisibleCharacters;
            return true;
        }

        public bool TryGetFirstHiddenLinkCharacterIndex(int endCharacterIndex, string hiddenLinkId, out int endLinkStringIndex)
        {
            foreach (var item in items)
            {
                if (item.NextVisibleCharacters > endCharacterIndex) continue;
                if (item.HiddenLinkId != hiddenLinkId) continue;

                endLinkStringIndex = item.EndLinkStringIndex;
                return true;
            }
            endLinkStringIndex = default;
            return false;
        }

        private class Item
        {
            public string HiddenLinkId { get; }
            public int NextVisibleCharacters { get; }
            public int EndLinkStringIndex { get; }

            public Item(string hiddenLinkId, int nextVisibleCharacters, int endLinkStringIndex)
            {
                HiddenLinkId = hiddenLinkId;
                NextVisibleCharacters = nextVisibleCharacters;
                EndLinkStringIndex = endLinkStringIndex;
            }
        }
    }
}
