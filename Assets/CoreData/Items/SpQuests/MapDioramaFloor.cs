using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class MapDioramaFloor : IDioramaFloor
    {
        public RogueTilemap Tilemap { get; private set; }

        private List<EvtFairyInfo> items;

        public void GenerateFloor(RogueObj player, RogueObj floor, IRogueRandom random)
        {
            var tilemap = new RogueTilemap(Tilemap);
            floor.Space.SetTilemap(tilemap);
            foreach (var item in items)
            {
                var itemInfoSet = item.CreateInfoSet();
                itemInfoSet.CreateObj(floor, random);
            }
            SpaceUtility.TryLocate(player, floor, Vector2Int.one);
        }

        public void AddAssets(RogueObj dioramaFloor, string rgpackID)
        {
            Tilemap = new RogueTilemap(dioramaFloor.Space.Tilemap);

            items = new List<EvtFairyInfo>();
            var spaceObjs = dioramaFloor.Space.Objs;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var spaceObj = spaceObjs[i];
                if (spaceObj == null) continue;

                var evtFairyInfo = EvtFairyInfo.Get(spaceObj);
                if (evtFairyInfo != null)
                {
                    evtFairyInfo.SetRgpackID(spaceObj, rgpackID);
                    items.Add(evtFairyInfo);
                }
            }
        }
    }
}
