using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Roguegard;

namespace RoguegardUnity
{
    public class TouchField : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler
    {
        [SerializeField] private TileCursor _cursor = null;
        [SerializeField] private ForwardCursor _forwardCursor = null;

        private Tilemap tilemap;
        private CameraController cameraController;

        public bool IsClick { get; private set; }
        public bool IsPointing { get; private set; }
        public Vector2Int PointingPosition { get; private set; }
        public bool FastForward { get; private set; }
        public bool IsLongDown { get; private set; }

        public bool StartsDrag { get; private set; }
        public bool Drag => (!pinchOnly) && ((pointer0?.dragging ?? false) || (pointer1?.dragging ?? false));
        public Vector2 DragRelativePosition { get; private set; }
        public Vector3 DeltaPosition { get; private set; }

        public float PowedZoom => Mathf.Pow(2f, currentZoom);

        private float currentZoom;
        private float pressZoom;
        private bool lastPinch;
        private bool pinchOnly;
        private readonly FieldPointerManager pointerManager = new FieldPointerManager();
        private PointerEventData pointer0;
        private PointerEventData pointer1;
        private Vector2 scrollDelta;
        private bool readyToLongDown;
        private float downSeconds;

        public float TouchResetTime { get; set; }
        public float ZoomClippingRadius { get; set; }
        public float[] WheelZoomingFilters { get; set; }
        public float MaxPowedZoom { get; set; }
        public float MinPowedZoom { get; set; }
        public float LongDownSeconds { get; set; }

        public void Initialize(Tilemap tilemap, CameraController cameraController)
        {
            this.tilemap = tilemap;
            this.cameraController = cameraController;
        }

        private void Update()
        {
            scrollDelta += Input.mouseScrollDelta;
            pointerManager.Update(Time.deltaTime);
        }

        public void UpdateField(bool visiblePlayer, Vector3 playerPosition, RogueDirection playerDirection, int deltaTime)
        {
            // 長押し判定
            if (readyToLongDown)
            {
                if (downSeconds >= LongDownSeconds)
                {
                    var pointer = pointer0 ?? pointer1;
                    IsPointing = true;
                    PointingPosition = GetCellPoint(pointer.position);
                    IsLongDown = true;
                    pointer0 = pointer1 = null;
                    readyToLongDown = false;
                }

                // 長押し時間を進める
                downSeconds += Time.deltaTime;
            }

            // ピンチイン・ピンチアウト
            var pinch = pointer0 != null && pointer1 != null;
            if (pinch)
            {
                var pressDistance = Vector2.Distance(pointer0.pressPosition, pointer1.pressPosition);
                var distance = Vector2.Distance(pointer0.position, pointer1.position);
                currentZoom = pressZoom + Mathf.Log(distance / pressDistance, 2f);
            }
            else if (lastPinch && PowedZoom >= 1f * (1f - ZoomClippingRadius))
            {
                // ピンチイン・ピンチアウト終了後、クリップ範囲内ならズーム倍率を整数にする。（ドットを正方形にするため）
                var clippedPowedZoom = Mathf.Round(PowedZoom);
                if (Mathf.Abs(PowedZoom - clippedPowedZoom) <= clippedPowedZoom * ZoomClippingRadius) { currentZoom = Mathf.Log(clippedPowedZoom, 2f); }
            }
            lastPinch = pinch;

            // マウスホイール
            if (scrollDelta.y != 0f)
            {
                var zoomIndex = GetZoomIndex(currentZoom, scrollDelta.y);
                var newZoom = WheelZoomingFilters[zoomIndex];

                // マウスの位置を中心にズームするようにスクロールする
                DeltaPosition = Input.mousePosition - new Vector3(Screen.width, Screen.height) / 2f;
                DeltaPosition *= (newZoom - currentZoom) / 50f;

                currentZoom = newZoom;
                scrollDelta = Vector2.zero;
            }
            else
            {
                DeltaPosition = Vector3.zero;
            }

            // ズーム倍率を最小～最大に収める。
            if (float.IsNaN(currentZoom)) { currentZoom = 0f; }
            currentZoom = Mathf.Log(Mathf.Clamp(PowedZoom, MinPowedZoom, MaxPowedZoom), 2f);

            // ドラッグでカメラ移動
            UpdateDrag();

            // ポインティングカーソルのアニメーション
            _cursor.UpdateAnimation(IsPointing, PointingPosition, deltaTime);

            // 方向表示のアニメーション
            _forwardCursor.UpdateAnimation(visiblePlayer, playerPosition, playerDirection);

            int GetZoomIndex(float currentZoom, float delta)
            {
                var index = WheelZoomingFilters.Length - 1;
                for (int i = 0; i < WheelZoomingFilters.Length; i++)
                {
                    if (WheelZoomingFilters[i] > currentZoom)
                    {
                        index = i - 1;
                        break;
                    }
                }
                index += Mathf.FloorToInt(delta);
                index = Mathf.Clamp(index, 0, WheelZoomingFilters.Length - 1);
                return index;
            }
        }

        private void UpdateDrag()
        {
            if (pinchOnly)
            {
                // pinchOnly == true のピンチイン・ピンチアウト中はカメラ移動を無効化する。
                if (pointer0 != null && pointer1 != null) return;

                // pinchOnly == true のピンチイン・ピンチアウトが終了したとき、残った指は非ドラッグの新規タッチ扱いにする。
                // （ただしクリックは無効のままにする）
                if (pointer0 != null)
                {
                    pointer0.pressPosition = pointer0.position;
                    pointer0.dragging = false;
                }
                if (pointer1 != null)
                {
                    pointer1.pressPosition = pointer1.position;
                    pointer1.dragging = false;
                }
                pinchOnly = false;
            }

            if (pointer0 != null && pointer1 != null)
            {
                // ピンチイン・ピンチアウトの指と指の中間点にズームするように、ドラッグ位置を補正する。
                var pressPosition = (pointer0.pressPosition + pointer1.pressPosition) / 2f;
                var screenCenter = new Vector2(Screen.width, Screen.height) / 2f;
                DragRelativePosition = -(pressPosition - screenCenter) * (Mathf.Pow(2f, currentZoom - pressZoom) - 1f);

                var position = (pointer0.position + pointer1.position) / 2f;
                DragRelativePosition += position - pressPosition;
                DragRelativePosition /= PowedZoom;
            }
            else if (pointer0 != null)
            {
                DragRelativePosition = (pointer0.position - pointer0.pressPosition) / PowedZoom;
            }
            else if (pointer1 != null)
            {
                DragRelativePosition = (pointer1.position - pointer1.pressPosition) / PowedZoom;
            }
        }

        public void ResetClick()
        {
            IsClick = false;
        }

        public void ClearPointing()
        {
            IsPointing = false;
            FastForward = false;
            IsLongDown = false;
        }

        public void ClearTouches()
        {
            pointer0 = pointer1 = null;
            readyToLongDown = false;
        }

        public void ResetStartsDrag()
        {
            StartsDrag = false;
        }

        private Vector2Int GetCellPoint(Vector2 position)
        {
            var worldPoint = cameraController.ScreenPointToWorldPoint(position);
            var cellPoint = tilemap.WorldToCell(worldPoint);
            return new Vector2Int(cellPoint.x, cellPoint.y);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerId < 0 && eventData.button != PointerEventData.InputButton.Left) return;

            // WebGL のタッチ処理で不具合が起きたときのため、クリック操作でタッチ状態をリセットする。
            pointer0 = pointer1 = null;
            readyToLongDown = false;

            if (IsPointing)
            {
                // 既にポインティングされていたらキャンセルする。
                IsPointing = false;
                return;
            }

            // カメラモード時にポインティングされたら早送りする。
            FastForward = cameraController.IsCameraMode;

            IsClick = true;
            IsPointing = true;
            PointingPosition = GetCellPoint(eventData.position);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
            => pointerManager.OnPointerDown(eventData);

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
            => pointerManager.OnPointerUp(eventData);

        void IDragHandler.OnDrag(PointerEventData eventData)
            => pointerManager.OnDrag(eventData);

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
            => pointerManager.OnBeginDrag(eventData);
    }
}
