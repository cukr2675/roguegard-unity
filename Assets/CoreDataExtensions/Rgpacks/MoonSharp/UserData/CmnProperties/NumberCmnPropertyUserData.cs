using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    public class NumberCmnPropertyUserData
    {
        private float _value;

        public DynValue val
        {
            get => DynValue.NewNumber(_value);
            set => _value = (float?)value.CastToNumber() ?? 0f;
        }
    }
}
