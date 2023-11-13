using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public interface IRogueObjUpdater : IActiveRogueMethodCaller
    {
        /// <summary>
        /// プレイヤーの入力からの行動操作用 <see cref="IRogueObjUpdater"/> は 0f 、
        /// <see cref="RogueObj"/> 自身の行動 <see cref="IRogueObjUpdater"/> は 1f 。
        /// </summary>
        float Order { get; }

        RogueObjUpdaterContinueType UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex);
    }
}
