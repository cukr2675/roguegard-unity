using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class FlareCannonSkill : MpSkillIntrinsicOptionScript
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
            public override IRogueMethodRange Range => LineOfSight10RogueMethodRange.Instance;
            public override int RequiredMp => 5;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                var direction = RogueMethodUtility.GetTargetDirection(self, arg);
                SpaceUtility.Raycast(self.Location, self.Position, direction, 10, true, true, true, out _, out _, out var position);
                if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
                {
                    using var handler = h;
                    handler.AppendText(":ActivateSkillMsg::2").AppendText(self).AppendText(this).AppendText("\n");
                    MainCharacterWorkUtility.TryAddSkill(self);
                    MainCharacterWorkUtility.TryAddShot(self);
                    handler.EnqueueWork(RogueCharacterWork.CreateEffect(position, CoreMotions.Bomb, false));
                }

                //// 攻撃力(x0)+2ダメージの攻撃。
                //using var damage = EffectableValue.Get();
                //StatsEffectedValues.GetAtk(self, damage);
                //damage.MainValue += -damage.BaseMainValue + 2;

                //var value = EffectableValue.Get();
                //value.Initialize(DungeonInfo.GetLocationVisibleRadius(self));
                //ValueEffectState.AffectValue(StdKw.View, value, self);
                //var visibleRadius = value.MainValue;
                //using var predicator = Target.GetPredicator(user, 0f, null);
                //if (!user.Location.Space.TryGetRoomView(user.Position, out var room, out _))
                //{
                //    room = new RectInt(0, 0, 0, 0);
                //}
                //Range.Predicate(predicator, user, 0f, null, visibleRadius, room);
                //predicator.EndPredicate();
                //var objs = predicator.GetObjs(user.Position);
                //for (int i = 0; i < objs.Count; i++)
                //{
                //    this.TryHurt(objs[i], self, activationDepth, damage);
                //}
                return true;
            }

            public override int GetAtk(RogueObj self, out bool additionalEffect)
            {
                // 攻撃力(x0)+2ダメージの攻撃。
                using var damageValue = EffectableValue.Get();
                StatsEffectedValues.GetAtk(self, damageValue);
                damageValue.MainValue += -damageValue.BaseMainValue + 2;

                var hpDamage = Mathf.FloorToInt(damageValue.MainValue);
                additionalEffect = false;
                return hpDamage;
            }
        }
    }
}
