using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IEquipmentInfo
    {
        Spanning<IKeyword> EquipParts { get; }

        /// <summary>
        /// -1 のとき未装備。
        /// </summary>
        int EquipIndex { get; }

        bool CanStackWhileEquipped { get; }

        IApplyRogueMethod BeEquipped { get; }

        // ApplyEffect にすると Locate で装備解除させるぶん RogueMethod が増えて面倒なので ChangeEffect にする。
        IChangeEffectRogueMethod BeUnequipped { get; }

        bool TryOpen(RogueObj equipment, int index, EquipRogueEffect equipEffect = null);

        void RemoveClose(RogueObj equipment);
    }
}
