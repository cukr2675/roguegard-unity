using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Roguegard.Extensions
{
    [SuppressMessage("Style", "IDE0060")]
    public static class EquipmentRogueMethodExtensions
    {
        public static bool TryEquip(
            this IApplyRogueMethodCaller method, RogueObj equipment, RogueObj user, float activationDepth, bool replaceEquipments = true)
        {
            var equipmentInfo = equipment.Main.GetEquipmentInfo(equipment);
            if (equipmentInfo == null) return false;

            var owner = equipment.Location;
            var ownerEquipmentState = owner.Main.GetEquipmentState(owner);
            if (ownerEquipmentState == null)
            {
                Debug.LogError("対象は装備できません。");
                return false;
            }

            var equipmentSlots = equipmentInfo.EquipmentSlots;

            if (replaceEquipments)
            {
                // 既に何か装備していたら外す
                foreach (var equipmentSlot in equipmentSlots)
                {
                    var equipmentSubslot = EquipmentUtility.GetEquipmentSubslot(ownerEquipmentState, equipmentSlot);
                    var preEquipment = ownerEquipmentState.GetEquipment(equipmentSlot, equipmentSubslot);
                    if (preEquipment != null) { method.TryUnequip(preEquipment, user, activationDepth, true); }
                }
            }

            // 装備する
            var count = equipmentSlots.Length >= 1 ? EquipmentUtility.GetEquipmentSubslot(ownerEquipmentState, equipmentSlots[0]) : 0;
            var equipArg = new RogueMethodArgument(count: count);
            var equipResult = RogueMethodAspectState.Invoke(
                MainInfoKw.Equip, equipmentInfo.BeEquipped, equipment, user, activationDepth, equipArg);
            return equipResult;
        }

        public static bool TryUnequip(
            this IChangeEffectRogueMethodCaller method, RogueObj equipment, RogueObj user, float activationDepth, bool hideMessage = false)
        {
            // 装備を外す
            var equipmentInfo = equipment.Main.GetEquipmentInfo(equipment);
            if (equipmentInfo == null) return false;

            var arg = new RogueMethodArgument(count: hideMessage ? 1 : 0);
            var unequipResult = RogueMethodAspectState.Invoke(
                MainInfoKw.Unequip, equipmentInfo.BeUnequipped, equipment, user, activationDepth, arg);
            return unequipResult;
        }
    }
}
