using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;
using Roguegard.CharacterCreation;
using Roguegard.Extensions;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    public class RogueObjUserData
    {
        public RogueObj Obj { get; }

        public RogueObjUserData(RogueObj obj)
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

        public override string ToString()
        {
            return Obj.GetName();
        }

        [MoonSharpUserDataMetamethod("__concat")] public static string Concat(RogueObjUserData o, string v) => o.ToString() + v;
        [MoonSharpUserDataMetamethod("__concat")] public static string Concat(string v, RogueObjUserData o) => o.ToString() + v;
        [MoonSharpUserDataMetamethod("__concat")] public static string Concat(RogueObjUserData o1, RogueObjUserData o2) => o1.ToString() + o2.ToString();
    }
}
