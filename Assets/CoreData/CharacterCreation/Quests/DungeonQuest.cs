using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class DungeonQuest : IRogueDescribable
    {
        public string Name => _objectives[0].Name;
        public Sprite Icon => _objectives[0].Icon;
        public Color Color => _objectives[0].Color;
        public string Caption => _objectives[0].Caption;
        public IRogueDetails Details => _objectives[0].Details;

        public DungeonCreationDataAsset Dungeon { get; }
        public int Seed { get; }

        private readonly IntrinsicList _objectives;
        public Spanning<IReadOnlyIntrinsic> Objectives => _objectives.Span;

        private readonly IntrinsicList _environments;
        public Spanning<IReadOnlyIntrinsic> Environments => _environments.Span;

        [System.NonSerialized] private ISortedIntrinsicList _sortedEffects;
        private ISortedIntrinsicList SortedEffects
            => _sortedEffects ??= new SortedIntrinsicList(_objectives.Concat(_environments), defaultCharacterCreationData);

        private readonly StartingItemTable _lootTable;
        public Spanning<IWeightedRogueObjGeneratorList> LootTable => _lootTable.Span;

        private static readonly ICharacterCreationData defaultCharacterCreationData = new CharacterCreationData();

        private DungeonQuest()
        {
        }

        public DungeonQuest(
            DungeonCreationDataAsset dungeon, int seed, IEnumerable<IReadOnlyIntrinsic> objectives, IEnumerable<IReadOnlyIntrinsic> environments,
            IEnumerable<IEnumerable<IReadOnlyStartingItem>> lootTable)
        {
            if (!objectives.Any()) throw new System.ArgumentException();

            Dungeon = dungeon;
            Seed = seed;
            _objectives = new IntrinsicList();
            _objectives.AddClones(objectives);
            _environments = new IntrinsicList();
            _environments.AddClones(environments);
            _lootTable = new StartingItemTable();
            _lootTable.AddClones(lootTable);
        }

        public void Start(RogueObj player)
        {
            if (DungeonQuestInfo.TryGetQuest(player, out var quest))
            {
                throw new RogueException($"すでにクエスト ({quest.Name}: {quest.Caption}) を開始しています。");
            }

            RogueRandom.Primary = new RogueRandom(Seed);

            Dungeon.StartDungeon(player, RogueRandom.Primary);

            var effect = new Effect() { quest = this };
            player.Main.RogueEffects.AddOpen(player, effect);

            Dungeon.StartFloor(player, RogueRandom.Primary);
        }

        [Objforming.Formable]
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

            public bool CanStack(RogueObj self, RogueObj comingObj, IRogueEffect coming) => false;
            public IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => this;
            public IRogueEffect ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
