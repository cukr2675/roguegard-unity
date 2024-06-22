using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IRogueObjGenerator
    {
        IMainInfoSet InfoSet { get; }

        int Lv { get; }

        int Stack { get; }

        /// <summary>
        /// オブジェクト生成時の所持アイテム。宝箱の中身もここで設定する。
        /// </summary>
        Spanning<IWeightedRogueObjGeneratorList> StartingItemTable { get; }

        RogueObj CreateObj(RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default);
    }
}
