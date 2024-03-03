using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace Roguegard.Scripting.MoonSharp
{
    public class MoonSharpTableSerial
    {
        private Table table;
        private Table sTable;

        public Table Table => sTable;

        private Dictionary<string, DynValue> loadTable;

        public void Load(string key, DynValue value)
        {
            if (loadTable == null) { loadTable = new Dictionary<string, DynValue>(); }

            loadTable.Add(key, value);
        }

        public void SetTable(Table table)
        {
            this.table = table;
            sTable = table.Get("__s").Table;

            if (loadTable != null)
            {
                foreach (var pair in loadTable)
                {
                    sTable.Set(pair.Key, pair.Value);
                }
                loadTable = null;
            }
        }

        public bool CanStack(MoonSharpTableSerial other)
        {
            if (other.sTable.Length != sTable.Length) return false;

            foreach (var pair in sTable.Pairs)
            {
                var otherValue = other.sTable.Get(pair.Key);
                if (!pair.Value.Equals(otherValue)) return false;
            }
            return true;
        }

        public void CopyTo(MoonSharpTableSerial destination)
        {
            destination.table.Set("__s", DynValue.NewTable(sTable));
        }

        public MoonSharpTableSerial DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf, Table cloneTable)
        {
            var clone = new MoonSharpTableSerial();
            clone.SetTable(cloneTable);
            clone.ReplaceCloned(self, clonedSelf);
            return clone;
        }

        public void ReplaceCloned(RogueObj obj, RogueObj clonedObj)
        {
            foreach (var pair in sTable.Pairs)
            {
                if (pair.Value.UserData.Object is AnonWrapper<RogueObj> wrapper &&
                    wrapper.Value == obj)
                {
                    wrapper.Value = clonedObj;
                }
            }
        }
    }
}
