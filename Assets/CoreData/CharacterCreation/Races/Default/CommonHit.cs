using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CommonHit : ReferableScript, IAffectRogueMethod
    {
        public static CommonHit Instance { get; } = new CommonHit();

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var damageValue = arg.RefValue;
            var useValue = false;
            var heal = false;
            if (damageValue != null)
            {
                useValue = AttackUtility.GetUseValue(damageValue);
                heal = damageValue.SubValues.Is(StdKw.Heal);
            }
            if (useValue)
            {
                var damage = StatsEffectedValues.GetDamage(self, damageValue, RogueRandom.Primary, out var critical, out var guard);
                damage = Mathf.Max(damage, 0);

                var stats = self.Main.Stats;
                var visible = MainCharacterWorkUtility.VisibleAt(self.Location, self.Position);
                if (heal)
                {
                    stats.SetHP(self, stats.HP + damage);
                    if (visible)
                    {
                        if (damage == 0)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, self);
                            RogueDevice.Add(DeviceKw.AppendText, "に効果はない！\n");
                            RogueDevice.Add(DeviceKw.EnqueueSE, StdKw.NoDamage);
                        }
                        else
                        {
                            RogueDevice.Add(DeviceKw.AppendText, self);
                            RogueDevice.Add(DeviceKw.AppendText, "は");
                            RogueDevice.Add(DeviceKw.AppendText, damage);
                            RogueDevice.Add(DeviceKw.AppendText, "回復した！\n");
                        }
                    }
                }
                else
                {
                    stats.SetHP(self, stats.HP - damage);
                    if (visible)
                    {
                        if (damage == 0)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, self);
                            RogueDevice.Add(DeviceKw.AppendText, "にダメージはない！\n");
                            RogueDevice.Add(DeviceKw.EnqueueSE, StdKw.NoDamage);
                        }
                        else
                        {
                            RogueDevice.Add(DeviceKw.AppendText, self);
                            RogueDevice.Add(DeviceKw.AppendText, "に");
                            RogueDevice.Add(DeviceKw.AppendText, damage);
                            RogueDevice.Add(DeviceKw.AppendText, "のダメージ！\n");
                            RogueDevice.Add(DeviceKw.EnqueueSE, MainInfoKw.Hit);
                        }
                    }
                }
                if (visible)
                {
                    var positioning = RogueCharacterWork.CreateSyncPositioning(self);
                    RogueDevice.AddWork(DeviceKw.EnqueueWork, positioning);
                    {
                        var item = RogueCharacterWork.CreatePopupNumber(self, RogueCharacterWork.PopSignType.None, damage, critical, true);
                        RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
                    }
                    if (heal)
                    {
                        RogueDevice.Add(DeviceKw.EnqueueSE, StdKw.Heal);
                        var item = RogueCharacterWork.CreateEffect(self.Position, CoreMotions.Heal, false);
                        RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
                    }
                    else if (guard)
                    {
                        RogueDevice.Add(DeviceKw.EnqueueSE, StatsKw.Guard);
                        var item = RogueCharacterWork.CreateSpriteMotion(self, KeywordSpriteMotion.Guard, false);
                        RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
                    }
                    else if (damage == 0)
                    {
                        var item = RogueCharacterWork.CreateSpriteMotion(self, KeywordSpriteMotion.NoDamage, false);
                        RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
                    }
                    else
                    {
                        var item = RogueCharacterWork.CreateSpriteMotion(self, KeywordSpriteMotion.Hit, false);
                        RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
                    }
                }

                if (stats.HP <= 0)
                {
                    damageValue.SubValues[MainInfoKw.BeDefeated] = 1f;
                }
            }
            if (arg.Other is IAffectCallback callback)
            {
                callback.AffectTo(self, user, activationDepth, arg);
            }

            return true;
        }
    }
}
