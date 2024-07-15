using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class MeltErosion : StackableStatusEffect, IEquipmentRogueEffect, IClosableStatusEffect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new MeltErosion());

        public override string Name => ":MeltErosion";
        public override IKeyword EffectCategory => EffectCategoryKw.Erosion;
        protected override int MaxStack => 1;

        [System.NonSerialized] private readonly EquipmentEffect equipmentEffect = new EquipmentEffect();

        private MeltErosion() { }

        protected override void NewAffectTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            if (RogueDevice.Primary.Player.Main.Stats.Party.Members.Contains(target.Location))
            {
                RogueDevice.Add(DeviceKw.AppendText, ":MeltMsg::1");
                RogueDevice.Add(DeviceKw.AppendText, target);
                RogueDevice.Add(DeviceKw.AppendText, "\n");
            }
            default(IChangeStateRogueMethodCaller).Polymorph(target, user, target.Main.InfoSet, activationDepth);
        }

        protected override void PreAffectedTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            NewAffectTo(target, user, activationDepth, arg, statusEffect);
        }

        protected override void RemoveClose(RogueObj self, StatusEffectCloseType closeType = StatusEffectCloseType.Manual)
        {
            ((IEquipmentRogueEffect)this).CloseEquip(self);
            base.RemoveClose(self, closeType);
        }

        void IEquipmentRogueEffect.OpenEquip(RogueObj equipment)
        {
            var owner = equipment.Location;
            RogueEffectUtility.AddFromRogueEffect(owner, equipmentEffect);
        }

        void IEquipmentRogueEffect.CloseEquip(RogueObj equipment)
        {
            var owner = equipment.Location;
            RogueEffectUtility.Remove(owner, equipmentEffect);
        }

        public override void GetEffectedName(RogueNameBuilder refName, RogueObj self)
        {
            refName.Insert0("溶けた");
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is MeltErosion erosion && erosion.Stack == Stack;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new MeltErosion() { Stack = Stack };
        }

        private class EquipmentEffect : IValueEffect
        {
            public float Order => 0f;

            public void AffectValue(IKeyword keyword, EffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.DEF)
                {
                    // ガード率 -10%
                    value.SubValues[StatsKw.GuardRate] -= .1f;
                }
            }
        }
    }
}
