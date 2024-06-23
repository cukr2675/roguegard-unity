using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class EngulfingSkill : MPSkillIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(parent, lv);
        }

        [Objforming.Formable]
        private class SortedIntrinsic : MPSkillSortedIntrinsic<SortedIntrinsic>
        {
            public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
            public override IRogueMethodRange Range => FrontRogueMethodRange.Instance;
            public override int RequiredMP => 4;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (RaycastAssert.RequireTarget(FrontRogueMethodRange.Instance, self, arg, out var target)) return false;
                if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h1))
                {
                    using var handler = h1;
                    handler.AppendText(":ActivateSkillMsg::2").AppendText(self).AppendText(this).AppendText("\n");
                    MainCharacterWorkUtility.TryAddSkill(self);
                    MainCharacterWorkUtility.TryAddShot(self);
                }

                // 空間移動
                if (!this.Locate(target, self, self, activationDepth))
                {
                    if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h2))
                    {
                        using var handler = h2;
                        handler.AppendText(":MissMsg").AppendText("\n");
                    }
                    return true;
                }

                // 状態付与
                this.Affect(target, activationDepth, Effect.Callback, user: self);
                if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h3))
                {
                    using var handler = h3;
                    handler.AppendText(":EngulfingMsg::2").AppendText(target).AppendText(self).AppendText("\n");
                }
                return true;
            }

            public override int GetATK(RogueObj self, out bool additionalEffect)
            {
                additionalEffect = true;
                return 1;
            }
        }

        [Objforming.Formable]
        private class Effect : StackableStatusEffect, IRogueObjUpdater, IValueEffect
        {
            public static IAffectCallback Callback { get; } = new AffectCallback(new Effect());

            public override string Name => ":EngulfingSkill";
            protected override int MaxStack => 1;
            public override IKeyword EffectCategory => null;

            float IRogueObjUpdater.Order => 100f;
            float IValueEffect.Order => 0f;

            private RogueObj user;
            private int count;

            private Effect() { }

            public override void Open(RogueObj self)
            {
                base.Open(self);
                SpeedCalculator.SetDirty(self);
            }

            protected override void RemoveClose(RogueObj self, StatusEffectCloseType closeType = StatusEffectCloseType.Manual)
            {
                base.RemoveClose(self);
                SpeedCalculator.SetDirty(self);
            }

            RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                // 対象の中にいなければ解除する
                if (self.Location == null || self.Location != user)
                {
                    RemoveClose(self);
                    return default;
                }

                // 1 の固定ダメージ
                using var damage = AffectableValue.Get();
                damage.Initialize(1f);
                damage.SubValues[StatsKw.GuaranteedDamage] = 1f;
                damage.SubValues[StatsKw.GuardRate] = 0.1f * count; // 1 ターンでガード率 10% 上昇
                this.Hurt(self, user, AttackUtility.GetActivationDepthCantCounter(activationDepth), damage);
                this.TryDefeat(self, user, activationDepth, damage);

                count++;

                // ガードに成功するか 10 ターン経過したら解除する
                if (damage.SubValues.Is(StatsKw.Guard) || count >= 10)
                {
                    // 親オブジェクトの隣に空間移動する
                    for (int i = 0; i < 8; i++)
                    {
                        var direction = new RogueDirection(i);
                        if (this.Locate(self, null, self.Location.Location, self.Location.Position + direction.Forward, activationDepth)) break;
                    }

                    // 移動に失敗したらランダム位置へワープする。それでも失敗したら変化なし
                    if (self.Location == user)
                    {
                        var position = self.Location.Location.Space.GetRandomPositionInRoom(RogueRandom.Primary);
                        if (!this.Locate(self, null, self.Location.Location, position, activationDepth)) return default;
                    }

                    RemoveClose(self);
                    return default;
                }

                return default;
            }

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.Speed)
                {
                    // 行動不可
                    value.SubValues[StatsKw.BeInhibited] = 1f;
                }
            }

            public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
            {
                return other is Effect effect && effect.Stack == Stack && effect.user == user && effect.count == count;
            }

            public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return new Effect()
                {
                    Stack = Stack,
                    user = user,
                    count = count
                };
            }

            private new class AffectCallback : IAffectCallback
            {
                private readonly Effect effect;

                public AffectCallback(Effect effect)
                {
                    this.effect = effect;
                }

                IRogueEffect IAffectCallback.AffectTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg)
                {
                    var statusEffect = effect.AffectTo(target, user, activationDepth, arg);
                    ((Effect)statusEffect).user = user;
                    return statusEffect;
                }
            }
        }
    }
}
