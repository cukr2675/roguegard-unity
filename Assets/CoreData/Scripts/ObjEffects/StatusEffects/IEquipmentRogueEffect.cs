using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// 装備品にかかる <see cref="IRogueEffect"/> によって装備者に影響を与える場合に実装するインターフェース
    /// </summary>
    public interface IEquipmentRogueEffect : IRogueEffect
    {
        void OpenEquip(RogueObj equipment);

        void CloseEquip(RogueObj equipment);
    }
}
