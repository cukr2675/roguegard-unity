using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    [ObjectFormer.RequireRelationalComponent]
    public abstract class MainInfoSet : IRogueDescription, System.IEquatable<MainInfoSet>
    {
        public abstract string Name { get; }
        public abstract Sprite Icon { get; }
        public abstract Color Color { get; }
        public abstract string Caption { get; }
        public abstract IRogueDetails Details { get; }

        public abstract IKeyword Category { get; }

        public abstract int MaxHP { get; }
        public abstract int MaxMP { get; }
        public abstract int ATK { get; }
        public abstract int DEF { get; }
        public abstract float Weight { get; }
        public abstract float LoadCapacity { get; }
        public abstract ISerializableKeyword Faction { get; }
        public abstract Spanning<ISerializableKeyword> TargetFactions { get; }

        // Ability, Material, Gender は ValueEffect でもいいが Open でいちいち付与するのが面倒なのでプロパティにする。
        public abstract MainInfoSetAbility Ability { get; }
        public abstract IRogueMaterial Material { get; }
        public abstract IRogueGender Gender { get; }
        public abstract string HPName { get; }
        public abstract string MPName { get; }
        public abstract float Cost { get; }
        public abstract bool CostIsUnknown { get; }

        public abstract Spanning<IWeightedRogueObjGeneratorList> LootTable { get; }

        public abstract IActiveRogueMethod Walk { get; }
        public abstract IActiveRogueMethod Wait { get; }
        public abstract ISkill Attack { get; }
        public abstract ISkill Throw { get; }
        public abstract IActiveRogueMethod PickUp { get; }
        public abstract IActiveRogueMethod Put { get; }
        public abstract IEatActiveRogueMethod Eat { get; }

        public abstract IAffectRogueMethod Hit { get; }
        public abstract IAffectRogueMethod BeDefeated { get; }
        public abstract IChangeStateRogueMethod Locate { get; }
        public abstract IChangeStateRogueMethod Polymorph { get; }

        public abstract IApplyRogueMethod BeApplied { get; }
        public abstract IApplyRogueMethod BeThrown { get; }
        public abstract IApplyRogueMethod BeEaten { get; }

        /// <summary>
        /// 変化時に呼び出すメソッド。戻り値を実際の <see cref="MainRogueObjInfo.InfoSet"/> として使用する。
        /// <see cref="MainRogueObjInfo.InfoSetState"/> != <see cref="RogueEffectOpenState.Finished"/> のとき
        /// エフェクト系の利用は禁止。（<see cref="RogueEffectState.Contains(IRogueEffect)"/> は可能）
        /// </summary>
        /// <param name="polymorph2Base">変化状態から変化解除するときのみ true 。変化状態からさらに変化しても false となる</param>
        public abstract MainInfoSet Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base);

        /// <summary>
        /// 変化時に呼び出すメソッド。
        /// <see cref="MainRogueObjInfo.InfoSetState"/> != <see cref="RogueEffectOpenState.Finished"/> のとき
        /// エフェクト系の利用は禁止。（<see cref="RogueEffectState.Contains(IRogueEffect)"/> は可能）
        /// </summary>
        /// <param name="base2Polymorph">無変化状態から変化するとき true 。変化状態からさらに変化しても false となる</param>
        public abstract void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph);

        /// <summary>
        /// レベルアップ時などでインスタンスを開きなおす。
        /// Close して Open するとスキルの順番が変わってしまうためこのメソッドが必要。
        /// 戻り値を実際の <see cref="MainRogueObjInfo.InfoSet"/> として使用する。
        /// </summary>
        public abstract MainInfoSet Reopen(RogueObj self, MainInfoSetType infoSetType);

        public abstract void GetObjSprite(RogueObj self, out RogueObjSprite objSprite, out IMotionSet motionSet);

        public abstract IEquipmentState GetEquipmentState(RogueObj self);

        public abstract IEquipmentInfo GetEquipmentInfo(RogueObj self);

        public abstract bool Equals(MainInfoSet other);
        public override bool Equals(object obj) => obj is MainInfoSet other && Equals(other);
        public override int GetHashCode() => 0;

        public static bool operator ==(MainInfoSet left, MainInfoSet right) => left?.Equals(right) ?? right is null;
        public static bool operator !=(MainInfoSet left, MainInfoSet right) => !(left?.Equals(right) ?? right is null);

        // 状態を持つことを想定しないため、クローン生成は実装しない。
    }
}
