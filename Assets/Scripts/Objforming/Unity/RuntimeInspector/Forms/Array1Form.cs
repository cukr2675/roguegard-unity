using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objforming.Unity.RuntimeInspector
{
    public static class Array1Form
    {
        public static RelationalForm Create(System.Type type, LinkElement linkElementPrefab)
        {
            if (!type.IsArray) throw new System.ArgumentException($"éwíËÇÃå^ {type} ÇÕîzóÒå^Ç≈ÇÕÇ†ÇËÇ‹ÇπÇÒÅB");

            var elementType = type.GetElementType();
            var formType = typeof(Array1Form<>).MakeGenericType(elementType);
            return (RelationalForm)System.Activator.CreateInstance(formType, new object[] { linkElementPrefab });
        }
    }

    public class Array1Form<T> : LinkRelationalForm
    {
        public override System.Type InstanceType => typeof(T[]);

        public override IReadOnlyList<System.Type> FieldTypes => empty;
        private static readonly IReadOnlyList<System.Type> empty = new[] { typeof(T) };

        public Array1Form(LinkElement linkElementPrefab)
            : base(linkElementPrefab)
        {
        }

        public override void SetPageTo(FormInspector inspector, object value)
        {
            var array = (T[])value;
            for (int i = 0; i < array.Length; i++)
            {
                inspector.AppendElement($"{i}", typeof(T), () => array[i], x => array[i] = (T)x);
            }
        }
    }
}
