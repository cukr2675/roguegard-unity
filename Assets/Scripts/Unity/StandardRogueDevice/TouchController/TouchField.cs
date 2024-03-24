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

        private TouchInputInfo info;
        private FieldPointerManager pointerManager;
        private MouseInputManager mouseInputManager;
        private float minPowedZoom;
        private float maxPowedZoom;

        public bool IsClick => info.IsClick;
        public bool IsPointing => info.IsPointing;
        public Vector2Int PointingPosition => info.PointingPosition;
        public bool FastForward => info.FastForward;
        public bool IsLongDown => info.IsLongDown;

        public bool StartsDrag => info.StartsDrag;
        public bool Drag => info.Drag;
        public Vector2 DragRelativePosition => info.DragRelativePosition;

        public Vector3 DeltaPosition => info.DeltaPosition;

        public float PowedZoom => info.PowedZoom;

        public void Initialize(Tilemap tilemap, CameraController cameraController)
        {
            info = new TouchInputInfo();
            pointerManager = new FieldPointerManager(info, tilemap, cameraController, .1f);
            pointerManager.LongDownSeconds = .5f;
            pointerManager.ZoomClippingRadius = .2f;
            pointerManager.AntiJumpDistanceThreshold = 40f;
            pointerManager.AntiJumpValidSeconds = .1f;
            mouseInputManager = new MouseInputManager(info);
            mouseInputManager.WheelZoomingFilters = new[]
            {
                -4f, -3.5f, -3f, -2.5f, -2f, -1.5f, -1f, -.5f,
                Mathf.Log(1f, 2f), Mathf.Log(1.5f, 2f), Mathf.Log(2f, 2f), Mathf.Log(3f, 2f), Mathf.Log(4f, 2f), Mathf.Log(6f, 2f), Mathf.Log(8f, 2f)
            };
            minPowedZoom = Mathf.Pow(2f, mouseInputManager.WheelZoomingFilters[0]);
            maxPowedZoom = Mathf.Pow(2f, mouseInputManager.WheelZoomingFilters[mouseInputManager.WheelZoomingFilters.Length - 1]);
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            pointerManager.Update(deltaTime);
            mouseInputManager.Update(deltaTime);
        }

        public void UpdateField(bool visiblePlayer, Vector3 playerPosition, RogueDirection playerDirection, int deltaTime)
        {
            pointerManager.UpdateField();

            // マウスホイール
            mouseInputManager.UpdateField();

            // ズーム倍率を最小～最大に収める。
            if (float.IsNaN(info.Zoom)) { info.Zoom = 0f; }
            info.Zoom = Mathf.Log(Mathf.Clamp(PowedZoom, minPowedZoom, maxPowedZoom), 2f);

            // ドラッグでカメラ移動
            pointerManager.UpdateDrag();

            // ポインティングカーソルのアニメーション
            _cursor.UpdateAnimation(IsPointing, PointingPosition, deltaTime);

            // 方向表示のアニメーション
            _forwardCursor.UpdateAnimation(visiblePlayer, playerPosition, playerDirection);
        }

        public void ResetClick()
        {
            info.IsClick = false;
        }

        public void ClearPointing()
        {
            info.ClearPointing();
        }

        public void ClearTouches()
        {
            pointerManager.ClearTouches();
        }

        public void ResetStartsDrag()
        {
            info.StartsDrag = false;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
            => pointerManager.OnPointerClick(eventData);

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
