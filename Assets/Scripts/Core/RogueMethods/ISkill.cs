using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface ISkill : IActiveRogueMethod, ISkillDescription, IRogueDescription, System.IEquatable<ISkill>
    {
    }
}
