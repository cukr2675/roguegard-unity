using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Dungeon Quest Generator")]
    public class DungeonQuestGeneratorAsset : ScriptableObject
    {
        [SerializeField] private QuestEffectIntrinsicOptionAsset[] _objectiveEffects = null;
        [SerializeField] private QuestEffectIntrinsicOptionAsset[] _environmentEffects = null;
        [SerializeField] private CharacterCreationDataAsset _money = null;
        [SerializeField] private float _moneyPerQuestCost = 0f;

        private static List<DungeonCreationDataAsset> dungeons;

        private static readonly ICharacterCreationData defaultCharacterCreationData = new CharacterCreationDataBuilder();

        public DungeonQuest GenerateQuest(IRogueRandom random)
        {
            if (dungeons == null)
            {
                dungeons = new List<DungeonCreationDataAsset>();
                foreach (var optionValue in RoguegardSettings.CharacterCreationDatabase.StartingItemOptions)
                {
                    if (optionValue is DungeonCreationDataAsset dungeonValue)
                    {
                        if (dungeonValue.Floors.Length == 0) continue;

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
            var seed = random.Next(int.MinValue, int.MaxValue);
            var quest = new DungeonQuest(dungeon, seed, objectives, environments, lootTable);
            return quest;
        }

        private IReadOnlyIntrinsic[] GenerateObjectives(DungeonCreationDataAsset dungeon, IRogueRandom random)
        {
            var objectiveGenerator = random.Choice(_objectiveEffects);
            var objective = objectiveGenerator.GenerateEffect(dungeon, RoguegardSettings.CharacterCreationDatabase, random);
            return new[] { objective };
        }

        private IReadOnlyIntrinsic[] GenerateEnvironments(DungeonCreationDataAsset dungeon, IRogueRandom random)
        {
            return new IReadOnlyIntrinsic[0];

            var environmentGenerator = random.Choice(_environmentEffects);
            var environment = environmentGenerator.GenerateEffect(dungeon, RoguegardSettings.CharacterCreationDatabase, random);
            return new[] { environment };
        }

        private StartingItemBuilderTable GenerateLootTable(float cost, DungeonCreationDataAsset dungeon, IRogueRandom random)
        {
            var lootTable = new StartingItemBuilderTable();
            var money = lootTable.Add().Add();
            money.Option = _money;
            money.Stack = Mathf.FloorToInt(-cost * _moneyPerQuestCost);
            //var dungeonItemTable = dungeon.Levels[dungeon.Levels.Count - 1].ItemTable;
            //if (dungeonItemTable.Count >= 1 && dungeonItemTable[0].Span.Count >= 1)
            //{
            //    var item = lootTable.Add().Add();
            //    item.Option = dungeonItemTable[0].Span[0].InfoSet;
            //}
            return lootTable;
        }
    }
}
