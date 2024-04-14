using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class ScrollOfDestroyArmorBeApplied : ConsumeApplyRogueMethod
    {
        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => UserRogueMethodRange.Instance;

        protected override bool BeApplied(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var visible = MessageWorkListener.TryOpenHandler(user.Location, user.Position, out var handler);
            if (visible)
            {
                handler.AppendText(user).AppendText("は").AppendText(self).AppendText("を読んだ！\n");
            }

            var equipment = EquipmentUtility.GetRandomArmor(user, RogueRandom.Primary);
            if (equipment != null)
            {
                equipment.TrySetStack(0, user);
                if (visible)
                {
                    handler.AppendText(user).AppendText("が装備していた").AppendText(equipment).AppendText("が砕け散った！\n");
                }
            }
            else
            {
                if (visible)
                {
                    handler.AppendText("しかし何も起こらなかった！\n");
                }
            }
            handler?.Dispose();
            return true;
        }
    }
}
