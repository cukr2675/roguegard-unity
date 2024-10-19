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

        /// <param name="pageTurnHiddenLinkID">�e�L�X�g���ő�s�����牺�ɂ͂ݏo���Ƃ����s����郊���NID��</param>
        /// <param name="eofHiddenLinkID">�e�L�X�g�̏I�[�̕\�������������Ƃ����s����郊���NID</param>
        public TextTypingEffect(
            TMP_Text text, int maxLineCount, string pageTurnHiddenLinkID, string eofHiddenLinkID, UnityEvent<string> onReachHiddenLink)
        {
            if (text.lineSpacing != 0f) { Debug.LogWarning($"{nameof(text.lineSpacing)} != 0 �̓T�|�[�g����Ă��܂���B"); }

            this.text = text;
            this.maxLineCount = maxLineCount;
            this.pageTurnHiddenLinkID = pageTurnHiddenLinkID;
            this.eofHiddenLinkID = eofHiddenLinkID;
            this.onReachHiddenLink = onReachHiddenLink;
            hiddenLinkManager = new TextHiddenLinkManager();

            // �T�C�Y�擾�p�e�L�X�g��ݒ肵�Ĉ�s������̕����T���v�����O����
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

            // ������
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
            // ���t���[���Ăяo���Əd���̂ōX�V���̂݌Ăяo��
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
        /// 1�s���Ƃɕ\��
        /// </summary>
        public void UpdateUI(int linePosition)
        {
            if (!IsInProgress) return;

            // 1�t���[������1�s���ׂĕ\������
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

            // �����N�����m
            while (hiddenLinkManager.ForwardDetect(maxVisibleCharacters, out var hiddenLinkID, out var linkCharacterIndex))
            {
                text.maxVisibleCharacters = linkCharacterIndex;
                onReachHiddenLink.Invoke(hiddenLinkID);
            }

            // ���̍s�̏I���܂ŕ\������
            text.maxVisibleCharacters = maxVisibleCharacters;

            // �ݒ肳�ꂽ������̏I�[�����m
            if (!IsInProgress)
            {
                // �R���e�L�X�g�����ׂĕ\�����I������I�[�����NID�𔭍s
                onReachHiddenLink.Invoke(eofHiddenLinkID);
                return;
            }

            onReachHiddenLink.Invoke(pageTurnHiddenLinkID);
        }

        /// <summary>
        /// �^�C�s���O�G�t�F�N�g�Đ�
        /// </summary>
        public void UpdateUI(int deltaVisibleCharacters, int linePosition)
        {
            if (!IsInProgress) return;

            // �����N�����m
            if (hiddenLinkManager.ForwardDetect(text.maxVisibleCharacters + deltaVisibleCharacters, out var hiddenLinkID, out var linkCharacterIndex))
            {
                text.maxVisibleCharacters = linkCharacterIndex;
                onReachHiddenLink.Invoke(hiddenLinkID);
                return;
            }

            // �^�C�s���O�G�t�F�N�g
            text.maxVisibleCharacters += deltaVisibleCharacters;

            // �ݒ肳�ꂽ������̏I�[�����m
            if (!IsInProgress && linePosition >= text.textInfo.lineCount - maxLineCount)
            {
                // �R���e�L�X�g�����ׂĕ\�����I������I�[�����NID�𔭍s
                onReachHiddenLink.Invoke(eofHiddenLinkID);
                return;
            }

            // �e�L�X�g�����[����͂ݏo�������Ƃ����m
            var overLineIndex = maxLineCount + linePosition;
            if (overLineIndex < text.textInfo.lineCount)
            {
                var firstOverVisibleCharacterIndex = text.textInfo.lineInfo[overLineIndex].firstVisibleCharacterIndex;
                if (text.maxVisibleCharacters >= firstOverVisibleCharacterIndex + 1)
                {
                    // �͂ݏo������͂ݏo��������������������ĉ��y�[�W�𔭍s
                    text.maxVisibleCharacters = firstOverVisibleCharacterIndex;
                    onReachHiddenLink.Invoke(pageTurnHiddenLinkID);
                }
            }
        }

        /// <summary>
        /// ��O���ɂ͂ݏo�����e�L�X�g���폜���A�폜�����s�����擾����
        /// </summary>
        public int TrimBeforeVisibleLine()
        {
            // �͂ݏo���Ă���e�L�X�g�̂������s�ȑO���폜����
            // ���s�ȍ~���폜����Ǝ������s�ɉe�����o�āA�͂ݏo���Ă��Ȃ��e�L�X�g���ς���Ă��܂����Ƃ�����

            var stringIndex = text.textInfo.characterInfo[text.maxVisibleCharacters - 1].index;

            // �e�L�X�g�����s�ŏI����Ă���ꍇ��1�s������������
            if (stringIndex >= 1 && text.text[stringIndex - 1] == breakCharacter) { stringIndex--; }

            // �Ōォ�� maxLineCount �Ԗڂ̉��s�R�[�h�̈ʒu���擾����
            for (int i = 0; i < maxLineCount - 1; i++)
            {
                if (stringIndex <= 0) break;

                stringIndex = text.text.LastIndexOf(breakCharacter, stringIndex - 1);
            }

            // �폜�ł��镔����������Ȃ��ꍇ�A�������Ȃ�
            if (stringIndex <= 0) return 0;

            // �e�L�X�g���폜����i���s�R�[�h���܂߂č폜�j
            MeshUpdate(); // �e�L�X�g�̍s�����X�V����
            var beforeLineCount = text.textInfo.lineCount; // �폜�O�Ɏ擾
            Remove(0, stringIndex + 1);
            MeshUpdate(); // �e�L�X�g�̍s�����X�V����
            var removedLineCount = beforeLineCount - text.textInfo.lineCount; // �폜�����s�����擾

            return removedLineCount;
        }

        /// <summary>
        /// �w��̃����NID�̍ŏ��̏o���ʒu�����O���폜���A�ꕔ�ł��폜�����s�����擾����
        /// </summary>
        public int TrimBeforeFirstLinkID(string hiddenLinkID)
        {
            if (!hiddenLinkManager.TryGetFirstHiddenLinkCharacterIndex(text.maxVisibleCharacters, hiddenLinkID, out var linkCharacterIndex)) return 0;

            var linkCharacterInfo = text.textInfo.characterInfo[linkCharacterIndex];
            var linkStringIndex = linkCharacterInfo.index;

            // �e�L�X�g���폜����
            MeshUpdate(); // �e�L�X�g�̍s�����X�V����
            var beforeLineCount = text.textInfo.lineCount; // �폜�O�Ɏ擾
            Remove(0, linkStringIndex);
            MeshUpdate(); // �e�L�X�g�̍s�����X�V����
            var removedLineCount = beforeLineCount - text.textInfo.lineCount; // �폜�����s�����擾

            // �����N���n�[�܂��͏I�[�łȂ��ʒu�ɂ���ꍇ�A�ꕔ���폜�����s�Ƃ��ĉ��Z
            var linkCharacterLineInfo = text.textInfo.lineInfo[linkCharacterInfo.lineNumber];
            if (linkCharacterLineInfo.firstVisibleCharacterIndex != linkCharacterIndex ||
                linkCharacterLineInfo.lastVisibleCharacterIndex != linkCharacterIndex)
            {
                removedLineCount++;
            }

            return removedLineCount;
        }

        /// <summary>
        /// ���݂̕\���͈͂̃e�L�X�g���폜����
        /// </summary>
        public void TrimBeforeVisibleCharacter()
        {
            var visibleStringIndex = text.textInfo.characterInfo[text.maxVisibleCharacters].index;
            Remove(0, visibleStringIndex);
            MeshUpdate();
        }

        /// <summary>
        /// �r���𕶎��̈�Ƃ�����@
        /// </summary>
        private void SetUpHorizontalRule()
        {
            var faceInfo = text.font.faceInfo;
            faceInfo.lineHeight = 66; // �r���̗L���ōs������Ȃ��l�ɒ�������

            const int hrCharacter = -1; // �r���ɂ��镶�������w��
            var character = text.font.characterTable[hrCharacter];
            var metrics = character.glyph.metrics;
            metrics.width = 1e+10f; // �r���̉����͂���ƕ������Œ�������
            metrics.height = 1e+9f;
            character.glyph.metrics = metrics;
            character.scale = 1e-8f; // glyphTable[].scale �Ƃ͕ʕ��Ȃ̂Œ���
        }
    }
}
