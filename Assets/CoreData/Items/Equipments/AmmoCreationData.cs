using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Data/Ammo")]
    [Objforming.Referable]
    public class AmmoCreationData : ScriptableCharacterCreationData
    {
        [SerializeField] private AmmoRace _race = null;
        [SerializeField] private ScriptableAppearance[] _appearances = null;
        [SerializeField] private ScriptableIntrinsic[] _intrinsics = null;
        [SerializeField] private ScriptableStartingItemList[] _startingItemTable = null;

        [System.NonSerialized] private SortedIntrinsicList sortedIntrinsics;

        public override IReadOnlyRace Race => _race;
        public override Spanning<IReadOnlyAppearance> Appearances => _appearances;
        protected override ISortedIntrinsicList SortedIntrinsics => sortedIntrinsics;
        public override Spanning<IWeightedRogueObjGeneratorList> StartingItemTable => _startingItemTable;

        public override Spanning<IMemberSource> StartingItemOptionMemberSources => _startingItemOptionMemberSources;
        private static readonly IMemberSource[] _startingItemOptionMemberSources
            = new IMemberSource[] { EquipMember.SourceInstance };

        public override RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random,
            StackOption stackOption = StackOption.Default)
        {
            var item = base.CreateObj(startingItem, location, position, random, stackOption);
            var equipmentMember = EquipMember.GetMember(startingItem);
            if (equipmentMember.IsEquipped) { PreEquip(item, location); } // 装備するよう設定されていれば、生成時に装備する。

            return item;
        }

        /// <summary>
        /// 装備部位が被ったときは例外を投げる <see cref="Equip(RogueObj, RogueObj)"/> 。
        /// </summary>
        private static void PreEquip(RogueObj equipment, RogueObj self)
        {
            if (RogueMethodAspectState.ActivatingNow) throw new RogueException();
            if (!SpaceUtility.TryLocate(equipment, self)) throw new RogueException();

            if (self != null)
            {
                var equipEffect = new EquipRogueEffect(equipment); // 生成時点で装備済みにする
                equipEffect.SetIndex(0);
                self.Main.RogueEffects.AddOpen(self, equipEffect);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            sortedIntrinsics = new SortedIntrinsicList(_intrinsics, this);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            _race.Validate();
        }

        protected override void GetCost(out float cost, out bool costIsUnknown)
        {
            cost = Race.Option.Cost;
            costIsUnknown = Race.Option.CostIsUnknown;

            for (int i = 0; i < _intrinsics.Length; i++)
            {
                var intrinsic = _intrinsics[i];
                cost += Mathf.Max(intrinsic.Option.GetCost(intrinsic, this, out var intrinsicCostIsUnknown), 0f);
                costIsUnknown |= intrinsicCostIsUnknown;
            }
        }
    }
}
