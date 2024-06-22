using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard
{
    /// <summary>
    /// <see cref="ObjCommandTable"/> の項目となる <see cref="IDeviceCommandAction"/> 。
    /// <see cref="IDeviceCommandAction"/> と違い <see cref="ISkillDescription"/> を返すため、
    /// PointAttackCommandAction のようにメソッドを試行するクラスには適用しない。
    /// </summary>
    public interface IObjCommand : IDeviceCommandAction
    {
        IListMenuSelectOption SelectOption { get; }

        ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool);
    }
}
