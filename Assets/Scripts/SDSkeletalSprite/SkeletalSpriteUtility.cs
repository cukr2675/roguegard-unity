using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDSSprite
{
    public static class SDSSpriteUtility
    {
        public static int DefaultPixelsPerUnit => 32;
        public static float LightDarkThreshold => .4f;

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
