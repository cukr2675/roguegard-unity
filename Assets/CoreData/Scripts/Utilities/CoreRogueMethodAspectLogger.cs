using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CoreRogueMethodAspectLogger : IRogueMethodAspectLogger
    {
        public void LogInvoke(
            string rankName, IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var methodName = method is IRogueDescription methodDescription ? $"{method} [{methodDescription.Name}]" : $"{method}";
            var keywordName = keyword.Name;
            if (keyword == MainInfoKw.Locate && arg.TargetObj == null) { keywordName = "Destruct"; }
            Debug.Log($"[{rankName}] {user?.GetName()} => {self?.GetName()} {keywordName} {arg.Tool} {arg.TargetObj} {arg.Other} ({methodName})");
        }

        public void LogEndInvoke(
            string rankName, IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg, bool result)
        {
            var methodName = method is IRogueDescription methodDescription ? $"{method} [{methodDescription.Name}]" : $"{method}";
            var keywordName = keyword.Name;
            if (keyword == MainInfoKw.Locate && arg.TargetObj == null) { keywordName = "Destruct"; }
            Debug.Log($"[{result}] {user?.GetName()} => {self?.GetName()} {keywordName} {arg.Tool} {arg.TargetObj} {arg.Other} ({methodName})");
        }

        public void LogActiveAspect(
            IRogueMethodActiveAspect aspect, IKeyword keyword, IRogueMethod method,
            RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg)
        {
            var keywordName = keyword.Name;
            if (keyword == MainInfoKw.Locate && arg.TargetObj == null) { keywordName = "Destruct"; }
            Debug.Log($"[Active] {self?.GetName()} => {target?.GetName()} {keywordName} {arg.Tool} {arg.TargetObj} {arg.Other} ({aspect})");
        }

        public void LogPassiveAspect(
            IRogueMethodPassiveAspect aspect, IKeyword keyword, IRogueMethod method,
            RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var keywordName = keyword.Name;
            if (keyword == MainInfoKw.Locate && arg.TargetObj == null) { keywordName = "Destruct"; }
            Debug.Log($"[Passive] {user?.GetName()} => {self?.GetName()} {keywordName} {arg.Tool} {arg.TargetObj} {arg.Other} ({aspect})");
        }
    }
}
