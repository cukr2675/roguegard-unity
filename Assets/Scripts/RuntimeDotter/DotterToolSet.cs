using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace RuntimeDotter
{
    public class DotterToolSet : MonoBehaviour
    {
        [SerializeField] private DotterBoardView _boardView = null;
        [SerializeField] private DotterHoldButton _holdButton = null;
        [SerializeField] private DotterPaletteView _paletteView = null;
        [SerializeField] private Button _undoRedoButton = null;
        [SerializeField] private Button _switchGridButton = null;

        private bool prevHold;
        private Vector2Int prevPenPosition;
        private DotterBoard undoBoard;

        public DotterBoard Board => _boardView.Board;
        public IReadOnlyList<ShiftableColor> Palette => _paletteView.Palette;

        private void Start()
        {
            _paletteView.Picker.OnColorChanged.AddListener(x => _boardView.UpdateView(_paletteView.Palette, _paletteView.MainColor));
            _undoRedoButton.onClick.AddListener(() => Undo());
            _switchGridButton.onClick.AddListener(() => _boardView.SwitchGrid());
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
            var hold = _holdButton.IsDown;
            if (hold)
            {
                var penPosition = _boardView.PenPosition;
                if (!prevHold)
                {
                    prevPenPosition = penPosition;

                    // �A���h�D�p�̃{�[�h���X�V����
                    _boardView.Board.CopyTo(undoBoard);
                    _undoRedoButton.transform.localScale = Vector3.one;
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
            var scale = _undoRedoButton.image.transform.localScale;
            scale.x *= -1f;
            _undoRedoButton.image.transform.localScale = scale;
        }
    }
}
