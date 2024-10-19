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
        /// ��s���Ƃ̌r��
        /// </summary>
        private readonly CanvasGroup[] rules;

        private readonly List<bool> rulesIsEnabled;

        public TextHorizontalRuler(CanvasGroup horizontalRulePrefab, RectTransform rulesContent, int maxLineCount, float lineHeight, float marginTop)
        {
            this.rulesContent = rulesContent;
            this.lineHeight = lineHeight;

            // �r�� UI �̐���
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

            // ������
            ClearLines();
        }

        public void AddLine(int lineNumber)
        {
            // �e�L�X�g�� 1 �s���Ή�����r���\���t���O�𐶐�����
            while (rulesIsEnabled.Count <= lineNumber)
            {
                rulesIsEnabled.Add(false);
            }

            rulesIsEnabled[lineNumber] = true;
        }

        /// <summary>
        /// ���̃C���X�^���X���̍s�����폜����
        /// </summary>
        public void RemoveLinesRange(int index, int count)
        {
            count = Mathf.Min(rulesIsEnabled.Count - index, count);
            rulesIsEnabled.RemoveRange(index, count);
        }

        /// <summary>
        /// ���̃C���X�^���X���̍s�������ׂč폜���A�\�����N���A����
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
        /// �X�N���[���ړ�����e�L�X�g�\���̈�ɍ��킹�Čr���̕\���ʒu��؂�ւ��Ďg���܂킷�B
        /// ����ɂ��Œ���̌r���X�v���C�g���ŕ\���ł���
        /// </summary>
        public void UpdateLines(float scrollPosition)
        {
            // �e�L�X�g�\���̈�ɍ����r���̃C���f�b�N�X���擾����
            var ruleIndexOffset = Mathf.FloorToInt(scrollPosition / lineHeight);

            // �e�L�X�g�\���̈�ɍ����ŏ��̌r����K�؂Ȉʒu�Ɉړ�����
            rulesContent.localPosition = new Vector3(0f, -lineHeight * ruleIndexOffset);

            // �r���̕\����؂�ւ���
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
