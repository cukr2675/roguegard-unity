using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class StaticInitializable<T>
    {
        private StaticID staticID;

        private readonly Initializer initializer;

        private T _value;

        public T Value
        {
            get
            {
                if (staticID.IsValid) return _value;

                Value = initializer();
                return _value;
            }
            set
            {
                _value = value;
                staticID = StaticID.Current;
            }
        }

        public delegate T Initializer();

        public StaticInitializable(Initializer initializer)
        {
            this.initializer = initializer;
            _value = default;
            staticID = default;
        }

        //public static implicit operator T(StaticInitializable<T> value)
        //{
        //    return value.Value;
        //}
    }
}
