using Lysionium;
using Roguegard.Device;
using Roguegard.Extensions;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Character Creation Data/Location/Dungeon")]
    [Objforming.Referable]
    public class DungeonCreationDataAsset : CharacterCreationDataAsset
    {
        [SerializeField] private InlineRace _race = null;
        [SerializeField, DescribeElement] private AssetAppearance[] _appearances = null;
        [SerializeField, DescribeElement] private AssetIntrinsic[] _intrinsics = null;
        [SerializeField] private AssetStartingItemList[] _startingItemTable = null;

        [System.NonSerialized] private SortedIntrinsicList sortedIntrinsics;

        public override IReadOnlyRace Race => _race;
        public override Spanning<IReadOnlyAppearance> Appearances => _appearances;
        protected override ISortedIntrinsicList SortedIntrinsics => sortedIntrinsics;
        public override Spanning<IWeightedRogueObjGeneratorList> StartingItemTable => _startingItemTable;



        [Header("DungeonData")]
        [SerializeField] private FactionAsset _playerFaction = null;
        [SerializeField] private ScriptRef<ILevelInfoInitializer>[] _playerLevelInfos = null;

        [SerializeField] private DungeonLevelType _levelType = DungeonLevelType.Down;
        public DungeonLevelType LevelType => _levelType;

        [SerializeField] private float _visibleRadius = 2f;
        public float VisibleRadius => _visibleRadius;

        [SerializeField] private DungeonFloorAsset[] _floors = null;
        public Spanning<DungeonFloorAsset> Floors => _floors;

        // ターン経過で満腹度消費
        // 自然回復あり
        private static readonly UseNutritionLeaderEffect useNutritionLeaderEffect = new();

        public ISelectOption CreateDungeonSelectOption()
        {
            var floorMenu = new FloorMenu() { data = this };
            return SelectOption.Create<MMgr, MArg>(DescriptionName, (manager, arg) =>
            {
                manager.AddObject(DeviceKw.EnqueueSE, CategoryKw.DownStairs);
                manager.Done();
                RogueDevice.Primary.AddMenu(floorMenu.MenuScreen, arg.Self, arg.User, RogueMethodArgument.Identity);
            });
        }

        public void StartDungeon(RogueObj player, IRogueRandom random)
        {
            var party = new RogueParty(_playerFaction.Faction, _playerFaction.TargetFactions);
            RoguePartyUtility.AssignWithPartyMembers(player, party);

            var world = RogueWorldInfo.GetWorld(player);
            var dungeon = CreateObj(world, Vector2Int.zero, random);

            // ダンジョンのシード値を設定する
            var dungeonSeed = random.Next(int.MinValue, int.MaxValue);
            DungeonInfo.SetSeedTo(dungeon, dungeonSeed);

            RoguePartyUtility.Reset(party, useNutritionLeaderEffect);
            if (!RoguePartyUtility.TryLocateWithPartyMembers(player, dungeon, true)) throw new System.InvalidOperationException();

            // リーダーのレベルアップボーナスは HP, MP, 最大重量 から選択
            _playerLevelInfos[0].Ref.InitializeLv(player, 1);

            //StartFloor(player, random);
        }

        public void StartFloor(RogueObj player, IRogueRandom random)
        {
            // ランダム値リセット
            RogueRandom.Primary = random;

            var dungeon = player.Location;
            var lv = dungeon.Main.Stats.Lv;
            foreach (var level in _floors)
            {
                if (level.EndLv < lv) continue;

                var floor = CreateObj(player.Location, Vector2Int.zero, random);
                level.GenerateFloor(player, floor, random);
                return;
            }
            throw new System.InvalidOperationException();
        }

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

        protected override void OnValidate()
        {
            base.OnValidate();

            foreach (var openEffectSource in _race.OpenEffectSources)
            {
                if (openEffectSource.Ref?.GetType() == typeof(DungeonOpen)) return;
            }
            Debug.LogError($"{name} ({nameof(DungeonCreationDataAsset)}) の {nameof(IOpenEffect)} に {nameof(DungeonOpen)} が設定されていません。");
        }

        private class FloorMenu : FloorMenuAfterLoadRogueMethod
        {
            public DungeonCreationDataAsset data;

            protected override string GetName(MMgr manager, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
            {
                var initialLevelText = DungeonInfo.GetLevelText(data._levelType, 1);
                return $"{data.DescriptionName}\n{initialLevelText}";
            }

            protected override void Activate(MMgr manager, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
            {
                data.StartDungeon(player, RogueRandom.Primary);
                data.StartFloor(player, RogueRandom.Primary);
            }
        }
    }
}
