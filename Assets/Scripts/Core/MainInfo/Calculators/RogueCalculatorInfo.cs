using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RogueCalculatorInfo
    {
        private readonly Item<SpeedCalculator> speedItem = new Item<SpeedCalculator>(new SpeedCalculator());
        private readonly Item<MovementCalculator> movementItem = new Item<MovementCalculator>(new MovementCalculator());
        private readonly Item<WeightCalculator> weightItem = new Item<WeightCalculator>(new WeightCalculator());
        private readonly Dictionary<IRogueCalculatorSource, Item> items = new Dictionary<IRogueCalculatorSource, Item>();

        internal SpeedCalculator GetSpeed(RogueObj self) => speedItem.Get(self);
        internal void SetDirtyOfSpeed() => speedItem.SetDirty();

        internal MovementCalculator GetMovement(RogueObj self) => movementItem.Get(self);
        internal void SetDirtyOfMovement() => movementItem.SetDirty();

        internal WeightCalculator GetWeight(RogueObj self) => weightItem.Get(self);
        internal void SetDirtyOfWeight() => weightItem.SetDirty();

        public IRogueCalculator Get(RogueObj self, IRogueCalculatorSource calculator)
        {
            if (!items.TryGetValue(calculator, out var item))
            {
                item = new Item(calculator);
                items.Add(calculator, item);
            }
            return item.Get(self);
        }

        public void SetDirty(IRogueCalculatorSource calculator)
        {
            if (items.TryGetValue(calculator, out var item))
            {
                item.SetDirty();
            }
        }

        private class Item<T>
            where T : IRogueCalculator
        {
            private readonly T calculator;
            private bool isDirty;

            public Item(T calculator)
            {
                this.calculator = calculator;
                isDirty = true;
            }

            public T Get(RogueObj self)
            {
                if (isDirty) { calculator.Update(self); }
                isDirty = false;
                return calculator;
            }

            public void SetDirty()
            {
                isDirty = true;
            }
        }

        private class Item : Item<IRogueCalculator>
        {
            public Item(IRogueCalculatorSource source)
                : base(source.Create())
            {
            }
        }
    }
}
