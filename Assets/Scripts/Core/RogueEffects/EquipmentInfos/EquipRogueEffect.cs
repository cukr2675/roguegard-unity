namespace Roguegard
{
    /// <summary>
    /// このクラスのインスタンスは <see cref="IEquipmentInfo.TryOpen(RogueObj, int, EquipRogueEffect)"/> 内で生成する。
    /// （生成したインスタンスを <see cref="IEquipmentInfo"/> 側で使いまわすため）
    /// ただし、生成時点で装備済みにする場合は例外的に生成する。
    /// </summary>
    [Objforming.Formable]
    public class EquipRogueEffect : IRogueEffect
    {
        /// <summary>
        /// -1 のとき未装備。
        /// </summary>
        public int EquipmentSubslot { get; private set; }

        private RogueObj equipment;

        [Objforming.CreateInstance]
        private EquipRogueEffect() { }

        public EquipRogueEffect(RogueObj equipment)
        {
            this.equipment = equipment;
            EquipmentSubslot = -1;
        }

        public void SetEquipmentSubslot(int equipmentSubslot)
        {
            if (equipmentSubslot < 0) throw new System.ArgumentOutOfRangeException(nameof(equipmentSubslot));

            EquipmentSubslot = equipmentSubslot;
        }

        /// <summary>
        /// 注意：このメソッドは必ず <see cref="IEquipmentInfo.RemoveClose(RogueObj)"/> 内で実行する。
        /// （このメソッドでは <paramref name="owner"/> からエフェクトを解除するのみで、装備品には影響を与えないため）
        /// </summary>
        public void RemoveClose(RogueObj owner)
        {
            owner.Main.RogueEffects.Remove(this);
            EquipmentSubslot = -1;
        }

        void IRogueEffect.Open(RogueObj owner)
        {
            var info = equipment.Main.GetEquipmentInfo(equipment);
            info.TryOpen(equipment, EquipmentSubslot, this);
        }

        /// <summary>
        /// 装備状態のスタック判定は <see cref="MainRogueObjInfo.CanStack(RogueObj, RogueObj)"/> 内で行う。
        /// （<see cref="IEquipmentInfo.CanStackWhileEquipped"/> が絡むため）
        /// </summary>
        bool IRogueEffect.CanStack(RogueObj self, RogueObj comingObj, IRogueEffect coming) => true;

        IRogueEffect IRogueEffect.DeepOrShallowCopy(RogueObj owner, RogueObj clonedOwner)
        {
            var clone = new EquipRogueEffect(equipment) { EquipmentSubslot = EquipmentSubslot };
            return clone;
        }

        IRogueEffect IRogueEffect.ReplaceObj(RogueObj obj, RogueObj clonedObj)
        {
            if (equipment == obj) { equipment = clonedObj; }
            return this;
        }
    }
}
