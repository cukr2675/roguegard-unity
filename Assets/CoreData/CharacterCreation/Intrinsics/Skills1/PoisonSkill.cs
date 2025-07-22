using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class PoisonSkill : MpSkillIntrinsicScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOptionAsset parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(parent, lv);
        }

        [Objforming.Formable]
        private class SortedIntrinsic : MpSkillSortedIntrinsic<SortedIntrinsic>
        {
            public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
            public override IRogueMethodRange Range => FrontRogueMethodRange.Instance;
            public override int RequiredMp => 2;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOptionAsset parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (RaycastAssert.RequireTarget(FrontRogueMethodRange.Instance, self, arg, out var target)) return false;
                if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
                {
                    using var handler = h;
                    MainCharacterWorkUtility.TryAddAttack(self);
                    handler.AppendText(self).AppendText("は").AppendText(this).AppendText("を放った！\n");
                }

                // 毒付与
                using var value = EffectableValue.Get();
                value.Initialize(0f);
                value.SubValues[MainInfoKw.Skill] = 1f;
                this.TryAffect(target, activationDepth, PoisonStatusEffect.Callback, null, self, value);
                return true;
            }

            public override int GetAtk(RogueObj self, out bool additionalEffect)
            {
                // 毒付与
                additionalEffect = true;
                return 0;
            }
        }
    }
}
