using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/DungeonQuestGenerator")]
    public class DungeonQuestGenerator : ScriptableObject
    {
        [SerializeField] private QuestEffectIntrinsicOption[] _objectiveEffects = null;
        [SerializeField] private QuestEffectIntrinsicOption[] _environmentEffects = null;

        private static List<DungeonCreationData> dungeons;

        public DungeonQuest GenerateQuest(IRogueRandom random)
        {
            if (dungeons == null)
            {
                dungeons = new List<DungeonCreationData>();
                var allOptions = RoguegardSettings.CharacterCreationDatabase.StartingItemOptions;
                for (int i = 0; i < allOptions.Count; i++)
                {
                    var value = allOptions[i];
                    if (value is DungeonCreationData dungeonValue) { dungeons.Add(dungeonValue); }
                }
            }

            var dungeon = random.Choice(dungeons);
            var objectives = GenerateObjectives(dungeon, random);
            var environments = GenerateEnvironments(dungeon, random);
            var lootTable = new IReadOnlyStartingItem[0][];
            var quest = new DungeonQuest(dungeon, objectives, environments, lootTable);
            return quest;
        }

        private IReadOnlyIntrinsic[] GenerateObjectives(DungeonCreationData dungeon, IRogueRandom random)
        {
            var objectiveGenerator = random.Choice(_objectiveEffects);
            var objective = objectiveGenerator.GenerateEffect(dungeon, RoguegardSettings.CharacterCreationDatabase, random);
            return new[] { objective };
        }

        private IReadOnlyIntrinsic[] GenerateEnvironments(DungeonCreationData dungeon, IRogueRandom random)
        {
            var environmentGenerator = random.Choice(_environmentEffects);
            var environment = environmentGenerator.GenerateEffect(dungeon, RoguegardSettings.CharacterCreationDatabase, random);
            return new[] { environment };
        }
    }
}
