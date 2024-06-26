using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class AdversityAbility : AbilityIntrinsicOptionScript
    {
		public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(lv);
        }

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueMethodActiveAspect
        {
            float IRogueMethodActiveAspect.Order => 0f;

            public SortedIntrinsic(int lv) : base(lv) { }

            bool IRogueMethodActiveAspect.ActiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.ActiveChain chain)
            {
                if (keyword == MainInfoKw.Hit && AttackUtility.GetUseValue(arg.RefValue))
                {
                    // 周囲8マスに敵が4体以上いるときダメージ追加
                    var enemyCount = 0;
                    var spaceObjs = self.Location.Space.Objs;
                    for (int i = 0; i < spaceObjs.Count; i++)
                    {
                        var obj = spaceObjs[i];
                        if (obj == null) continue;

                        if (RogueMethodUtility.GetAdjacent(self, obj) && StatsEffectedValues.AreVS(self, obj)) { enemyCount++; }
                    }
                    arg.RefValue.MainValue += Mathf.Max((enemyCount - 3) * 2, 0);
                }
                return chain.Invoke(keyword, method, self, target, activationDepth, arg);
            }
        }
    }
}
