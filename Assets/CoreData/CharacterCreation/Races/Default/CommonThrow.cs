using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class CommonThrow : MPSkill
    {
        public override string Name => MainInfoKw.Throw.Name;

        public override IRogueMethodTarget Target => DependsOnThrownRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => DependsOnThrownRogueMethodRange.Instance;
        public override int RequiredMP => 0;

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.RequireAmmo(self, arg, out var ammo) || ammo.AsTile) return false;

            if (!arg.TryGetTargetPosition(out var targetPosition))
            {
                targetPosition = self.Position + self.Main.Stats.Direction.Forward;
            }

            self.Main.Stats.Direction = RogueMethodUtility.GetTargetDirection(self, arg);
            if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(self).AppendText("は").AppendText(ammo).AppendText("を投げた！\n");
                handler.EnqueueSE(MainInfoKw.Skill);
                handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, CoreMotions.Discus, false));
            }

            return this.Throw(ammo, self, activationDepth, targetPosition);
        }
    }
}
