using System;
using System.Collections;
using System.Collections.Generic;

namespace ObjectFormer
{
    public class RelationTable : IReadOnlyDictionary<Type, IEnumerable<Type>>
    {
        private readonly Dictionary<Type, Value> table = new Dictionary<Type, Value>();

        public IEnumerable<Type> this[Type key] => table[key];
        public IEnumerable<Type> Keys => table.Keys;
        public IEnumerable<IEnumerable<Type>> Values => table.Values;
        public int Count => table.Count;

        public RelationTable()
        {
        }

        public RelationTable(IReadOnlyDictionary<Type, IEnumerable<Type>> source)
        {
            foreach (var pair in source)
            {
                var value = new Value();
                value.UnionWith(pair.Value);
                table.Add(pair.Key, value);
            }
        }

        public static RelationTable GenerateRelations(IEnumerable<Type> types)
        {
            var relations = new RelationTable();
            foreach (var field in types)
            {
                var value = relations.GetOrAdd(field);
                foreach (var instance in types)
                {
                    if (instance == field) continue;
                    if (!field.IsAssignableFrom(instance)) continue;

                    // この時点で instance は field に代入できることが判明している。
                    // relations 全体での同じ型の出現回数を減らすため、間に入る型が存在した場合、 instance は追加しない。
                    if (GetSeparator(field, instance)) continue;

                    value.Add(instance);
                }
            }
            return relations;

            bool GetSeparator(Type field, Type instance)
            {
                foreach (var middle in types)
                {
                    if (middle == field || middle == instance) continue;
                    if (!field.IsAssignableFrom(middle)) continue;

                    // この時点で instance と middle は field に代入できることが判明している。
                    // このとき instance が middle に代入可能であれば、 field は middle を内包し、 middle は　instance を内包する関係になるため、間に入る型となる。

                    if (middle.IsAssignableFrom(instance)) return true;
                }
                return false;
            }
        }

        public Value GetOrAdd(Type type)
        {
            if (!table.TryGetValue(type, out var value))
            {
                value = new Value();
                table.Add(type, value);
            }
            return value;
        }

        public bool TryGetValue(Type key, out IEnumerable<Type> value)
        {
            if (table.TryGetValue(key, out var types))
            {
                value = types;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public bool ContainsKey(Type key) => table.ContainsKey(key);
        public IEnumerator<KeyValuePair<Type, IEnumerable<Type>>> GetEnumerator()
        {
            foreach (var pair in table)
            {
                yield return new KeyValuePair<Type, IEnumerable<Type>>(pair.Key, pair.Value);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public class Value : IEnumerable<Type>
        {
            private readonly HashSet<Type> set = new HashSet<Type>();

            public void Add(Type type)
            {
                set.Add(type);
            }

            public void UnionWith(IEnumerable<Type> types)
            {
                set.UnionWith(types);
            }

            public IEnumerator<Type> GetEnumerator() => set.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => set.GetEnumerator();
        }
    }
}
