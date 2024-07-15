using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class PoolBeApplied : BaseApplyRogueMethod
    {
        private PoolBeApplied() { }

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (activationDepth < 20f)
            {
                var userMovement = MovementCalculator.Get(user);
                if (!userMovement.SubIs(StdKw.PoolMovement))
                {
                    if (MessageWorkListener.TryOpenHandler(user.Location, user.Position, out var h))
                    {
                        using var handler = h;
                        handler.AppendText(user).AppendText("は川に流された！\n");
                    }

                    // 水上を移動できないキャラのとき、最大 HP の半分のダメージを与える
                    using var damageValue = EffectableValue.Get();
                    var baseDamage = Mathf.FloorToInt(StatsEffectedValues.GetMaxHP(user) / 2f);
                    damageValue.Initialize(baseDamage);
                    this.Hurt(user, self, AttackUtility.GetActivationDepthCantCounter(activationDepth), damageValue);
                    if (this.TryDefeat(user, self, activationDepth, damageValue)) return true;

                    // 倒れていなければテレポートさせる。テレポートに失敗したら即死
                    if (!user.Location.Space.TryGetRandomPositionInRoom(RogueRandom.Primary, out var position) ||
                        !SpaceUtility.TryLocate(user, user.Location, position))
                    {
                        if (MessageWorkListener.TryOpenHandler(user.Location, user.Position, out var h2))
                        {
                            using var handler = h2;
                            handler.AppendText(user).AppendText("はそのまま流されてしまった…\n");
                        }

                        if (this.Defeat(user, self, activationDepth)) return true;
                    }
                }
                else
                {
                    //RogueDevice.Add(DeviceKw.AppendMessage, user, user);
                    //RogueDevice.Invoke(DeviceKw.AppendMessage, user, "は水に沈んでいる。（窒息まであと");
                    //RogueDevice.Invoke(DeviceKw.AppendMessage, user, 3.ToString());
                    //RogueDevice.Invoke(DeviceKw.AppendMessage, user, "）\n");
                }
            }
            return true;
        }
    }
}
