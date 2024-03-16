using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    public class AppearanceBoneSpriteTable
    {
        private readonly BaseEffect baseEffect = new BaseEffect();
        private readonly List<EquipmentItem> equipmentItems = new List<EquipmentItem>();

        public AffectableBoneSpriteTable BaseTable => baseEffect.Table;

        public bool TryGetNewEquipmentTable(Spanning<IKeyword> equipParts, float order, out AffectableBoneSpriteTable table)
        {
            table = null;
            foreach (var item in equipmentItems)
            {
                // EquipParts �� Order �����S��v����v�f�̃e�[�u����Ԃ�
                if (!ListEquals(item.EquipParts, equipParts, out var partialMatch))
                {
                    if (partialMatch) return false; // ������v����v�f���������玸�s������
                    else continue;
                }
                if (item.Order != order) continue;

                table = item.Table;
                return true;
            }
            {
                // ���S��v����v�f��������Ȃ���ΐV�����ǉ�����
                var item = new EquipmentItem(equipParts, order);
                equipmentItems.Add(item);
                table = item.Table;
                return true;
            }
        }

        private static bool ListEquals(Spanning<IKeyword> a, Spanning<IKeyword> b, out bool partialMatch)
        {
            var exactMatch = a.Count == b.Count;
            partialMatch = false;
            var length = Mathf.Min(a.Count, b.Count);
            for (int i = 0; i < length; i++)
            {
                if (a[i] == b[i]) { partialMatch = true; }
                else { exactMatch = false; }
            }
            return exactMatch;
        }

        public void AddEffectFromInfoSet(RogueObj self)
        {
            if (BaseTable.Any)
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
            if (BaseTable.Any)
            {
                RogueEffectUtility.Remove(self, baseEffect);
            }
            foreach (var item in equipmentItems)
            {
                RogueEffectUtility.Remove(self, item);
            }
        }

        private class BaseEffect : IBoneSpriteEffect
        {
            public float Order => -200f;

            public AffectableBoneSpriteTable Table { get; } = new AffectableBoneSpriteTable();

            public void AffectSprite(RogueObj self, IBoneNode boneRoot, AffectableBoneSpriteTable boneSpriteTable)
            {
                Table.AddTo(boneSpriteTable);
            }
        }

        private class EquipmentItem : IBoneSpriteEffect
        {
            private readonly IKeyword[] _equipParts;
            public Spanning<IKeyword> EquipParts => _equipParts;

            public float Order { get; }

            public AffectableBoneSpriteTable Table { get; }

            public EquipmentItem(Spanning<IKeyword> equipParts, float order)
            {
                _equipParts = equipParts.ToArray();
                Order = order;
                Table = new AffectableBoneSpriteTable();
            }

            public void AffectSprite(RogueObj self, IBoneNode boneRoot, AffectableBoneSpriteTable boneSpriteTable)
            {
                // ���ꕔ�ʂ܂��� Innerwear �ɉ�����������Ă�����G�t�F�N�g������
                // �i���ʂ��[���̃G�t�F�N�g�͖������ĕ\���j
                if (_equipParts.Length >= 1)
                {
                    for (int i = 0; i < _equipParts.Length; i++)
                    {
                        if (Any(self, _equipParts[i])) return;
                    }
                    if (Any(self, RoguegardCharacterCreationSettings.EquipPartOfInnerwear)) return;
                }

                Table.AddTo(boneSpriteTable);
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
