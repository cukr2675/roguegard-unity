using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    /// <summary>
    /// 敵対する存在を発見したとき停止する。
    /// </summary>
    public class FoundEnemyWalkStopper : IStatedWalkStopper
    {
        private RogueObjList lastViews = new RogueObjList();

        public bool GetStop(RogueObj self)
        {
            var view = ViewInfo.Get(self);
            var checkView = CheckView(view);
            lastViews.Clear();
            for (int i = 0; i < view.VisibleObjCount; i++)
            {
                var viewObj = view.GetVisibleObj(i);
                lastViews.Add(viewObj);
            }
            return checkView;

            bool CheckView(ViewInfo view)
            {
                for (int i = 0; i < view.VisibleObjCount; i++)
                {
                    var viewObj = view.GetVisibleObj(i);
                    if (StatsEffectedValues.AreVS(self, viewObj) && !lastViews.Contains(viewObj)) return true;
                }
                return false;
            }
        }
    }
}
