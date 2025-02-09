using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OchalikeSprites;

namespace Roguegard.CharacterCreation
{
    public class AppearanceMorph
    {
        private readonly BaseEffect baseEffect = new BaseEffect();
        private readonly List<EquipmentItem> equipmentItems = new List<EquipmentItem>();

        public OchalikeMorph BaseEffectOchalikeMorph => baseEffect.OchalikeMorph;

        public bool TryGetNewEquipmentTable(Spanning<IKeyword> equipParts, float order, out OchalikeMorph ochalikeMorph)
        {
            // 部分一致する要素があったら失敗させる
            foreach (var item in equipmentItems)
            {
                for (int i = 0; i < item.EquipParts.Count; i++)
                {
                    if (equipParts.Contains(item.EquipParts[i]))
                    {
                        ochalikeMorph = null;
                        return false;
                    }
                }
            }

            {
                // 完全一致する要素が見つからなければ新しく追加する
                var item = new EquipmentItem(equipParts, order);
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
            private readonly IKeyword[] _equipParts;
            public Spanning<IKeyword> EquipParts => _equipParts;

            public float Order { get; }

            public OchalikeMorph OchalikeMorph { get; }

            public EquipmentItem(Spanning<IKeyword> equipParts, float order)
            {
                _equipParts = equipParts.ToArray();
                Order = order;
                OchalikeMorph = new OchalikeMorph();
            }

            public void AffectSprite(RogueObj self, IReadOnlyOchalikeBone rootBone, OchalikeMorph ochalikeMorph)
            {
                // 同一部位または Innerwear に何か装備されていたらエフェクト無効化
                // （部位がゼロのエフェクトは無視して表示）
                if (_equipParts.Length >= 1)
                {
                    for (int i = 0; i < _equipParts.Length; i++)
                    {
                        if (Any(self, _equipParts[i])) return;
                    }
                    if (Any(self, RoguegardCharacterCreationSettings.EquipPartOfInnerwear)) return;
                }

                OchalikeMorph.AddTo(ochalikeMorph);
            }

            private static bool Any(RogueObj self, IKeyword equipPart)
            {
                var equipmentState = self.Main.GetEquipmentState(self);
                var length = equipmentState?.GetLength(equipPart) ?? 0;
                for (int i = 0; i < length; i++)
                {
                    var equipment = equipmentState.GetEquipment(equipPart, i);
                    if (equipment != null) return true;
                }
                return false;
            }
        }
    }
}
