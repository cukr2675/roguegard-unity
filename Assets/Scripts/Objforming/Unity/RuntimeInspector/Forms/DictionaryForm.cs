using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objforming.Unity.RuntimeInspector
{
    public static class DictionaryForm
    {
        public static RelationalForm Create(System.Type type, LinkElement linkElementPrefab)
        {
            var typeDefinition = type.GetGenericTypeDefinition();
            if (typeDefinition != typeof(Dictionary<,>)) throw new System.ArgumentException($"éwíËÇÃå^ {type} ÇÕ {typeof(Dictionary<,>)} Ç≈ÇÕÇ†ÇËÇ‹ÇπÇÒÅB");

            var formType = typeof(DictionaryForm<,>).MakeGenericType(type.GenericTypeArguments);
            return (RelationalForm)System.Activator.CreateInstance(formType, new object[] { linkElementPrefab });
        }
    }

    public class DictionaryForm<TKey, TValue> : LinkRelationalForm
    {
        public override System.Type InstanceType => typeof(Dictionary<TKey, TValue>);

        public override IReadOnlyList<System.Type> FieldTypes => empty;
        private static readonly IReadOnlyList<System.Type> empty = new[] { typeof(TKey), typeof(TValue) };

        public DictionaryForm(LinkElement linkElementPrefab)
            : base(linkElementPrefab)
        {
        }

        public override void SetPageTo(FormInspector inspector, object value)
        {
            var table = (Dictionary<TKey, TValue>)value;
            var index = 0;
            foreach (var pair in table)
            {
                var buffer = pair;
                inspector.AppendElement($"key{index}", typeof(TKey), () => buffer.Key, x => SetKeyTo(table, buffer, x));
                inspector.AppendElement($"value{index}", typeof(TKey), () => buffer.Value, x => table[buffer.Key] = (TValue)x);
            }
        }

        private static void SetKeyTo(Dictionary<TKey, TValue> table, KeyValuePair<TKey, TValue> pair, object newKey)
        {
            table[(TKey)newKey] = pair.Value;
            table.Remove(pair.Key);
        }
    }
}
