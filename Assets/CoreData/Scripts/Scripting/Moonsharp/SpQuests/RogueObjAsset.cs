using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;
using Roguegard.Extensions;

namespace Roguegard.Scripting.MoonSharp
{
    [MoonSharpUserData]
    public class RogueObjAsset
    {
        public RogueObj Obj { get; }

        public RogueObjAsset(RogueObj obj)
        {
            Obj = obj;
        }

        public void walkUP(int steps) => walk(RogueDirection.Up, steps);
        public void walkDN(int steps) => walk(RogueDirection.Down, steps);
        public void walkRT(int steps) => walk(RogueDirection.Right, steps);
        public void walkLT(int steps) => walk(RogueDirection.Left, steps);
        public void walkUR(int steps) => walk(RogueDirection.UpperRight, steps);
        public void walkDR(int steps) => walk(RogueDirection.LowerRight, steps);
        public void walkUL(int steps) => walk(RogueDirection.UpperLeft, steps);
        public void walkDL(int steps) => walk(RogueDirection.LowerLeft, steps);

        private void walk(RogueDirection direction, int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                default(IActiveRogueMethodCaller).Walk(Obj, direction, 1f);
            }
        }
    }
}
