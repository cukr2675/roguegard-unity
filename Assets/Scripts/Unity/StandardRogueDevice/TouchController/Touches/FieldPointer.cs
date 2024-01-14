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
        public int PointerID => Data?.pointerId ?? -2; // -1 �� Unity �̋K��l�Ŏg�p�ς݂Ȃ̂� -2 ���g��
        public Vector2 Position
        {
            get => Data.position;
            set => Data.position = value;
        }
        public Vector2 PressPosition
        {
            get => Data.pressPosition;
            set => Data.pressPosition = value;
        }
        public bool Dragging
        {
            get => Data?.dragging ?? false;
            set => Data.dragging = value;
        }
        public bool EligibleForClick
        {
            get => Data?.eligibleForClick ?? false;
            set => Data.eligibleForClick = value;
        }

        public float TouchResetTime { get; set; }

        private Vector2 lastPointerPosition;
        private float lastPointerMoveSecond;

        public void Update(float deltaTime)
        {
            // ��莞�ԕω��̂Ȃ������h���b�O����͒��f����B
            if (IsHeldDown && Dragging)
            {
                if (Position == lastPointerPosition)
                {
                    lastPointerMoveSecond += deltaTime;
                    if (lastPointerMoveSecond >= TouchResetTime)
                    {
                        Data = null;
                    }
                }
                else
                {
                    lastPointerMoveSecond = 0;
                    lastPointerPosition = Position;
                }
            }
        }

        public void SetEventData(PointerEventData eventData)
        {
            Data = eventData;
        }
    }
}
