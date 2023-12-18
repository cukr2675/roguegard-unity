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
                    if (RogueDevice.Primary.VisibleAt(user.Location, user.Position))
                    {
                        RogueDevice.Add(DeviceKw.AppendText, user);
                        RogueDevice.Add(DeviceKw.AppendText, "は川に流された！");
                    }

                    // 水上を移動できないキャラのとき、最大 HP の半分のダメージを与える
                    using var damageValue = AffectableValue.Get();
                    var baseDamage = Mathf.FloorToInt(StatsEffectedValues.GetMaxHP(user) / 2f);
                    damageValue.Initialize(baseDamage);
                    this.Hurt(user, self, AttackUtility.GetActivationDepthCantCounter(activationDepth), damageValue);
                    if (this.TryDefeat(user, self, activationDepth, damageValue)) return true;

                    // 倒れていなければテレポートさせる
                    var position = user.Location.Space.GetRandomPositionInRoom(RogueRandom.Primary);
                    SpaceUtility.TryLocate(user, user.Location, position);
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