using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoguegardUnity
{
    public class TouchInputInfo
    {
        public float Zoom { get; set; }

        public float PowedZoom => Mathf.Pow(2f, Zoom); // Not sqr
    }
}
