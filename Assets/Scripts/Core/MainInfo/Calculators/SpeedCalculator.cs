using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class SpeedCalculator : IRogueCalculator
    {
        private readonly AffectableValue value = AffectableValue.Get();

        public int Speed { get; private set; }
        public bool BeInhibited { get; private set; }
        public bool Hungry { get; private set; }

        float IRogueCalculator.MainBaseValue => value.BaseMainValue;
        float IRogueCalculator.MainValue => value.MainValue;

        internal SpeedCalculator() { }

        public static SpeedCalculator Get(RogueObj obj)
        {
            return obj.Main.Calculators.GetSpeed(obj);
        }

        public static void SetDirty(RogueObj obj)
        {
            obj.Main.Calculators.SetDirtyOfSpeed();
        }

        void IRogueCalculator.Update(RogueObj self)
        {
            value.Initialize(0);
            value.SubValues[StatsKw.BeInhibited] = self.Main.InfoSet.Ability.HasFlag(MainInfoSetAbility.Movable) ? 0f : 1f;
            ValueEffectState.AffectValue(StatsKw.Speed, value, self);
            Speed = Mathf.FloorToInt(value.MainValue);
            BeInhibited = value.SubValues.Is(StatsKw.BeInhibited);
            Hungry = value.SubValues.Is(StatsKw.Hungry);
        }

        public float SubValues(IKeyword key) => value.SubValues[key];
    }
}
