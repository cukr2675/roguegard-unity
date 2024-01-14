using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoguegardUnity
{
    public class TouchInputInfo
    {
        public bool IsClick { get; set; }

        public bool IsPointing { get; private set; }
        public Vector2Int PointingPosition { get; private set; }
        public bool FastForward { get; private set; }
        public bool IsLongDown { get; private set; }

        public bool StartsDrag { get; set; }
        public bool Drag { get; set; }
        public Vector2 DragRelativePosition { get; set; }

        public Vector3 DeltaPosition { get; set; }

        public float Zoom { get; set; }

        // Zoom ^ 2 ‚Å‚Í‚È‚­ 2 ^ Zoom ‚È‚Ì‚Å sqr ‚Å‚Í‚È‚¢
        public float PowedZoom => Mathf.Pow(2f, Zoom);

        public void SetPointing(Vector2Int position, bool fastForward = false, bool longDown = false)
        {
            IsPointing = true;
            PointingPosition = position;
            FastForward = fastForward;
            IsLongDown = longDown;
        }

        public void ClearPointing()
        {
            IsPointing = false;
            FastForward = false;
            IsLongDown = false;
        }
    }
}
