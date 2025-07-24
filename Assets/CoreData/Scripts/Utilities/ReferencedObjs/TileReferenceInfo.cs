using System.Collections.Generic;

namespace Roguegard
{
    [Objforming.Formable]
    public class TileReferenceInfo
    {
        private readonly List<IRogueTile> tiles;

        public int Count => tiles.Count;

        private TileReferenceInfo()
        {
            tiles = new List<IRogueTile>();
        }

        private TileReferenceInfo(IEnumerable<IRogueTile> tiles)
        {
            this.tiles = new List<IRogueTile>(tiles);
        }

        public IRogueTile Get(int index)
        {
            return tiles[index];
        }

        public static TileReferenceInfo Get(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.info;
            }
            return null;
        }

        public static void SetTo(RogueObj obj, IEnumerable<IRogueTile> tiles)
        {
            if (!obj.TryGet<Info>(out _))
            {
                obj.SetInfo(new Info
                {
                    info = new TileReferenceInfo(tiles)
                });
            }
            else
            {
                throw new System.InvalidOperationException("上書き不可");
            }
        }

        public static void SetTo(RogueObj obj, params IRogueTile[] tiles)
        {
            SetTo(obj, (IEnumerable<IRogueTile>)tiles);
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public TileReferenceInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo coming)
            {
                if (coming is not Info comingInfo) return false;
                if (info.Count != comingInfo.info.Count) return false;
                for (int i = 0; i < info.Count; i++)
                {
                    if (info.Get(i) != comingInfo.info.Get(i)) return false;
                }
                return true;
            }

            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return new Info() { info = new TileReferenceInfo(info.tiles) };
            }

            public IRogueObjInfo ReplaceObj(RogueObj obj, RogueObj clonedObj)
            {
                return this;
            }
        }
    }
}
