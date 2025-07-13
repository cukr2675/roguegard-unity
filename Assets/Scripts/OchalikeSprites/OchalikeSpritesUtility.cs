using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    public static class OchalikeSpritesUtility
    {
        public static int DefaultPixelsPerUnit => 32;

        private static float LightDarkThreshold => .4f;

        /// <summary>
        /// 指定の色がグレーの輪郭線と被るか判定する。被る場合は黒い輪郭線を使用する
        /// </summary>
        public static bool IsSimilarToLightOutline(Color hairColor)
        {
            // ユークリッド距離で十分そうなので使わない
            //return CalculateCie76(hairColor, Color.white * .25f) < LightDarkThreshold * 100f;

            var c = new Vector3(hairColor.r, hairColor.g, hairColor.b);
            var outlineColor = Vector3.one * .25f; // まぶたと顔の輪郭線のグレー
            return Vector3.Distance(c, outlineColor) < LightDarkThreshold;
        }

        private static float CalculateCie76(Color color1, Color color2)
        {
            var xyz1 = Rgb2Xyz(color1);
            var xyz2 = Rgb2Xyz(color2);

            var lab1 = Xyz2Lab(xyz1);
            var lab2 = Xyz2Lab(xyz2);

            return Vector3.Distance(lab1, lab2);
        }

        private static Vector3 Rgb2Xyz(Color sRGB)
        {
            var r = sRGB.r > 0.04045f ? Mathf.Pow((sRGB.r + 0.055f) / 1.055f, 2.4f) : (sRGB.r / 12.92f);
            var g = sRGB.g > 0.04045f ? Mathf.Pow((sRGB.g + 0.055f) / 1.055f, 2.4f) : (sRGB.g / 12.92f);
            var b = sRGB.b > 0.04045f ? Mathf.Pow((sRGB.b + 0.055f) / 1.055f, 2.4f) : (sRGB.b / 12.92f);

            var x = r * 0.4124564f + g * 0.3575761f + b * 0.1804375f;
            var y = r * 0.2126729f + g * 0.7151522f + b * 0.0721750f;
            var z = r * 0.0193339f + g * 0.1191920f + b * 0.9503041f;
            return new Vector3(x, y, z);
        }

        private static Vector3 Xyz2Lab(Vector3 xyz)
        {
            var x = xyz.x / 0.95047f;
            var y = xyz.y / 1.00000f;
            var z = xyz.z / 1.08883f;

            x = x > 0.008856f ? Mathf.Pow(x, 1.0f / 3.0f) : (7.787f * x) + (16.0f / 116.0f);
            y = y > 0.008856f ? Mathf.Pow(y, 1.0f / 3.0f) : (7.787f * y) + (16.0f / 116.0f);
            z = z > 0.008856f ? Mathf.Pow(z, 1.0f / 3.0f) : (7.787f * z) + (16.0f / 116.0f);

            var L = (116.0f * y) - 16.0f;
            var a = 500.0f * (x - y);
            var b = 200.0f * (y - z);
            return new Vector3(L, a, b);
        }

        /// <summary>
        /// <paramref name="resolution"/> 個に分割した角度のうち、右向きから反時計回りでの <paramref name="degree"/> 度が何個目にあたるかを取得する。
        /// </summary>
        private static int GetAngle(float degree, int resolution)
        {
            degree = Mathf.Repeat(degree + 180f / resolution, 360f);
            for (int i = 1; i < resolution; i++)
            {
                if (degree < 360f * i / resolution) return i - 1;
            }
            return resolution - 1;
        }
    }
}
