using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class ViewInfo : IRogueObjInfo, IRogueTilemapView
    {
        private ViewMap viewMap;

        [field: System.NonSerialized] public bool QueueHasItem { get; private set; }

        public int Width => viewMap.Width;
        public int Height => viewMap.Height;

        Vector2Int IRogueTilemapView.Size => new Vector2Int(Width, Height);

        Spanning<RogueObj> IRogueTilemapView.VisibleObjs => viewMap.VisibleObjs;

        public int VisibleObjCount => viewMap.VisibleObjs.Count;

        public RogueObj Location => viewMap.Location;

        bool IRogueObjInfo.IsExclusedWhenSerialize => false;

        public ViewInfo()
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

        public RogueObj GetVisibleObj(int index)
        {
            return viewMap.GetVisibleObj(index);
        }

        public bool ContainsVisible(RogueObj obj)
        {
            return viewMap.VisibleObjs.Contains(obj);
        }

        public void GetTile(Vector2Int position, out bool visible, out IRogueTile tile, out RogueObj tileObj)
        {
            viewMap.GetTile(position, out visible, out tile, out tileObj);
        }

        public bool HasStopperAt(Vector2Int position, bool invisibleIsStopper = true)
        {
            GetTile(position, out _, out var tile, out var tileObj);
            if (tile != null) return tile.Info.HasCollider || tile.Info.Category == CategoryKw.Pool;
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

        bool IRogueObjInfo.CanStack(IRogueObjInfo other) => false;
        IRogueObjInfo IRogueObjInfo.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
        IRogueObjInfo IRogueObjInfo.ReplaceCloned(RogueObj obj, RogueObj clonedObj) => null;
    }
}
