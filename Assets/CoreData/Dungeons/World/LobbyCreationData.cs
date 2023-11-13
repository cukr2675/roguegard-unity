using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Data/Location/Lobby")]
    [ObjectFormer.Referable]
    public class LobbyCreationData : ItemCreationData
    {
        [SerializeField] private ScriptableRogueTile _floor = null;
        [SerializeField] private ScriptableRogueTile _roomWall = null;
        [SerializeField] private ScriptableRogueTile _wall = null;
        [SerializeField] private ScriptableStartingItem _doorL = null;
        [SerializeField] private ScriptableStartingItem _doorR = null;
        [SerializeField] private ScriptableStartingItem _storage = null;

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
                    tilemap.Set(_floor, x, y);
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

            _storage.Option.CreateObj(_storage, lobby, new Vector2Int(width / 2 + 1, 3), random);
            return lobby;
        }
    }
}
