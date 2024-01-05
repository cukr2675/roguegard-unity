using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class ExploreForStairsBehaviourNode : IRogueBehaviourNode
    {
        public IPathBuilder PathBuilder { get; set; }
        public RoguePositionSelector PositionSelector { get; set; }

        private static readonly CommandFloorDownStairs apply = new CommandFloorDownStairs();
        private static readonly CommandDownStairs apply2 = new CommandDownStairs();

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (!self.TryGet<ViewInfo>(out var view)) return RogueObjUpdaterContinueType.Continue;

            view.ReadyView(self.Location);
            view.AddView(self);

            for (int i = 0; i < view.VisibleObjCount; i++)
            {
                var obj = view.GetVisibleObj(i);
                if (obj == null) continue;
                if (obj.Main.InfoSet.Category != CategoryKw.LevelDownStairs && obj.Main.InfoSet.Category != CategoryKw.DownStairs) continue;

                if (obj.Position == self.Position)
                {
                    // ŠK’i‚É‚Â‚¢‚½‚çŽg‚¤
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

                // ŠK’i‚ðŒ©‚Â‚¯‚½‚ç‚»‚±‚Ü‚ÅˆÚ“®
                if (!PathBuilder.UpdatePath(self, obj.Position)) return RogueObjUpdaterContinueType.Continue;
                if (!PathBuilder.TryGetNextPosition(self, out var nextDirection)) return RogueObjUpdaterContinueType.Continue;

                default(IActiveRogueMethodCaller).Walk(self, nextDirection, activationDepth);
                return RogueObjUpdaterContinueType.Break;
            }
            {
                // ŠK’i‚ªŒ©‚Â‚©‚ç‚È‚©‚Á‚½‚ç’Tõ
                PositionSelector.Update(view);
                if (!PositionSelector.TryGetTargetPosition(self.Position, out var targetPosition)) return RogueObjUpdaterContinueType.Continue;

                if (!PathBuilder.UpdatePath(self, targetPosition)) return RogueObjUpdaterContinueType.Continue;
                if (!PathBuilder.TryGetNextPosition(self, out var nextDirection)) return RogueObjUpdaterContinueType.Continue;

                default(IActiveRogueMethodCaller).Walk(self, nextDirection, activationDepth);
                return RogueObjUpdaterContinueType.Break;
            }
        }
    }
}
