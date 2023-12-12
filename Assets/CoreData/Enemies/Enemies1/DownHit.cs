using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class DownHit : ReferableScript, IAffectRogueMethod
    {
        private DownHit() { }

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var useValue = arg.RefValue != null;
            if (useValue)
            {
                var damage = arg.RefValue.MainValue != 0f && !arg.RefValue.SubValues.Is(StdKw.Heal);
                var result = CommonHit.Instance.Invoke(self, user, activationDepth, arg);

                if (damage && !arg.RefValue.SubValues.Is(MainInfoKw.BeDefeated))
                {
                    // ダメージの判定が出たうえで倒されていなければ、転倒状態になる。
                    DownStatusEffect.Callback.AffectTo(self, user, activationDepth, RogueMethodArgument.Identity);
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
