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

        private float pressZoom;

        private float downSeconds;
        private bool readyToLongDown;

        public FieldPointerManager()
        {
            pointer0 = new FieldPointer();
            pointer1 = new FieldPointer();
        }

        private void SetPointers(PointerEventData pointer0, PointerEventData pointer1)
        {
            // タッチを開始・停止したとき、すでにタッチ中の指も含めてタッチ開始位置を再設定する。（新規ドラッグ開始扱いにする）
            // ドラッグ開始時のカクツキを回避するため、もとの pressPosition は使わない。
            if (pointer0 != null) { pointer0.pressPosition = pointer0.position; }
            if (pointer1 != null) { pointer1.pressPosition = pointer1.position; }
            this.pointer0.SetEventData(pointer0);
            this.pointer1.SetEventData(pointer1);
            StartsDrag = true;
            //pressZoom = currentZoom;
            downSeconds = 0f;
        }

        private void SetPointer(PointerEventData eventData)
        {
            // WebGL では pointerId は 0 からの連番ではない

            if (eventData.pointerId == pointer0.PointerID || eventData.pointerId == pointer1.PointerID)
            {
                SetPointers(pointer0.Data, pointer1.Data);
            }
            if (!pointer0.IsHeldDown && pointer1.PointerID != eventData.pointerId)
            {
                SetPointers(eventData, pointer1.Data);
            }
            else if (!pointer1.IsHeldDown && pointer0.PointerID != eventData.pointerId)
            {
                SetPointers(pointer0.Data, eventData);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SetPointer(eventData);

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
                //eventData.dragging = false;
                SetPointer(eventData);
            }

            //// ドラッグしたら長押しを無効化する。
            //readyToLongDown = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // ドラッグ開始は新規タッチ扱いにする。
            SetPointer(eventData);

            // ドラッグしたらクリックを無効化する。
            eventData.eligibleForClick = false;

            // ドラッグしたら長押しを無効化する。
            readyToLongDown = false;
        }
    }
}
