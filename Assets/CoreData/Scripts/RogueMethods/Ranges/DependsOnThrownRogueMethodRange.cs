using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class DependsOnThrownRogueMethodRange : IRogueMethodRange
    {
        public static DependsOnThrownRogueMethodRange Instance { get; } = new DependsOnThrownRogueMethodRange();

        public string Name => "弾による";
        Sprite IRogueDescription.Icon => null;
        Color IRogueDescription.Color => Color.white;
        string IRogueDescription.Caption => null;
        object IRogueDescription.Details => null;

        public void Predicate(IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj ammo, float visibleRadius, RectInt room)
        {
            if (ammo == null)
            {
                ammo = EquipmentUtility.GetAmmo(self, out _);
            }
            if (ammo == null || predictionDepth >= 1f) return;

            var ammoRange = ammo.Main.InfoSet.BeThrown?.Range;
            ammoRange?.Predicate(predicator, self, 1f, ammo, visibleRadius, room);
        }

        public void Predicate(IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj ammo, Vector2Int targetPosition)
        {
            if (ammo == null)
            {
                ammo = EquipmentUtility.GetAmmo(self, out _);
            }
            if (ammo == null || predictionDepth >= 1f) return;

            var ammoRange = ammo.Main.InfoSet.BeThrown?.Range;
            ammoRange?.Predicate(predicator, self, 1f, ammo, targetPosition);
        }
    }
}
