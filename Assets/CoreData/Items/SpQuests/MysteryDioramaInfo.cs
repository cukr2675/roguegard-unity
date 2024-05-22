using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class MysteryDioramaInfo
    {
        public string ID { get; set; }
        private List<IDioramaFloor> floors;

        private MysteryDioramaInfo() { }

        public RogueObj StartDungeon(RogueObj player, IRogueRandom random)
        {
            var world = RogueWorldInfo.GetWorld(player);

            var info = new EvtFairyInfo();
            info.Position = Vector2Int.zero;
            var infoSet = new EvtInstanceInfoSet(info, RoguegardSettings.CharacterCreationDatabase.LoadPreset(0));
            var dungeon = infoSet.CreateObj(world, random);
            var floor = infoSet.CreateObj(dungeon, random);
            floors[0].GenerateFloor(player, floor, random);
            return dungeon;
        }

        public void AddAssets(RogueObj diorama, string rgpackID)
        {
            floors = new List<IDioramaFloor>();
            var dioramaFloors = diorama.Space.Objs;
            for (int i = 0; i < dioramaFloors.Count; i++)
            {
                var dioramaFloorObj = dioramaFloors[i];
                if (dioramaFloorObj == null) continue;

                var dioramaFloor = DioramaFloorInfo.Get(dioramaFloorObj);
                if (dioramaFloor != null)
                {
                    dioramaFloor.AddAssets(dioramaFloorObj, rgpackID);
                    floors.Add(dioramaFloor);
                }
            }
        }

        public static MysteryDioramaInfo Get(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.info;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// è„èëÇ´ïsâ¬
        /// </summary>
        public static void SetTo(RogueObj obj)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            // è„èëÇ´ïsâ¬
            if (info.info != null) throw new RogueException();

            info.info = new MysteryDioramaInfo();
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public MysteryDioramaInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other)
            {
                return false;
            }

            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return null;
            }

            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj)
            {
                return this;
            }
        }
    }
}
