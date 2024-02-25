using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace Roguegard.Scripting.MoonSharp
{
    public class MoonSharpRogueScript
    {
        private Script script;
        private bool initial;
        private StaticID staticID;

        private readonly Dictionary<string, NotepadQuote> sources;

        static MoonSharpRogueScript()
        {
            UserData.RegisterAssembly(typeof(MoonSharpScriptingType).Assembly);
        }

        public MoonSharpRogueScript()
        {
        }

        public MoonSharpRogueScript(IEnumerable<NotepadQuote> sources)
        {
            this.sources = new Dictionary<string, NotepadQuote>();
            foreach (var source in sources)
            {
                this.sources.Add(source.Name, source);
            }
        }

        public void Call(NotepadQuote code, RogueObj caster = null)
        {
            DoString(code, caster);
        }

        internal DynValue DoString(NotepadQuote code, RogueObj caster = null)
        {
            if (!staticID.IsValid)
            {
                script = new Script();
                script.Options.DebugPrint = x => Debug.Log(x);
                script.Globals.RegisterModuleType<RoguegardModule>();
                script.Globals.Set("__rgenv", UserData.Create(new AnonWrapper<MoonSharpRogueScript>(this)));
                if (sources != null)
                {
                    script.Globals.Set("__files", UserData.Create(new AnonWrapper<Dictionary<string, NotepadQuote>>(sources)));
                }
                script.Globals.Set("__loaded", UserData.Create(new AnonWrapper<List<NotepadQuote>>(new List<NotepadQuote>())));
                initial = true;
                staticID = StaticID.Current;
            }
            var initialStack = initial;
            if (initial)
            {
                initial = false;
            }

            var oldCaster = script.Globals.Get("__caster");
            var oldFile = script.Globals.Get("__file");
            if (caster != null)
            {
                script.Globals.Set("__caster", UserData.Create(new AnonWrapper<RogueObj>(caster)));
            }
            else
            {
                script.Globals.Remove("__caster");
            }
            script.Globals.Set("__file", UserData.Create(new AnonWrapper<NotepadQuote>(code)));

            // 依存関係を更新
            var loaded = ((AnonWrapper<List<NotepadQuote>>)script.Globals.Get("__loaded").UserData.Object).Value;
            loaded.Remove(code); // すでに読み込まれているコードが再度読み込まれたら末尾へ移動させる
            loaded.Add(code);

            var returnValue = script.DoString(code.Text);

            script.Globals.Set("__caster", oldCaster);
            script.Globals.Set("__file", oldFile);

            if (initialStack)
            {
                loaded.Clear();
                initial = true;
            }

            return returnValue;
        }
    }
}
