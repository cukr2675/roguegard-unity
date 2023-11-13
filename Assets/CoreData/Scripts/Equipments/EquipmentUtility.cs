using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class EquipmentUtility
    {
        private static readonly List<IKeyword> equipParts = new List<IKeyword>();

        /// <summary>
        /// 0 からチェックして空いている装備インデックスを取得する。
        /// どの装備インデックスも空いていなかったら最後のインデックスを取得する。
        /// </summary>
        public static int GetEquipIndex(IEquipmentState equipmentState, IKeyword keyword)
        {
            var length = equipmentState.GetLength(keyword);
            for (int i = 0; i < length; i++)
            {
                // 0 からチェックして空いている装備インデックスがあったらそれにする。
                var equipment = equipmentState.GetEquipment(keyword, i);
                if (equipment == null) return i;
            }

            // どの装備インデックスも空いていなかったら最後のインデックスにする。
            return length - 1;
        }

        public static RogueObj GetWeapon(RogueObj owner, out IWeaponEquipmentInfo weaponInfo)
        {
            weaponInfo = default;
            var ownerEquipmentState = owner.Main.GetEquipmentState(owner);
            if (ownerEquipmentState == null) return null;
            if (ownerEquipmentState.GetLength(EquipKw.Weapon) <= 0) return null;

            var weapon = ownerEquipmentState.GetEquipment(EquipKw.Weapon, 0);
            if (weapon == null) return null;

            weaponInfo = GetWeaponInfo(weapon);
            return weapon;
        }

        public static IWeaponEquipmentInfo GetWeaponInfo(RogueObj weapon)
        {
            var info = weapon.Main.GetEquipmentInfo(weapon);
            if (info is IWeaponEquipmentInfo weaponInfo)
            {
                return weaponInfo;
            }
            else
            {
                return null;
            }
        }

        public static RogueObj GetAmmo(RogueObj owner, out IAmmoEquipmentInfo ammoInfo)
        {
            ammoInfo = default;
            var ownerEquipmentState = owner.Main.GetEquipmentState(owner);
            if (ownerEquipmentState == null) return null;
            if (ownerEquipmentState.GetLength(EquipKw.Ammo) <= 0) return null;

            var ammo = ownerEquipmentState.GetEquipment(EquipKw.Ammo, 0);
            if (ammo == null) return null;

            ammoInfo = GetAmmoInfo(ammo);
            return ammo;
        }

        public static IAmmoEquipmentInfo GetAmmoInfo(RogueObj ammo)
        {
            var info = ammo.Main.GetEquipmentInfo(ammo);
            if (info is IAmmoEquipmentInfo ammoInfo)
            {
                return ammoInfo;
            }
            else
            {
                return null;
            }
        }

        public static bool MatchAmmo(RogueObj ammo, Spanning<IKeyword> ammoCategories)
        {
            var ammoInfo = GetAmmoInfo(ammo);
            if (ammoInfo == null) return false;

            return MatchAmmo(ammoInfo, ammoCategories);
        }

        public static bool MatchAmmo(IAmmoEquipmentInfo ammoInfo, Spanning<IKeyword> ammoCategories)
        {
            var ammoCategory = ammoInfo.AmmoCategory;
            for (int i = 0; i < ammoCategories.Count; i++)
            {
                if (ammoCategories[i] == ammoCategory) return true;
            }
            return false;
        }

        /// <summary>
        /// <paramref name="obj"/> が装備している装備品からランダムに一つ取得する。（コストがゼロのものは含めない）
        /// </summary>
        public static RogueObj GetRandomEquipment(RogueObj obj, IRogueRandom random)
        {
            var equipmentState = obj.Main.GetEquipmentState(obj);
            equipParts.Clear();
            for (int i = 0; i < equipmentState.Parts.Count; i++)
            {
                equipParts.Add(equipmentState.Parts[i]);
            }

            for (int i = equipmentState.Parts.Count - 1; i >= 0; i--)
            {
                var equipPartIndex = random.Next(0, equipParts.Count);
                var equipPart = equipParts[equipPartIndex];
                equipParts.RemoveAt(equipPartIndex);

                var length = equipmentState.GetLength(equipPart);
                for (int j = 0; j < length; j++)
                {
                    var equipment = equipmentState.GetEquipment(equipPart, j);
                    if (equipment == null) continue;

                    if (equipment.Main.InfoSet.Cost <= 0f) continue; // コストがゼロの装備品は含めない。

                    return equipment;
                }
            }
            return null;
        }

        /// <summary>
        /// <paramref name="obj"/> が装備している装備品からランダムに一つ取得する。（コストがゼロのものと武器・弾は含めない）
        /// </summary>
        public static RogueObj GetRandomArmor(RogueObj obj, IRogueRandom random)
        {
            var equipmentState = obj.Main.GetEquipmentState(obj);
            equipParts.Clear();
            for (int i = 0; i < equipmentState.Parts.Count; i++)
            {
                equipParts.Add(equipmentState.Parts[i]);
            }

            for (int i = equipmentState.Parts.Count - 1; i >= 0; i--)
            {
                var equipPartIndex = random.Next(0, equipParts.Count);
                var equipPart = equipParts[equipPartIndex];
                equipParts.RemoveAt(equipPartIndex);

                var length = equipmentState.GetLength(equipPart);
                for (int j = 0; j < length; j++)
                {
                    var equipment = equipmentState.GetEquipment(equipPart, j);
                    if (equipment == null) continue;

                    if (equipment.Main.InfoSet.Cost <= 0f) continue; // コストがゼロの装備品は含めない。

                    var equipmentInfo = equipment.Main.GetEquipmentInfo(equipment);
                    if (equipmentInfo is IWeaponEquipmentInfo || equipmentInfo is IAmmoEquipmentInfo) continue; // 武器と弾は含めない。

                    return equipment;
                }
            }
            return null;
        }
    }
}
