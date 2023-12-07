using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class DungeonQuest : IRogueDescription
    {
        public string Name => _objectives[0].Name;
        public Sprite Icon => _objectives[0].Icon;
        public Color Color => _objectives[0].Color;
        public string Caption => _objectives[0].Caption;
        public IRogueDetails Details => _objectives[0].Details;

        public DungeonCreationData Dungeon { get; }

        private readonly IntrinsicBuilderList _objectives;
        public Spanning<IReadOnlyIntrinsic> Objectives => _objectives;

        private readonly IntrinsicBuilderList _environments;
        public Spanning<IReadOnlyIntrinsic> Environments => _environments;

        [System.NonSerialized] private ISortedIntrinsicList _sortedEffects;
        private ISortedIntrinsicList SortedEffects
            => _sortedEffects ??= new SortedIntrinsicList(_objectives.Concat(_environments), defaultCharacterCreationData);

        private readonly StartingItemBuilderTable _lootTable;
        public Spanning<IWeightedRogueObjGeneratorList> LootTable => _lootTable;

        private static readonly ICharacterCreationData defaultCharacterCreationData = new CharacterCreationDataBuilder();

        private DungeonQuest()
        {
        }

        public DungeonQuest(
            DungeonCreationData dungeon, IEnumerable<IReadOnlyIntrinsic> objectives, IEnumerable<IReadOnlyIntrinsic> environments,
            IEnumerable<IEnumerable<IReadOnlyStartingItem>> lootTable)
        {
            if (!objectives.Any()) throw new System.ArgumentException();

            Dungeon = dungeon;
            _objectives = new IntrinsicBuilderList();
            _objectives.AddClones(objectives);
            _environments = new IntrinsicBuilderList();
            _environments.AddClones(environments);
            _lootTable = new StartingItemBuilderTable();
            _lootTable.AddClones(lootTable);
        }

        public void Start(RogueObj player)
        {
            if (DungeonQuestInfo.TryGetQuest(player, out var quest))
            {
                throw new RogueException($"すでにクエスト ({quest.Name}: {quest.Caption}) を開始しています。");
            }

            Dungeon.StartDungeon(player, RogueRandom.Primary);

            var effect = new Effect() { quest = this };
            player.Main.RogueEffects.AddOpen(player, effect);

            Dungeon.StartFloor(player, RogueRandom.Primary);
        }

        [ObjectFormer.Formable]
        private class Effect : IRogueEffect, IDungeonFloorCloser
        {
            public DungeonQuest quest;

            public void Open(RogueObj self)
            {
                DungeonQuestInfo.SetTo(self, quest);
                DungeonFloorCloserStateInfo.AddTo(self, this);
                quest.SortedEffects.Open(self, MainInfoSetType.Other, false);
            }

            void IDungeonFloorCloser.RemoveClose(RogueObj self, bool exitDungeon)
            {
                if (!exitDungeon) return;

                // ダンジョンから抜けるときクエストを終了させる
                self.Main.RogueEffects.Remove(this);
                DungeonQuestInfo.RemoveFrom(self);
                DungeonFloorCloserStateInfo.ReplaceWithNull(self, this);
                quest.SortedEffects.Close(self, MainInfoSetType.Other, false);
            }

            public bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => false;
            public IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => this;
            public IRogueEffect ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
