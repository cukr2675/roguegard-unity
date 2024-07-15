using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class WeightCalculator : IRogueCalculator
    {
        private readonly EffectableValue value = EffectableValue.Get();

        public float TotalWeight { get; private set; }
        public float Weight => value.MainValue;
        public float SpaceWeight { get; private set; }

        float IRogueCalculator.MainBaseValue => value.BaseMainValue;
        float IRogueCalculator.MainValue => value.MainValue;

        internal WeightCalculator() { }

        public static WeightCalculator Get(RogueObj obj)
        {
            return obj.Main.Calculators.GetWeight(obj);
        }

        /// <summary>
        /// 重量を再計算したら、親オブジェクトの重量も再計算させる。
        /// </summary>
        public static void SetDirty(RogueObj obj)
        {
            if (obj == null) return;

            obj.Main.Calculators.SetDirtyOfWeight();
            if (obj.Location != null) { SetDirty(obj.Location); }
        }

        void IRogueCalculator.Update(RogueObj self)
        {
            var selfWeight = self.Main.InfoSet.Weight;

            var spaceWeight = 0f;
            var spaceObjs = self.Space.Objs;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var spaceObj = spaceObjs[i];
                if (spaceObj == null) continue;

                var spaceObjWeight = spaceObj.Main.Calculators.GetWeight(spaceObj);
                spaceWeight += spaceObjWeight.TotalWeight;
            }

            value.Initialize(selfWeight);
            value.SubValues[StatsKw.SpaceWeight] = spaceWeight;
            ValueEffectState.AffectValue(StatsKw.Weight, value, self);

            var totalWeight = value.MainValue + value.SubValues[StatsKw.SpaceWeight];
            totalWeight = Mathf.Max(totalWeight, 0f);
            totalWeight *= self.Stack;
            TotalWeight = totalWeight;
            SpaceWeight = value.SubValues[StatsKw.SpaceWeight];
        }

        float IRogueCalculator.SubValues(IKeyword key) => value.SubValues[key];
    }
}
