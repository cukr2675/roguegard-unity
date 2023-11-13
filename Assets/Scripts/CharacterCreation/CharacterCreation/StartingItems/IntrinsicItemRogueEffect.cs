using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// 固有アイテムに付与する <see cref="IRogueEffect"/> 。
    /// </summary>
    [ObjectFormer.Formable]
    public class IntrinsicItemRogueEffect : IRogueEffect, IValueEffect, IRogueMethodPassiveAspect
    {
        private static readonly IntrinsicItemRogueEffect instance = new IntrinsicItemRogueEffect();
        private IntrinsicItemRogueEffect() { }

        float IValueEffect.Order => 100f;
        float IRogueMethodPassiveAspect.Order => 0f;

        public static void SetTo(RogueObj obj)
        {
            obj.Main.RogueEffects.AddOpen(obj, instance);
        }

        public static bool Is(RogueObj obj)
        {
            return obj.Main?.RogueEffects.Contains(instance) ?? false;
        }

        void IRogueEffect.Open(RogueObj self)
        {
            RogueEffectUtility.AddFromRogueEffect(self, this);
            MovementCalculator.SetDirty(self);
        }

        void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
        {
            if (keyword == StatsKw.Weight)
            {
                // 固有アイテムの重さはゼロにする。
                value.MainValue = 0f;
            }
            if (keyword == StatsKw.Movement)
            {
                // 固有アイテムの空間移動を禁止する。
                value.SubValues[CharacterCreationKw.Glued] = 1f;
            }
        }

        bool IRogueMethodPassiveAspect.PassiveInvoke(
            IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
            RogueMethodAspectState.PassiveNext next)
        {
            if (keyword == MainInfoKw.Locate || keyword == MainInfoKw.Polymorph)
            {
                // 固有アイテムの空間移動と変化を禁止する。
                return false;
            }

            return next.Invoke(keyword, method, self, user, activationDepth, arg);
        }

        bool IRogueEffect.CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => false;
        IRogueEffect IRogueEffect.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => this;
        IRogueEffect IRogueEffect.ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        public override bool Equals(object obj) => obj.GetType() == GetType();
        public override int GetHashCode() => GetType().GetHashCode();
    }
}
