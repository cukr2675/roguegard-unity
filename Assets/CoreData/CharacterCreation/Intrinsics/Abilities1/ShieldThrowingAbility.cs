using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class ShieldThrowingAbility : AbilityIntrinsicOptionScript
    {
		public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
		{
            return new SortedIntrinsic(lv);
		}

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueMethodActiveAspect
        {
            float IRogueMethodActiveAspect.Order => 0f;

            private static readonly CommonBeShot beShotMethod = new CommonBeShot();

            public SortedIntrinsic(int lv) : base(lv) { }

            bool IRogueMethodActiveAspect.ActiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.ActiveChain chain)
            {
                if (keyword == MainInfoKw.BeThrown && target != null)
                {
                    var shieldInfo = target.Main.GetEquipmentInfo(target);
                    if (shieldInfo != null && shieldInfo.EquipParts.Contains(EquipKw.Shield) && shieldInfo.EquipIndex != -1)
                    {
                        // 装備している盾を投げたとき、当たった敵に攻撃力ダメージを与えるようにメソッドを変更する
                        return chain.Invoke(keyword, beShotMethod, self, target, activationDepth, arg);
                    }
                }
                return chain.Invoke(keyword, method, self, target, activationDepth, arg);
            }
		}
    }
}
