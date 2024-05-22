using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class MapDioramaFloor : IDioramaFloor
    {
        public RogueTilemap Tilemap { get; private set; }

        private List<RgpackReference> items;

        public void GenerateFloor(RogueObj player, RogueObj floor, IRogueRandom random)
        {
            var tilemap = new RogueTilemap(Tilemap);
            floor.Space.SetTilemap(tilemap);
            foreach (var item in items)
            {
                var itemInfoSet = new EvtInstanceInfoSet(item);
                itemInfoSet.CreateObj(floor, random);
            }
            SpaceUtility.TryLocate(player, floor, Vector2Int.one);
        }

        public void AddAssets(RogueObj dioramaFloor, string rgpackID)
        {
            Tilemap = new RogueTilemap(dioramaFloor.Space.Tilemap);

            items = new List<RgpackReference>();
            var spaceObjs = dioramaFloor.Space.Objs;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var spaceObj = spaceObjs[i];
                if (spaceObj == null) continue;

                var eventFairyInfo = EvtFairyInfo.Get(spaceObj);
                if (eventFairyInfo != null)
                {
                    items.Add(new RgpackReference(rgpackID, eventFairyInfo.ID));
                }
            }
        }
    }
}
