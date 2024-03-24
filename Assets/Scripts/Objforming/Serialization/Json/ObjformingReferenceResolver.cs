using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Serialization;

namespace Objforming.Serialization.Json
{
    /// <summary>
    /// <see cref="object.Equals(object)"/> を使用して分類する <see cref="IReferenceResolver"/>
    /// </summary>
    internal class ObjformingReferenceResolver : IReferenceResolver
    {
        private readonly Dictionary<object, string> instance2IDTable = new Dictionary<object, string>();
        private readonly Dictionary<string, object> id2InstanceTable = new Dictionary<string, object>();
        private int referenceIndex = 0;
        //private object currentContext;

        public ObjformingReferenceResolver(bool enabledReferenceMerge)
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

        public bool IsReferenced(object context, object value)
        {
            //TryReset(context);
            return instance2IDTable.ContainsKey(value);
        }

        public string GetReference(object context, object value)
        {
            //TryReset(context);
            if (instance2IDTable.TryGetValue(value, out string reference))
            {
                return reference;
            }
            else
            {
                referenceIndex++;
                reference = referenceIndex.ToString();
                instance2IDTable.Add(value, reference);
                return reference;
            }
        }

        public void AddReference(object context, string reference, object value)
        {
            //TryReset(context);
            id2InstanceTable.Add(reference, value);
        }

        public object ResolveReference(object context, string reference)
        {
            //TryReset(context);
            if (id2InstanceTable.TryGetValue(reference, out object value))
            {
                return value;
            }
            else
            {
                throw new Exception($"$ref: {reference} が見つかりません。");
            }
        }

        //private void TryReset(object context)
        //{
        //    if (context != currentContext)
        //    {
        //        instance2IDTable.Clear();
        //        id2InstanceTable.Clear();
        //        currentContext = context;
        //    }
        //}
    }
}
