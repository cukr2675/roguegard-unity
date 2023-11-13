using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// このクラス自体が <see cref="IRogueMethod"/> になる選択肢モデルクラス
    /// </summary>
    public abstract class CommandRogueMethod : BaseObjCommand, IActiveRogueMethod
    {
        public abstract IKeyword Keyword { get; }

        public override string Name => Keyword.Name;

        public override bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            EnqueueMessageRule(Keyword);
            return RogueMethodAspectState.Invoke(Keyword, this, self, user, activationDepth, arg);
        }

        public abstract bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);
    }
}
