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
    public class EvtFairyReference : RgpackReference<IEvtAsset>, IMainInfoSet
    {
        [System.NonSerialized] private string _evtID;
        public string EvtID => _evtID ??= FullID.Substring(FullID.LastIndexOf('.') + 1);

        [System.NonSerialized] private readonly EvtFairyAsset.Point point;

        private const int initialLv = 0;

        public string Name => "インスタンス";
        [field: System.NonSerialized] public Sprite Icon { get; }
        public Color Color => Color.white;
        public string Caption => null;
        public IRogueDetails Details => null;

        public IKeyword Category => point.Category == EvtFairyInfo.Category.ApplyTool ? CategoryKw.ApplyTool : CategoryKw.Trap;

        public int MaxHP => 0;
        public int MaxMP => 0;
        public int ATK => 0;
        public int DEF => 0;
        public float Weight => 0f;
        public float LoadCapacity => 0f;

        public ISerializableKeyword Faction => RoguegardSettings.DefaultRaceOption.Faction;
        public Spanning<ISerializableKeyword> TargetFactions => RoguegardSettings.DefaultRaceOption.TargetFactions;
        public MainInfoSetAbility Ability => point.Category == EvtFairyInfo.Category.ApplyTool ? MainInfoSetAbility.WallObject : MainInfoSetAbility.TrapTile;
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

        [System.NonSerialized] private BeAppliedRogueMethod _beApplied;
        public IApplyRogueMethod BeApplied => _beApplied ??= new BeAppliedRogueMethod() { cmn = point.Cmn };
        public IApplyRogueMethod BeThrown => RoguegardSettings.DefaultRaceOption.BeThrown;
        public IApplyRogueMethod BeEaten => RoguegardSettings.DefaultRaceOption.BeEaten;

        public EvtFairyReference(string id, string envRgpackID, EvtFairyAsset.Point point)
            : base(id, envRgpackID)
        {
            this.point = point;
        }

        public IMainInfoSet Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base)
        {
            return Asset.GetInfoSet();
        }

        public void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph)
        {
        }

        public IMainInfoSet Reopen(RogueObj self, MainInfoSetType infoSetType, int deltaLv)
        {
            return Asset.GetInfoSet();
        }

        public void GetObjSprite(RogueObj self, out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            objSprite = point.Sprite.GetObjSprite();
            motionSet = point.Sprite.GetMotionSet();
        }

        public IEquipmentState GetEquipmentState(RogueObj self)
        {
            return null;
        }

        public IEquipmentInfo GetEquipmentInfo(RogueObj self)
        {
            return null;
        }

        public bool Equals(IMainInfoSet other)
        {
            return other is EvtFairyReference info && info.FullID == FullID;
        }

        public override bool Equals(object obj)
        {
            return obj is EvtFairyReference info && info.FullID == FullID;
        }

        public override int GetHashCode()
        {
            return 0;
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
    }
}
