using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Data/Location/Lobby")]
    [Objforming.Referable]
    public class LobbyCreationData : ScriptableCharacterCreationData
    {
        [SerializeField] private ObjectRace _race = null;
        [SerializeField, ElementDescription("_option")] private ScriptableAppearance[] _appearances = null;
        [SerializeField, ElementDescription("_option")] private ScriptableIntrinsic[] _intrinsics = null;
        [SerializeField] private ScriptableStartingItemList[] _startingItemTable = null;

        [System.NonSerialized] private SortedIntrinsicList sortedIntrinsics;

        public override IReadOnlyRace Race => _race;
        public override Spanning<IReadOnlyAppearance> Appearances => _appearances;
        protected override ISortedIntrinsicList SortedIntrinsics => sortedIntrinsics;
        public override Spanning<IWeightedRogueObjGeneratorList> StartingItemTable => _startingItemTable;



        [SerializeField] private ScriptableRogueTile _ground = null;
        [SerializeField] private ScriptableRogueTile _roomWall = null;
        [SerializeField] private ScriptableRogueTile _wall = null;
        [SerializeField] private ScriptableStartingItem _doorL = null;
        [SerializeField] private ScriptableStartingItem _doorR = null;
        [SerializeField] private ScriptableStartingItem _partyBoard = null;
        [SerializeField] private ScriptableStartingItem _questBoard = null;
        [SerializeField] private ScriptableStartingItem _storage = null;
        [SerializeField] private ScriptableStartingItem _seat = null;
        [SerializeField] private ScriptableStartingItem _sewingMachine = null;

        private const int w = 6;

        public override RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var lobby = base.CreateObj(startingItem, location, position, random, stackOption);
            var width = 32;
            var height = 16;
            var tilemap = new RogueTilemap(new Vector2Int(width, height));
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tilemap.Set(_ground, x, y);
                }
            }
            for (int y = 0; y < height; y++)
            {
                tilemap.Set(_roomWall, w, y);
                tilemap.Set(_roomWall, width - 1 - w, y);
                for (int x = 0; x < w; x++)
                {
                    tilemap.Set(_wall, x, y);
                    tilemap.Set(_wall, width - 1 - x, y);
                }
            }
            for (int x = w + 1; x < width - 1 - w; x++)
            {
                tilemap.Set(_roomWall, x, height - 1);
                tilemap.Set(_wall, x, height - 2);
                tilemap.Set(_wall, x, height - 3);
            }
            for (int x = w; x < width - w; x++)
            {
                tilemap.Remove(new Vector2Int(x, 0), RogueTileLayer.Building);
                tilemap.Remove(new Vector2Int(x, 1), RogueTileLayer.Building);
                tilemap.Set(_wall, x, 0);
                tilemap.Set(_wall, x, 1);
                tilemap.Set(_roomWall, x, 2);
            }
            lobby.Space.SetTilemap(tilemap);
            lobby.Space.SetRooms(new[] { tilemap.Rect });

            tilemap.Remove(new Vector2Int(width / 2 - 1, 2), RogueTileLayer.Building);
            _doorL.Option.CreateObj(_doorL, lobby, new Vector2Int(width / 2 - 1, 2), random);
            tilemap.Remove(new Vector2Int(width / 2, 2), RogueTileLayer.Building);
            _doorR.Option.CreateObj(_doorR, lobby, new Vector2Int(width / 2, 2), random);

            _partyBoard.Option.CreateObj(_partyBoard, lobby, new Vector2Int(width / 2 - 2, 3), random);
            _questBoard.Option.CreateObj(_questBoard, lobby, new Vector2Int(width / 2 - 2, 5), random);
            _storage.Option.CreateObj(_storage, lobby, new Vector2Int(width / 2 + 1, 3), random);
            _seat.Option.CreateObj(_seat, lobby, new Vector2Int(width / 2 + 1, 5), random);
            _sewingMachine.Option.CreateObj(_sewingMachine, lobby, new Vector2Int(width / 2 - 5, 3), random);
            return lobby;
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
    }
}
