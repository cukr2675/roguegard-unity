using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.Rgpacks
{
    public class MapDioramaFloorAsset : IDioramaFloorAsset
    {
        private readonly string fullId;
        private readonly RogueTilemap tilemap;

        private EvtFairyAsset[] evts;

        public MapDioramaFloorAsset(MapDioramaFloorInfo info, string fullId)
        {
            this.fullId = fullId;
            tilemap = new RogueTilemap(info.Tilemap);
        }

        public void GenerateFloor(RogueObj player, RogueObj floor, IRogueRandom random)
        {
            if (evts == null)
            {
                evts = RgpackReference.GetSubAssets<EvtFairyAsset>(fullId, "").ToArray();
            }

            var tilemap = new RogueTilemap(this.tilemap);
            floor.Space.SetTilemap(tilemap);
            floor.Space.SetRooms(new[] { floor.Space.Tilemap.Rect });
            SpaceUtility.TryLocate(player, floor, Vector2Int.one);

            foreach (var evt in evts)
            {
                evt.GetInfoSet().CreateObj(floor, RogueRandom.Primary);
            }
        }
    }
}
