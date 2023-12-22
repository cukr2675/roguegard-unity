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

                var statusEffects = obj.Main.GetStatusEffectState(obj).StatusEffects;
                for (int j = 0; j < statusEffects.Count; j++)
                {
                    var statusEffect = statusEffects[i];
                    if (statusEffect.EffectCategory == EffectCategoryKw.StatusAilment &&
                        statusEffect is IClosableStatusEffect closable)
                    {
                        closable.RemoveClose(obj);
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
