using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;

namespace Roguegard.Rgpacks.MoonSharp
{
    public class MoonSharpScriptEvaluator : ReferableScript, IScriptEvaluator
    {
        public IEnumerable<KeyValuePair<string, object>> Evaluate(string code)
        {
            var script = new MoonSharpRogueScript();
            //var module = Script.RunString(code);
            var module = script.DoString(code);
            foreach (var pair in module.Table.Pairs)
            {
                if (pair.Key.Type != DataType.String) continue;

                var value = pair.Value.Table;
                if (value.MetaTable?.Get("__type").String == "Cmn")
                {
                    yield return new KeyValuePair<string, object>(pair.Key.String, new Cmn(pair.Value));
                }
            }
        }

        private class Cmn : ICmnAssset
        {
            private readonly DynValue value;
            private readonly Table table;

            private static readonly DynValue[] parameters = new DynValue[1];

            public IReadOnlyDictionary<string, ICmnPropertySource> PropertySources { get; }

            public Cmn(DynValue value)
            {
                this.value = value;
                table = value.Table;
                var propertySources = new Dictionary<string, ICmnPropertySource>();
                foreach (var pair in table.Pairs)
                {
                    if (pair.Value.UserData?.Object is NumberCmnPropertyUserData)
                    {
                        propertySources.Add(pair.Key.String, Rgpacks.NumberCmnProperty.SourceInstance);
                    }
                }
                PropertySources = propertySources;
            }

            public object Invoke(IReadOnlyDictionary<string, ICmnProperty> properties)
            {
                if (properties != null)
                {
                    foreach (var pair in properties)
                    {
                        table.Set(pair.Key, UserData.Create(new NumberCmnPropertyUserData() { val = DynValue.NewNumber(((Rgpacks.NumberCmnProperty)pair.Value).Value) }));
                    }
                }
                var function = table.Get("invoke").Function;
                var coroutine = function.OwnerScript.CreateCoroutine(function).Coroutine;
                parameters[0] = value;
                var result = coroutine.Resume(parameters);
                if (result.Type == DataType.Number) return result.Number;
                if (result.Type == DataType.Tuple)
                {
                    var tuple = result.Tuple;
                    if (tuple.Length == 2)
                    {
                        return (tuple[0].ToObject(), tuple[1].ToObject());
                    }
                }
                return null;
            }
        }
    }
}
