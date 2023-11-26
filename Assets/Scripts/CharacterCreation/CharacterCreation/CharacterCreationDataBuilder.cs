using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class CharacterCreationDataBuilder : ICharacterCreationData
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public object Details { get; set; }
        public float Cost { get; set; }
        public bool CostIsUnknown { get; set; }
        public int Lv { get; set; }

        public RaceBuilder Race { get; set; }
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

        public CharacterCreationDataBuilder()
        {
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

        private void UpdateData()
        {
            sortedIntrinsics = new SortedIntrinsicList(Intrinsics, this);
            growingInfoSets = new GrowingInfoSetTable(this);
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
