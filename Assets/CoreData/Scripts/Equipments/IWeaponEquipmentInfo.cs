namespace Roguegard
{
    public interface IWeaponEquipmentInfo : IEquipmentInfo
    {
        ISkill Attack { get; }
        ISkill Throw { get; }
    }
}
