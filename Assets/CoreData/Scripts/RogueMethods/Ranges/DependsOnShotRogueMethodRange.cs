using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class DependsOnShotRogueMethodRange : IRogueMethodRange
    {
        public static DependsOnShotRogueMethodRange Instance { get; } = new DependsOnShotRogueMethodRange();

        public string Name => "弾による";
        Sprite IRogueDescription.Icon => null;
        Color IRogueDescription.Color => Color.white;
        string IRogueDescription.Caption => null;
        IRogueDetails IRogueDescription.Details => null;

        public void Predicate(IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj ammo, float visibleRadius, RectInt room)
        {
            if (ammo == null)
            {
                ammo = EquipmentUtility.GetAmmo(self, out _);
            }
            if (ammo == null || predictionDepth >= 1f) return;

            var ammoInfo = EquipmentUtility.GetAmmoInfo(ammo);
            if (ammoInfo == null) return;

            var ammoRange = ammoInfo.BeShot?.Range;
            ammoRange?.Predicate(predicator, self, 1f, ammo, visibleRadius, room);
        }

        public void Predicate(IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj ammo, Vector2Int targetPosition)
        {
            if (ammo == null)
            {
                ammo = EquipmentUtility.GetAmmo(self, out _);
            }
            if (ammo == null || predictionDepth >= 1f) return;

            var ammoInfo = EquipmentUtility.GetAmmoInfo(ammo);
            if (ammoInfo == null) return;

            var ammoRange = ammoInfo.BeShot?.Range;
            ammoRange?.Predicate(predicator, self, 1f, ammo, targetPosition);
        }
    }
}
