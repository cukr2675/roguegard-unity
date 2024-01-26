using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PaintMenuView : ModelsMenuView, IPointerMoveHandler, IDragHandler
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private RawImage _board = null;
        [SerializeField] private PaintButton _button = null;
        [SerializeField] private RectTransform _paletteContent = null;
        [SerializeField] private ModelsMenuViewItemButton _paletteItemPrefab = null;
        [SerializeField] private RectTransform _cursor = null;
        [SerializeField] private Image _splitLine = null;
        [SerializeField] private Image _upperPivot = null;
        [SerializeField] private Image _lowerPivot = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private ModelsMenuViewItemButton[] paletteItems;
        private Texture2D boardTexture;
        private SewedEquipmentData data;
        private Vector2 pressPenPosition;
        private Vector2 penPosition;
        private PaintSelector selector;
        private int colorIndex;
        private ModelsMenuViewItemButton exitButton;
        private ModelsMenuViewItemButton changeButton;

        private static readonly PaintPaletteMenu paintPaletteMenu = new PaintPaletteMenu();

        public void Initialize()
        {
            // WebGL でドットがつぶれないようミップマップを無効化する
            boardTexture = new Texture2D(RoguePaintData.BoardSize, RoguePaintData.BoardSize, TextureFormat.RGBA32, false);
            boardTexture.filterMode = FilterMode.Point;
            _board.texture = boardTexture;

            paletteItems = new ModelsMenuViewItemButton[16];
            var paletteItemSize = _paletteContent.rect.width / 2f;
            for (int y = 0; y < 4; y++)
            {
                // 部位変更ボタン
                for (int x = 0; x < 2; x++)
                {
                    var boneItem = Instantiate(_paletteItemPrefab, _paletteContent);
                    boneItem.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, y * paletteItemSize, paletteItemSize);
                    boneItem.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x * paletteItemSize, paletteItemSize);
                    boneItem.Initialize(this);
                    var selector = new PaintSelector((PaintSelector.Bone)y, (PaintSelector.Pose)x);
                    boneItem.SetItem(ChoicesModelsMenuItemController.Instance, new BoneChoice() { parent = this, selector = selector });
                }
            }
            for (int y = 0; y < 2; y++)
            {
                // 分割線移動ボタン
                var splitItem = Instantiate(_paletteItemPrefab, _paletteContent);
                splitItem.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (4 + y) * paletteItemSize, paletteItemSize);
                splitItem.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0 * paletteItemSize, paletteItemSize);
                splitItem.Initialize(this);
                splitItem.SetItem(ChoicesModelsMenuItemController.Instance, new SplitChoice() { parent = this, up = y == 0 });
            }
            for (int y = 0; y < 2; y++)
            {
                // 間隔変更ボタン
                var distanceItem = Instantiate(_paletteItemPrefab, _paletteContent);
                distanceItem.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (4 + y) * paletteItemSize, paletteItemSize);
                distanceItem.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 1 * paletteItemSize, paletteItemSize);
                distanceItem.Initialize(this);
                distanceItem.SetItem(ChoicesModelsMenuItemController.Instance, new PivotChoice() { parent = this, up = y == 0 });
            }
            {
                // 上書き切り替えボタン
                changeButton = Instantiate(_paletteItemPrefab, _paletteContent);
                changeButton.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 6 * paletteItemSize, paletteItemSize);
                changeButton.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0 * paletteItemSize, paletteItemSize * 2f);
                changeButton.Initialize(this);
            }
            for (int y = 0; y < 8; y++)
            {
                // パレット
                for (int x = 0; x < 2; x++)
                {
                    var index = x + y * 2;
                    var paletteItem = Instantiate(_paletteItemPrefab, _paletteContent);
                    paletteItem.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (7 + y) * paletteItemSize, paletteItemSize);
                    paletteItem.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x * paletteItemSize, paletteItemSize);
                    paletteItem.Initialize(this);
                    paletteItems[index] = paletteItem;
                }
            }
            {
                // パレット変更
                var paletteButton = Instantiate(_paletteItemPrefab, _paletteContent);
                paletteButton.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 16 * paletteItemSize, paletteItemSize);
                paletteButton.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 1 * paletteItemSize, paletteItemSize);
                paletteButton.Initialize(this);
                paletteButton.SetItem(ChoicesModelsMenuItemController.Instance, new ActionModelsMenuChoice("パレット", Palette));
            }
            {
                // 終了
                exitButton = Instantiate(_paletteItemPrefab, _paletteContent);
                exitButton.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 16 * paletteItemSize, paletteItemSize);
                exitButton.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0 * paletteItemSize, paletteItemSize);
                exitButton.Initialize(this);
            }

            _paletteContent.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, paletteItemSize * 17f);
            _paletteContent.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, paletteItemSize * 2f);
        }

        private static void Palette(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            root.OpenMenu(paintPaletteMenu, null, null, arg, arg);
        }

        public override void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            SetArg(root, self, user, arg);
            data = (SewedEquipmentData)arg.Other;
            for (int i = 0; i < 16; i++)
            {
                var choice = new PaletteChoice();
                choice.parent = this;
                choice.colorIndex = i;
                paletteItems[i].SetItem(ChoicesModelsMenuItemController.Instance, choice);
            }
            exitButton.SetItem(ChoicesModelsMenuItemController.Instance, models[0]);

            selector = new PaintSelector(PaintSelector.Bone.Body, PaintSelector.Pose.NormalFront);
            UpdateBoardTexture();
            MenuController.Show(_canvasGroup, true);
        }

        public override float GetPosition()
        {
            throw new System.NotImplementedException();
        }

        public override void SetPosition(float position)
        {
            throw new System.NotImplementedException();
        }

        private void Update()
        {
            if (!_canvasGroup.interactable) return;

            _cursor.anchorMin = _cursor.anchorMax = penPosition / RoguePaintData.BoardSize;

            if (_button.IsDown || Input.GetKey(KeyCode.Space))
            {
                var x = Mathf.FloorToInt(penPosition.x);
                var y = Mathf.FloorToInt(penPosition.y);
                var paintData = selector.Get(data.Items);
                var pixel = paintData.GetPixel(new Vector2Int(x, y));
                if (pixel != colorIndex)
                {
                    paintData.SetPixel(new Vector2Int(x, y), colorIndex);
                    UpdateBoardTexture();
                }
            }
        }

        private void UpdateBoardTexture()
        {
            var item = selector.GetItem(data.Items);
            var paintData = selector.Get(data.Items);

            // ペイント内容を表示
            paintData.SetPixelsTo(boardTexture, data.Palette);

            if (selector.SelectsSplitBone)
            {
                // 分割線を表示
                _splitLine.rectTransform.anchorMin = new Vector2(0f, (float)item.SplitY / RoguePaintData.BoardSize);
                _splitLine.rectTransform.anchorMax = new Vector2(1f, (float)item.SplitY / RoguePaintData.BoardSize);

                // 中心点を表示
                _upperPivot.rectTransform.anchorMin = _upperPivot.rectTransform.anchorMax = item.GetUpperPivot();
                _lowerPivot.rectTransform.anchorMin = _lowerPivot.rectTransform.anchorMax = item.GetLowerPivot();

                _splitLine.enabled = _upperPivot.enabled = _lowerPivot.enabled = true;
            }
            else
            {
                _splitLine.enabled = _upperPivot.enabled = _lowerPivot.enabled = false;
            }

            // 上書き切り替えを更新
            changeButton.SetItem(ChoicesModelsMenuItemController.Instance, new ChangeChoice() { parent = this });

            _board.color = data.MainColor;
            boardTexture.Apply();
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null || eventData.dragging) return;
            if (eventData.pointerDrag != gameObject) return;

            // ドラッグ開始までのずれをなくすため、マウスボタン押下後すぐにドラッグ扱いにする。
            // IPointerDownHandler を使うと ScrollView のスワイプによるスクロールができなくなってしまうので IPointerMoveHandler を使う。
            eventData.dragging = true;
            pressPenPosition = penPosition;
            eventData.pressPosition = eventData.position;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerDrag != gameObject) return;

            penPosition = pressPenPosition + (eventData.position - eventData.pressPosition) / (944f / 2f / RoguePaintData.BoardSize);
            penPosition.x = Mathf.Clamp(penPosition.x, .5f, RoguePaintData.BoardSize - .5f);
            penPosition.y = Mathf.Clamp(penPosition.y, .5f, RoguePaintData.BoardSize - .5f);
        }

        private class BoneChoice : IModelsMenuChoice
        {
            public PaintMenuView parent;
            public PaintSelector selector;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return selector.GetName();
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                parent.selector = selector;
                parent.UpdateBoardTexture();
            }
        }

        private class SplitChoice : IModelsMenuChoice
        {
            public PaintMenuView parent;
            public bool up;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (up) return "分割▲";
                else return "分割▼";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var item = parent.selector.GetItem(parent.data.Items);
                if (up) { item.SplitY++; }
                else { item.SplitY--; }
                item.SplitY = Mathf.Clamp(item.SplitY, 1, 31);
                parent.UpdateBoardTexture();
            }
        }

        private class PivotChoice : IModelsMenuChoice
        {
            public PaintMenuView parent;
            public bool up;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (up) return "間隔＋";
                else return "間隔－";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var item = parent.selector.GetItem(parent.data.Items);
                if (up) { item.PivotDistance++; }
                else { item.PivotDistance--; }
                item.PivotDistance = Mathf.Clamp(item.PivotDistance, 1, 31);
                parent.UpdateBoardTexture();
            }
        }

        private class ChangeChoice : IModelsMenuChoice
        {
            public PaintMenuView parent;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var item = parent.selector.GetItem(parent.data.Items);
                return $"切替: {(item.EquipmentSprite != null ? "装" : "変")}";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var item = parent.selector.GetItem(parent.data.Items);
                if (item.EquipmentSprite != null)
                {
                    item.FirstSprite = item.EquipmentSprite;
                    item.EquipmentSprite = null;
                }
                else
                {
                    item.EquipmentSprite = item.FirstSprite;
                    item.FirstSprite = null;
                }
                parent.UpdateBoardTexture();
            }
        }

        private class PaletteChoice : IModelsMenuChoice, IModelsMenuIcon
        {
            public PaintMenuView parent;
            public int colorIndex;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return null;
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                parent.colorIndex = colorIndex;
            }

            public void GetIcon(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, out Sprite sprite, out Color color)
            {
                sprite = parent.data.Palette[colorIndex].ToIcon();
                color = parent.data.MainColor;
            }
        }
    }
}
