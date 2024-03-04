using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class ScrollOfCleansingBeApplied : ConsumeApplyRogueMethod
    {
        public override IRogueMethodTarget Target => ForPartyMemberRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => UserRogueMethodRange.Instance;

        protected override bool BeApplied(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var visible = MainCharacterWorkUtility.VisibleAt(user.Location, user.Position);
            if (visible)
            {
                RogueDevice.Add(DeviceKw.AppendText, user);
                RogueDevice.Add(DeviceKw.AppendText, "は");
                RogueDevice.Add(DeviceKw.AppendText, self);
                RogueDevice.Add(DeviceKw.AppendText, "を読んだ！\n");
            }

            var spaceObjs = user.Space.Objs;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var obj = spaceObjs[i];
                if (obj == null) continue;

                var statusEffectState = obj.Main.GetStatusEffectState(obj);
                for (int j = 0; j < statusEffectState.StatusEffects.Count; j++)
                {
                    var statusEffect = statusEffectState.StatusEffects[j];
                    if (statusEffect.EffectCategory == EffectCategoryKw.Erosion &&
                        statusEffect is IClosableStatusEffect closable)
                    {
                        closable.RemoveClose(obj);
                        j--;
                    }
                }
            }

            if (visible)
            {
                RogueDevice.Add(DeviceKw.AppendText, user);
                RogueDevice.Add(DeviceKw.AppendText, "の持ち物が浄化された！\n");
            }
            return true;
        }
    }
}
