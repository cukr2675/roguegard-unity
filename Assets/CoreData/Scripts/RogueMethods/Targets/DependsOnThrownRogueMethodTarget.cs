using UnityEngine;

namespace Roguegard
{
    public class DependsOnThrownRogueMethodTarget : IRogueMethodTarget
    {
        public static DependsOnThrownRogueMethodTarget Instance { get; } = new DependsOnThrownRogueMethodTarget();

        public string Name => "弾による";
        Sprite IRogueDescribable.Icon => null;
        Color IRogueDescribable.Color => Color.white;
        string IRogueDescribable.Caption => null;
        IRogueDetails IRogueDescribable.Details => null;
        Spanning<IKeyword> IRogueDescribable.Tags => Spanning<IKeyword>.Empty;

        public IRoguePredicator GetPredicator(RogueObj self, float predictionDepth, RogueObj ammo)
        {
            ammo ??= EquipmentUtility.GetAmmo(self, out _);
            if (ammo == null || predictionDepth >= 1f) return null;

            var ammoRange = ammo.Main.InfoSet.BeThrown?.Target;
            return ammoRange?.GetPredicator(self, 1f, ammo);
        }
    }
}
