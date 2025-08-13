using MoonSharp.Interpreter;
using System.Diagnostics.CodeAnalysis;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    [SuppressMessage("Style", "IDE1006")]
    public class NumberCmnPropertyUserData
    {
        private float _value;

        public DynValue val
        {
            get => DynValue.NewNumber(_value);
            set => _value = (float?)value.CastToNumber() ?? 0f;
        }

        public NumberCmnPropertyUserData()
        {
        }

        public NumberCmnPropertyUserData(NumberCmnProperty cmnProperty)
        {
            _value = cmnProperty.Value;
        }
    }
}
