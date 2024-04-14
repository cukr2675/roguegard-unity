using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class FloatingEyeHit : ReferableScript, IAffectRogueMethod
    {
        private FloatingEyeHit() { }

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var useValue = AttackUtility.GetUseValue(arg.RefValue);
            if (useValue)
            {
                var damage = !arg.RefValue.SubValues.Is(StdKw.Heal);
                var result = CommonHit.Instance.Invoke(self, user, activationDepth, arg);

                if (damage && activationDepth < 1f && !arg.RefValue.SubValues.Is(MainInfoKw.BeDefeated) && RogueMethodUtility.GetAdjacent(self, user))
                {
                    // ダメージの判定が出たうえで倒されておらず、かつ隣接していれば、しびれにらみで反撃する。
                    if (MessageWorkListener.TryOpenHandler(user.Location, user.Position, out var h))
                    {
                        using var handler = h;
                        handler.AppendText(self).AppendText("は").AppendText(user).AppendText("をにらみ返した！\n");
                    }
                    default(IActiveRogueMethodCaller).TryAffect(user, 1f, ParalysisStatusEffect.Callback, null, self);
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
