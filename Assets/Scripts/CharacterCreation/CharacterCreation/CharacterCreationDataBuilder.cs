using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class CharacterCreationDataBuilder : ICharacterCreationData
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public IRogueDetails Details { get; set; }
        public float Cost { get; set; }
        public bool CostIsUnknown { get; set; }
        public int Lv { get; set; }

        public RaceBuilder Race { get; }
        public AppearanceBuilderList Appearances { get; }
        public IntrinsicBuilderList Intrinsics { get; }
        public StartingItemBuilderTable StartingItemTable { get; }

        [System.NonSerialized] private SortedIntrinsicList sortedIntrinsics;
        [System.NonSerialized] private GrowingInfoSetTable growingInfoSets;

        Sprite IRogueDescription.Icon => Race.Option.Icon;
        Color IRogueDescription.Color => Race.Option.Color;
        string IRogueDescription.Caption => Race.Caption;

        IReadOnlyRace ICharacterCreationData.Race => Race;
        Spanning<IReadOnlyAppearance> ICharacterCreationData.Appearances => Appearances;
        ISortedIntrinsicList ICharacterCreationData.SortedIntrinsics
        {
            get
            {
                if (sortedIntrinsics == null) { UpdateData(); }
                return sortedIntrinsics;
            }
        }
        Spanning<IWeightedRogueObjGeneratorList> ICharacterCreationData.StartingItemTable => StartingItemTable;

        public MainInfoSet PrimaryInfoSet => TryGetGrowingInfoSet(Race.Option, Race.Gender, out var value) ? value : throw new RogueException();

        public CharacterCreationDataBuilder()
        {
            Race = new RaceBuilder();
            Appearances = new AppearanceBuilderList();
            Intrinsics = new IntrinsicBuilderList();
            StartingItemTable = new StartingItemBuilderTable();
        }

        public CharacterCreationDataBuilder(CharacterCreationDataBuilder builder)
        {
            Name = builder.Name;
            ShortName = builder.ShortName;
            Details = builder.Details;
            Cost = builder.Cost;
            CostIsUnknown = builder.CostIsUnknown;
            Lv = builder.Lv;

            Race = new RaceBuilder(builder.Race);
            Appearances = new AppearanceBuilderList();
            Appearances.AddClones(builder.Appearances);
            Intrinsics = new IntrinsicBuilderList();
            Intrinsics.AddClones(builder.Intrinsics);
            StartingItemTable = new StartingItemBuilderTable();
            StartingItemTable.AddClones(builder.StartingItemTable);
        }

        public void Set(CharacterCreationDataBuilder builder)
        {
            Name = builder.Name;
            ShortName = builder.ShortName;
            Details = builder.Details;
            Cost = builder.Cost;
            CostIsUnknown = builder.CostIsUnknown;
            Lv = builder.Lv;

            Race.Set(builder.Race);
            Appearances.Clear();
            Appearances.AddClones(builder.Appearances);
            Intrinsics.Clear();
            Intrinsics.AddClones(builder.Intrinsics);
            StartingItemTable.Clear();
            StartingItemTable.AddClones(builder.StartingItemTable);
        }

        private void UpdateData()
        {
            sortedIntrinsics = new SortedIntrinsicList(Intrinsics, this);
            growingInfoSets = new GrowingInfoSetTable(this);
        }

        public void UpdateCost()
        {
            Cost = Race.Option.Cost;
            CostIsUnknown = Race.Option.CostIsUnknown;

            for (int i = 0; i < Intrinsics.Count; i++)
            {
                var intrinsic = Intrinsics[i];
                Cost += Mathf.Max(intrinsic.Option.GetCost(intrinsic, this, out var intrinsicCostIsUnknown), 0f);
                CostIsUnknown |= intrinsicCostIsUnknown;
            }
        }

        public RogueObj CreateObj(RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            if (growingInfoSets == null) { UpdateData(); }
            if (!growingInfoSets.TryGetValue(Race.Option, Race.Gender, out var infoSet)) throw new RogueException();

            return infoSet.CreateObj(location, position, random, stackOption);
        }

        public bool TryGetGrowingInfoSet(IRaceOption raceOption, IRogueGender gender, out MainInfoSet growingInfoSet)
        {
            if (raceOption == null) throw new System.ArgumentNullException(nameof(raceOption));
            if (gender == null) throw new System.ArgumentNullException(nameof(gender));

            if (growingInfoSets == null) { UpdateData(); }
            var result = growingInfoSets.TryGetValue(raceOption, gender, out var value);
            growingInfoSet = value;
            return result;
        }
    }
}
