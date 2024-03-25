using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace RuntimeDotter
{
    public class DotterToolSet : Selectable
    {
        [SerializeField] private DotterBoardView _boardView = null;
        [SerializeField] private DotterHoldButton _holdButton = null;
        [SerializeField] private DotterPaletteView _paletteView = null;
        [SerializeField] private Button _undoRedoButton = null;
        [SerializeField] private Button _switchGridButton = null;
        [SerializeField] private Button _copyBoardButton = null;
        [SerializeField] private Button _pasteBoardButton = null;

        private bool prevHold;
        private Vector2Int prevPenPosition;
        private DotterBoard undoBoard;

        public DotterBoard Board => _boardView.Board;
        public Color32 MainColor => _paletteView.MainColor;
        public IReadOnlyList<ShiftableColor> Palette => _paletteView.Palette;

        protected override void Start()
        {
            base.Start();

            _paletteView.Picker.OnColorChanged.AddListener(x => _boardView.UpdateView(_paletteView.Palette, _paletteView.MainColor));
            _undoRedoButton?.onClick.AddListener(() => Undo());
            _switchGridButton?.onClick.AddListener(() => _boardView.SwitchGrid());
            _copyBoardButton?.onClick.AddListener(() => Copy());
            _pasteBoardButton?.onClick.AddListener(() => Paste());
        }

        public void Load(DotterBoard board, IReadOnlyList<ShiftableColor> palette, Color32 mainColor)
        {
            board = new DotterBoard(board);
            _boardView.Load(board);
            _paletteView.Load(palette, mainColor);
            undoBoard = new DotterBoard(_boardView.Board);

            _boardView.UpdateView(palette, mainColor);
        }

        private void Update()
        {
            if (!IsInteractable()) return;

            var hold = _holdButton.IsDown;
            if (hold)
            {
                var penPosition = _boardView.PenPosition;
                if (!prevHold)
                {
                    prevPenPosition = penPosition;

                    // �A���h�D�p�̃{�[�h���X�V����
                    _boardView.Board.CopyTo(undoBoard);
                    if (_undoRedoButton != null) { _undoRedoButton.transform.localScale = Vector3.one; }
                }
                else if (penPosition == prevPenPosition) return; // �y���������Ă��Ȃ��Ƃ��������Ȃ�

                // �O�t���[���̈ʒu���猻�݈ʒu�܂ł̐�������
                var length = Vector2Int.Distance(prevPenPosition, penPosition);
                length = Mathf.Max(length, 1f);
                for (int j = 0; j < length; j++)
                {
                    var pixelPosition = Vector2.Lerp(penPosition, prevPenPosition, j / length);
                    var pixelPositionInt = new Vector2Int((int)pixelPosition.x, (int)pixelPosition.y);
                    _boardView.Board.SetPixel(pixelPositionInt, _paletteView.ColorIndex);
                }
                prevPenPosition = penPosition;

                // �X�V���ʂ�\��
                _boardView.UpdateView(_paletteView.Palette, _paletteView.MainColor);
            }
            prevHold = hold;
        }

        private void Undo()
        {
            // ���݂̃{�[�h�ƃA���h�D�p�{�[�h�����ւ���
            var tempBoard = _boardView.Board;
            _boardView.Load(undoBoard);
            undoBoard = tempBoard;

            // �X�V���ʂ�\��
            _boardView.UpdateView(_paletteView.Palette, _paletteView.MainColor);

            // �A���h�D�A�C�R�������E���]����
            if (_undoRedoButton != null)
            {
                var scale = _undoRedoButton.image.transform.localScale;
                scale.x *= -1f;
                _undoRedoButton.image.transform.localScale = scale;
            }
        }

        private void Copy()
        {
            // ���݂̃{�[�h�S�̂��N���b�v�{�[�h�ɐݒ肷��
            GUIUtility.systemCopyBuffer = _boardView.Board.ToJson();
        }

        private void Paste()
        {
            // �N���b�v�{�[�h����{�[�h�S�̂��y�[�X�g����
            var board = DotterBoard.FromJson(GUIUtility.systemCopyBuffer, _boardView.Board.PaletteSize);

            // �A���h�D�p�̃{�[�h���X�V����
            _boardView.Board.CopyTo(undoBoard);
            _undoRedoButton.transform.localScale = Vector3.one;

            // �A���h�D�p�{�[�h�X�V��y�[�X�g
            board.CopyTo(_boardView.Board);

            // �X�V���ʂ�\��
            _boardView.UpdateView(_paletteView.Palette, _paletteView.MainColor);
        }
    }
}
