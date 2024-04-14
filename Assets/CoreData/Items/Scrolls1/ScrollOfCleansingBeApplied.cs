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
            var visible = MessageWorkListener.TryOpenHandler(user.Location, user.Position, out var handler);
            if (visible)
            {
                handler.AppendText(user).AppendText("は").AppendText(self).AppendText("を読んだ！\n");
            }

            var spaceObjs = user.Space.Objs;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var obj = spaceObjs[i];
                if (obj == null) continue;

                EquipmentUtility.Cleansing(obj);
            }

            if (visible)
            {
                handler.AppendText(user).AppendText("の持ち物が浄化された！\n");
                handler.Dispose();
            }
            return true;
        }
    }
}
