using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IAmmoEquipmentInfo : IEquipmentInfo
    {
        IKeyword AmmoCategory { get; }

        IApplyRogueMethod BeShot { get; }
    }
}
