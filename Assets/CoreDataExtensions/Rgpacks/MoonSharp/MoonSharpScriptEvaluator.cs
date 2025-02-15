using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;

namespace Roguegard.Rgpacks.MoonSharp
{
    public class MoonSharpScriptEvaluator : ReferableScript, IScriptEvaluator
    {
        public IEnumerable<KeyValuePair<string, object>> Evaluate(string code, string envRgpackId)
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
                    yield return new KeyValuePair<string, object>(pair.Key.String, new Cmn(pair.Value, envRgpackId));
                }
            }
        }

        private class Cmn : ICmnAssset
        {
            private readonly DynValue value;
            private readonly Table table;
            private readonly string envRgpackId;

            private static readonly List<DynValue> dynArguments = new();

            public IReadOnlyDictionary<string, ICmnPropertySource> PropertySources { get; }

            public Cmn(DynValue value, string envRgpackId)
            {
                this.value = value;
                table = value.Table;
                this.envRgpackId = envRgpackId;

                var propertySources = new Dictionary<string, ICmnPropertySource>();
                foreach (var pair in table.Pairs)
                {
                    var obj = pair.Value.UserData?.Object;
                    if (obj is NumberCmnPropertyUserData)
                    {
                        propertySources.Add(pair.Key.String, NumberCmnProperty.SourceInstance);
                    }
                    else if (obj is StartingItemCmnPropertyUserData)
                    {
                        propertySources.Add(pair.Key.String, StartingItemCmnProperty.SourceInstance);
                    }
                    else if (obj is StartingItemTableCmnPropertyUserData)
                    {
                        propertySources.Add(pair.Key.String, StartingItemTableCmnProperty.SourceInstance);
                    }
                }
                PropertySources = propertySources;
            }

            public object Invoke(IReadOnlyDictionary<string, ICmnProperty> properties, Spanning<object> arguments)
            {
                if (properties != null)
                {
                    foreach (var pair in properties)
                    {
                        if (pair.Value is NumberCmnProperty numberCmnProperty)
                        {
                            table.Set(pair.Key, UserData.Create(new NumberCmnPropertyUserData(numberCmnProperty)));
                        }
                        else if (pair.Value is StartingItemCmnProperty startingItemCmnProperty)
                        {
                            table.Set(pair.Key, UserData.Create(new StartingItemCmnPropertyUserData(startingItemCmnProperty, envRgpackId)));
                        }
                        else if (pair.Value is StartingItemTableCmnProperty startingItemTableCmnProperty)
                        {
                            table.Set(pair.Key, UserData.Create(new StartingItemTableCmnPropertyUserData(startingItemTableCmnProperty, envRgpackId)));
                        }
                    }
                }
                var function = table.Get("invoke").Function;
                var coroutine = function.OwnerScript.CreateCoroutine(function).Coroutine;
                dynArguments.Clear();
                dynArguments.Add(value); // self
                for (int i = 0; i < arguments.Count; i++)
                {
                    if (arguments[i] is RogueObj obj)
                    {
                        dynArguments.Add(UserData.Create(new RogueObjUserData(obj)));
                    }
                    else
                    {
                        dynArguments.Add(DynValue.Nil);
                    }
                }

                var oldRgpackId = function.OwnerScript.Globals.Get("__rgpack");
                function.OwnerScript.Globals.Set("__rgpack", DynValue.NewString(envRgpackId));

                DynValue result;
                try
                {
                    result = coroutine.Resume(dynArguments.ToArray());
                }
                catch (InterpreterException ex)
                {
                    Debug.LogError(string.Join("\n", ex.CallStack));
                    throw;
                }

                function.OwnerScript.Globals.Set("__rgpack", oldRgpackId);

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
