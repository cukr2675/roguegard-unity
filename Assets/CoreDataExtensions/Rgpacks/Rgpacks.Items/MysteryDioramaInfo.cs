namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class MysteryDioramaInfo
    {
        private MysteryDioramaInfo() { }

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
        /// 上書き不可
        /// </summary>
        public static void SetTo(RogueObj obj)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            // 上書き不可
            if (info.info != null) throw new System.InvalidOperationException();

            info.info = new MysteryDioramaInfo();
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public MysteryDioramaInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo coming) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
