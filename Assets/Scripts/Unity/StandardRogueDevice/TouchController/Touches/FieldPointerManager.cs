using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

namespace RoguegardUnity
{
    public class FieldPointerManager
    {
        private readonly FieldPointer pointer0;
        private readonly FieldPointer pointer1;

        public bool StartsDrag { get; private set; }

        public float LongDownSeconds { get; set; }

        public float ZoomClippingRadius { get; set; }

        private bool lastPinch;
        private bool pinchOnly;
        private float pressZoom;

        private float downSeconds;
        private bool readyToLongDown;
        private TouchInputInfo info;
        private CameraController cameraController;

        public FieldPointerManager()
        {
            pointer0 = new FieldPointer();
            pointer1 = new FieldPointer();
        }

        public void Update(float deltaTime)
        {
            pointer0.Update(deltaTime);
            pointer1.Update(deltaTime);
        }

        public void UpdateField(bool visiblePlayer, Vector3 playerPosition, RogueDirection playerDirection, int deltaTime)
        {
            // 長押し判定
            if (readyToLongDown)
            {
                // 長押し時間が一定を超えたとき実行
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
                var pressDistance = Vector2.Distance(pointer0.Data.pressPosition, pointer1.Data.pressPosition);
                var distance = Vector2.Distance(pointer0.Data.position, pointer1.Data.position);
                info.Zoom = pressZoom + Mathf.Log(distance / pressDistance, 2f);
            }
            else if (lastPinch && info.PowedZoom >= 1f * (1f - ZoomClippingRadius))
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

        private void SetPointers(PointerEventData pointer0, PointerEventData pointer1)
        {
            // タッチを開始・停止したとき、すでにタッチ中の指も含めてタッチ開始位置を再設定する。（新規ドラッグ開始扱いにする）
            // ドラッグ開始時のカクつきを回避するため、もとの pressPosition は使わない。
            if (pointer0 != null) { pointer0.pressPosition = pointer0.position; }
            if (pointer1 != null) { pointer1.pressPosition = pointer1.position; }
            this.pointer0.SetEventData(pointer0);
            this.pointer1.SetEventData(pointer1);
            StartsDrag = true;
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
                //if (!cameraController.IsCameraMode) { pinchOnly = true; }
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
                //eventData.dragging = false;
                PointerDown(eventData);
            }

            //// ドラッグしたら長押しを無効化する。
            //readyToLongDown = false;
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
