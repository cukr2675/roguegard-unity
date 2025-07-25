namespace Roguegard
{
    public interface IEquipmentInfo
    {
        Spanning<IKeyword> EquipmentSlots { get; }

        /// <summary>
        /// -1 のとき未装備。
        /// </summary>
        int EquippedSubslot { get; }

        bool CanStackWhileEquipped { get; }

        IApplyRogueMethod BeEquipped { get; }

        // ApplyEffect にすると Locate で装備解除させるぶん RogueMethod が増えて面倒なので ChangeEffect にする。
        IChangeEffectRogueMethod BeUnequipped { get; }

        bool TryOpen(RogueObj equipment, int index, EquipRogueEffect equipEffect = null);

        void RemoveClose(RogueObj equipment);
    }
}
