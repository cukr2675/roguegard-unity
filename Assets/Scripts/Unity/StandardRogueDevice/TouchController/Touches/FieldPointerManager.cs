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
            // ����������
            if (readyToLongDown)
            {
                // ���������Ԃ����𒴂����Ƃ����s
                if (downSeconds >= LongDownSeconds)
                {
                    var pointer = pointer0 ?? pointer1;
                    IsPointing = true;
                    PointingPosition = GetCellPoint(pointer.position);
                    IsLongDown = true;
                    pointer0 = pointer1 = null;
                    readyToLongDown = false;
                }

                // ���������Ԃ�i�߂�
                downSeconds += Time.deltaTime;
            }

            // �s���`�C���E�s���`�A�E�g
            var pinch = pointer0 != null && pointer1 != null;
            if (pinch)
            {
                var pressDistance = Vector2.Distance(pointer0.Data.pressPosition, pointer1.Data.pressPosition);
                var distance = Vector2.Distance(pointer0.Data.position, pointer1.Data.position);
                info.Zoom = pressZoom + Mathf.Log(distance / pressDistance, 2f);
            }
            else if (lastPinch && info.PowedZoom >= 1f * (1f - ZoomClippingRadius))
            {
                // �s���`�C���E�s���`�A�E�g�I����A�N���b�v�͈͓��Ȃ�Y�[���{���𐮐��ɂ���B�i�h�b�g�𐳕��`�ɂ��邽�߁j
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
            // �^�b�`���J�n�E��~�����Ƃ��A���łɃ^�b�`���̎w���܂߂ă^�b�`�J�n�ʒu���Đݒ肷��B�i�V�K�h���b�O�J�n�����ɂ���j
            // �h���b�O�J�n���̃J�N����������邽�߁A���Ƃ� pressPosition �͎g��Ȃ��B
            if (pointer0 != null) { pointer0.pressPosition = pointer0.position; }
            if (pointer1 != null) { pointer1.pressPosition = pointer1.position; }
            this.pointer0.SetEventData(pointer0);
            this.pointer1.SetEventData(pointer1);
            StartsDrag = true;
            pressZoom = info.Zoom;
            downSeconds = 0f;
        }

        /// <summary>
        /// ��ʂ������n�߂��Ƃ��ɌĂяo�����\�b�h
        /// </summary>
        private void PointerDown(PointerEventData eventData)
        {
            // WebGL �ł� pointerId �� 0 ����̘A�Ԃł͂Ȃ�

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
                // �ʏ�̃h���b�O����Ƌ�ʂ��邽�߁A������� ID �Ƃ���v���Ȃ����Ƃ��m�F����B
                //eventData.dragging = false;
                PointerDown(eventData);
            }

            //// �h���b�O�����璷�����𖳌�������B
            //readyToLongDown = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // �h���b�O�J�n�͐V�K�^�b�`�����ɂ���B
            PointerDown(eventData);

            // �h���b�O������N���b�N�𖳌�������B
            eventData.eligibleForClick = false;

            // �h���b�O�����璷�����𖳌�������B
            readyToLongDown = false;
        }
    }
}
