using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// <see cref="IValueEffect"/> の計算結果をキャッシュするインターフェース
    /// </summary>
    public interface IRogueCalculator
    {
        float MainBaseValue { get; }
        float MainValue { get; }

        float SubValues(IKeyword key);

        void Update(RogueObj self);
    }
}
