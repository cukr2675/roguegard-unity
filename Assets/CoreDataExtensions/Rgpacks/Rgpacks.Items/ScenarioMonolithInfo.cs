namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class ScenarioMonolithInfo
    {
        public string MainChart { get; set; }

        public static RogueObj GetAtelierByCharacter(RogueObj character)
        {
            var location = character;
            while (location != null)
            {
                foreach (var spaceObj in location.Space.Objs)
                {
                    if (spaceObj == null) continue;

                    // シナリオモノリスを含む空間をアトリエとして返す
                    var info = Get(spaceObj);
                    if (info != null) return location;
                }

                location = location.Location;
            }
            return null;
        }

        public static ScenarioMonolithInfo Get(RogueObj monolith)
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
            if (info.info != null) throw new System.InvalidOperationException();

            info.info = new ScenarioMonolithInfo();
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public ScenarioMonolithInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo coming)
            {
                throw new System.NotImplementedException();
            }

            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                throw new System.NotImplementedException();
            }

            public IRogueObjInfo ReplaceObj(RogueObj obj, RogueObj clonedObj)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
