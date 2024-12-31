using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class Vector3KeyFrameList
    {
        public FloatKeyFrameList XKeys { get; }
        public FloatKeyFrameList YKeys { get; }
        public FloatKeyFrameList ZKeys { get; }

        public Vector3KeyFrameList()
        {
            XKeys = new FloatKeyFrameList();
            YKeys = new FloatKeyFrameList();
            ZKeys = new FloatKeyFrameList();
        }

        [Objforming.CreateInstance] private Vector3KeyFrameList(bool dummy) { }
    }
}
