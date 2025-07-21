using UnityEngine;
using UnityEngine.Tilemaps;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "Roguegard/Tiles/Rule Tile/Null Equivalent Rule Tile")]
    public class NullEquivalentRuleTile : RuleTile
    {
        public override bool RuleMatch(int neighbor, TileBase other)
        {
            if (other == null)
            {
                switch (neighbor)
                {
                    case TilingRuleOutput.Neighbor.This: return true;
                    case TilingRuleOutput.Neighbor.NotThis: return false;
                }
            }
            return base.RuleMatch(neighbor, other);
        }
    }
}
