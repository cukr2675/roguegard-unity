using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class SpQuestMonolithInfo
    {
        public string MainChart { get; set; }

        public static RogueObj GetAtelierByCharacter(RogueObj character)
        {
            var location = character;
            while (location != null)
            {
                var spaceObjs = location.Space.Objs;
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    if (spaceObjs[i] == null) continue;

                    // スペクエモノリスを含む空間をアトリエとして返す
                    var info = Get(spaceObjs[i]);
                    if (info != null) return location;
                }

                location = location.Location;
            }
            return null;
        }

        public static SpQuestMonolithInfo Get(RogueObj monolith)
        {
            if (monolith.TryGet<Info>(out var info))
            {
                return info.info;
            }
            else
            {
                return null;
            }
        }

        public static void SetTo(RogueObj monolith)
        {
            if (!monolith.TryGet<Info>(out var info))
            {
                info = new Info();
                monolith.SetInfo(info);
            }

            // 上書き不可
            if (info.info != null) throw new RogueException();

            info.info = new SpQuestMonolithInfo();
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public SpQuestMonolithInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other)
            {
                throw new System.NotImplementedException();
            }

            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                throw new System.NotImplementedException();
            }

            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
