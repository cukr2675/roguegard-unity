using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class PolymorphStatusEffect : TimeLimitedStackableStatusEffect
    {
        private static readonly PolymorphStatusEffect instance = new PolymorphStatusEffect();

        public override string Name => "変化";
        public override IKeyword EffectCategory => EffectCategoryKw.StatusAilment;
        protected override int MaxStack => 1;
        protected override int InitialLifeTime => 100;

        /// <summary>
        /// 変化前の装備状態を記憶するテーブル。
        /// </summary>
        private readonly Dictionary<RogueObj, int> equipments = new Dictionary<RogueObj, int>();

        private PolymorphStatusEffect() { }

        public static void AffectTo(RogueObj target, IMainInfoSet infoSet, int lifeTime)
        {
            var effect = (PolymorphStatusEffect)instance.AffectTo(target, null, 0f, new(other: infoSet));
            effect.LifeTime = lifeTime;
        }

        protected override IRogueEffect AffectTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (!(arg.Other is IMainInfoSet)) return null;

            return base.AffectTo(target, user, activationDepth, arg);
        }

        protected override void NewAffectTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            // 新しく変化したとき
            if (target.Main.InfoSetState == MainInfoSetType.Base)
            {
                // 変化前の装備を記憶する。
                var effect = (PolymorphStatusEffect)statusEffect;
                effect.equipments.Clear();
                var equipmentState = target.Main.GetEquipmentState(target);
                var parts = equipmentState.Parts;
                for (int i = 0; i < parts.Count; i++)
                {
                    var part = parts[i];
                    var length = equipmentState.GetLength(part);
                    for (int j = 0; j < length; j++)
                    {
                        var equipment = equipmentState.GetEquipment(part, j);
                        if (equipment == null) continue;

                        var equipmentInfo = equipment.Main.GetEquipmentInfo(equipment);
                        var index = equipmentInfo.EquipIndex;
                        effect.equipments[equipment] = index;
                    }
                }
            }

            var infoSet = (IMainInfoSet)arg.Other;
            target.Main.Polymorph(target, infoSet);
        }

        protected override void RemoveClose(RogueObj self, StatusEffectCloseType closeType = StatusEffectCloseType.Manual)
        {
            // 変化解除
            self.Main.Polymorph(self, self.Main.BaseInfoSet);

            // 空いている装備枠に変化前に装備していたものを使用する。
            var equipmentState = self.Main.GetEquipmentState(self);
            foreach (var pair in equipments)
            {
                var equipment = pair.Key;
                var equipmentInfo = equipment.Main.GetEquipmentInfo(equipment);
                var index = pair.Value;

                // 変化中になくなった装備品は装備しない。
                if (equipment.Location != self) continue;

                // 装備枠が埋まっていたら装備しない。
                if (equipmentState.GetEquipment(equipmentInfo.EquipParts[0], index) != null) continue;

                equipmentInfo.TryOpen(equipment, index);
            }

            base.RemoveClose(self);
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is PolymorphStatusEffect effect && effect.Stack == Stack && effect.LifeTime == LifeTime;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new PolymorphStatusEffect()
            {
                Stack = Stack,
                LifeTime = LifeTime
            };
        }
    }
}
