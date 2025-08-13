namespace Roguegard.Rgpacks
{
    public static class DioramaFloorInfo
    {
        public static IDioramaFloorInfo Get(RogueObj obj)
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
        public static void SetTo(RogueObj obj, IDioramaFloorInfo floor)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            // 上書き不可
            if (info.info != null) throw new System.InvalidOperationException();

            info.info = floor;
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public IDioramaFloorInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo coming) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
