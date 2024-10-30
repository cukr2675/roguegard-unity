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
        [SerializeField, Tooltip("1�y�[�W������̃e�L�X�g�\���s��")] private int _maxLineCount = 1;

        [Header("Display")]
        [SerializeField, Tooltip("�e�L�X�g�\���A�j���[�V����")] private VisibleMode _visibleMode = VisibleMode.Static;
        [SerializeField, Tooltip("VisibleMode == Typing: �e�L�X�g�\�����x�@��b������̕�����")] private float _characterPerSecond = 60f;
        public float CharacterPerSecond { get => _characterPerSecond; set => _characterPerSecond = value; }
        [SerializeField, Tooltip("VisibleMode == Typing: �y�[�W��؂�̃����NID")] private string _pageBreakHiddenLinkID = "PageBreak";
        [SerializeField, Tooltip("�r���̃����NID")] private string _horizontalRuleHiddenLinkID = "HorizontalRule";

        [Header("Animation")]
        [SerializeField, Tooltip("0 (�X�N���[���J�n�ʒu) �` 1 (�X�N���[���ڕW�ʒu)\n*�����l�� 1 ����*")] private float _normalizedLineOffset = 1f;
        public float NormalizedLineOffset
        {
            get => _normalizedLineOffset;
            set => _normalizedLineOffset = value;
        }
        [SerializeField, Tooltip("�e�L�X�g�����[����͂ݏo�����Ƃ����s����郊���NID")] private string _hiddenLinkIDOnPageOver = "PageBreak";
        public string HiddenLinkIDOnPageOver
        {
            get => _hiddenLinkIDOnPageOver;
            set => _hiddenLinkIDOnPageOver = value;
        }
        [SerializeField, Tooltip("�e�L�X�g�̏I�[�ɓ��B�����Ƃ����s����郊���NID")] private string _hiddenLinkIDOnEOF = "EOF";
        public string HiddenLinkIDOnEOF
        {
            get => _hiddenLinkIDOnEOF;
            set => _hiddenLinkIDOnEOF = value;
        }

        [Space]
        [SerializeField, Tooltip("�e�L�X�g���܂܂Ȃ�<link>�^�O�܂ŕ\�������B�����Ƃ����΂���C�x���g\n�i��̃v���p�e�B���甭�s�������̂��܂ށj")]
        private ReachHiddenLinkEvent _onReachHiddenLink = null;
        public ReachHiddenLinkEvent OnReachHiddenLink => _onReachHiddenLink;

        private TextTypingEffect textTypingEffect;
        private TextHorizontalRuler textRuleEffect;

        private float characterCount;
        private int linePosition;
        private float startLineOffset;
        public bool isScrollingNow;

        private float ScrollPosition
        {
            get => _content.localPosition.y;
            set => _content.localPosition = new Vector3(0f, value);
        }

        public bool IsInProgress => textTypingEffect.IsInProgress;
        public bool IsTypingNow => enabled && _characterPerSecond > 0f && textTypingEffect.IsInProgress && !isScrollingNow;

        private void Awake()
        {
            textTypingEffect = new TextTypingEffect(_text, _maxLineCount, _hiddenLinkIDOnPageOver, _hiddenLinkIDOnEOF, _onReachHiddenLink);
            textRuleEffect = new TextHorizontalRuler(_horizontalRulePrefab, _rulesContent, _maxLineCount, textTypingEffect.LineHeight, _text.margin.y);
            SetTextTransform(_text.transform);
            SetTextTransform(_rulesContent);

            // ������
            Clear();

            _onReachHiddenLink.AddListener(hiddenLinkID =>
            {
                if (hiddenLinkID == _horizontalRuleHiddenLinkID) { InsertHorizontalRule(); }
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
            if (!isScrollingNow)
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

            textRuleEffect.UpdateLines(ScrollPosition);
            ScrollPosition = Mathf.LerpUnclamped(startLineOffset, CalculateTargetTextOffset(), _normalizedLineOffset);
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
            // �ꕶ�����ݒ肳��Ă��Ȃ��Ƃ��͉������Ȃ�
            if (textTypingEffect.LineCount == 0) return;

            // �e�L�X�g�� 1 �s���Ή�����r���\���t���O�𐶐�����
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
            // �X�N���[���ڕW�n�_�̓e�L�X�g�̍ŉ��[���I�o����ʒu
            textTypingEffect.MeshUpdate();
            return textTypingEffect.LineHeight * linePosition;
        }

        public void StartScrollAndAddLinePosition(int lines)
        {
            linePosition += lines;
            isScrollingNow = true;
        }

        /// <summary>
        /// <see cref="VisibleMode.Static"/> �̏ꍇ�� += 1 �A <see cref="VisibleMode.Typing"/> �̏ꍇ�� += <see cref="_maxLineCount"/>
        /// </summary>
        public void StartScrollAndAddLinePositionAuto(int multiplier)
        {
            if (_visibleMode == VisibleMode.Static) { linePosition += 1 * multiplier; }
            else { linePosition += _maxLineCount * multiplier; }
            isScrollingNow = true;
        }

        public void EndScroll()
        {
            isScrollingNow = false;
        }

        /// <summary>
        /// �s�X�N���[���A�j���[�V�����������ɌĂяo�����\�b�h
        /// </summary>
        public void EndScrollAndTrimBeforeAuto()
        {
            isScrollingNow = false;

            // �\���ɕK�v�Ȃ��Ȃ����e�L�X�g���폜����
            int removedLineCount;
            if (_visibleMode == VisibleMode.Static)
            {
                // �^�C�s���O�G�t�F�N�g�������̏ꍇ

                // �S�e�L�X�g���\���������Ă��Ȃ���Ή������Ȃ�
                if (textTypingEffect.IsInProgress) return;

                // �\���ɕK�v�Ȃ��Ȃ����e�L�X�g���s�P�ʂō폜����
                removedLineCount = textTypingEffect.TrimBeforeVisibleLine();
                if (removedLineCount == 0) return;

                textTypingEffect.SeekToEndOfText();
            }
            else
            {
                // �^�C�s���O�G�t�F�N�g���L���̏ꍇ

                // �����NID�ō폜����
                removedLineCount = textTypingEffect.TrimBeforeFirstLinkID(_pageBreakHiddenLinkID);
                if (removedLineCount == 0) return;

                // �����NID�̑���Ɍ��ݕ\���ʒu�ō폜���邱�Ƃ��ł��邪�A�F�ύX�^�O�Ȃǂ��폜����Ă��܂�
                //textTypingEffect.TrimBeforeVisibleCharacter();

                textTypingEffect.SeekToStartOfText();
            }

            // �e�L�X�g���폜�����s���Ԃ�r�����폜
            textRuleEffect.RemoveLinesRange(0, removedLineCount);

            // �e�L�X�g���폜�����Ԃ�s�\���ʒu���ړ�
            linePosition = 0;
        }

        public void InvokeReachHiddenLink(string hiddenLinkID)
        {
            _onReachHiddenLink.Invoke(hiddenLinkID);
        }

        private enum VisibleMode
        {
            Static,
            Typing
        }

        [System.Serializable] public class ReachHiddenLinkEvent : UnityEvent<string> { }
    }
}
