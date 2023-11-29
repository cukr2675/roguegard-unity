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
                RogueMethodAspectState.ActiveNext next)
            {
                if (keyword == MainInfoKw.Hit && arg.RefValue?.MainValue > 0f)
                {
                    // 周囲8マスに敵が3体以上いるときダメージ追加
                    var enemyCount = 0;
                    var spaceObjs = self.Location.Space.Objs;
                    for (int i = 0; i < spaceObjs.Count; i++)
                    {
                        var obj = spaceObjs[i];
                        if (obj == null) continue;

                        if (RogueMethodUtility.GetAdjacent(self, obj) && StatsEffectedValues.AreVS(self, obj)) { enemyCount++; }
                    }
                    arg.RefValue.MainValue += Mathf.Max((enemyCount - 2) * 2, 0);
                }
                return next.Invoke(keyword, method, self, target, activationDepth, arg);
            }
        }
    }
}
