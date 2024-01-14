using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoguegardUnity
{
    public class MouseInputManager
    {
        private readonly TouchInputInfo info;

        private Vector2 scrollDelta;

        public float[] WheelZoomingFilters { get; set; }

        public MouseInputManager(TouchInputInfo info)
        {
            this.info = info;
        }

        public void Update(float deltaTime)
        {
            // マウスホイール操作を毎フレーム取得する
            scrollDelta += Input.mouseScrollDelta;
        }

        public void UpdateField()
        {
            // マウスホイール
            if (scrollDelta.y != 0f)
            {
                var zoomIndex = GetZoomIndex(info.Zoom, scrollDelta.y);
                var newZoom = WheelZoomingFilters[zoomIndex];

                // マウスの位置を中心にズームするようにスクロールする
                var screenCenter = new Vector3(Screen.width, Screen.height) / 2f;
                info.DeltaPosition = Input.mousePosition - screenCenter;
                info.DeltaPosition *= (newZoom - info.Zoom) / 50f;

                info.DragRelativePosition = -(Input.mousePosition - screenCenter) * (Mathf.Pow(2f, newZoom - info.Zoom) - 1f);

                info.DragRelativePosition /= Mathf.Pow(2f, newZoom);

                info.Zoom = newZoom;
                scrollDelta = Vector2.zero;
            }
            else
            {
                info.DeltaPosition = Vector3.zero;
            }

            int GetZoomIndex(float currentZoom, float delta)
            {
                var currentZoomIndex = WheelZoomingFilters.Length - 1;
                for (int i = 0; i < WheelZoomingFilters.Length; i++)
                {
                    if (WheelZoomingFilters[i] > currentZoom)
                    {
                        currentZoomIndex = i - 1;
                        break;
                    }
                }
                currentZoomIndex += Mathf.FloorToInt(delta);
                currentZoomIndex = Mathf.Clamp(currentZoomIndex, 0, WheelZoomingFilters.Length - 1);
                return currentZoomIndex;
            }
        }
    }
}
