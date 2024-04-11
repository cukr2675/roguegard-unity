using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IAttachedRogueDetails
    {
        public Spanning<object> Attachments { get; }
    }
}
