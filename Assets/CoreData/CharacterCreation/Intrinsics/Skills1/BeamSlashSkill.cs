using OchalikeSprites;
using Roguegard.Extensions;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class BeamSlashSkill : MpSkillIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(parent, lv);
        }

        [Objforming.Formable]
        private class SortedIntrinsic : MpSkillSortedIntrinsic<SortedIntrinsic>
        {
            public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
            public override IRogueMethodRange Range => FacingAnd2FlankingRogueMethodRange.Instance;
            public override int RequiredMp => 2;

            private static ISpriteMotion effect;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                using var predicator = ForEnemyRogueMethodTarget.Instance.GetPredicator(self, 0f, null);
                RaycastAssert.RequireTarget(predicator, FacingAnd2FlankingRogueMethodRange.Instance, self, arg, out var position);
                if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
                {
                    using var handler = h;
                    effect ??= new VariantSpriteMotion(CoreMotions.PowerSlash, Color.yellow);

                    handler.AppendText(":ActivateSkillMsg::2").AppendText(self).AppendText(this).AppendText("\n");
                    handler.EnqueueSE(MainInfoKw.Skill);
                    var item = RogueCharacterWork.CreateSpriteMotion(self, CoreMotions.Discus, false);
                    RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
                    RogueDevice.Add(DeviceKw.EnqueueSE, StdKw.PowerSlash);
                    var work = RogueCharacterWork.CreateEffect(self.Position + self.Main.Stats.Direction.Forward, self.Main.Stats.Direction, effect, false);
                    RogueDevice.AddWork(DeviceKw.EnqueueWork, work);
                }

                foreach (var target in predicator.GetObjs(position))
                {
                    // 攻撃力(x2)+2ダメージの攻撃。
                    using var damageValue = EffectableValue.Get();
                    StatsEffectedValues.GetAtk(self, damageValue);
                    damageValue.MainValue += damageValue.BaseMainValue + 2;
                    damageValue.SubValues[MainInfoKw.Skill] = 1f;
                    this.TryHurt(target, self, activationDepth, damageValue);
                    this.TryDefeat(target, self, activationDepth, damageValue);
                }
                return true;
            }

            public override int GetAtk(RogueObj self, out bool additionalEffect)
            {
                // 攻撃力(x2)+2ダメージの攻撃。
                using var damageValue = EffectableValue.Get();
                StatsEffectedValues.GetAtk(self, damageValue);
                damageValue.MainValue += damageValue.BaseMainValue + 2;

                var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
                additionalEffect = false;
                return hpDamage;
            }
        }
    }
}
