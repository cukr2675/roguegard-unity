using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class MapDioramaFloorInfo : IDioramaFloorInfo
    {
        public RogueTilemap Tilemap { get; }

        public MapDioramaFloorInfo()
        {
        }

        public MapDioramaFloorInfo(RogueObj mapFloorObj)
        {
            Tilemap = new RogueTilemap(mapFloorObj.Space.Tilemap);
        }
    }
}
