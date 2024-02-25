using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using MoonSharp.Interpreter;

namespace Roguegard.Scripting.MoonSharp
{
    [MoonSharpUserData]
    [ObjectFormer.Formable]
    public class MoonSharpScriptingType : System.IEquatable<MoonSharpScriptingType>
    {
        private readonly string name;
        private readonly NotepadQuote[] sources;
        
        [System.NonSerialized] private Table table;

        private static readonly List<MoonSharpScriptingType> caches = new List<MoonSharpScriptingType>();

        private MoonSharpScriptingType(string name, IEnumerable<NotepadQuote> sources)
        {
            this.name = name;
            this.sources = sources.ToArray();
        }

        public static MoonSharpScriptingType Get(string typeName, IEnumerable<NotepadQuote> sources)
        {
            foreach (var cache in caches)
            {
                // 名前が同じかつソースを網羅するキャッシュを取得
                if (cache.Match(typeName, sources)) return cache;
            }
            
            // キャッシュが見つからなかったら生成
            var type = new MoonSharpScriptingType(typeName, sources);
            caches.Add(type);
            return type;
        }

        private bool Match(string typeName, IEnumerable<NotepadQuote> sources)
        {
            if (typeName != name) return false;

            foreach (var source in sources)
            {
                if (System.Array.IndexOf(this.sources, source) == -1) return false;
            }
            return true;
        }

        public Table GetTable()
        {
            if (table != null) return table;

            var script = new MoonSharpRogueScript(sources);
            table = script.DoString(sources[0]).Table.Get(name).Table;
            return table;
        }

        public bool Equals(MoonSharpScriptingType other)
        {
            if (other.name != name) return false;
            if (other.sources.Length != sources.Length) return false;
            for (int i = 0; i < sources.Length; i++)
            {
                if (other.sources[i].Text != sources[i].Text) return false;
            }
            return true;
        }

        public override int GetHashCode() => name.GetHashCode();
    }
}
