using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;

namespace Roguegard.Scripting.MoonSharp
{
    public class MoonSharpScriptingEvent : IScriptingCmn
    {
        private readonly MoonSharpScriptingType type;
        private Table scriptingInstance;

        public MoonSharpScriptingEvent(MoonSharpScriptingType type)
        {
            this.type = type;
        }

        public void Invoke()
        {
            if (scriptingInstance == null)
            {
                var type = this.type.GetTable();
                var typeNew = type.Get("new").Function;
                scriptingInstance = typeNew.Call(type).Table;
            }

            var invokeMethod = scriptingInstance.MetaTable.Get("invoke").Function;
            invokeMethod.Call();
        }
    }
}
