using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class MovementCalculator : IRogueCalculator
    {
        private readonly AffectableValue value = AffectableValue.Get();

        public bool AsTile { get; private set; }
        public bool HasCollider { get; private set; }
        public bool HasTileCollider { get; private set; }
        public bool HasSightCollider { get; private set; }

        float IRogueCalculator.MainBaseValue => value.BaseMainValue;
        float IRogueCalculator.MainValue => value.MainValue;

        internal MovementCalculator() { }

        public static MovementCalculator Get(RogueObj obj)
        {
            return obj.Main.Calculators.GetMovement(obj);
        }

        /// <summary>
        /// <see cref="StatsKw.Movement"/> に関わる <see cref="IValueEffect"/> を付与または解除するときに実行する。
        /// </summary>
        public static void SetDirty(RogueObj obj)
        {
            obj.Main.Calculators.SetDirtyOfMovement();
        }

        void IRogueCalculator.Update(RogueObj self)
        {
            var ability = self.Main.InfoSet.Ability;
            value.Initialize(0f);
            value.SubValues[StatsKw.AsTile] = ability.HasFlag(MainInfoSetAbility.AsTile) ? 1f : 0f;
            value.SubValues[StatsKw.HasCollider] = ability.HasFlag(MainInfoSetAbility.HasCollider) ? 1f : 0f;
            value.SubValues[StatsKw.HasTileCollider] = ability.HasFlag(MainInfoSetAbility.HasTileCollider) ? 1f : 0f;
            value.SubValues[StatsKw.HasSightCollider] = ability.HasFlag(MainInfoSetAbility.HasSightCollider) ? 1f : 0f;
            ValueEffectState.AffectValue(StatsKw.Movement, value, self);
            AsTile = value.SubValues.Is(StatsKw.AsTile);
            HasCollider = value.SubValues.Is(StatsKw.HasCollider);
            HasTileCollider = value.SubValues.Is(StatsKw.HasTileCollider);
            HasSightCollider = value.SubValues.Is(StatsKw.HasSightCollider);
        }

        public float SubValues(IKeyword key) => value.SubValues[key];
    }
}
