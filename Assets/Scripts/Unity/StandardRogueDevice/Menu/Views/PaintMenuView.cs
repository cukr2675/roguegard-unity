using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RuntimeDotter;
using UnityEngine.UI;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PaintMenuView : ModelsMenuView, IPaintModelsMenuView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private DotterToolSet _toolSet = null;
        [SerializeField] private Image _splitLine = null;
        [SerializeField] private Image _upperPivot = null;
        [SerializeField] private Image _lowerPivot = null;
        [SerializeField] private ModelsMenuViewItemButton _exitButton = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private PaintBoneSpriteTable baseTable;
        private List<DotterBoard> _boards;

        public Spanning<DotterBoard> Boards
        {
            get
            {
                _boards[0] = _toolSet.Board;
                return _boards;
            }
        }

        public void Initialize()
        {
            _boards = new List<DotterBoard>();
            _exitButton.Initialize(this);
        }

        public override void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            SetArg(root, self, user, arg);
            baseTable = (PaintBoneSpriteTable)arg.Other;
            _boards.Clear();
            for (int i = 0; i < models.Count; i++)
            {
                if (!(models[i] is DotterBoard board)) continue;

                _boards.Add(board);
            }
            _toolSet.Load(_boards[0], baseTable.Palette.ToArray(), baseTable.MainColor);
            MenuController.Show(_canvasGroup, true);
        }

        public void ShowSplitLine(bool show, Spanning<Vector2> pivots)
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

        public override float GetPosition()
        {
            return 0f;
        }

        public override void SetPosition(float position)
        {
        }

        void IScrollModelsMenuView.ShowExitButton(IModelsMenuChoice exitItem)
        {
            _exitButton.SetItem(ChoicesModelsMenuItemController.Instance, exitItem);
        }
    }
}
