using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class ViewInfo : IRogueTilemapView
    {
        [System.NonSerialized] private ViewMap viewMap;

        [field: System.NonSerialized] public bool QueueHasItem { get; private set; }

        public int Width => viewMap.Width;
        public int Height => viewMap.Height;

        Vector2Int IRogueTilemapView.Size => new Vector2Int(Width, Height);

        Spanning<RogueObj> IRogueTilemapView.VisibleObjs => viewMap.VisibleObjs;

        public int VisibleObjCount => viewMap.VisibleObjs.Count;

        public RogueObj Location => viewMap.Location;

        private ViewInfo()
        {
            viewMap = new ViewMap(RoguegardSettings.MaxTilemapSize);
        }

        private ViewInfo(bool flag)
        {
            viewMap = new ViewMap(RoguegardSettings.MaxTilemapSize);
        }

        /// <summary>
        /// <paramref name="location"/> にカメラを空間移動させる
        /// </summary>
        public void ReadyView(RogueObj location)
        {
            viewMap.ResetVisibles(location);
        }

        /// <summary>
        /// このインスタンスに <paramref name="self"/> からの視界を追加する。
        /// </summary>
        public void AddView(RogueObj self)
        {
            viewMap.AddView(self);
        }

        public void AddVisibleObj(RogueObj obj, bool addTile)
        {
            viewMap.AddUnique(obj);
            if (addTile) { viewMap.AddPoint(obj.Position); }
        }

        public RogueObj GetVisibleObj(int index)
        {
            return viewMap.GetVisibleObj(index);
        }

        public bool ContainsVisible(RogueObj obj)
        {
            return viewMap.VisibleObjs.Contains(obj);
        }

        public void GetTile(Vector2Int position, out bool visible, out IRogueTile groundTile, out IRogueTile buildingTile, out RogueObj tileObj)
        {
            viewMap.GetTile(position, out visible, out groundTile, out buildingTile, out tileObj);
        }

        public bool HasStopperAt(Vector2Int position, bool invisibleIsStopper = true)
        {
            GetTile(position, out _, out var groundTile, out var buildingTile, out var tileObj);
            var topTile = buildingTile ?? groundTile;
            if (topTile != null) return topTile.Info.HasCollider || topTile.Info.Category == CategoryKw.Pool;
            if (tileObj != null) return tileObj.HasCollider;
            return invisibleIsStopper;
        }

        /// <summary>
        /// 視界を固定する。
        /// </summary>
        public void EnqueueState()
        {
            QueueHasItem = true;
        }

        /// <summary>
        /// 視界の固定を解除する。
        /// </summary>
        public void DequeueState()
        {
            QueueHasItem = false;
        }

        public static ViewInfo Get(RogueObj obj)
        {
            return obj.Get<Info>().info;
        }

        public static bool TryGet(RogueObj obj, out ViewInfo info)
        {
            if (obj.TryGet<Info>(out var value))
            {
                info = value.info;
                return true;
            }
            else
            {
                info = null;
                return false;
            }
        }

        public static ViewInfo SetTo(RogueObj obj)
        {
            var info = new Info();
            info.info = new ViewInfo(false);
            obj.SetInfo(info);
            return info.info;
        }

        public static void RemoveFrom(RogueObj obj)
        {
            obj.RemoveInfo(typeof(Info));
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public ViewInfo info;

            bool IRogueObjInfo.IsExclusedWhenSerialize => false;

            bool IRogueObjInfo.CanStack(IRogueObjInfo other) => false;
            IRogueObjInfo IRogueObjInfo.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            IRogueObjInfo IRogueObjInfo.ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
