using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class CharacterCreationInfoSet : MainInfoSet
    {
        public override string Name => Data.Name;
        public override Sprite Icon => CurrentRaceOption.Icon;
        public override Color Color => CurrentRaceOption.Color;
        public override string Caption => Data.Caption;
        public override object Details => Data.Details;

        public ICharacterCreationData Data { get; }

        private readonly int currentRaceOptionIndex;
        [System.NonSerialized] private IRaceOption _currentRaceOption;
        public IRaceOption CurrentRaceOption
        {
            get
            {
                if (_currentRaceOption != null) return _currentRaceOption;


                if (Data.Race.Option.GrowingOptions.Count == 0) { _currentRaceOption = Data.Race.Option; }
                else { _currentRaceOption = Data.Race.Option.GrowingOptions[currentRaceOptionIndex]; }
                return _currentRaceOption;
            }
        }

        private readonly IRogueGender _gender;

        [System.NonSerialized] private float _weight;
        [System.NonSerialized] private IBoneNode boneNode;
        [System.NonSerialized] private AppearanceBoneSpriteTable characterBoneSpriteTable;

        public override IKeyword Category => CurrentRaceOption.Category;

		public override int MaxHP => CurrentRaceOption.MaxHP;
        public override int MaxMP => CurrentRaceOption.MaxMP;
        public override int ATK => CurrentRaceOption.ATK;
        public override int DEF => CurrentRaceOption.DEF;
        public override float Weight
        {
            get
            {
                if (boneNode == null) { Reload(); }

                return _weight;
            }
        }
        public override float LoadCapacity => CurrentRaceOption.LoadCapacity;
        public override ISerializableKeyword Faction => CurrentRaceOption.Faction;
        public override Spanning<ISerializableKeyword> TargetFactions => CurrentRaceOption.TargetFactions;

        public override MainInfoSetAbility Ability => CurrentRaceOption.Ability;
        public override IRogueMaterial Material => CurrentRaceOption.Material;
        public override IRogueGender Gender => Data.Race.Gender ?? _gender;
        public override string HPName => Data.Race.HPName;
        public override string MPName => Data.Race.MPName;
        public override float Cost => Data.Cost;
        public override bool CostIsUnknown => Data.CostIsUnknown;
        public override Spanning<IWeightedRogueObjGeneratorList> LootTable => CurrentRaceOption.LootTable;

        public override IActiveRogueMethod Walk => CurrentRaceOption.Walk;
        public override IActiveRogueMethod Wait => CurrentRaceOption.Wait;
        public override ISkill Attack => CurrentRaceOption.Attack;
        public override ISkill Throw => CurrentRaceOption.Throw;
        public override IActiveRogueMethod PickUp => CurrentRaceOption.PickUp;
        public override IActiveRogueMethod Put => CurrentRaceOption.Put;
        public override IEatActiveRogueMethod Eat => CurrentRaceOption.Eat;

        public override IAffectRogueMethod Hit => CurrentRaceOption.Hit;
        public override IAffectRogueMethod BeDefeated => CurrentRaceOption.BeDefeated;
        public override IChangeStateRogueMethod Locate => CurrentRaceOption.Locate;
        public override IChangeStateRogueMethod Polymorph => CurrentRaceOption.Polymorph;

        public override IApplyRogueMethod BeApplied => CurrentRaceOption.BeApplied;
        public override IApplyRogueMethod BeThrown => CurrentRaceOption.BeThrown;
        public override IApplyRogueMethod BeEaten => CurrentRaceOption.BeEaten;

        [ObjectFormer.CreateInstance]
        private CharacterCreationInfoSet() { }

        public CharacterCreationInfoSet(ICharacterCreationData data, int growingOptionIndex, IRogueGender gender)
        {
            Data = data ?? throw new System.ArgumentNullException(nameof(data));
            currentRaceOptionIndex = growingOptionIndex;
            _gender = gender ?? throw new System.ArgumentNullException(nameof(gender));

            Reload();
        }

        /// <summary>
        /// <see cref="ICharacterCreationData"/> を再読み込みし、見た目などを再構成する。
        /// </summary>
        public void Reload()
        {
            _weight = CurrentRaceOption.GetWeight(CurrentRaceOption, Data);
            CurrentRaceOption.GetSpriteValues(CurrentRaceOption, Data, _gender, out var mainNode, out characterBoneSpriteTable);

            if (mainNode != null)
            {
                var appearances = Data.Appearances;
                for (int i = 0; i < appearances.Count; i++)
                {
                    var appearance = appearances[i];
                    appearance.Option.Affect(mainNode, characterBoneSpriteTable, appearance, Data);
                }
                boneNode = mainNode;
            }
        }

        public override MainInfoSet Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base)
        {
            if (boneNode == null) { Reload(); }

            var newRaceOption = CurrentRaceOption.Open(self, infoSetType, polymorph2Base, CurrentRaceOption, Data);
            characterBoneSpriteTable.AddEffectFromInfoSet(self);
            Data.SortedIntrinsics.Open(self, infoSetType, polymorph2Base);

            if (newRaceOption == CurrentRaceOption) return this;
            else if (Data.TryGetGrowingInfoSet(newRaceOption, _gender, out var growingInfoSet)) return growingInfoSet;

            Debug.LogError($"予期しない {nameof(IRaceOption)} を取得しました。");
            return this;
        }

        public override void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph)
        {
            if (boneNode == null) { Reload(); }

            CurrentRaceOption.Close(self, infoSetType, base2Polymorph, CurrentRaceOption, Data);
            characterBoneSpriteTable.Remove(self);
            Data.SortedIntrinsics.Close(self, infoSetType, base2Polymorph);
        }

        public override MainInfoSet Reopen(RogueObj self, MainInfoSetType infoSetType)
        {
            var newRaceOption = CurrentRaceOption.Reopen(self, infoSetType, CurrentRaceOption, Data);
            Data.SortedIntrinsics.Reopen(self, infoSetType);

            if (newRaceOption == CurrentRaceOption) return this;
            else if (Data.TryGetGrowingInfoSet(newRaceOption, _gender, out var growingInfoSet)) return growingInfoSet;

            Debug.LogError($"予期しない {nameof(IRaceOption)} を取得しました。");
            return this;
        }

        public override IEquipmentState GetEquipmentState(RogueObj self)
        {
            return CurrentRaceOption.GetEquipmentState(self, CurrentRaceOption, Data);
        }

        public override IEquipmentInfo GetEquipmentInfo(RogueObj self)
        {
            return CurrentRaceOption.GetEquipmentInfo(self, CurrentRaceOption, Data);
        }

        public override void GetObjSprite(RogueObj self, out RogueObjSprite objSprite, out IMotionSet motionSet)
        {
            if (boneNode == null) { Reload(); }

            CurrentRaceOption.GetObjSprite(CurrentRaceOption, Data, _gender, self, boneNode, out objSprite, out motionSet);
        }

        public RogueObj CreateObj(RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var obj = new RogueObj();
            obj.Main = new MainRogueObjInfo();
            obj.Main.SetBaseInfoSet(obj, this);
            RoguegardCharacterCreationSettings.LevelInfoInitializer.InitializeLv(obj, Data.Race.Lv);
            var stats = obj.Main.Stats;
            // キャラは下向き　アイテムは矢の向きを考慮して左下向き
            stats.Direction = CurrentRaceOption.Ability.HasFlag(MainInfoSetAbility.HasCollider) ? RogueDirection.Down : RogueDirection.LowerLeft;
            stats.Reset(obj);
            WeightedRogueObjGeneratorUtility.CreateObjs(Data.StartingItemTable, obj, random);
            if (!SpaceUtility.TryLocate(obj, location, position, stackOption)) throw new RogueException("生成したオブジェクトの移動に失敗しました。");

            CurrentRaceOption.InitializeObj(obj, CurrentRaceOption, Data);
            return obj;
        }

        public override bool Equals(MainInfoSet other)
        {
            return other is CharacterCreationInfoSet c && Data == c.Data && currentRaceOptionIndex == c.currentRaceOptionIndex && _gender == c._gender;
        }
    }
}
