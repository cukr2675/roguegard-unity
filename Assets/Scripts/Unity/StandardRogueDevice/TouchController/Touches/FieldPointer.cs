using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

namespace RoguegardUnity
{
    public class FieldPointer
    {
        public PointerEventData Data { get; private set; }

        public bool IsHeldDown => Data != null;
        public int PointerID => Data?.pointerId ?? -1;
        public bool Dragging
        {
            get => Data.dragging;
            set => Data.dragging = value;
        }
        public bool EligibleForClick
        {
            get => Data.eligibleForClick;
            set => Data.eligibleForClick = value;
        }

        private Vector2 lastPointerPosition;
        private float lastPointerMoveSecond;

        public void SetEventData(PointerEventData eventData)
        {
            Data = eventData;
        }

        public static void SetPointer(PointerEventData eventData, FieldPointer[] pointers)
        {
            var nullIndex = -1;
            for (int i = 0; i < pointers.Length; i++)
            {
                var pointer = pointers[i];
                if (pointer.Data?.pointerId == eventData.pointerId)
                {
                    // ���łɔz����Ɋ܂܂�Ă����牽�����Ȃ�
                    return;
                }
                if (nullIndex == -1 && pointer.Data == null) { nullIndex = i; }
            }

            // �z����ɑ��݂��Ȃ��ꍇ�A
            pointers[nullIndex].Data = eventData;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Data = eventData;

            //if (pointer0 != null && pointer1 != null)
            //{
            //    // ��u�ł���_�^�b�v��ԂɂȂ����Ƃ��A�h���b�O�����ɂ���B
            //    pointer0.dragging = true;
            //    pointer0.eligibleForClick = false;
            //    pointer1.dragging = true;
            //    pointer1.eligibleForClick = false;
            //    readyToLongDown = false;

            //    // �J�������[�h�ɐ؂�ւ������ɓ�_�^�b�v��ԂɂȂ����Ƃ��́A�s���`����݂̂ɂ���B
            //    if (!cameraController.IsCameraMode) { pinchOnly = true; }
            //}
        }
    }
}
