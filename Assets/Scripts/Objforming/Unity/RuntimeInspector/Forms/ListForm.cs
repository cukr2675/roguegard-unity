using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objforming.Unity.RuntimeInspector
{
    public static class ListForm
    {
        public static RelationalForm Create(System.Type type, LinkElement linkElementPrefab)
        {
            var typeDefinition = type.GetGenericTypeDefinition();
            if (typeDefinition != typeof(List<>)) throw new System.ArgumentException($"éwíËÇÃå^ {type} ÇÕ {typeof(List<>)} Ç≈ÇÕÇ†ÇËÇ‹ÇπÇÒÅB");

            var formType = typeof(ListForm<>).MakeGenericType(type.GenericTypeArguments);
            return (RelationalForm)System.Activator.CreateInstance(formType, new object[] { linkElementPrefab });
        }
    }

    public class ListForm<T> : LinkRelationalForm
    {
        public override System.Type InstanceType => typeof(List<T>);

        public override IReadOnlyList<System.Type> FieldTypes => empty;
        private static readonly IReadOnlyList<System.Type> empty = new[] { typeof(T) };

        public ListForm(LinkElement linkElementPrefab)
            : base(linkElementPrefab)
        {
        }

        public override void SetPageTo(FormInspector inspector, object value)
        {
            var list = (List<T>)value;
            for (int i = 0; i < list.Count; i++)
            {
                var buffer = i;
                var key = i.ToString();
                inspector.AppendElement($"{i}", typeof(T),
                    () =>
                    {
                        if (buffer < list.Count) return list[buffer];
                        else return null;
                    },
                    x =>
                    {
                        if (buffer < list.Count) { list[buffer] = (T)x; }
                    });
            }
        }
    }
}
