using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;
using Roguegard.CharacterCreation;

namespace Roguegard
{
    /// <summary>
    /// <see cref="CharacterCreation"/> に依存しない実装にするため、 <see cref="ICharacterCreationData"/> を実装しない
    /// </summary>
    [Objforming.Formable]
    public class SewedEquipmentInfoSet : IMainInfoSet
    {
        private readonly SewedEquipmentData data;

        private const int equipmentInitialLv = 0;

        public string Name => data.Name;
        [field: System.NonSerialized] public Sprite Icon { get; }
        public Color Color => data.BoneSprites.MainColor;
        public string Caption => null;
        public IRogueDetails Details => null;

        public IKeyword Category => CategoryKw.Equipment;

        public int MaxHP => 0;
        public int MaxMP => 0;
        public int ATK => 0;
        public int DEF => 0;
        public float Weight => 0f;
        public float LoadCapacity => 0f;

        public ISerializableKeyword Faction => RoguegardSettings.DefaultRaceOption.Faction;
        public Spanning<ISerializableKeyword> TargetFactions => RoguegardSettings.DefaultRaceOption.TargetFactions;
        public MainInfoSetAbility Ability => MainInfoSetAbility.Object;
        public IRogueMaterial Material => RoguegardSettings.DefaultRaceOption.Material;
        public IRogueGender Gender => RoguegardSettings.DefaultRaceOption.Genders[0];
        public string HPName => null;
        public string MPName => null;
        public float Cost => 0f;
        public bool CostIsUnknown => false;

        public Spanning<IWeightedRogueObjGeneratorList> LootTable => Spanning<IWeightedRogueObjGeneratorList>.Empty;

        public IActiveRogueMethod Walk => RoguegardSettings.DefaultRaceOption.Walk;
        public IActiveRogueMethod Wait => RoguegardSettings.DefaultRaceOption.Wait;
        public ISkill Attack => RoguegardSettings.DefaultRaceOption.Attack;
        public ISkill Throw => RoguegardSettings.DefaultRaceOption.Throw;
        public IActiveRogueMethod PickUp => RoguegardSettings.DefaultRaceOption.PickUp;
        public IActiveRogueMethod Put => RoguegardSettings.DefaultRaceOption.Put;
        public IEatActiveRogueMethod Eat => RoguegardSettings.DefaultRaceOption.Eat;

        public IAffectRogueMethod Hit => RoguegardSettings.DefaultRaceOption.Hit;
        public IAffectRogueMethod BeDefeated => RoguegardSettings.DefaultRaceOption.BeDefeated;
        public IChangeStateRogueMethod Locate => RoguegardSettings.DefaultRaceOption.Locate;
        public IChangeStateRogueMethod Polymorph => RoguegardSettings.DefaultRaceOption.Polymorph;

        public IApplyRogueMethod BeApplied => RoguegardSettings.DefaultRaceOption.BeApplied;
        public IApplyRogueMethod BeThrown => RoguegardSettings.DefaultRaceOption.BeThrown;
        public IApplyRogueMethod BeEaten => RoguegardSettings.DefaultRaceOption.BeEaten;

        public SewedEquipmentInfoSet(SewedEquipmentData data)
        {
            this.data = new SewedEquipmentData(data);
            Icon = data.BoneSprites.GetIcon();
        }

        private SewedEquipmentInfoSet() { }

        public SewedEquipmentData GetDataClone()
        {
            return new SewedEquipmentData(data);
        }

        public IMainInfoSet Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base)
        {
            return this;
        }

        public void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph)
        {
        }

        public IMainInfoSet Reopen(RogueObj self, MainInfoSetType infoSetType, int deltaLv)
        {
            return this;
        }

        public void GetObjSprite(RogueObj self, out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            RoguegardSettings.DefaultRaceOption.GetObjSprite(null, null, null, self, null, out objSprite, out motionSet);
        }

        public IEquipmentState GetEquipmentState(RogueObj self)
        {
            return null;
        }

        public IEquipmentInfo GetEquipmentInfo(RogueObj self)
        {
            return new EquipmentInfo(data, self);
        }

        public bool Equals(IMainInfoSet other)
        {
            return other is SewedEquipmentInfoSet info && info.data == data;
        }

        public override bool Equals(object obj)
        {
            return obj is SewedEquipmentInfoSet info && info.data == data;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public RogueObj CreateObj(
            RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var obj = new RogueObj();
            obj.Main = new MainRogueObjInfo();
            obj.Main.SetBaseInfoSet(obj, this);
            RoguegardCharacterCreationSettings.LevelInfoInitializer.InitializeLv(obj, equipmentInitialLv);
            var stats = obj.Main.Stats;
            stats.Direction = RogueDirection.LowerLeft;
            stats.Reset(obj);
            if (!SpaceUtility.TryLocate(obj, location, position, stackOption)) throw new RogueException("生成したオブジェクトの移動に失敗しました。");

            return obj;
        }

        protected class EquipmentInfo : BaseEquipmentInfo, IBoneSpriteEffect
        {
            public override Spanning<IKeyword> EquipParts => data.EquipParts;
            public override bool CanStackWhileEquipped => false;
            float IBoneSpriteEffect.Order => data.BoneSpriteEffectOrder;

            private readonly SewedEquipmentData data;
            private readonly RogueObj self;
            private readonly EffectableBoneSpriteTable table;
            private bool colorIsInitialized;
            private Color color;

            public EquipmentInfo(SewedEquipmentData data, RogueObj self)
            {
                this.data = data;
                this.self = self;
                table = data.BoneSprites.GetEffectableTable();
                colorIsInitialized = false;
            }

            protected override void AddEffect(RogueObj equipment)
            {
                var owner = equipment.Location;
                if (table.Any)
                {
                    var equipmentSpriteState = owner.Main.GetBoneSpriteEffectState(owner);
                    equipmentSpriteState.AddFromRogueEffect(owner, this);
                }
            }

            protected override void RemoveEffect(RogueObj equipment)
            {
                var owner = equipment.Location;
                var equipmentSpriteState = owner.Main.GetBoneSpriteEffectState(owner);
                equipmentSpriteState.Remove(this);
            }

            void IBoneSpriteEffect.AffectSprite(RogueObj owner, IReadOnlyNodeBone rootNode, EffectableBoneSpriteTable boneSpriteTable)
            {
                var baseColor = data.BoneSprites.MainColor;
                if (!colorIsInitialized)
                {
                    color = RogueColorUtility.GetColor(self);
                    colorIsInitialized = true;
                }
                table.ColoredAddTo(boneSpriteTable, baseColor, color);
            }
        }
    }
}
