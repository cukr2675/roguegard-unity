using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard.Rgpacks
{
    /// <summary>
    /// <see cref="CharacterCreation"/> に依存しない実装にするため、 <see cref="ICharacterCreationData"/> を実装しない
    /// </summary>
    [Objforming.Formable]
    public class EvtFairyReference : MainInfoSet
    {
        public string FullID => reference.FullID;
        public string RgpackID => reference.RgpackID;

        [System.NonSerialized] private string _evtID;
        public string EvtID => _evtID ??= reference.FullID.Substring(reference.FullID.LastIndexOf('.') + 1);

        private readonly Reference reference;
        [System.NonSerialized] private readonly EvtFairyAsset.Point point;

        private const int initialLv = 0;

        public override string Name => "インスタンス";
        [field: System.NonSerialized] public override Sprite Icon { get; }
        public override Color Color => Color.white;
        public override string Caption => null;
        public override IRogueDetails Details => null;

        public override IKeyword Category => point.Category == EvtFairyInfo.Category.ApplyTool ? CategoryKw.ApplyTool : CategoryKw.Trap;

        public override int MaxHP => 0;
        public override int MaxMP => 0;
        public override int ATK => 0;
        public override int DEF => 0;
        public override float Weight => 0f;
        public override float LoadCapacity => 0f;

        public override ISerializableKeyword Faction => RoguegardSettings.DefaultRaceOption.Faction;
        public override Spanning<ISerializableKeyword> TargetFactions => RoguegardSettings.DefaultRaceOption.TargetFactions;
        public override MainInfoSetAbility Ability => point.Category == EvtFairyInfo.Category.ApplyTool ? MainInfoSetAbility.WallObject : MainInfoSetAbility.TrapTile;
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

        [System.NonSerialized] private BeAppliedRogueMethod _beApplied;
        public override IApplyRogueMethod BeApplied => _beApplied ??= new BeAppliedRogueMethod() { cmn = point.Cmn };
        public override IApplyRogueMethod BeThrown => RoguegardSettings.DefaultRaceOption.BeThrown;
        public override IApplyRogueMethod BeEaten => RoguegardSettings.DefaultRaceOption.BeEaten;

        public EvtFairyReference(string id, string envRgpackID, EvtFairyAsset.Point point)
        {
            reference = new Reference(id, envRgpackID);
            this.point = point;
        }

        public override MainInfoSet Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base)
        {
            return reference.Asset.GetInfoSet();
        }

        public override void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph)
        {
        }

        public override MainInfoSet Reopen(RogueObj self, MainInfoSetType infoSetType, int deltaLv)
        {
            return reference.Asset.GetInfoSet();
        }

        public override void GetObjSprite(RogueObj self, out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            objSprite = point.Sprite.GetObjSprite();
            motionSet = point.Sprite.GetMotionSet();
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
            return other is EvtFairyReference info && info.reference.FullID == reference.FullID;
        }

        public RogueObj CreateObj(RogueObj location, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var obj = new RogueObj();
            obj.Main = new MainRogueObjInfo();
            obj.Main.SetBaseInfoSet(obj, this);
            RoguegardCharacterCreationSettings.LevelInfoInitializer.InitializeLv(obj, initialLv);
            var stats = obj.Main.Stats;
            stats.Direction = RogueDirection.LowerLeft;
            stats.Reset(obj);
            if (!SpaceUtility.TryLocate(obj, location, point.Position, stackOption)) throw new RogueException("生成したオブジェクトの移動に失敗しました。");

            return obj;
        }

        private class BeAppliedRogueMethod : BaseApplyRogueMethod
        {
            public PropertiedCmnReference cmn;

            public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                cmn.Invoke();
                return true;
            }
        }

        [Objforming.Formable]
        private class Reference : RgpackReference<IEvtAsset>
        {
            public new IEvtAsset Asset => base.Asset;

            public Reference(string id, string envRgpackID)
                : base(id, envRgpackID)
            {
            }
        }
    }
}
