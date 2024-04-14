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
                var visible = MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var handler);
                if (heal)
                {
                    stats.SetHP(self, stats.HP + damage);
                    if (visible)
                    {
                        if (damage == 0)
                        {
                            handler.AppendText(self).AppendText("に効果はない！\n");
                            handler.EnqueueSE(StdKw.NoDamage);
                        }
                        else
                        {
                            handler.AppendText(self).AppendText("は").AppendText(damage).AppendText("回復した！\n");
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
                            handler.AppendText(self).AppendText("にダメージはない！\n");
                            handler.EnqueueSE(StdKw.NoDamage);
                        }
                        else
                        {
                            handler.AppendText(self).AppendText("に").AppendText(damage).AppendText("のダメージ！\n");
                            handler.EnqueueSE(MainInfoKw.Hit);
                        }
                    }
                }
                if (visible)
                {
                    handler.EnqueueWork(RogueCharacterWork.CreateSyncPositioning(self));
                    handler.EnqueueWork(RogueCharacterWork.CreatePopupNumber(self, RogueCharacterWork.PopSignType.None, damage, critical, true));
                    if (heal)
                    {
                        handler.EnqueueSE(StdKw.Heal);
                        handler.EnqueueWork(RogueCharacterWork.CreateEffect(self.Position, CoreMotions.Heal, false));
                    }
                    else if (guard)
                    {
                        handler.EnqueueSE(StatsKw.Guard);
                        handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, KeywordSpriteMotion.Guard, false));
                    }
                    else if (damage == 0)
                    {
                        handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, KeywordSpriteMotion.NoDamage, false));
                    }
                    else
                    {
                        handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, KeywordSpriteMotion.Hit, false));
                    }
                    handler.Dispose();
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
