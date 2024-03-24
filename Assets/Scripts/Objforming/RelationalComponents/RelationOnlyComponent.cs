using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

namespace Objforming
{
    public class RelationOnlyComponent : IRelationalComponent
    {
        public Type InstanceType { get; }

        public IReadOnlyList<Type> FieldTypes { get; }

        public RelationOnlyComponent(Type instanceType, params Type[] fieldTypes)
        {
            InstanceType = instanceType;
            FieldTypes = fieldTypes.ToArray();
        }

        bool IRelationalComponent.Overrides(IRelationalComponent other)
        {
            return other.InstanceType == InstanceType;
        }
    }
}
