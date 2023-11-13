using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class BowThrow : MPSkill
    {
		public override string Name => MainInfoKw.Throw.Name;

		public override IRogueMethodTarget Target => DependsOnShotRogueMethodTarget.Instance;
		public override IRogueMethodRange Range => DependsOnShotRogueMethodRange.Instance;
        public override Spanning<IKeyword> AmmoCategories => lazyAmmoCategories.Value;
        private static readonly System.Lazy<IKeyword[]> lazyAmmoCategories = new System.Lazy<IKeyword[]>(() => new IKeyword[] { AmmoKw.Arrow });
        public override int RequiredMP => 0;

        protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.RequireMatchedAmmo(self, arg, AmmoCategories, out var ammo, out var ammoInfo)) return false;

            self.Main.Stats.Direction = RogueMethodUtility.GetTargetDirection(self, arg);
            if (arg.TryGetTargetPosition(out var targetPosition))
            {
                return this.Shot(ammoInfo, ammo, self, activationDepth, targetPosition, arg.TargetObj);
            }
            else
            {
                return this.Shot(ammoInfo, ammo, self, activationDepth);
            }
        }

        public override int GetATK(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = true;
            return 0;
        }
    }
}
