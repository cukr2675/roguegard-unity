using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    internal class ExampleCalculator : IRogueCalculator
    {
        private readonly AffectableValue value = AffectableValue.Get();

        private static readonly Source source = new Source();

        float IRogueCalculator.MainBaseValue => value.BaseMainValue;
        float IRogueCalculator.MainValue => value.MainValue;

        private ExampleCalculator() { }

        public static ExampleCalculator Get(RogueObj obj)
        {
            return (ExampleCalculator)obj.Main.Calculators.Get(obj, source);
        }

        public static void SetDirty(RogueObj obj)
        {
            obj.Main.Calculators.SetDirty(source);
        }

        void IRogueCalculator.Update(RogueObj self)
        {
            ValueEffectState.AffectValue(null, value, self);
        }

        float IRogueCalculator.SubValues(IKeyword key) => value.SubValues[key];

        private class Source : IRogueCalculatorSource
        {
            public IRogueCalculator Create() => new ExampleCalculator();
        }
    }
}
