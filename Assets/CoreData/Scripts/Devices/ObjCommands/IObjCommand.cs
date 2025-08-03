using Lysionium;
using Roguegard.Device;

namespace Roguegard
{
    // 命名メモ: I(Rogue)DeviceCommand インターフェースが存在するため IRogueCommand は不明瞭

    /// <summary>
    /// <see cref="ObjCommandTableAsset"/> の項目となる <see cref="IDeviceCommand"/> 。
    /// <see cref="IDeviceCommand"/> と違い <see cref="ISkillDescribable"/> を返すため、
    /// PointAttackCommand のようにメソッドを試行するクラスには適用しない。
    /// </summary>
    public interface IObjCommand : IDeviceCommand
    {
        ISelectOption SelectOption { get; }

        ISkillDescribable GetSkillDescribable(RogueObj self, RogueObj tool);
    }
}
