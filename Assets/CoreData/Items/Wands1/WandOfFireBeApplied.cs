using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class WandOfFireBeApplied : ConsumeApplyRogueMethod
    {
        private WandOfFireBeApplied() { }

        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => LineOfSight10RogueMethodRange.Instance;

        protected override bool BeApplied(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (MessageWorkListener.TryOpenHandler(user.Location, user.Position, out var h))
            {
                using var handler = h;
                handler.EnqueueWork(RogueCharacterWork.CreateSyncPositioning(user));
            }
            var direction = user.Main.Stats.Direction;
            ((IProjectileRogueMethodRange)LineOfSight10RogueMethodRange.Instance).Raycast(
                user.Location, user.Position, direction, true, true, out var hitObj, out _, out _);

            using var damageValue = AffectableValue.Get();
            damageValue.Initialize(3f);
            //damageValue.SubValues[CommonKw.Magic] = 1f;
            //damageValue.SubValues[CommonKw.Fire] = 1f;
            this.Hurt(hitObj, user, activationDepth, damageValue);
            return true;
        }
    }
}
