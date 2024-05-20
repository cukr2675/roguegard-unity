using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    [Objforming.Formable]
    public class EventInstanceInfoSet : MainInfoSet
    {
        private readonly EventFairyInfo info;
        private readonly ICharacterCreationData data;

        private const int equipmentInitialLv = 0;

        public override string Name => data.Name;
        [field: System.NonSerialized] public override Sprite Icon { get; }
        public override Color Color => data.Color;
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

        public EventInstanceInfoSet(RgpackReference reference)
        {
            var eventFairyInfo = reference.GetData<EventFairyInfo>();
            info = eventFairyInfo;
            data = eventFairyInfo.Appearance.GetData<KyarakuriFigurineInfo>().Main;
        }

        public EventInstanceInfoSet(EventFairyInfo info, ICharacterCreationData data)
        {
            this.info = info;
            this.data = data;
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

        public override void GetObjSprite(RogueObj self, out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            RoguegardSettings.DefaultRaceOption.GetObjSprite(null, null, null, self, null, out objSprite, out motionSet);
        }

        public override IEquipmentState GetEquipmentState(RogueObj self)
        {
            return null;
        }

        public override IEquipmentInfo GetEquipmentInfo(RogueObj self)
        {
            return null;
        }

        public override bool Equals(MainInfoSet other)
        {
            return other is EventInstanceInfoSet info && info.info == this.info;
        }

        public RogueObj CreateObj(
            RogueObj location, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var obj = new RogueObj();
            obj.Main = new MainRogueObjInfo();
            obj.Main.SetBaseInfoSet(obj, this);
            RoguegardCharacterCreationSettings.LevelInfoInitializer.InitializeLv(obj, equipmentInitialLv);
            var stats = obj.Main.Stats;
            stats.Direction = RogueDirection.LowerLeft;
            stats.Reset(obj);
            if (!SpaceUtility.TryLocate(obj, location, info.Position, stackOption)) throw new RogueException("生成したオブジェクトの移動に失敗しました。");

            return obj;
        }
    }
}
