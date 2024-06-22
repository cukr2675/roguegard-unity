using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.Rgpacks
{
    public class MysteryDioramaAsset : IEvtAsset
    {
        private readonly EvtFairyReference infoSet;

        private IDioramaFloorAsset[] floors;

        public MysteryDioramaAsset(MysteryDioramaInfo info, string envRgpackID, string fullID)
        {
            var point = new EvtFairyAsset.Point();
            infoSet = new EvtFairyReference(fullID, envRgpackID, point);
        }

        public RogueObj StartDungeon(RogueObj player, IRogueRandom random)
        {
            if (floors == null)
            {
                floors = RgpackReference.GetSubAssets<IDioramaFloorAsset>(infoSet.FullID, infoSet.RgpackID).ToArray();
            }

            var world = RogueWorldInfo.GetWorld(player);
            var dungeon = infoSet.CreateObj(world, random);
            var floor = infoSet.CreateObj(dungeon, random);
            floors[0].GenerateFloor(player, floor, random);
            return dungeon;
        }

        public EvtFairyReference GetInfoSet()
        {
            return infoSet;
        }
    }
}
