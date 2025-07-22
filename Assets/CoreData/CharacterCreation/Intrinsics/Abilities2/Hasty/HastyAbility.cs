namespace Roguegard.CharacterCreation
{
    public class HastyAbility : PartyAbilityIntrinsicScript
    {
		public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOptionAsset parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(lv);
        }

        private class SortedIntrinsic : AbilitySortedIntrinsic, IValueEffect
        {
            float IValueEffect.Order => AttackUtility.CupValueEffectOrder;

            public SortedIntrinsic(int lv) : base(lv) { }

            void IValueEffect.AffectValue(IKeyword keyword, EffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.Def && value.SubValues[StatsKw.GuardRate] > 0f && value.SubValues[ElementKw.Thunder] > 0f)
                {
                    // 雷属性ダメージをガードしうるとき10%でガード
                    value.SubValues[StatsKw.GuardRate] = AttackUtility.Cup(value.SubValues[StatsKw.GuardRate], 0.1f);
                }
            }
        }
    }
}
