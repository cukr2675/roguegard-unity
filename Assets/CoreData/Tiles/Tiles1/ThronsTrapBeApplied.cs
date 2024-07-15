using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class ThronsTrapBeApplied : BaseApplyRogueMethod
    {
        [SerializeField] private ScriptableRogueTileInfo trapTileInfo = null;

        private IAffectCallback callback;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (arg.Other is UserRogueTile userTile && !StatsEffectedValues.AreVS(userTile.User, user))
            {
                // 敵対していないキャラが罠を踏んでも起動しないようにする
                return false;
            }

            if (callback == null) { callback = Effect.CreateCallback(trapTileInfo); }
            return this.TryAffect(user, activationDepth, callback);
        }

        [Objforming.Formable]
        private class Effect : StackableStatusEffect, IRogueObjUpdater
        {
            public override string Name => ":ThronsTrap";
            protected override int MaxStack => 1;
            public override IKeyword EffectCategory => null;

            float IRogueObjUpdater.Order => 100f;

            private IRogueTileInfo trapTileInfo;

            private Effect() { }

            public static IAffectCallback CreateCallback(IRogueTileInfo trapTileInfo)
            {
                return new AffectCallback(new Effect(), trapTileInfo);
            }

            RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                var tile = GetTile(self);
                if (tile == null || !tile.Info.Equals(trapTileInfo))
                {
                    // 足元が罠でなければ解除する
                    RogueEffectUtility.RemoveClose(self, this);
                    return default;
                }

                if (tile is UserRogueTile userTile)
                {
                    var user = userTile.User;

                    // 基礎攻撃力ぶんの継続ダメージ
                    using var damage = EffectableValue.Get();
                    StatsEffectedValues.GetATK(user, damage);
                    damage.Initialize(damage.BaseMainValue);
                    this.Hurt(self, user, AttackUtility.GetActivationDepthCantCounter(activationDepth), damage);
                    this.TryDefeat(self, user, activationDepth, damage);
                }
                else
                {
                    // 1 の継続ダメージ
                    using var damage = EffectableValue.Get();
                    damage.Initialize(1f);
                    this.Hurt(self, null, AttackUtility.GetActivationDepthCantCounter(activationDepth), damage);
                    this.TryDefeat(self, null, activationDepth, damage);
                }
                return default;
            }

            private static IRogueTile GetTile(RogueObj obj)
            {
                if (obj.Location == null || obj.Location.Space.Tilemap == null) return null;

                var tile = obj.Location.Space.Tilemap.GetTop(obj.Position);
                return tile;
            }

            public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
            {
                return other is Effect effect && effect.Stack == Stack;
            }

            public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return new Effect()
                {
                    Stack = Stack
                };
            }

            private new class AffectCallback : IAffectCallback
            {
                private readonly Effect effect;
                private readonly IRogueTileInfo trapTileInfo;

                public AffectCallback(Effect effect, IRogueTileInfo trapTileInfo)
                {
                    this.effect = effect;
                    this.trapTileInfo = trapTileInfo;
                }

                IRogueEffect IAffectCallback.AffectTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg)
                {
                    var statusEffect = effect.AffectTo(target, user, activationDepth, arg);
                    ((Effect)statusEffect).trapTileInfo = trapTileInfo;
                    return statusEffect;
                }
            }
        }
    }
}
