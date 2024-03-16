using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [ObjectFormer.Formable]
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
            if (MainCharacterWorkUtility.VisibleAt(self.Location, self.Position))
            {
                RogueDevice.Add(DeviceKw.AppendText, self);
                RogueDevice.Add(DeviceKw.AppendText, "は");
                RogueDevice.Add(DeviceKw.AppendText, ammo);
                RogueDevice.Add(DeviceKw.AppendText, "を投げた！\n");
                RogueDevice.Add(DeviceKw.EnqueueSE, MainInfoKw.Skill);
                var item = RogueCharacterWork.CreateSpriteMotion(self, CoreMotions.Discus, false);
                RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
            }

            return this.Throw(ammo, self, activationDepth, targetPosition);
        }
    }
}
