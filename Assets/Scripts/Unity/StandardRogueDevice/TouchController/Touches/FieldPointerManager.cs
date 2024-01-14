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
            // ����������
            if (readyToLongDown)
            {
                // ���������Ԃ����𒴂����Ƃ����s
                if (downSeconds >= LongDownSeconds)
                {
                    var pointer = pointer0.Data ?? pointer1.Data;
                    info.SetPointing(GetCellPoint(pointer.position), longDown: true);
                    ClearTouches();
                }

                // ���������Ԃ�i�߂�
                downSeconds += Time.deltaTime;
            }

            // �s���`�C���E�s���`�A�E�g
            var pinch = pointer0.IsHeldDown && pointer1.IsHeldDown;
            if (pinch)
            {
                var pressDistance = Vector2.Distance(pointer0.PressPosition, pointer1.PressPosition);
                var distance = Vector2.Distance(pointer0.Position, pointer1.Position);
                info.Zoom = pressZoom + Mathf.Log(distance / pressDistance, 2f);
            }
            else if (lastPinch && info.PowedZoom >= 1f - ZoomClippingRadius)
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

        public void UpdateDrag()
        {
            info.Drag = !pinchOnly && (pointer0.Dragging || pointer1.Dragging);

            if (pinchOnly)
            {
                // pinchOnly == true �̃s���`�C���E�s���`�A�E�g���̓J�����ړ��𖳌�������B
                if (pointer0.IsHeldDown && pointer1.IsHeldDown) return;

                // pinchOnly == true �̃s���`�C���E�s���`�A�E�g���I�������Ƃ��A�c�����w�͔�h���b�O�̐V�K�^�b�`�����ɂ���B
                // �i�������N���b�N�͖����̂܂܂ɂ���j
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
                // �h���b�O���n�߂��u�Ԃ̃X�N���[���͖���
                info.DragRelativePosition = Vector2.zero;
            }
            else
            {
                if (pointer0.IsHeldDown && pointer1.IsHeldDown)
                {
                    // �s���`�C���E�s���`�A�E�g�̎w�Ǝw�̒��ԓ_�ɃY�[������悤�ɁA�h���b�O�ʒu��␳����B
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

            // �^�b�`���肪�W�����v���錻�ۂ��y������
            if (pointer0.IsHeldDown && pointer1.IsHeldDown)
            {
                // �����^�b�`���͎w�̊Ԃ̋������L�^����
                lastDistance = Vector2.Distance(pointer0.Position, pointer1.Position);
            }
            else if (pointer0.IsHeldDown || pointer1.IsHeldDown)
            {
                // �Е��̎w�𗣂������ƁA�L�^����������蒷���������ړ����Ă����炻�̑���𖳌�������
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
            // �^�b�`���J�n�E��~�����Ƃ��A���łɃ^�b�`���̎w���܂߂ă^�b�`�J�n�ʒu���Đݒ肷��B�i�V�K�h���b�O�J�n�����ɂ���j
            // �h���b�O�J�n���̃J�N����������邽�߁A���Ƃ� pressPosition �͎g��Ȃ��B
            if (pointer0 != null) { pointer0.pressPosition = pointer0.position; }
            if (pointer1 != null) { pointer1.pressPosition = pointer1.position; }
            this.pointer0.SetEventData(pointer0);
            this.pointer1.SetEventData(pointer1);
            info.StartsDrag = true;
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

        public void OnPointerClick(PointerEventData eventData)
        {
            // �}�E�X�̏ꍇ�͍��N���b�N�݈̂���
            // �^�b�`����ł͂��ׂč��N���b�N����
            if (eventData.pointerId < 0 && eventData.button != PointerEventData.InputButton.Left) return;

            if (info.IsPointing)
            {
                // ���Ƀ|�C���e�B���O����Ă�����L�����Z������B
                info.ClearPointing();
                return;
            }

            // �J�������[�h���Ƀ|�C���e�B���O���ꂽ�瑁���肷��B
            info.SetPointing(GetCellPoint(eventData.position), fastForward: cameraController.IsCameraMode);

            info.IsClick = true;
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
                // ���f�����^�b�`�������ĊJ����B
                // �ʏ�̃h���b�O����Ƌ�ʂ��邽�߁A������� ID �Ƃ���v���Ȃ����Ƃ��m�F����B
                PointerDown(eventData);
            }
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
