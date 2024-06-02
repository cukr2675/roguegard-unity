using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;

namespace Roguegard.Scripting.MoonSharp
{
    public class MoonSharpScriptingEvaluator : ReferableScript, IScriptingEvaluator
    {
        public void Evaluate(string code, RgpackBuilder rgpack)
        {
            var quote = new NotepadQuote("", code);
            var script = new MoonSharpRogueScript(new[] { quote });
            //var module = Script.RunString(code);
            var module = script.DoString(quote);
            foreach (var pair in module.Table.Pairs)
            {
                if (pair.Key.Type != DataType.String) continue;

                var value = pair.Value.Table;
                if (value.Get("__type").String == "Event")
                {
                    rgpack.AddAsset(pair.Key.String, new Cmn(value));
                }
            }
        }

        private class Cmn : IScriptingCmn
        {
            private readonly Table table;

            public Cmn(Table table)
            {
                this.table = table;
            }

            public void Invoke()
            {
                var function = table.Get("invoke").Function;
                var coroutine = function.OwnerScript.CreateCoroutine(function).Coroutine;
                coroutine.Resume();
            }
        }
    }
}
