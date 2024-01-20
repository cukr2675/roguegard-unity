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
        [SerializeField] private Image _cursor = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private ModelsMenuViewItemButton[] paletteItems;
        private Texture2D boardTexture;
        private SewedEquipmentData data;
        private Vector2 pressPenPosition;
        private Vector2 penPosition;
        PaintSelector selector;
        private int colorIndex;
        private ModelsMenuViewItemButton exitButton;

        private static readonly PaintPaletteMenu paintPaletteMenu = new PaintPaletteMenu();

        public void Initialize()
        {
            // WebGL でドットがつぶれないようミップマップを無効化する
            boardTexture = new Texture2D(RoguePaintData.BoardSize, RoguePaintData.BoardSize, TextureFormat.RGBA32, false);
            boardTexture.filterMode = FilterMode.Point;
            _board.texture = boardTexture;

            paletteItems = new ModelsMenuViewItemButton[16];
            var paletteItemSize = _paletteContent.rect.width / 2f;
            for (int y = 0; y < 3; y++)
            {
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
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    var index = x + y * 2;
                    var paletteItem = Instantiate(_paletteItemPrefab, _paletteContent);
                    paletteItem.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (3 + y) * paletteItemSize, paletteItemSize);
                    paletteItem.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x * paletteItemSize, paletteItemSize);
                    paletteItem.Initialize(this);
                    paletteItems[index] = paletteItem;
                }
            }
            {
                var paletteButton = Instantiate(_paletteItemPrefab, _paletteContent);
                paletteButton.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 12 * paletteItemSize, paletteItemSize);
                paletteButton.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0 * paletteItemSize, paletteItemSize);
                paletteButton.Initialize(this);
                paletteButton.SetItem(ChoicesModelsMenuItemController.Instance, new ActionModelsMenuChoice("パレット", Palette));
            }
            {
                exitButton = Instantiate(_paletteItemPrefab, _paletteContent);
                exitButton.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 12 * paletteItemSize, paletteItemSize);
                exitButton.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 1 * paletteItemSize, paletteItemSize);
                exitButton.Initialize(this);
            }

            _paletteContent.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, paletteItemSize * 13f);
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

            SetBoneIndex(new PaintSelector(PaintSelector.Bone.Body, PaintSelector.Pose.NormalFront));
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

            _cursor.rectTransform.anchorMin = penPosition / RoguePaintData.BoardSize;
            _cursor.rectTransform.anchorMax = penPosition / RoguePaintData.BoardSize;

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

        private void SetBoneIndex(PaintSelector selector)
        {
            this.selector = selector;
            UpdateBoardTexture();
        }

        private void UpdateBoardTexture()
        {
            var paintData = selector.Get(data.Items);
            paintData.SetPixelsTo(boardTexture, data.Palette);
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
                parent.SetBoneIndex(selector);
            }
        }

        private class PaletteChoice : IModelsMenuChoice
        {
            public PaintMenuView parent;
            public int colorIndex;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var color = parent.data.Palette[colorIndex];
                var colorCode = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", color.ROrShade, color.GOrSaturation, color.BOrValue, color.ByteA);
                return $"<#{colorCode}>■";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                parent.colorIndex = colorIndex;
            }
        }
    }
}
