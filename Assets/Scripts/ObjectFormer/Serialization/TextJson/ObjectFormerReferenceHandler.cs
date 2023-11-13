using System;
using System.Collections;
using System.Collections.Generic;

using System.Text.Json.Serialization;

namespace ObjectFormer.Serialization.TextJson
{
    /// <summary>
    /// <see cref="object.Equals(object)"/> を使用して分類する <see cref="ReferenceHandler"/>
    /// </summary>
    internal class ObjectFormerReferenceHandler : ReferenceHandler
    {
        private readonly Resolver resolver;

        public ObjectFormerReferenceHandler(bool enabledReferenceMerge)
        {
            resolver = new Resolver(enabledReferenceMerge);
        }

        public override ReferenceResolver CreateResolver() => resolver;

        private class Resolver : ReferenceResolver
        {
            private readonly Dictionary<object, string> instance2IDTable;
            private readonly Dictionary<string, object> id2InstanceTable = new Dictionary<string, object>();
            private int referenceIndex = 0;

            public Resolver(bool enabledReferenceMerge)
            {
                if (enabledReferenceMerge)
                {
                    instance2IDTable = new Dictionary<object, string>();
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            public override string GetReference(object value, out bool alreadyExists)
            {
                if (instance2IDTable.TryGetValue(value, out string reference))
                {
                    alreadyExists = true;
                    return reference;
                }
                else
                {
                    referenceIndex++;
                    reference = referenceIndex.ToString();
                    instance2IDTable.Add(value, reference);

                    alreadyExists = false;
                    return reference;
                }
            }

            public override void AddReference(string reference, object value)
            {
                id2InstanceTable.Add(reference, value);
            }

            public override object ResolveReference(string reference)
            {
                if (id2InstanceTable.TryGetValue(reference, out object value))
                {
                    return value;
                }
                else
                {
                    throw new Exception($"$ref: {reference} が見つかりません。");
                }
            }
        }
    }
}
