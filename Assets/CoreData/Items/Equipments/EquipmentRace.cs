using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class EquipmentRace : ObjectRace
    {
        [Header("EquipmentRace")]

        [SerializeField] private bool _isCosmetic;

        [SerializeField] private EquipKeywordData[] _equipParts;

        [SerializeField] private bool _isNotStackableWhileEquipped;

        [SerializeField] private AffectableBoneSpriteTableData.Value _boneSpriteTable;

        [Tooltip("この値が設定されているとき、装備者の指定のボーンの色をスポイトする")]
        [SerializeField] private KeywordData _eyeDropBoneName;

        [SerializeField] private bool _overridesBoneSpriteEffectOrder;

        [SerializeField] private float _boneSpriteEffectOrder;

        [SerializeField] private ScriptField<IApplyRogueMethod> _beEquipped;

        [SerializeField] private ScriptField<IChangeEffectRogueMethod> _beUnequipped;

        [SerializeField] private ScriptField<IEquippedEffectSource>[] _equippedEffectSources;

        public override IEquipmentInfo GetEquipmentInfo(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            var info = new EquipmentInfo<EquipmentRace>(this, self);
            return info;
        }

        public void Validate()
        {
            if (!_overridesBoneSpriteEffectOrder && _equipParts.Length >= 1 && _equipParts[0] != null)
            {
                _boneSpriteEffectOrder = _equipParts[0].Order;
            }
        }

        public void Affect(AffectableBoneSpriteTable boneSpriteTable, Color color)
        {
            var baseColor = Color;
            _boneSpriteTable.Table.ColoredAddTo(boneSpriteTable, baseColor, color);
        }

        protected class EquipmentInfo<T> : BaseEquipmentInfo, IBoneSpriteEffect
            where T : EquipmentRace
        {
            public override Spanning<IKeyword> EquipParts => Data._isCosmetic ? Spanning<IKeyword>.Empty : Data._equipParts;
            public override bool IsNotStackableWhileEquipped => Data._isNotStackableWhileEquipped;
            public override IApplyRogueMethod BeEquipped => Data._beEquipped.Ref ?? base.BeEquipped;
            public override IChangeEffectRogueMethod BeUnequipped => Data._beUnequipped.Ref ?? base.BeUnequipped;
            float IBoneSpriteEffect.Order => Data._boneSpriteEffectOrder;

            protected T Data { get; }
            private readonly RogueObj self;
            private readonly IEquippedEffect[] effects;
            private bool colorIsInitialized;
            private Color color;

            public EquipmentInfo(T data, RogueObj self)
            {
                this.Data = data;
                this.self = self;
                effects = data._equippedEffectSources.Select(x => x.Ref.CreateOrReuse(self, null)).ToArray();
                colorIsInitialized = false;
            }

            protected override void AddEffect(RogueObj equipment)
            {
                var owner = equipment.Location;
                if (Data._boneSpriteTable.Table.Any)
                {
                    var equipmentSpriteState = owner.Main.GetBoneSpriteEffectState(owner);
                    equipmentSpriteState.AddFromRogueEffect(owner, this);
                }

                foreach (var effect in effects)
                {
                    effect.AddEffect(equipment, owner);
                }
            }

            protected override void RemoveEffect(RogueObj equipment)
            {
                var owner = equipment.Location;
                var equipmentSpriteState = owner.Main.GetBoneSpriteEffectState(owner);
                equipmentSpriteState.Remove(this);

                foreach (var effect in effects)
                {
                    effect.RemoveEffect(equipment, owner);
                }
            }

            void IBoneSpriteEffect.AffectSprite(IBoneNode boneRoot, AffectableBoneSpriteTable boneSpriteTable)
            {
                var baseColor = Data.Color;
                if (Data._eyeDropBoneName != null)
                {
                    var color = RogueColorUtility.GetFirstColor(Data._eyeDropBoneName, boneRoot, boneSpriteTable);
                    Data._boneSpriteTable.Table.ColoredAddTo(boneSpriteTable, baseColor, color);
                    return;
                }

                if (!colorIsInitialized)
                {
                    color = RogueColorUtility.GetColor(self);
                    colorIsInitialized = true;
                }
                Data._boneSpriteTable.Table.ColoredAddTo(boneSpriteTable, baseColor, color);
            }
        }
    }
}
