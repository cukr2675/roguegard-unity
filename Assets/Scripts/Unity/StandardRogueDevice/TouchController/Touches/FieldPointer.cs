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
        public int PointerID => Data?.pointerId ?? -2; // -1 は Unity の規定値で使用済みなので -2 を使う
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
            // 一定時間変化のなかったドラッグ操作は中断する。
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
