using Lysionium;
using Roguegard.Device;

namespace Roguegard
{
    /// <summary>
    /// <see cref="ObjCommandTable"/> の項目となる <see cref="IDeviceCommandAction"/> 。
    /// <see cref="IDeviceCommandAction"/> と違い <see cref="ISkillDescribable"/> を返すため、
    /// PointAttackCommandAction のようにメソッドを試行するクラスには適用しない。
    /// </summary>
    public interface IObjCommand : IDeviceCommandAction
    {
        ISelectOption SelectOption { get; }

        ISkillDescribable GetSkillDescribable(RogueObj self, RogueObj tool);
    }
}
