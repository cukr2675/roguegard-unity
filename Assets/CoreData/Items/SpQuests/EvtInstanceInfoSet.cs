using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;
using Roguegard.CharacterCreation;

namespace Roguegard
{
    [Objforming.Formable]
    public class EvtInstanceInfoSet : MainInfoSet
    {
        private readonly EvtFairyInfo info;
        private readonly EvtFairyInfo.Point point;
        private readonly ICharacterCreationData data;

        public string ID => info.ID;

        private int currentRaceOptionIndex;
        [System.NonSerialized] private IRaceOption _currentRaceOption;
        public IRaceOption CurrentRaceOption
        {
            get
            {
                if (_currentRaceOption != null) return _currentRaceOption;

                if (data.Race.Option.GrowingOptions.Count == 0)
                {
                    _currentRaceOption = data.Race.Option;
                }
                else
                {
                    if (currentRaceOptionIndex >= data.Race.Option.GrowingOptions.Count)
                    {
                        Debug.LogWarning($"{nameof(currentRaceOptionIndex)} ({currentRaceOptionIndex}) が不正です。");
                        currentRaceOptionIndex = data.Race.Option.GrowingOptions.IndexOf(data.Race.Option);
                    }
                    _currentRaceOption = data.Race.Option.GrowingOptions[currentRaceOptionIndex];
                }
                return _currentRaceOption;
            }
        }

        private readonly IRogueGender _gender;

        [System.NonSerialized] private float _weight;
        [System.NonSerialized] private IReadOnlyNodeBone nodeBone;
        [System.NonSerialized] private AppearanceBoneSpriteTable characterBoneSpriteTable;

        private const int equipmentInitialLv = 0;

        public override string Name => data.Name;
        [field: System.NonSerialized] public override Sprite Icon { get; }
        public override Color Color => data.Color;
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
        public override IApplyRogueMethod BeApplied => _beApplied ??= new BeAppliedRogueMethod() { cmn = point.Cmn.GetData<IScriptingCmn>() };
        public override IApplyRogueMethod BeThrown => RoguegardSettings.DefaultRaceOption.BeThrown;
        public override IApplyRogueMethod BeEaten => RoguegardSettings.DefaultRaceOption.BeEaten;

        //public EvtInstanceInfoSet(RgpackReference reference)
        //{
        //    var eventFairyInfo = reference.GetData<EvtFairyInfo>();
        //    info = eventFairyInfo;
        //    data = eventFairyInfo.Appearance.GetData<KyarakuriFigurineInfo>().Main;
        //}

        public EvtInstanceInfoSet(EvtFairyInfo info, EvtFairyInfo.Point point, ICharacterCreationData data = null)
        {
            this.info = info;
            this.point = point;
            this.data = data ?? point.Appearance.GetData<KyarakuriFigurineInfo>().Main;
        }

        /// <summary>
        /// <see cref="ICharacterCreationData"/> を再読み込みし、見た目などを再構成する。
        /// </summary>
        private void Reload()
        {
            _currentRaceOption = null;
            _weight = CurrentRaceOption.GetWeight(CurrentRaceOption, data);
            CurrentRaceOption.GetSpriteValues(CurrentRaceOption, data, _gender, out var mainNode, out characterBoneSpriteTable);

            if (mainNode != null)
            {
                var appearances = data.Appearances;
                for (int i = 0; i < appearances.Count; i++)
                {
                    var appearance = appearances[i];
                    appearance.Option.Affect(mainNode, characterBoneSpriteTable, appearance, data);
                }
                nodeBone = mainNode;
            }
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
            if (nodeBone == null) { Reload(); }

            CurrentRaceOption.GetObjSprite(CurrentRaceOption, data, _gender, self, nodeBone, out objSprite, out motionSet);
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
            return other is EvtInstanceInfoSet info && info.info == this.info;
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
            if (!SpaceUtility.TryLocate(obj, location, point.Position, stackOption)) throw new RogueException("生成したオブジェクトの移動に失敗しました。");

            return obj;
        }

        private class BeAppliedRogueMethod : BaseApplyRogueMethod
        {
            public IScriptingCmn cmn;

            public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                cmn.Invoke();
                return true;
            }
        }
    }
}
