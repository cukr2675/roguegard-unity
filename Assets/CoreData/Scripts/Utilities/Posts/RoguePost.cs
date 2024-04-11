using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class RoguePost
    {
        public string Name { get; set; }
        public IRogueDetails Details { get; set; }
        public RogueObj From { get; set; }
        public string DateTime { get; set; }
        public RoguePostLiveState LiveState { get; set; }
        public bool IsRead { get; set; }
    }
}
