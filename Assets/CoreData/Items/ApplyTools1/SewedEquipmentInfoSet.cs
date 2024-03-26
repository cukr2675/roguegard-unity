using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;
using Roguegard.CharacterCreation;

namespace Roguegard
{
    [Objforming.Formable]
    public class SewedEquipmentInfoSet : MainInfoSet
    {
        private readonly SewedEquipmentData data;

        private const int equipmentInitialLv = 0;

        public override string Name => data.Name;
        [field: System.NonSerialized] public override Sprite Icon { get; }
        public override Color Color => data.BoneSprites.MainColor;
        public override string Caption => null;
        public override IRogueDetails Details => null;

        public override IKeyword Category => CategoryKw.Equipment;

        public override int MaxHP => 0;
        public override int MaxMP => 0;
        public override int ATK => 0;
        public override int DEF => 0;
        public override float Weight => 0f;
        public override float LoadCapacity => 0f;

        public override ISerializableKeyword Faction => RoguegardSettings.DefaultRaceOption.Faction;
        public override Spanning<ISerializableKeyword> TargetFactions => RoguegardSettings.DefaultRaceOption.TargetFactions;
        public override MainInfoSetAbility Ability => MainInfoSetAbility.Object;
        public override IRogueMaterial Material => RoguegardSettings.DefaultRaceOption.Material;
        public override IRogueGender Gender => RoguegardSettings.DefaultRaceOption.Genders[0];
        public override string HPName => null;
        public override string MPName => null;
        public override float Cost => 0f;
        public override bool CostIsUnknown => false;

        public override Spanning<IWeightedRogueObjGeneratorList> LootTable => Spanning<IWeightedRogueObjGeneratorList>.Empty;

        public override IActiveRogueMethod Walk => RoguegardSettings.DefaultRaceOption.Walk;
        public override IActiveRogueMethod Wait => RoguegardSettings.DefaultRaceOption.Wait;
        public override ISkill Attack => RoguegardSettings.DefaultRaceOption.Attack;
        public override ISkill Throw => RoguegardSettings.DefaultRaceOption.Throw;
        public override IActiveRogueMethod PickUp => RoguegardSettings.DefaultRaceOption.PickUp;
        public override IActiveRogueMethod Put => RoguegardSettings.DefaultRaceOption.Put;
        public override IEatActiveRogueMethod Eat => RoguegardSettings.DefaultRaceOption.Eat;

        public override IAffectRogueMethod Hit => RoguegardSettings.DefaultRaceOption.Hit;
        public override IAffectRogueMethod BeDefeated => RoguegardSettings.DefaultRaceOption.BeDefeated;
        public override IChangeStateRogueMethod Locate => RoguegardSettings.DefaultRaceOption.Locate;
        public override IChangeStateRogueMethod Polymorph => RoguegardSettings.DefaultRaceOption.Polymorph;

        public override IApplyRogueMethod BeApplied => RoguegardSettings.DefaultRaceOption.BeApplied;
        public override IApplyRogueMethod BeThrown => RoguegardSettings.DefaultRaceOption.BeThrown;
        public override IApplyRogueMethod BeEaten => RoguegardSettings.DefaultRaceOption.BeEaten;

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

        public override MainInfoSet Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base)
        {
            return this;
        }

        public override void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph)
        {
        }

        public override MainInfoSet Reopen(RogueObj self, MainInfoSetType infoSetType, int deltaLv)
        {
            return this;
        }

        public override void GetObjSprite(RogueObj self, out RogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            RoguegardSettings.DefaultRaceOption.GetObjSprite(null, null, null, self, null, out objSprite, out motionSet);
        }

        public override IEquipmentState GetEquipmentState(RogueObj self)
        {
            return null;
        }

        public override IEquipmentInfo GetEquipmentInfo(RogueObj self)
        {
            return new EquipmentInfo(data, self);
        }

        public override bool Equals(MainInfoSet other)
        {
            return other is SewedEquipmentInfoSet info && info.data == data;
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
            private readonly AffectableBoneSpriteTable table;
            private bool colorIsInitialized;
            private Color color;

            public EquipmentInfo(SewedEquipmentData data, RogueObj self)
            {
                this.data = data;
                this.self = self;
                table = data.BoneSprites.GetAffectableTable();
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

            void IBoneSpriteEffect.AffectSprite(RogueObj owner, IReadOnlyNodeBone rootNode, AffectableBoneSpriteTable boneSpriteTable)
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
