using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IMainInfoSet : IRogueDescription, System.IEquatable<IMainInfoSet>
    {
        IKeyword Category { get; }

        int MaxHP { get; }
        int MaxMP { get; }
        int ATK { get; }
        int DEF { get; }
        float Weight { get; }
        float LoadCapacity { get; }
        ISerializableKeyword Faction { get; }
        Spanning<ISerializableKeyword> TargetFactions { get; }

        // Ability, Material, Gender は ValueEffect でもいいが Open でいちいち付与するのが面倒なのでプロパティにする。
        MainInfoSetAbility Ability { get; }
        IRogueMaterial Material { get; }
        IRogueGender Gender { get; }
        string HPName { get; }
        string MPName { get; }
        float Cost { get; }
        bool CostIsUnknown { get; }

        Spanning<IWeightedRogueObjGeneratorList> LootTable { get; }

        IActiveRogueMethod Walk { get; }
        IActiveRogueMethod Wait { get; }
        ISkill Attack { get; }
        ISkill Throw { get; }
        IActiveRogueMethod PickUp { get; }
        IActiveRogueMethod Put { get; }
        IEatActiveRogueMethod Eat { get; }

        IAffectRogueMethod Hit { get; }
        IAffectRogueMethod BeDefeated { get; }
        IChangeStateRogueMethod Locate { get; }
        IChangeStateRogueMethod Polymorph { get; }

        IApplyRogueMethod BeApplied { get; }
        IApplyRogueMethod BeThrown { get; }
        IApplyRogueMethod BeEaten { get; }
        IApplyRogueMethod BeSteppedOnAsTile { get; }
        // 罠を踏んだ時の動作を BeApplied にすると、アイテムをタイル化した際予期せず罠になってしまうため BeSteppedOnAsTile に分ける

        /// <summary>
        /// 変化時に呼び出すメソッド。戻り値を実際の <see cref="MainRogueObjInfo.InfoSet"/> として使用する。
        /// <see cref="MainRogueObjInfo.InfoSetState"/> != <see cref="RogueEffectOpenState.Finished"/> のとき
        /// エフェクト系の利用は禁止。（<see cref="RogueEffectState.Contains(IRogueEffect)"/> は可能）
        /// </summary>
        /// <param name="polymorph2Base">変化状態から変化解除するときのみ true 。変化状態からさらに変化しても false となる</param>
        IMainInfoSet Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base);

        /// <summary>
        /// 変化時に呼び出すメソッド。
        /// <see cref="MainRogueObjInfo.InfoSetState"/> != <see cref="RogueEffectOpenState.Finished"/> のとき
        /// エフェクト系の利用は禁止。（<see cref="RogueEffectState.Contains(IRogueEffect)"/> は可能）
        /// </summary>
        /// <param name="base2Polymorph">無変化状態から変化するとき true 。変化状態からさらに変化しても false となる</param>
        void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph);

        /// <summary>
        /// レベルアップ時などでインスタンスを開きなおす。
        /// Close して Open するとスキルの順番が変わってしまうためこのメソッドが必要。
        /// 戻り値を実際の <see cref="MainRogueObjInfo.InfoSet"/> として使用する。
        /// </summary>
        IMainInfoSet Reopen(RogueObj self, MainInfoSetType infoSetType, int deltaLv);

        void GetObjSprite(RogueObj self, out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet);

        IEquipmentState GetEquipmentState(RogueObj self);

        IEquipmentInfo GetEquipmentInfo(RogueObj self);

        // 状態を持つことを想定しないため、クローン生成は実装しない。
    }
}
