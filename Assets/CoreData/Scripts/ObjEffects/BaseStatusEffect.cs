using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;
using Roguegard.Extensions;

namespace Roguegard
{
    /// <summary>
    /// 重複して付与できるステータスエフェクト。
    /// </summary>
    public abstract class BaseStatusEffect : IRogueEffect, IStatusEffect, IClosableStatusEffect, IDungeonFloorCloser
    {
        public abstract string Name { get; }
        public virtual Sprite Icon => null;
        public virtual Color Color => Color.white;
        public virtual string Caption => null;
        public virtual IRogueDetails Details => null;

        public abstract IKeyword EffectCategory { get; }
        public virtual RogueObj Effecter => null;
        public virtual ISpriteMotion HeadIcon => null;
        protected virtual float StatusEffectOrder => 0f;
        float IStatusEffect.Order => StatusEffectOrder;

        protected virtual IRogueEffect AffectTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var statusEffect = DeepOrShallowCopy(null, null);
            target.Main.RogueEffects.AddOpen(target, statusEffect);
            return statusEffect;
        }

        public virtual void Open(RogueObj self)
        {
            RogueEffectUtility.AddFromRogueEffect(self, this);
            DungeonFloorCloserStateInfo.AddTo(self, this);
        }

        protected virtual void RemoveClose(RogueObj self, StatusEffectCloseType closeType = StatusEffectCloseType.Manual)
        {
            RogueEffectUtility.RemoveClose(self, this);
            DungeonFloorCloserStateInfo.ReplaceWithNull(self, this);
        }

        void IClosableStatusEffect.RemoveClose(RogueObj self)
        {
            RemoveClose(self, StatusEffectCloseType.Manual);
        }

        void IDungeonFloorCloser.RemoveClose(RogueObj self, bool exitDungeon)
        {
            var statusEffectState = self.Main.GetStatusEffectState(self);
            if (!statusEffectState.TryGetStatusEffect<BaseStatusEffect>(GetType(), out _))
            {
                // 呼び出されたときすでに解除されていた場合は何もせず true を返して削除させる
                // DungeonFloorCloser 以外の方法で解除されたとき必要になる
                return;
            }

            RemoveClose(self, exitDungeon ? StatusEffectCloseType.ExitDungeon : StatusEffectCloseType.ExitDungeonFloor);
        }

        public static bool Close<T>(RogueObj obj)
            where T : BaseStatusEffect
        {
            var statusEffectState = obj.Main.GetStatusEffectState(obj);
            if (statusEffectState.TryGetStatusEffect<T>(out var statusEffect))
            {
                statusEffect.RemoveClose(obj, StatusEffectCloseType.Manual);
                return true;
            }
            return false;
        }

        public virtual void GetEffectedName(RogueNameBuilder refName, RogueObj self)
        {
        }

        public abstract bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other);

        /// <summary>
        /// クローンを生成する場合、クローンの状態は現在の複製にする。
        /// </summary>
        public abstract IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf);

        public virtual IRogueEffect ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;

        /// <summary>
        /// ステータスエフェクトが
        /// <see cref="CoreRogueMethodExtension.Affect(IAffectRogueMethodCaller, RogueObj, RogueObj, float, IAffectCallback)"/>
        /// で付与されるときに呼び出されるメソッド。
        /// </summary>
        protected class AffectCallback : IAffectCallback
        {
            private readonly BaseStatusEffect effect;

            public AffectCallback(BaseStatusEffect effect)
            {
                this.effect = effect;
            }

            IRogueEffect IAffectCallback.AffectTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                var statusEffect = effect.AffectTo(target, user, activationDepth, arg);
                return statusEffect;
            }
        }
    }
}
