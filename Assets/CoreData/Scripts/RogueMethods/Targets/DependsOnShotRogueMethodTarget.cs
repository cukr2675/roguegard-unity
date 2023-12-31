using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class DependsOnShotRogueMethodTarget : IRogueMethodTarget
    {
        public static DependsOnShotRogueMethodTarget Instance { get; } = new DependsOnShotRogueMethodTarget();

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

            var ammoInfo = EquipmentUtility.GetAmmoInfo(ammo);
            if (ammoInfo == null) return null;

            var ammoTarget = ammoInfo.BeShot?.Target;
            return ammoTarget?.GetPredicator(self, 1f, ammo);
        }
    }
}
