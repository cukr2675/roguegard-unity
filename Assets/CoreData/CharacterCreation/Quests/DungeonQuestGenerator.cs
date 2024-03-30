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
        [SerializeField] private ScriptableCharacterCreationData _money = null;
        [SerializeField] private float _moneyPerQuestCost = 0f;

        private static List<DungeonCreationData> dungeons;

        private static readonly ICharacterCreationData defaultCharacterCreationData = new CharacterCreationDataBuilder();

        public DungeonQuest GenerateQuest(IRogueRandom random)
        {
            if (dungeons == null)
            {
                dungeons = new List<DungeonCreationData>();
                var allOptions = RoguegardSettings.CharacterCreationDatabase.StartingItemOptions;
                for (int i = 0; i < allOptions.Count; i++)
                {
                    var value = allOptions[i];
                    if (value is DungeonCreationData dungeonValue)
                    {
                        if (dungeonValue.Levels.Count == 0) continue;

                        dungeons.Add(dungeonValue);
                    }
                }
            }

            var dungeon = random.Choice(dungeons);
            var objectives = GenerateObjectives(dungeon, random);
            var environments = GenerateEnvironments(dungeon, random);

            var cost = 0f;
            foreach (var objective in objectives)
            {
                cost += objective.Option.GetCost(objective, defaultCharacterCreationData, out _);
            }
            foreach (var environment in environments)
            {
                cost += environment.Option.GetCost(environment, defaultCharacterCreationData, out _);
            }

            var lootTable = GenerateLootTable(cost, dungeon, random);
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
            return new IReadOnlyIntrinsic[0];

            var environmentGenerator = random.Choice(_environmentEffects);
            var environment = environmentGenerator.GenerateEffect(dungeon, RoguegardSettings.CharacterCreationDatabase, random);
            return new[] { environment };
        }

        private StartingItemBuilderTable GenerateLootTable(float cost, DungeonCreationData dungeon, IRogueRandom random)
        {
            var lootTable = new StartingItemBuilderTable();
            var money = lootTable.Add().Add();
            money.Option = _money;
            money.Stack = Mathf.FloorToInt(-cost * _moneyPerQuestCost);
            //var dungeonItemTable = dungeon.Levels[dungeon.Levels.Count - 1].ItemTable;
            //if (dungeonItemTable.Count >= 1 && dungeonItemTable[0].Spanning.Count >= 1)
            //{
            //    var item = lootTable.Add().Add();
            //    item.Option = dungeonItemTable[0].Spanning[0].InfoSet;
            //}
            return lootTable;
        }
    }
}
