using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    public class WarbroomEquipped : ReferableScript, IEquippedEffectSource
    {
        private WarbroomEquipped() { }

        IEquippedEffect IEquippedEffectSource.CreateOrReuse(RogueObj equipment, IEquippedEffect effect)
        {
            if (effect is Effect effect1 && effect1.Effecter == equipment)
            {
                return effect1;
            }
            else
            {
                return new Effect(equipment);
            }
        }

        private class Effect : BaseEquippedEffect, IStatusEffect, IValueEffect, IRogueMethodPassiveAspect, ISpriteMotionEffect
        {
            string IRogueDescription.Name => "飛行";
            Sprite IRogueDescription.Icon => null;
            Color IRogueDescription.Color => Color.white;
            string IRogueDescription.Caption => null;
            IRogueDetails IRogueDescription.Details => null;
            ISpriteMotion IStatusEffect.HeadIcon => null;

            public IKeyword EffectCategory => StdKw.Vehicle;
            public RogueObj Effecter { get; }

            float IStatusEffect.Order => 0f;
            float IValueEffect.Order => 0f;
            float IRogueMethodPassiveAspect.Order => 0f;
            float ISpriteMotionEffect.Order => 0f;

            private readonly StatusEffect statusEffect;

            public Effect(RogueObj effecter)
            {
                Effecter = effecter;
                statusEffect = new StatusEffect();
                statusEffect.parent = this;
            }

            public override void AddEffect(RogueObj vehicle, RogueObj owner)
            {
                if (vehicle.Location != owner) throw new RogueException();

                owner.Main.Stats.ChargedSpeed++;

                SpeedCalculator.SetDirty(owner);
                MovementCalculator.SetDirty(owner);
                base.AddEffect(vehicle, owner);
                RogueEffectUtility.AddFromRogueEffect(owner, statusEffect);
            }

            public override void RemoveEffect(RogueObj vehicle, RogueObj owner)
            {
                if (vehicle.Location != owner) throw new RogueException();

                SpeedCalculator.SetDirty(owner);
                MovementCalculator.SetDirty(owner);
                base.RemoveEffect(vehicle, owner);
                RogueEffectUtility.Remove(owner, statusEffect);
            }

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.Speed)
                {
                    // 騎乗中は移動速度を +1
                    value.MainValue += 1f;
                }
                if (keyword == StatsKw.Movement)
                {
                    // 騎乗中は飛行状態にする。
                    value.SubValues[StdKw.Levitation] = 1f;
                }
                if (keyword == StatsKw.MPRegenerationPermille)
                {
                    // 騎乗中はMP自然回復を止める。
                    value.MainValue = 0f;
                }
            }

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveChain chain)
            {
                if (keyword == MainInfoKw.Walk)
                {
                    // 騎乗中は移動時に MP-1
                    var stats = self.Main.Stats;
                    stats.SetMP(self, stats.MP - 1);
                    if (stats.MP == 0 && activationDepth < 1f)
                    {
                        // MP が切れたら降ろす
                        var vehicle = RideRogueEffect.GetVehicle(self);
                        var vehicleInfo = VehicleInfo.Get(vehicle);
                        RogueMethodAspectState.Invoke(StdKw.Unride, vehicleInfo.BeUnridden, vehicle, vehicle, 1f, RogueMethodArgument.Identity);
                    }
                }
                else if (method is IActiveRogueMethod && activationDepth == 0f)
                {
                    // 移動以外の能動的行動をしたとき、そこでターンを終了する
                    self.Main.Stats.ChargedSpeed = -100;
                }

                return chain.Invoke(keyword, method, self, user, activationDepth, arg);
            }

            void IStatusEffect.GetEffectedName(RogueNameBuilder refName, RogueObj self) { }

            void ISpriteMotionEffect.ApplyTo(
                ISpriteMotionSet motionSet, IKeyword keyword, int animationTime, RogueDirection direction, ref SkeletalSpriteTransform transform)
            {
                LevitationStatusEffect.MotionApplyTo(motionSet, keyword, animationTime, direction, ref transform);
            }

            private class StatusEffect : IStatusEffect
            {
                public Effect parent;

                public IKeyword EffectCategory => StdKw.Vehicle;
                public RogueObj Effecter => parent.Effecter;
                public ISpriteMotion HeadIcon => null;

                public string Name => "倍速";
                public Sprite Icon => null;
                public Color Color => Color.white;
                public string Caption => null;
                public IRogueDetails Details => null;

                float IStatusEffect.Order => 0f;
                void IStatusEffect.GetEffectedName(RogueNameBuilder refName, RogueObj self) { }
            }
        }
    }
}
