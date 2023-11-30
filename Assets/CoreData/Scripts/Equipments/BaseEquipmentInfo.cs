using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class BaseEquipmentInfo : IEquipmentInfo
    {
        /// <summary>
        /// 生成したインスタンスを使いまわす
        /// </summary>
        [System.NonSerialized] private EquipRogueEffect equipEffect;

        /// <summary>
        /// 重複した部位の指定禁止
        /// </summary>
        public abstract Spanning<IKeyword> EquipParts { get; }

        public int EquipIndex => equipEffect?.Index ?? -1;

        public virtual bool CanStackWhileEquipped => false;

        public virtual IApplyRogueMethod BeEquipped => _beEquipped;
        private static readonly IApplyRogueMethod _beEquipped = new BeEquippedRogueMethod();

        public virtual IChangeEffectRogueMethod BeUnequipped => _beUnequipped;
        private static readonly IChangeEffectRogueMethod _beUnequipped = new BeUnequippedRogueMethod();

        bool IEquipmentInfo.TryOpen(RogueObj equipment, int index, EquipRogueEffect equipEffect)
        {
            var owner = equipment.Location;
            var equipmentInfo = equipment.Main.GetEquipmentInfo(equipment);
            if (equipmentInfo != this)
            {
                Debug.LogError("変化した装備品を装備しようとしました。");
                return false;
            }

            var addEffect = false;
            if (equipEffect == null)
            {
                addEffect = true;
                if (this.equipEffect == null) { this.equipEffect = new EquipRogueEffect(equipment); }
                equipEffect = this.equipEffect;
            }
            if (this.equipEffect != null && this.equipEffect.Index >= 0)
            {
                // すでに装備されていたら失敗させる。
                return false;
            }
            equipEffect.SetIndex(index);
            this.equipEffect = equipEffect;

            var ownerEquipmentState = owner.Main.GetEquipmentState(owner);
            for (int i = 0; i < EquipParts.Count; i++)
            {
                var equipPart = EquipParts[i];
                int equipIndex;
                if (i == 0) { equipIndex = index; } // 最初の部位だけ外部からの指定を受ける。
                else { equipIndex = EquipmentUtility.GetEquipIndex(ownerEquipmentState, equipPart); }
                var equipment1 = ownerEquipmentState.GetEquipment(equipPart, equipIndex);
                if (equipment1 != null)
                {
                    // 既に枠が埋まっていたら失敗させる。
                    Debug.LogError("この装備枠はすでに設定されています。");
                    Unequip(equipment); // 失敗するまでに装備した部位を外す。
                    return false;
                }

                ownerEquipmentState.SetEquipment(equipPart, equipIndex, equipment);
            }

            if (addEffect) { owner.Main.RogueEffects.AddOpen(owner, equipEffect); }
            AddEffect(equipment);

            // 装備品にかかったステータスエフェクトの一部を付与する。
            var equipmentEffects = equipment.Main.RogueEffects.Effects;
            for (int i = 0; i < equipmentEffects.Count; i++)
            {
                var effect = equipmentEffects[i];
                if (effect is IEquipmentRogueEffect equipmentEffect)
                {
                    equipmentEffect.OpenEquip(equipment);
                }
            }

            return true;
        }

        protected virtual void AddEffect(RogueObj equipment)
        {
            var owner = equipment.Location;
            RogueEffectUtility.AddFromRogueEffect(owner, this);
        }

        void IEquipmentInfo.RemoveClose(RogueObj equipment)
        {
            Unequip(equipment);
        }

        private void Unequip(RogueObj equipment)
        {
            var owner = equipment.Location;
            var ownerEquipmentState = owner.Main.GetEquipmentState(owner);
            for (int i = 0; i < EquipParts.Count; i++)
            {
                var equipPart = EquipParts[i];
                var length = ownerEquipmentState.GetLength(equipPart);
                for (int j = 0; j < length; j++)
                {
                    var equipment1 = ownerEquipmentState.GetEquipment(equipPart, j);
                    if (equipment1 != equipment) continue;

                    ownerEquipmentState.RemoveEquipment(equipPart, j);
                }
            }

            RemoveEffect(equipment);

            // 装備品にかかったステータスエフェクトの一部を解除する。
            var equipmentEffects = equipment.Main.RogueEffects.Effects;
            for (int i = 0; i < equipmentEffects.Count; i++)
            {
                var effect = equipmentEffects[i];
                if (effect is IEquipmentRogueEffect equipmentEffect)
                {
                    equipmentEffect.CloseEquip(equipment);
                }
            }

            equipEffect.RemoveClose(owner);
        }

        protected virtual void RemoveEffect(RogueObj equipment)
        {
            var owner = equipment.Location;
            RogueEffectUtility.Remove(owner, this);
        }

        private class BeEquippedRogueMethod : BaseApplyRogueMethod
        {
            public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                var owner = self.Location;
                var info = self.Main.GetEquipmentInfo(self);
                if (!info.CanStackWhileEquipped && self.Stack >= 2)
                {
                    // 装備中スタック不可の装備品がスタックしていたら一つだけ装備する。
                    if (!SpaceUtility.TryDividedLocate(self, 1, out self)) return false;

                    info = self.Main.GetEquipmentInfo(self);
                }

                if (info.EquipIndex >= 0)
                {
                    // 既に誰かに装備されている場合は失敗させる。
                    Debug.LogError("この装備品は既に装備されています。");
                    return false;
                }

                var result = info.TryOpen(self, arg.Count);

                // 装備時の効果メッセージより先に表示させるため、装備開始前にメッセージを出す。
                if (result && RogueDevice.Primary.VisibleAt(owner.Location, owner.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, ":EquipMsg::2");
                    RogueDevice.Add(DeviceKw.AppendText, user);
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, "\n");
                    RogueDevice.Add(DeviceKw.EnqueueSE, MainInfoKw.Equip);
                }
                else if (!result && RogueDevice.Primary.Player == user)
                {
                    RogueDevice.Add(DeviceKw.AppendText, ":EquipMissMsg::1");
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, "\n");
                    RogueDevice.Add(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                }

                return result;
            }
        }

        private class BeUnequippedRogueMethod : IChangeEffectRogueMethod
        {
            public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                var owner = self.Location;
                var info = self.Main.GetEquipmentInfo(self);
                info.RemoveClose(self);
                SpaceUtility.Restack(self);

                if (arg.Count == 0 && RogueDevice.Primary.VisibleAt(owner.Location, owner.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, ":UnequipMsg::2");
                    RogueDevice.Add(DeviceKw.AppendText, user);
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, "\n");
                    RogueDevice.Add(DeviceKw.EnqueueSE, MainInfoKw.Equip);
                }

                return true;
            }
        }
    }
}
