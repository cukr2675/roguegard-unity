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
        private class SortedIntrinsic : MPSkillSortedIntrinsic<SortedIntrinsic>
        {
            public override IRogueMethodTarget Target => ForPartyMemberRogueMethodTarget.Instance;
            public override IRogueMethodRange Range => UserRogueMethodRange.Instance;
            public override int RequiredMP => 1;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (MainCharacterWorkUtility.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, ":ActivateSkillMsg::2");
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, this);
                    RogueDevice.Add(DeviceKw.AppendText, "\n");
                    MainCharacterWorkUtility.TryAddSkill(self);
                }

                StatusEffect.Callback.AffectTo(self, self, activationDepth, RogueMethodArgument.Identity);
                return true;
            }

            public override int GetATK(RogueObj self, out bool additionalEffect)
            {
                additionalEffect = true;
                return 0;
            }
        }

        [ObjectFormer.Formable]
        private class StatusEffect : TimeLimitedStackableStatusEffect, IValueEffect, IRogueMethodPassiveAspect
		{
            public static IAffectCallback Callback { get; } = new AffectCallback(new StatusEffect());

			public override string Name => ":ShieldJumpSkill";

			public override IKeyword EffectCategory => null;
            protected override int MaxStack => 1;
            protected override int InitialLifeTime => 2;

            float IValueEffect.Order => 0f;
            float IRogueMethodPassiveAspect.Order => 0f;

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.DEF)
                {
                    // 防御力 +1
                    value.MainValue -= 1f;
                }
            }

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveNext next)
            {
                var useValue = AttackUtility.GetUseValue(arg.RefValue);
                var result = next.Invoke(keyword, method, self, user, activationDepth, arg);

                if (result && keyword == MainInfoKw.Hit && activationDepth < 1f && useValue)
                {
                    // 発動した瞬間に解除
                    RemoveClose(self);

                    var targetPosition = self.Position + self.Main.Stats.Direction.Forward * 2;
                    var targetObj = self.Location.Space.GetColliderObj(targetPosition);
                    var visible = MainCharacterWorkUtility.VisibleAt(self.Location, self.Position);
                    if (visible)
                    {
                        RogueDevice.Add(DeviceKw.AppendText, ":ShieldJumpMsg::1");
                        RogueDevice.Add(DeviceKw.AppendText, self);
                        RogueDevice.Add(DeviceKw.AppendText, "\n");

                        var direction = RogueDirection.FromSignOrLowerLeft(targetPosition - self.Position);
                        var syncItem = RogueCharacterWork.CreateSyncPositioning(self);
                        var item = RogueCharacterWork.CreateWalk(self, targetPosition, direction, KeywordBoneMotion.Walk, false);
                        RogueDevice.AddWork(DeviceKw.EnqueueWork, syncItem);
                        RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
                    }

                    var jumpBack = false;
                    if (targetObj == null)
                    {
                        // 正面2マス先に何もなければそこに移動する
                        jumpBack = !SpaceUtility.TryLocate(self, self.Location, targetPosition);
                    }
                    else
                    {
                        // 正面2マス先に誰かいるときそれに攻撃力(x2)+2ダメージ
                        using var damage = AffectableValue.Get();
                        StatsEffectedValues.GetATK(self, damage);
                        damage.MainValue += damage.BaseMainValue + 2;
                        default(IAffectRogueMethodCaller).Hurt(targetObj, self, 1f, damage);
                        jumpBack = true;
                    }

                    if (jumpBack && visible)
                    {
                        var direction = RogueDirection.FromSignOrLowerLeft(self.Position - targetPosition);
                        var item = RogueCharacterWork.CreateWalk(self, self.Position, direction, KeywordBoneMotion.Walk, false);
                        RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
                    }
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
