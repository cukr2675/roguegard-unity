using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PointAttackCommandAction : IDeviceCommandAction
    {
        public static PointAttackCommandAction Instance { get; } = new PointAttackCommandAction();

        public bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            EquipmentUtility.GetWeapon(self, out var weapon);
            if (weapon != null)
            {
                // 武器で攻撃する。
                if (weapon.Attack != null)
                {
                    var result = RogueMethodAspectState.Invoke(MainInfoKw.Attack, weapon.Attack, self, user, activationDepth, arg);
                    if (result) return true;
                }
                if (weapon.Throw != null)
                {
                    var result = RogueMethodAspectState.Invoke(MainInfoKw.Throw, weapon.Throw, self, user, activationDepth, arg);
                    if (result) return true;
                }

                // 弓や銃などには AttackSkill を設定しないことを想定する。
                // （弓と石ころを同時装備しているとき、弓で殴らずに石ころを投げてほしい）
            }
            {
                // 素手で投げる。
                var ammo = EquipmentUtility.GetAmmo(self, out _);

                // 対応しない弾種は投げない。（素手で矢を投げないようにする）
                if (ammo != null && ammo.Main.InfoSet.BeThrown.Target == ForEnemyRogueMethodTarget.Instance)
                {
                    var skill = self.Main.InfoSet.Throw;
                    var result = RogueMethodAspectState.Invoke(MainInfoKw.Throw, skill, self, user, activationDepth, arg);
                    if (result) return true;
                }
            }
            {
                // 素手で攻撃する。
                var skill = self.Main.InfoSet.Attack;
                var result = RogueMethodAspectState.Invoke(MainInfoKw.Attack, skill, self, user, activationDepth, arg);
                if (result) return true;
            }

            if (RogueDevice.Primary.Player == self)
            {
                RogueDevice.Add(DeviceKw.AppendText, "そこには攻撃できない\n");
            }
            return false;
        }

        public static RogueObj GetVisibleTarget(RogueObj self)
        {
            var viewMap = self.Get<ViewInfo>();
            if (viewMap == null || viewMap.Location != self.Location) return null;
            if (self.Location == null || self.Location.Space.Tilemap == null) return null;

            EquipmentUtility.GetWeapon(self, out var weapon);
            if (weapon != null)
            {
                // 武器で攻撃する。
                var skill = weapon.Attack;
                var predicator = skill?.Target?.GetPredicator(self, 0f, null);
                if (predicator != null)
                {
                    skill.Range?.Predicate(predicator, self, 0f, null, self.Position + self.Main.Stats.Direction.Forward);
                    predicator.EndPredicate();
                    if (predicator.Positions.Count >= 1)
                    {
                        var objs = predicator.GetObjs(predicator.Positions[0]);
                        if (!viewMap.ContainsVisible(objs[0])) return null;

                        return objs[0];
                    }
                }
            }
            {
                // 素手で攻撃する。
                var skill = self.Main.InfoSet.Attack;
                var predicator = skill?.Target?.GetPredicator(self, 0f, null);
                if (predicator != null)
                {
                    skill.Range?.Predicate(predicator, self, 0f, null, self.Position + self.Main.Stats.Direction.Forward);
                    predicator.EndPredicate();
                    if (predicator.Positions.Count >= 1)
                    {
                        var objs = predicator.GetObjs(predicator.Positions[0]);
                        if (!viewMap.ContainsVisible(objs[0])) return null;

                        return objs[0];
                    }
                }
            }
            return null;
        }
    }
}
