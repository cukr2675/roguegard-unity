using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Data/Location/Dungeon")]
    [ObjectFormer.Referable]
    public class DungeonCreationData : ItemCreationData
    {
        [Header("DungeonData")]
        [SerializeField] private ScriptableFaction _playerFaction = null;
        [SerializeField] private ScriptField<ILevelInfoInitializer>[] _playerLevelInfos = null;

        [SerializeField] private DungeonLevelType _levelType = DungeonLevelType.Down;
        public DungeonLevelType LevelType => _levelType;

        [SerializeField] private float _visibleRadius = 2f;
        public float VisibleRadius => _visibleRadius;

        [SerializeField] private RogueDungeonLevel[] _levels = null;
        public Spanning<RogueDungeonLevel> Levels => _levels;

        public IModelsMenuChoice CreateDungeonChoice()
        {
            return new MenuChoice(this);
        }

        public void StartDungeon(RogueObj player, IRogueRandom random)
        {
            var oldParty = player.Main.Stats.Party;
            var party = new RogueParty(_playerFaction.Faction, _playerFaction.TargetFactions);
            while (oldParty.Members.Count >= 1)
            {
                var member = oldParty.Members[0];
                member.Main.Stats.TryAssignParty(member, party);
            }

            DungeonFloorCloserStateInfo.CloseAndRemoveNull(player, true);

            // ターン経過で満腹度消費
            // 自然回復あり
            UseNutritionLeaderEffect.Initialize(player);

            // レベルアップボーナスは HP, MP, 最大重量 から選択
            _playerLevelInfos[0].Ref.InitializeLv(player, 1);

            // 探索開始前に全回復する
            player.Main.Stats.Reset(player);

            var world = RogueWorldInfo.GetWorld(player);
            var dungeon = CreateObj(world, Vector2Int.zero, random);

            // ダンジョンのシード値を設定する
            var dungeonSeed = random.Next(int.MinValue, int.MaxValue);
            DungeonInfo.SetSeedTo(dungeon, dungeonSeed);

            player.Main.Stats.Direction = RogueDirection.Down;
            if (!SpaceUtility.TryLocate(player, dungeon)) throw new RogueException();

            //StartFloor(player, random);
        }

        public void StartFloor(RogueObj player, IRogueRandom random)
        {
            // ランダム値リセット
            RogueRandom.Primary = random;

            var dungeon = player.Location;
            var lv = dungeon.Main.Stats.Lv;
            foreach (var level in _levels)
            {
                if (level.EndLv < lv) continue;

                var floor = CreateObj(player.Location, Vector2Int.zero, random);
                level.GenerateFloor(player, floor, random);
                return;
            }
            throw new RogueException();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            for (int i = 0; i < _race.OpenEffectSources.Count; i++)
            {
                if (_race.OpenEffectSources[i].Ref?.GetType() == typeof(DungeonOpen)) return;
            }
            Debug.LogError($"{name} ({nameof(DungeonCreationData)}) の {nameof(IOpenEffect)} に {nameof(DungeonOpen)} が設定されていません。");
        }

        private class MenuChoice : IModelsMenuChoice
        {
            private readonly DungeonCreationData data;
            private readonly FloorMenu floorMenu;

            public MenuChoice(DungeonCreationData data)
            {
                this.data = data;
                floorMenu = new FloorMenu() { data = data };
            }

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return data.DescriptionName;
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, CategoryKw.DownStairs);
                root.Done();
                RogueDevice.Primary.AddMenu(floorMenu, self, user, RogueMethodArgument.Identity);
            }
        }

        private class FloorMenu : FloorMenuAfterLoadRogueMethod
        {
            public DungeonCreationData data;

            public override string GetName(IModelsMenuRoot root, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
            {
                var initialLevelText = DungeonInfo.GetLevelText(data._levelType, 1);
                return $"{data.DescriptionName}\n{initialLevelText}";
            }

            public override void Activate(IModelsMenuRoot root, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
            {
                data.StartDungeon(player, RogueRandom.Primary);
                data.StartFloor(player, RogueRandom.Primary);
            }
        }
    }
}
