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
            for (int i = 0; i < text.textInfo.linkCount; i++)
            {
                var linkInfo = text.textInfo.linkInfo[i];
                if (linkInfo.linkTextLength != 0) continue; // ���������[���łȂ��i'Hidden' �łȂ��j�����N�^�O�͖���
                if (linkInfo.linkTextfirstCharacterIndex < text.maxVisibleCharacters) continue; // �\���ς݃e�L�X�g���̃����N�^�O�͖���

                var hiddenLinkID = text.text.Substring(linkInfo.linkIdFirstCharacterIndex, linkInfo.linkIdLength);
                var item = new Item(hiddenLinkID, linkInfo.linkTextfirstCharacterIndex);
                items.Add(item);
            }
            nextItemIndex = 0;
        }

        public bool ForwardDetect(int endCharacterIndex, out string hiddenLinkID, out int linkCharacterIndex)
        {
            if (nextItemIndex >= items.Count)
            {
                hiddenLinkID = null;
                linkCharacterIndex = default;
                return false;
            }

            var nextItem = items[nextItemIndex];
            if (endCharacterIndex < nextItem.LinkTextFirstCharacterIndex)
            {
                hiddenLinkID = null;
                linkCharacterIndex = default;
                return false;
            }

            nextItemIndex++;

            hiddenLinkID = nextItem.HiddenLinkID;
            linkCharacterIndex = nextItem.LinkTextFirstCharacterIndex;
            return true;
        }

        public bool TryGetFirstHiddenLinkCharacterIndex(int endCharacterIndex, string hiddenLinkID, out int linkCharacterIndex)
        {
            foreach (var item in items)
            {
                if (item.LinkTextFirstCharacterIndex > endCharacterIndex) continue;
                if (item.HiddenLinkID != hiddenLinkID) continue;

                linkCharacterIndex = item.LinkTextFirstCharacterIndex;
                return true;
            }
            linkCharacterIndex = default;
            return false;
        }

        private class Item
        {
            public string HiddenLinkID { get; }
            public int LinkTextFirstCharacterIndex { get; }

            public Item(string hiddenLinkID, int linkTextFirstCharacterIndex)
            {
                HiddenLinkID = hiddenLinkID;
                LinkTextFirstCharacterIndex = linkTextFirstCharacterIndex;
            }
        }
    }
}
