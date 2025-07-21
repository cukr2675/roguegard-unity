using Roguegard.Extensions;

namespace Roguegard
{
    public class WanderingBehaviourNode : IRogueBehaviourNode
    {
        private readonly WanderingWalker walker = new(RoguegardSettings.MaxTilemapSize);

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            var nextPosition = walker.GetWalk(self, false);
            if (MovementUtility.TryGetApproachDirection(self, nextPosition, true, out var approachDirection))
            {
                default(IActiveRogueMethodCaller).Walk(self, approachDirection, activationDepth);
                walker.GetWalk(self, false); // 移動した直後の視界でパスを更新
            }
            return RogueObjUpdaterContinueType.Break;
        }
    }
}
