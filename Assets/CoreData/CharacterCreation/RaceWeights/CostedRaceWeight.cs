using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class CostedRaceWeight : ReferableScript, IRaceOptionWeight
    {
        public float GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            if (raceOption.Ability.HasFlag(MainInfoSetAbility.HasCollider))
            {
                // 当たり判定がある場合は重くする。
                return 21;
            }
            else
            {
                // 当たり判定がなければコストに準拠する。（0 ~ 1 の範囲）
                // コスト 1 あたり重さ 1 とする
                return Mathf.Clamp01(raceOption.Cost);
            }
        }
    }
}
