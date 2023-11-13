using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class GrowingInfoSetTable
    {
        private Dictionary<IRaceOption, Dictionary<IRogueGender, CharacterCreationInfoSet>> table;

        public CharacterCreationInfoSet this[IRaceOption growingOption, IRogueGender gender]
            => table[growingOption][gender];

        public GrowingInfoSetTable(ICharacterCreationData data)
        {
            var race = data.Race;
            var raceOption = race.Option;
            var growingOptions = raceOption.GrowingOptions;
            var genders = raceOption.Genders;
            if (growingOptions.Count >= 1)
            {
                table = new Dictionary<IRaceOption, Dictionary<IRogueGender, CharacterCreationInfoSet>>();
                for (int i = 0; i < growingOptions.Count; i++)
                {
                    var growingOption = growingOptions[i];
                    var genderTable = new Dictionary<IRogueGender, CharacterCreationInfoSet>();
                    for (int j = 0; j < genders.Count; j++)
                    {
                        var gender = genders[j];
                        genderTable.Add(gender, new CharacterCreationInfoSet(data, i, gender));
                    }
                    table.Add(growingOption, genderTable);
                }
            }
            else
            {
                table = new Dictionary<IRaceOption, Dictionary<IRogueGender, CharacterCreationInfoSet>>();
                var genderTable = new Dictionary<IRogueGender, CharacterCreationInfoSet>();
                for (int j = 0; j < genders.Count; j++)
                {
                    var gender = genders[j];
                    genderTable.Add(gender, new CharacterCreationInfoSet(data, 0, gender));
                }
                table.Add(raceOption, genderTable);
            }
        }

        public bool TryGetValue(IRaceOption growingOption, IRogueGender gender, out CharacterCreationInfoSet infoSet)
        {
            infoSet = null;
            return table.TryGetValue(growingOption, out var genderTable) && genderTable.TryGetValue(gender, out infoSet);
        }
    }
}
