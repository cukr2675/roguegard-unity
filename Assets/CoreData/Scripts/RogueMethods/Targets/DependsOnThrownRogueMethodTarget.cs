using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class DependsOnThrownRogueMethodTarget : IRogueMethodTarget
    {
        public static DependsOnThrownRogueMethodTarget Instance { get; } = new DependsOnThrownRogueMethodTarget();

        public string Name => "弾による";
        Sprite IRogueDescription.Icon => null;
        Color IRogueDescription.Color => Color.white;
        string IRogueDescription.Caption => null;
        IRogueDetails IRogueDescription.Details => null;

        public IRoguePredicator GetPredicator(RogueObj self, float predictionDepth, RogueObj ammo)
        {
            if (ammo == null)
            {
                ammo = EquipmentUtility.GetAmmo(self, out _);
            }
            if (ammo == null || predictionDepth >= 1f) return null;

            var ammoRange = ammo.Main.InfoSet.BeThrown?.Target;
            return ammoRange?.GetPredicator(self, 1f, ammo);
        }
    }
}
