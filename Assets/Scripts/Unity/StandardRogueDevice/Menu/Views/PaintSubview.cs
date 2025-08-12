using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RuntimeDotter;
using UnityEngine.UI;
using Lysionium;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PaintSubview : Subview, IPaintElementsSubview
    {
        [SerializeField] private DotterToolSet _toolSet = null;
        [SerializeField] private Image _splitLine = null;
        [SerializeField] private Image _upperPivot = null;
        [SerializeField] private Image _lowerPivot = null;

        private readonly List<DotterBoard> _boards = new();
        private readonly List<ShiftableColor> _palette = new();

        public Spanning<DotterBoard> Boards
        {
            get
            {
                _boards[0] = _toolSet.Board;
                return Spanning.Get(_boards);
            }
        }

        public Color32 MainColor => _toolSet.MainColor;

        public Spanning<ShiftableColor> Palette
        {
            get
            {
                _palette.Clear();
                for (int i = 0; i < _toolSet.Palette.Count; i++)
                {
                    _palette.Add(_toolSet.Palette[i]);
                }
                return Spanning.Get(_palette);
            }
        }

        public override void SetParameters(
            IReadOnlyList<object> list, IViewItemHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref ISubviewStateProvider stateProvider)
            => throw new System.NotSupportedException();

        void IPaintElementsSubview.SetPaint(
            IReadOnlyList<DotterBoard> dotterBoards, Spanning<ShiftableColor> palette, Color32 mainColor, bool showSplitLine, Vector2[] pivots)
        {
            _boards.Clear();
            for (int i = 0; i < dotterBoards.Count; i++)
            {
                _boards.Add(dotterBoards[i]);
            }
            _toolSet.Load(_boards[0], palette.ToArray(), mainColor);

            ShowSplitLine(showSplitLine, pivots);
        }

        public override void Show(ListMenuEventHandler onEndAnimation = null, ListMenuEventHandler onHide = null)
        {
            base.Show(onEndAnimation, onHide);
            _toolSet.enabled = true;
        }

        public override void Hide(bool back, ListMenuEventHandler onEndAnimation = null)
        {
            base.Hide(back, onEndAnimation);
            _toolSet.enabled = false;
        }

        private void ShowSplitLine(bool show, Spanning<Vector2> pivots)
        {
            if (show)
            {
                // 分割線を表示
                _splitLine.rectTransform.anchorMin = new Vector2(0f, .5f);
                _splitLine.rectTransform.anchorMax = new Vector2(1f, .5f);

                // 中心点を表示
                _upperPivot.rectTransform.anchorMin = _upperPivot.rectTransform.anchorMax = pivots[0];
                _lowerPivot.rectTransform.anchorMin = _lowerPivot.rectTransform.anchorMax = pivots[1];

                _splitLine.enabled = _upperPivot.enabled = _lowerPivot.enabled = true;
            }
            else
            {
                _splitLine.enabled = _upperPivot.enabled = _lowerPivot.enabled = false;
            }
        }
    }
}
