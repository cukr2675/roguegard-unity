using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IRogueMethodAspectLogger
    {
        void LogInvoke(
            string rankName, IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);

        void LogEndInvoke(
            string rankName, IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
            bool result);

        void LogActiveAspect(
            IRogueMethodActiveAspect aspect, IKeyword keyword, IRogueMethod method,
            RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg);

        void LogPassiveAspect(
            IRogueMethodPassiveAspect aspect, IKeyword keyword, IRogueMethod method,
            RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);
    }
}
