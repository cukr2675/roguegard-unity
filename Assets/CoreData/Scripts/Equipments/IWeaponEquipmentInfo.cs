using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IWeaponEquipmentInfo : IEquipmentInfo
    {
        ISkill Attack { get; }
        ISkill Throw { get; }
    }
}
