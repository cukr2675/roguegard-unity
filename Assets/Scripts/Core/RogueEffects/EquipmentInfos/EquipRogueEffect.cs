using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// このクラスのインスタンスは <see cref="IEquipmentInfo.TryOpen(RogueObj, int, EquipRogueEffect)"/> 内で生成する。
    /// （生成したインスタンスを <see cref="IEquipmentInfo"/> 側で使いまわすため）
    /// ただし、生成時点で装備済みにする場合は例外的に生成する。
    /// </summary>
    [ObjectFormer.Formable]
    public class EquipRogueEffect : IRogueEffect
    {
        /// <summary>
        /// -1 のとき未装備。
        /// </summary>
        public int Index { get; private set; }

        private RogueObj equipment;

        [ObjectFormer.CreateInstance]
        private EquipRogueEffect() { }

        public EquipRogueEffect(RogueObj equipment)
        {
            this.equipment = equipment;
            Index = -1;
        }

        public void SetIndex(int index)
        {
            if (index < 0) throw new System.ArgumentOutOfRangeException(nameof(index));

            Index = index;
        }

        /// <summary>
        /// 注意：このメソッドは必ず <see cref="IEquipmentInfo.Close(RogueObj)"/> 内で実行する。
        /// （このメソッドでは <paramref name="owner"/> からエフェクトを解除するのみで、装備品には影響を与えないため）
        /// </summary>
        public void Close(RogueObj owner)
        {
            owner.Main.RogueEffects.Remove(this);
            Index = -1;
        }

        void IRogueEffect.Open(RogueObj owner)
        {
            var info = equipment.Main.GetEquipmentInfo(equipment);
            info.TryOpen(equipment, Index, this);
        }

        /// <summary>
        /// 装備状態のスタック判定は <see cref="MainRogueObjInfo.CanStack(RogueObj, RogueObj)"/> 内で行う。
        /// （<see cref="IEquipmentInfo.IsNotStackableWhileEquipped"/> が絡むため）
        /// </summary>
        bool IRogueEffect.CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => true;

        IRogueEffect IRogueEffect.DeepOrShallowCopy(RogueObj owner, RogueObj clonedOwner)
        {
            var clone = new EquipRogueEffect(equipment);
            clone.Index = Index;
            return clone;
        }

        IRogueEffect IRogueEffect.ReplaceCloned(RogueObj obj, RogueObj clonedObj)
        {
            if (equipment == obj) { equipment = clonedObj; }
            return this;
        }
    }
}
