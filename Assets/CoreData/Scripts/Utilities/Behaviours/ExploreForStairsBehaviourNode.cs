using Roguegard.Extensions;
using UnityEngine;

namespace Roguegard
{
    public class ExploreForStairsBehaviourNode : IRogueBehaviourNode
    {
        public IPathBuilder PathBuilder { get; set; }
        public RoguePositionSelector PositionSelector { get; set; }

        private bool selectedPositionIsEnabled;
        private Vector2Int selectedPosition;

        private static readonly CommandFloorDownStairs apply = new();
        private static readonly CommandDownStairs apply2 = new();

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (!ViewInfo.TryGet(self, out var view))
            {
                view = ViewInfo.SetTo(self);
            }

            view.ReadyView(self.Location);
            view.AddView(self);

            for (int i = 0; i < view.VisibleObjCount; i++)
            {
                var obj = view.GetVisibleObj(i);
                if (obj == null) continue;
                if (obj.Main.InfoSet.Category != CategoryKw.LevelDownStairs && obj.Main.InfoSet.Category != CategoryKw.DownStairs) continue;

                if (obj.Position == self.Position)
                {
                    // 階段についたら使う
                    if (obj.Main.InfoSet.Category == CategoryKw.LevelDownStairs)
                    {
                        if (apply.CommandInvoke(self, null, activationDepth, new(tool: obj)))
                        {
                            return default;
                        }
                    }
                    else
                    {
                        if (apply2.CommandInvoke(self, null, activationDepth, new(tool: obj)))
                        {
                            return default;
                        }
                    }
                }

                // 階段を見つけたらそこまで移動
                if (!PathBuilder.UpdatePath(self, obj.Position)) return RogueObjUpdaterContinueType.Continue;
                if (!PathBuilder.TryGetNextDirection(self, out var nextDirection)) return RogueObjUpdaterContinueType.Continue;

                default(IActiveRogueMethodCaller).Walk(self, nextDirection, activationDepth, true);
                selectedPositionIsEnabled = false;
                return RogueObjUpdaterContinueType.Break;
            }
            {
                if (!selectedPositionIsEnabled ||
                    !PathBuilder.UpdatePath(self, selectedPosition) ||
                    !PathBuilder.TryGetNextDirection(self, out _))
                {
                    selectedPositionIsEnabled = false;
                }
                if (!selectedPositionIsEnabled)
                {
                    // 階段が見つからなかったら探索
                    if (!UpdateSelectedPosition(self, view)) return RogueObjUpdaterContinueType.Continue;
                }
                if (!PathBuilder.UpdatePath(self, selectedPosition) ||
                    !PathBuilder.TryGetNextDirection(self, out var nextDirection)) return RogueObjUpdaterContinueType.Continue;

                default(IActiveRogueMethodCaller).Walk(self, nextDirection, activationDepth, true);
                return RogueObjUpdaterContinueType.Break;
            }
        }

        private bool UpdateSelectedPosition(RogueObj self, ViewInfo view)
        {
            PositionSelector.Update(view);
            if (!PositionSelector.TryGetTargetPosition(self.Position, out var targetPosition)) return false;

            selectedPositionIsEnabled = true;
            selectedPosition = targetPosition;
            return true;
        }
    }
}
