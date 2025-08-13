using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using System.Collections.Generic;

namespace Roguegard.Rgpacks.MoonSharp
{
    public class MoonSharpTableSerial
    {
        private Table table;
        private Table sTable;

        public Table Table => sTable;

        private Dictionary<string, DynValue> loadTable;

        public void Load(string key, DynValue value)
        {
            loadTable ??= new Dictionary<string, DynValue>();
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

        public bool CanStack(MoonSharpTableSerial coming)
        {
            if (coming.sTable.Length != sTable.Length) return false;

            foreach (var pair in sTable.Pairs)
            {
                var otherValue = coming.sTable.Get(pair.Key);
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
            clone.ReplaceObj(self, clonedSelf);
            return clone;
        }

        public void ReplaceObj(RogueObj obj, RogueObj clonedObj)
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
