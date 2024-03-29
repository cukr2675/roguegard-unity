using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/Tiles/NullEquivalentRuleTile")]
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
