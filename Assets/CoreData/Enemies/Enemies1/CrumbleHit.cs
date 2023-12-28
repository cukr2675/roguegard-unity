using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CrumbleHit : ReferableScript, IAffectRogueMethod
    {
        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var useValue = AttackUtility.GetUseValue(arg.RefValue);
            if (useValue)
            {
                var damage = !arg.RefValue.SubValues.Is(StdKw.Heal);
                var result = CommonHit.Instance.Invoke(self, user, activationDepth, arg);

                if (damage && !arg.RefValue.SubValues.Is(MainInfoKw.BeDefeated))
                {
                    // 倒されていなければ、防御力 -1
                    CrumbleStatusEffect.Callback.AffectTo(self, user, activationDepth, RogueMethodArgument.Identity);
                }
                return result;
            }
            else
            {
                var result = CommonHit.Instance.Invoke(self, user, activationDepth, arg);
                return result;
            }
        }
    }
}
