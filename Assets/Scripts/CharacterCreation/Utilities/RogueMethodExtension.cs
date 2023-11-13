using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Extensions
{
    public static class RogueMethodExtension
    {
        public static bool Locate(
            this IChangeStateRogueMethodCaller method, RogueObj self, RogueObj user, RogueObj location, float activationDepth)
        {
            var locateMethod = self.Main.InfoSet.Locate;
            var arg = new RogueMethodArgument(targetObj: location);
            return RogueMethodAspectState.Invoke(MainInfoKw.Locate, locateMethod, self, user, activationDepth, arg);
        }

        public static bool Locate(
            this IChangeStateRogueMethodCaller method, RogueObj self, RogueObj user, RogueObj location, Vector2Int position, float activationDepth)
        {
            var locateMethod = self.Main.InfoSet.Locate;
            var arg = new RogueMethodArgument(targetObj: location, targetPosition: position);
            return RogueMethodAspectState.Invoke(MainInfoKw.Locate, locateMethod, self, user, activationDepth, arg);
        }

        public static bool Polymorph(
            this IChangeStateRogueMethodCaller method, RogueObj self, RogueObj user, MainInfoSet infoSet, float activationDepth)
        {
            var polymorphMethod = self.Main.InfoSet.Polymorph;
            var arg = new RogueMethodArgument(other: infoSet);
            return RogueMethodAspectState.Invoke(MainInfoKw.Polymorph, polymorphMethod, self, user, activationDepth, arg);
        }

        public static void Coloring(this IChangeStateRogueMethodCaller method, RogueObj obj, RogueObj user, Color color, float activationDepth)
        {
            ColoringEffect.ColorChange(obj, color);

            // 装備品の色変更を適用するため Polymorph を実行する。
            // （装備品を Polymorph させることによって再装備処理が入る）
            method.Polymorph(obj, user, obj.Main.InfoSet, activationDepth);
        }

        public static void RemoveColoring(this IChangeStateRogueMethodCaller method, RogueObj obj, RogueObj user, float activationDepth)
        {
            ColoringEffect.RemoveColor(obj);

            // 装備品の色変更を適用するため Polymorph を実行する。
            method.Polymorph(obj, user, obj.Main.InfoSet, activationDepth);
        }
    }
}
