using UnityEngine;

namespace Roguegard
{
    public class DependsOnThrownRogueMethodRange : IRogueMethodRange
    {
        public static DependsOnThrownRogueMethodRange Instance { get; } = new DependsOnThrownRogueMethodRange();

        public string Name => "弾による";
        Sprite IRogueDescribable.Icon => null;
        Color IRogueDescribable.Color => Color.white;
        string IRogueDescribable.Caption => null;
        IRogueDetails IRogueDescribable.Details => null;

        public void Predicate(IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj ammo, float visibleRadius, RectInt room)
        {
            ammo ??= EquipmentUtility.GetAmmo(self, out _);
            if (ammo == null || predictionDepth >= 1f) return;

            var ammoRange = ammo.Main.InfoSet.BeThrown?.Range;
            ammoRange?.Predicate(predicator, self, 1f, ammo, visibleRadius, room);
        }

        public void Predicate(IRoguePredicator predicator, RogueObj self, float predictionDepth, RogueObj ammo, Vector2Int targetPosition)
        {
            ammo ??= EquipmentUtility.GetAmmo(self, out _);
            if (ammo == null || predictionDepth >= 1f) return;

            var ammoRange = ammo.Main.InfoSet.BeThrown?.Range;
            ammoRange?.Predicate(predicator, self, 1f, ammo, targetPosition);
        }
    }
}
