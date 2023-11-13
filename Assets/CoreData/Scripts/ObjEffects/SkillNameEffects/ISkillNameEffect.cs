using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface ISkillNameEffect
    {
        float Order { get; }

        void GetEffectedName(RogueNameBuilder refName, ISkill skill);
    }
}
