using System.Linq;

namespace Roguegard.Rgpacks
{
    public class MysteryDioramaAsset : IEvtAsset
    {
        private readonly EvtFairyReference infoSet;

        private IDioramaFloorAsset[] floors;

        public MysteryDioramaAsset(string envRgpackId, string fullId)
        {
            var point = new EvtFairyAsset.Page();
            infoSet = new EvtFairyReference(fullId, envRgpackId, point);
        }

        public RogueObj StartDungeon(RogueObj player, IRogueRandom random)
        {
            floors ??= RgpackReference.GetSubAssets<IDioramaFloorAsset>(infoSet.FullId, infoSet.RgpackId).ToArray();

            var world = RogueWorldInfo.GetWorld(player);
            var dungeon = infoSet.CreateObj(world);
            var floor = infoSet.CreateObj(dungeon);
            floors[0].GenerateFloor(player, floor, random);
            return dungeon;
        }

        public EvtFairyReference GetInfoSet()
        {
            return infoSet;
        }
    }
}
