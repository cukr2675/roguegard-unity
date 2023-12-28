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
            // �^�b�`���J�n�E��~�����Ƃ��A���łɃ^�b�`���̎w���܂߂ă^�b�`�J�n�ʒu���Đݒ肷��B�i�V�K�h���b�O�J�n�����ɂ���j
            // �h���b�O�J�n���̃J�N�c�L��������邽�߁A���Ƃ� pressPosition �͎g��Ȃ��B
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
            // WebGL �ł� pointerId �� 0 ����̘A�Ԃł͂Ȃ�

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
                // ��u�ł���_�^�b�v��ԂɂȂ����Ƃ��A�h���b�O�����ɂ���B
                pointer0.Dragging = true;
                pointer0.EligibleForClick = false;
                pointer1.Dragging = true;
                pointer1.EligibleForClick = false;
                readyToLongDown = false;

                // �J�������[�h�ɐ؂�ւ������ɓ�_�^�b�v��ԂɂȂ����Ƃ��́A�s���`����݂̂ɂ���B
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
                // ���f�����^�b�`�������ĊJ����B
                //eventData.dragging = false;
                SetPointer(eventData);
            }

            //// �h���b�O�����璷�����𖳌�������B
            //readyToLongDown = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // �h���b�O�J�n�͐V�K�^�b�`�����ɂ���B
            SetPointer(eventData);

            // �h���b�O������N���b�N�𖳌�������B
            eventData.eligibleForClick = false;

            // �h���b�O�����璷�����𖳌�������B
            readyToLongDown = false;
        }
    }
}
