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

                    // アンドゥ用のボードを更新する
                    _boardView.Board.CopyTo(undoBoard);
                    if (_undoRedoButton != null) { _undoRedoButton.transform.localScale = Vector3.one; }
                }
                else if (penPosition == prevPenPosition) return; // ペンが動いていないとき何もしない

                // 前フレームの位置から現在位置までの線を引く
                var length = Vector2Int.Distance(prevPenPosition, penPosition);
                length = Mathf.Max(length, 1f);
                for (int j = 0; j < length; j++)
                {
                    var pixelPosition = Vector2.Lerp(penPosition, prevPenPosition, j / length);
                    var pixelPositionInt = new Vector2Int((int)pixelPosition.x, (int)pixelPosition.y);
                    _boardView.Board.SetPixel(pixelPositionInt, _paletteView.ColorIndex);
                }
                prevPenPosition = penPosition;

                // 更新結果を表示
                _boardView.UpdateView(_paletteView.Palette, _paletteView.MainColor);
            }
            prevHold = hold;
        }

        private void Undo()
        {
            // 現在のボードとアンドゥ用ボードを入れ替える
            var tempBoard = _boardView.Board;
            _boardView.Load(undoBoard);
            undoBoard = tempBoard;

            // 更新結果を表示
            _boardView.UpdateView(_paletteView.Palette, _paletteView.MainColor);

            // アンドゥアイコンを左右反転する
            if (_undoRedoButton != null)
            {
                var scale = _undoRedoButton.image.transform.localScale;
                scale.x *= -1f;
                _undoRedoButton.image.transform.localScale = scale;
            }
        }

        private void Copy()
        {
            // 現在のボード全体をクリップボードに設定する
            GUIUtility.systemCopyBuffer = _boardView.Board.ToJson();
        }

        private void Paste()
        {
            // クリップボードからボード全体をペーストする
            var board = DotterBoard.FromJson(GUIUtility.systemCopyBuffer, _boardView.Board.PaletteSize);

            // アンドゥ用のボードを更新する
            _boardView.Board.CopyTo(undoBoard);
            _undoRedoButton.transform.localScale = Vector3.one;

            // アンドゥ用ボード更新後ペースト
            board.CopyTo(_boardView.Board);

            // 更新結果を表示
            _boardView.UpdateView(_paletteView.Palette, _paletteView.MainColor);
        }
    }
}
