using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class RogueCalculatorExtension
    {
        /// <summary>
        /// 指定のキーの値が非ゼロであれば true を取得する。
        /// </summary>
        public static bool SubIs(this IRogueCalculator calculator, IKeyword key)
        {
            return calculator.SubValues(key) != 0f;
        }
    }
}
