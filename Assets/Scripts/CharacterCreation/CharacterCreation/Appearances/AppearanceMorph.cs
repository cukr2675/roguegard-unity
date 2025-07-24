using OchalikeSprites;
using System.Collections.Generic;

namespace Roguegard.CharacterCreation
{
    public class AppearanceMorph
    {
        private readonly BaseEffect baseEffect = new();
        private readonly List<EquipmentItem> equipmentItems = new();

        public OchalikeMorph BaseEffectOchalikeMorph => baseEffect.OchalikeMorph;

        public bool TryGetNewEquipmentTable(Spanning<IKeyword> equipmentSlots, float order, out OchalikeMorph ochalikeMorph)
        {
            // 部分一致する要素があったら失敗させる
            foreach (var item in equipmentItems)
            {
                foreach (var equipmentSlot in item.EquipmentSlots)
                {
                    if (equipmentSlots.Contains(equipmentSlot))
                    {
                        ochalikeMorph = null;
                        return false;
                    }
                }
            }

            {
                // 完全一致する要素が見つからなければ新しく追加する
                var item = new EquipmentItem(equipmentSlots, order);
                equipmentItems.Add(item);
                ochalikeMorph = item.OchalikeMorph;
                return true;
            }
        }

        public void AddEffectFromInfoSet(RogueObj self)
        {
            if (BaseEffectOchalikeMorph.Any)
            {
                RogueEffectUtility.AddFromInfoSet(self, baseEffect);
            }
            foreach (var item in equipmentItems)
            {
                RogueEffectUtility.AddFromInfoSet(self, item);
            }
        }

        public void Remove(RogueObj self)
        {
            if (BaseEffectOchalikeMorph.Any)
            {
                RogueEffectUtility.Remove(self, baseEffect);
            }
            foreach (var item in equipmentItems)
            {
                RogueEffectUtility.Remove(self, item);
            }
        }

        // 命名メモ: AddEquipment も使用するので BareEffect ではない
        private class BaseEffect : IBoneSpriteEffect
        {
            public float Order => -200f;

            public OchalikeMorph OchalikeMorph { get; } = new OchalikeMorph();

            public void AffectSprite(RogueObj self, IReadOnlyOchalikeBone rootBone, OchalikeMorph ochalikeMorph)
            {
                OchalikeMorph.AddTo(ochalikeMorph);
            }
        }

        private class EquipmentItem : IBoneSpriteEffect
        {
            private readonly IKeyword[] _equipmentSlots;
            public Spanning<IKeyword> EquipmentSlots => _equipmentSlots;

            public float Order { get; }

            public OchalikeMorph OchalikeMorph { get; }

            public EquipmentItem(Spanning<IKeyword> equipmentSlots, float order)
            {
                _equipmentSlots = equipmentSlots.ToArray();
                Order = order;
                OchalikeMorph = new OchalikeMorph();
            }

            public void AffectSprite(RogueObj self, IReadOnlyOchalikeBone rootBone, OchalikeMorph ochalikeMorph)
            {
                // 同一部位または Innerwear に何か装備されていたらエフェクト無効化
                // （部位がゼロのエフェクトは無視して表示）
                if (_equipmentSlots.Length >= 1)
                {
                    for (int i = 0; i < _equipmentSlots.Length; i++)
                    {
                        if (Any(self, _equipmentSlots[i])) return;
                    }
                    if (Any(self, RoguegardCharacterCreationSettings.EquipmentSlotOfInnerwear)) return;
                }

                OchalikeMorph.AddTo(ochalikeMorph);
            }

            private static bool Any(RogueObj self, IKeyword equipmentSlot)
            {
                var equipmentState = self.Main.GetEquipmentState(self);
                var length = equipmentState?.GetLength(equipmentSlot) ?? 0;
                for (int i = 0; i < length; i++)
                {
                    var equipment = equipmentState.GetEquipment(equipmentSlot, i);
                    if (equipment != null) return true;
                }
                return false;
            }
        }
    }
}
