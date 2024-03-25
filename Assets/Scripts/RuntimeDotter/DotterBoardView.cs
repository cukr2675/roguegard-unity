using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RuntimeDotter
{
    public class DotterBoardView : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] private RawImage _board = null;
        [SerializeField] private RectTransform _cursor = null;
        [Header("Grid")]
        [SerializeField] private RawImage _grid = null;
        [SerializeField] private int _gridResolution = 16;
        [SerializeField] private Color _gridColorA = Color.white;
        [SerializeField] private Color _gridColorB = Color.black;

        private Texture2D boardTexture;
        private Vector2 pressPenPosition;
        private Vector2 penPosition;

        public DotterBoard Board { get; private set; }
        public Vector2Int PenPosition { get; private set; }

        public void Load(DotterBoard board)
        {
            if (boardTexture == null || boardTexture.width != board.Size.x || boardTexture.height != board.Size.y)
            {
                // WebGL でドットがつぶれないようミップマップを無効化する
                boardTexture = new Texture2D(board.Size.x, board.Size.y, TextureFormat.RGBA32, false);
                boardTexture.filterMode = FilterMode.Point;
                _board.texture = boardTexture;

                _grid.texture = CreateGridTexture(board.Size, _gridResolution, _gridColorA, _gridColorB);
            }

            Board = board;
            UpdatePenPosition(null);

            // 親要素の高さに合わせる
            var height = ((RectTransform)transform.parent.transform).rect.height;
            ((RectTransform)transform).sizeDelta = Vector2.one * height;
        }

        public void UpdateView(IReadOnlyList<ShiftableColor> palette, Color32 mainColor)
        {
            _board.color = mainColor;
            Board.SetPixelsTo(boardTexture, palette);
            boardTexture.Apply();
        }

        public void SwitchGrid()
        {
            _grid.enabled = !_grid.enabled;
        }

        private static Texture2D CreateGridTexture(Vector2Int size, int resolution, Color colorA, Color colorB)
        {
            var gridTexture = new Texture2D(size.x * resolution, size.y * resolution, TextureFormat.RGBA32, false);
            gridTexture.filterMode = FilterMode.Point;
            for (int x = 0; x < gridTexture.width; x++)
            {
                for (int y = 0; y < gridTexture.height; y++)
                {
                    gridTexture.SetPixel(x, y, Color.clear);
                }
            }

            // 横線を引く
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < gridTexture.width; x++)
                {
                    var color = x % 2 == 0 ? colorA : colorB;
                    gridTexture.SetPixel(x, y * resolution, color);
                }
            }

            // 縦線を引く
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < gridTexture.height; y++)
                {
                    var color = y % 2 == 0 ? colorA : colorB;
                    gridTexture.SetPixel(x * resolution, y, color);
                }
            }

            // 中心線を引く
            {
                var y = size.y / 2 * resolution;
                for (int x = 0; x < gridTexture.width; x++)
                {
                    gridTexture.SetPixel(x, y - 1, colorB);
                    gridTexture.SetPixel(x, y, colorA);
                }
            }
            {
                var x = size.x / 2 * resolution;
                for (int y = 0; y < gridTexture.height; y++)
                {
                    if (y == size.y / 2 * resolution) continue;

                    gridTexture.SetPixel(x - 1, y, colorA);
                    gridTexture.SetPixel(x, y, colorB);
                }
            }

            gridTexture.Apply();
            return gridTexture;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            // ドラッグ開始までのずれをなくすため、マウスボタン押下後すぐにドラッグ扱いにする。
            eventData.dragging = true;
            pressPenPosition = penPosition;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            UpdatePenPosition(eventData);
        }

        private void UpdatePenPosition(PointerEventData eventData)
        {
            var rectTransform = ((RectTransform)transform);
            var boardRect = rectTransform.rect;
            _cursor.sizeDelta = rectTransform.sizeDelta;
            _cursor.localScale = Vector3.one / Board.Size.y * 2f;

            if (eventData != null)
            {
                var ratio = Board.Size.y / boardRect.height;
                penPosition = pressPenPosition + (eventData.position - eventData.pressPosition) * ratio * 2f;
            }
            penPosition.x = Mathf.Clamp(penPosition.x, .5f, Board.Size.x - .5f);
            penPosition.y = Mathf.Clamp(penPosition.y, .5f, Board.Size.y - .5f);
            _cursor.anchorMin = _cursor.anchorMax = penPosition / Board.Size;

            PenPosition = new Vector2Int((int)penPosition.x, (int)penPosition.y);
        }
    }
}
