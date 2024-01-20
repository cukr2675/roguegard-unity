using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class RoguePaintColor
    {
        public byte ROrShade { get; set; }
        public byte GOrSaturation { get; set; }
        public byte BOrValue { get; set; }

        private float _a;
        public float A
        {
            get
            {
                return _a;
            }
            set
            {
                if (value < 0f || 1f < value) throw new System.ArgumentOutOfRangeException();

                _a = value;
            }
        }

        public byte ByteA => (byte)Mathf.Lerp(0f, 255f, _a);

        public bool IsShift { get; set; }

        public RoguePaintColor() { }

        public RoguePaintColor(RoguePaintColor color)
        {
            Set(color);
        }

        public void Set(RoguePaintColor color)
        {
            ROrShade = color.ROrShade;
            GOrSaturation = color.GOrSaturation;
            BOrValue = color.BOrValue;
            _a = color._a;
            IsShift = color.IsShift;
        }

        private float GetAlpha()
        {
            if (_a == 0f) return 0f;

            var alpha = _a == 1f ? .45f : Mathf.Lerp(0.0f, 0.4f, _a);
            if (IsShift) { alpha += .5f; }
            return alpha;
        }

        public Color ToColor()
        {
            return new Color(ROrShade / 255f, GOrSaturation / 255f, BOrValue / 255f, GetAlpha());
        }

        public static RoguePaintColor FromColor(Color color)
        {
            var instance = new RoguePaintColor();
            instance.ROrShade = (byte)(color.r * 255f);
            instance.GOrSaturation = (byte)(color.g * 255f);
            instance.BOrValue = (byte)(color.b * 255f);
            instance.A = Mathf.InverseLerp(0.0f, 0.4f, color.a - (color.a >= .5f ? .5f : 0f));
            instance.IsShift = color.a >= .5f;
            return instance;
        }
    }
}
