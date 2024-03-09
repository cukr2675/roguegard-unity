using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeDotter
{
    public struct ShiftableColor
    {
        public byte ROrShade { get; set; }
        public byte GOrSaturation { get; set; }
        public byte BOrValue { get; set; }
        public int APercent { get; private set; }
        public bool IsShift { get; set; }

        public ShiftableColor(Color32 pickerColor, bool shift)
        {
            ROrShade = pickerColor.r;
            GOrSaturation = pickerColor.g;
            BOrValue = pickerColor.b;
            APercent = 0;
            IsShift = shift;
            SetA(pickerColor.a);
        }

        private void SetA(int a) => APercent = Mathf.Clamp(a, 0, 100);
        private void SetA(byte a) => APercent = (int)(Mathf.InverseLerp(0f, 255f, a) * 100f);
        public void SetA(float a) => SetA((int)(a * 100f));

        private float GetAlpha()
        {
            if (APercent == 0) return 0f;

            var alpha = APercent == 100 ? .45f : Mathf.Lerp(0.0f, 0.4f, APercent / 100f);
            if (IsShift) { alpha += .5f; }
            return alpha;
        }

        public Color ToSpriteColor()
        {
            return new Color(ROrShade / 255f, GOrSaturation / 255f, BOrValue / 255f, GetAlpha());
        }

        public Color ToPickerColor()
        {
            return new Color(ROrShade / 255f, GOrSaturation / 255f, BOrValue / 255f, APercent / 100f);
        }

        public static ShiftableColor FromColor(Color color)
        {
            var instance = new ShiftableColor();
            instance.ROrShade = (byte)(color.r * 255f);
            instance.GOrSaturation = (byte)(color.g * 255f);
            instance.BOrValue = (byte)(color.b * 255f);
            instance.SetA(Mathf.InverseLerp(0.0f, 0.4f, color.a - (color.a >= .5f ? .5f : 0f)));
            instance.IsShift = color.a >= .5f;
            return instance;
        }

        public Sprite ToIcon()
        {
            var pixels = new Color32[32 * 32];
            var colorA = ToSpriteColor();
            var colorB = IsShift ? Color.gray : colorA;
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    pixels[x + (31 - y) * 32] = (x + y >= 16) ? colorA : colorB;
                }
            }

            var texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            texture.SetPixels32(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, 32f, 32f), new Vector2(.5f, .5f));
        }
    }
}
