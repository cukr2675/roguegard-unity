using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    [Objforming.Formable]
    public class StandardRogueDeviceData
    {
        public RogueObj Player { get; set; }
        public RogueObj Subject { get; set; }
        public RogueObj World { get; set; }
        public RogueOptions Options { get; set; }
        public IRogueRandom CurrentRandom { get; set; }
        public string SaveDateTime { get; set; }
    }
}
