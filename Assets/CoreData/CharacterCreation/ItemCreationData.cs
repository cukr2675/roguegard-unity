using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Data/Item")]
    [Objforming.Referable]
    public class ItemCreationData : ScriptableCharacterCreationData
    {
        [SerializeField] protected ObjectRace _race = null;
        [SerializeField] private ScriptableAppearance[] _appearances = null;
        [SerializeField] private ScriptableIntrinsic[] _intrinsics = null;
        [SerializeField] private ScriptableStartingItemList[] _startingItemTable = null;

        [System.NonSerialized] private SortedIntrinsicList sortedIntrinsics;

        public override IReadOnlyRace Race => _race;
        public override Spanning<IReadOnlyAppearance> Appearances => _appearances;
        protected override ISortedIntrinsicList SortedIntrinsics => sortedIntrinsics;
        public override Spanning<IWeightedRogueObjGeneratorList> StartingItemTable => _startingItemTable;

        protected override void Initialize()
        {
            base.Initialize();
            sortedIntrinsics = new SortedIntrinsicList(_intrinsics, this);
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
