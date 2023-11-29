using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class ShieldJumpSkill : MPSkillIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(parent, lv);
        }

        [ObjectFormer.Formable]
        private class SortedIntrinsic : MPSkillSortedIntrinsic<BeamBulletSkill>
        {
            public override IRogueMethodTarget Target => ForPartyMemberRogueMethodTarget.Instance;
            public override IRogueMethodRange Range => UserRogueMethodRange.Instance;
            public override int RequiredMP => 1;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (RaycastAssert.RequireTarget(LineOfSight10RogueMethodRange.Instance, self, arg, out var target)) return false;
                if (RogueDevice.Primary.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, ":ActivateSkillMsg::2");
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, this);
                    RogueDevice.Add(DeviceKw.AppendText, "\n");
                    MainCharacterWorkUtility.TryAddSkill(self);
                    MainCharacterWorkUtility.TryAddShot(self);
                }

                // 攻撃力(x2)ダメージの攻撃。
                using var damageValue = AffectableValue.Get();
                StatsEffectedValues.GetATK(self, damageValue);
                damageValue.MainValue += damageValue.BaseMainValue;
                damageValue.SubValues[MainInfoKw.Skill] = 1f;
                this.TryHurt(target, self, activationDepth, damageValue);
                this.TryDefeat(target, self, activationDepth, damageValue);
                return true;
            }

            public override int GetATK(RogueObj self, out bool additionalEffect)
            {
                // 攻撃力(x2)ダメージの攻撃。
                using var damageValue = AffectableValue.Get();
                StatsEffectedValues.GetATK(self, damageValue);
                damageValue.MainValue += damageValue.BaseMainValue;

                var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
                additionalEffect = false;
                return hpDamage;
            }
        }

		private class StatusEffect : BaseStatusEffect, IValueEffect, IRogueMethodPassiveAspect
		{
            public static IAffectCallback Callback { get; } = new AffectCallback(new StatusEffect());

			public override string Name => ":ShieldJumpSkill";

			public override IKeyword EffectCategory => null;

            float IValueEffect.Order => 0f;
            float IRogueMethodPassiveAspect.Order => 0f;

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.DEF)
                {
                    // 防御力 +1
                    value.MainValue += 1f;
                }
            }

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveNext next)
            {
                var result = next.Invoke(keyword, method, self, user, activationDepth, arg);

                if (result && keyword == MainInfoKw.Hit && activationDepth < 1f && arg.RefValue?.MainValue > 0f)
                {
                    // 正面2マス先に移動する
                    //this.
                }
                return result;
            }

            public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
			{
                return other is StatusEffect;
			}

			public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
			{
                return this;
			}

			public override bool Equals(object obj)
			{
                return obj is StatusEffect;
			}

			public override int GetHashCode()
			{
				return typeof(StatusEffect).GetHashCode();
			}
		}
	}
}
