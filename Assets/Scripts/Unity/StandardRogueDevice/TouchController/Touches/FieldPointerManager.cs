using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

namespace RoguegardUnity
{
    public class FieldPointerManager
    {
        private readonly TouchInputInfo info;
        private readonly Tilemap tilemap;
        private readonly CameraController cameraController;
        private readonly FieldPointer pointer0;
        private readonly FieldPointer pointer1;

        public float LongDownSeconds { get; set; }
        public float ZoomClippingRadius { get; set; }
        public float AntiJumpDistanceThreshold { get; set; }

        private bool lastPinch;
        private bool pinchOnly;
        private float pressZoom;
        private float lastDistance;

        private float downSeconds;
        private bool readyToLongDown;

        public FieldPointerManager(TouchInputInfo info, Tilemap tilemap, CameraController cameraController, float touchResetTime)
        {
            this.info = info;
            this.tilemap = tilemap;
            this.cameraController = cameraController;
            pointer0 = new FieldPointer();
            pointer1 = new FieldPointer();
            pointer0.TouchResetTime = pointer1.TouchResetTime = touchResetTime;
            lastDistance = Mathf.Infinity;
        }

        public void Update(float deltaTime)
        {
            if (pointer0.IsHeldDown && pointer1.IsHeldDown)
            {
                pointer0.Update(deltaTime);
                pointer1.Update(deltaTime);
            }
        }

        public void UpdateField()
        {
            // 長押し判定
            if (readyToLongDown)
            {
                // 長押し時間が一定を超えたとき実行
                if (downSeconds >= LongDownSeconds)
                {
                    var pointer = pointer0.Data ?? pointer1.Data;
                    info.SetPointing(GetCellPoint(pointer.position), longDown: true);
                    ClearTouches();
                }

                // 長押し時間を進める
                downSeconds += Time.deltaTime;
            }

            // ピンチイン・ピンチアウト
            var pinch = pointer0.IsHeldDown && pointer1.IsHeldDown;
            if (pinch)
            {
                var pressDistance = Vector2.Distance(pointer0.PressPosition, pointer1.PressPosition);
                var distance = Vector2.Distance(pointer0.Position, pointer1.Position);
                info.Zoom = pressZoom + Mathf.Log(distance / pressDistance, 2f);
            }
            else if (lastPinch && info.PowedZoom >= 1f - ZoomClippingRadius)
            {
                // ピンチイン・ピンチアウト終了後、クリップ範囲内ならズーム倍率を整数にする。（ドットを正方形にするため）
                var clippedPowedZoom = Mathf.Round(info.PowedZoom);
                if (Mathf.Abs(info.PowedZoom - clippedPowedZoom) <= clippedPowedZoom * ZoomClippingRadius)
                {
                    info.Zoom = Mathf.Log(clippedPowedZoom, 2f);
                }
            }
            lastPinch = pinch;
        }

        public void UpdateDrag()
        {
            info.Drag = !pinchOnly && (pointer0.Dragging || pointer1.Dragging);

            if (pinchOnly)
            {
                // pinchOnly == true のピンチイン・ピンチアウト中はカメラ移動を無効化する。
                if (pointer0.IsHeldDown && pointer1.IsHeldDown) return;

                // pinchOnly == true のピンチイン・ピンチアウトが終了したとき、残った指は非ドラッグの新規タッチ扱いにする。
                // （ただしクリックは無効のままにする）
                if (pointer0.IsHeldDown)
                {
                    pointer0.PressPosition = pointer0.Position;
                    pointer0.Dragging = false;
                }
                if (pointer1.IsHeldDown)
                {
                    pointer1.PressPosition = pointer1.Position;
                    pointer1.Dragging = false;
                }
                pinchOnly = false;
            }

            if (info.StartsDrag)
            {
                // ドラッグを始めた瞬間のスクロールは無効
                info.DragRelativePosition = Vector2.zero;
            }
            else
            {
                if (pointer0.IsHeldDown && pointer1.IsHeldDown)
                {
                    // ピンチイン・ピンチアウトの指と指の中間点にズームするように、ドラッグ位置を補正する。
                    var pressPosition = (pointer0.PressPosition + pointer1.PressPosition) / 2f;
                    var screenCenter = new Vector2(Screen.width, Screen.height) / 2f;
                    info.DragRelativePosition = -(pressPosition - screenCenter) * (Mathf.Pow(2f, info.Zoom - pressZoom) - 1f);

                    var position = (pointer0.Position + pointer1.Position) / 2f;
                    info.DragRelativePosition += position - pressPosition;
                    info.DragRelativePosition /= info.PowedZoom;
                }
                else if (pointer0.IsHeldDown)
                {
                    info.DragRelativePosition = (pointer0.Position - pointer0.PressPosition) / info.PowedZoom;
                }
                else if (pointer1.IsHeldDown)
                {
                    info.DragRelativePosition = (pointer1.Position - pointer1.PressPosition) / info.PowedZoom;
                }
            }

            // タッチ判定がジャンプする現象を軽減する
            if (pointer0.IsHeldDown && pointer1.IsHeldDown)
            {
                // 同時タッチ中は指の間の距離を記録する
                lastDistance = Vector2.Distance(pointer0.Position, pointer1.Position);
            }
            else if (pointer0.IsHeldDown || pointer1.IsHeldDown)
            {
                // 片方の指を離したあと、記録した距離より長い距離を移動していたらその操作を無効化する
                if (info.DragRelativePosition.magnitude * info.PowedZoom >= lastDistance - AntiJumpDistanceThreshold)
                {
                    if (pointer0.IsHeldDown) { pointer0.PressPosition = pointer0.Position; }
                    if (pointer1.IsHeldDown) { pointer1.PressPosition = pointer1.Position; }
                    info.DragRelativePosition = Vector2.zero;
                }
            }
        }

        private Vector2Int GetCellPoint(Vector2 position)
        {
            var worldPoint = cameraController.ScreenPointToWorldPoint(position);
            var cellPoint = tilemap.WorldToCell(worldPoint);
            return new Vector2Int(cellPoint.x, cellPoint.y);
        }

        public void ClearTouches()
        {
            SetPointers(null, null);
            readyToLongDown = false;
        }

        private void SetPointers(PointerEventData pointer0, PointerEventData pointer1)
        {
            // タッチを開始・停止したとき、すでにタッチ中の指も含めてタッチ開始位置を再設定する。（新規ドラッグ開始扱いにする）
            // ドラッグ開始時のカクつきを回避するため、もとの pressPosition は使わない。
            if (pointer0 != null) { pointer0.pressPosition = pointer0.position; }
            if (pointer1 != null) { pointer1.pressPosition = pointer1.position; }
            this.pointer0.SetEventData(pointer0);
            this.pointer1.SetEventData(pointer1);
            info.StartsDrag = true;
            pressZoom = info.Zoom;
            downSeconds = 0f;
        }

        /// <summary>
        /// 画面を押し始めたときに呼び出すメソッド
        /// </summary>
        private void PointerDown(PointerEventData eventData)
        {
            // WebGL では pointerId は 0 からの連番ではない

            if (eventData.pointerId == pointer0.PointerID || eventData.pointerId == pointer1.PointerID)
            {
                SetPointers(pointer0.Data, pointer1.Data);
            }
            else if (!pointer0.IsHeldDown)
            {
                SetPointers(eventData, pointer1.Data);
            }
            else if (!pointer1.IsHeldDown)
            {
                SetPointers(pointer0.Data, eventData);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // マウスの場合は左クリックのみ扱う
            // タッチ操作ではすべて左クリック扱い
            if (eventData.pointerId < 0 && eventData.button != PointerEventData.InputButton.Left) return;

            if (info.IsPointing)
            {
                // 既にポインティングされていたらキャンセルする。
                info.ClearPointing();
                return;
            }

            // カメラモード時にポインティングされたら早送りする。
            info.SetPointing(GetCellPoint(eventData.position), fastForward: cameraController.IsCameraMode);

            info.IsClick = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown(eventData);
            readyToLongDown = true;

            if (pointer0.IsHeldDown && pointer1.IsHeldDown)
            {
                // 一瞬でも二点タップ状態になったとき、ドラッグ扱いにする。
                pointer0.Dragging = true;
                pointer0.EligibleForClick = false;
                pointer1.Dragging = true;
                pointer1.EligibleForClick = false;
                readyToLongDown = false;

                // カメラモードに切り替えるより先に二点タップ状態になったときは、ピンチ操作のみにする。
                if (!cameraController.IsCameraMode) { pinchOnly = true; }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId == pointer0.PointerID)
            {
                SetPointers(null, pointer1.Data);
            }
            else if (eventData.pointerId == pointer1.PointerID)
            {
                SetPointers(pointer0.Data, null);
            }
            readyToLongDown = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (pointer0.PointerID != eventData.pointerId && pointer1.PointerID != eventData.pointerId)
            {
                // 中断したタッチ処理を再開する。
                // 通常のドラッグ操作と区別するため、いずれの ID とも一致しないことを確認する。
                PointerDown(eventData);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // ドラッグ開始は新規タッチ扱いにする。
            PointerDown(eventData);

            // ドラッグしたらクリックを無効化する。
            eventData.eligibleForClick = false;

            // ドラッグしたら長押しを無効化する。
            readyToLongDown = false;
        }
    }
}
