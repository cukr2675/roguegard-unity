using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class CharacterCreationInfoSet : IMainInfoSet
    {
        public string Name => Data.Name;
        public Sprite Icon => CurrentRaceOption.Icon;
        public Color Color => CurrentRaceOption.Color;
        public string Caption => Data.Caption;
        public IRogueDetails Details => Data.Details;

        public ICharacterCreationData Data { get; }

        private int currentRaceOptionIndex;
        [System.NonSerialized] private IRaceOption _currentRaceOption;
        public IRaceOption CurrentRaceOption
        {
            get
            {
                if (_currentRaceOption != null) return _currentRaceOption;

                if (Data.Race.Option.GrowingOptions.Count == 0)
                {
                    _currentRaceOption = Data.Race.Option;
                }
                else
                {
                    if (currentRaceOptionIndex >= Data.Race.Option.GrowingOptions.Count)
                    {
                        Debug.LogWarning($"{nameof(currentRaceOptionIndex)} ({currentRaceOptionIndex}) が不正です。");
                        currentRaceOptionIndex = Data.Race.Option.GrowingOptions.IndexOf(Data.Race.Option);
                    }
                    _currentRaceOption = Data.Race.Option.GrowingOptions[currentRaceOptionIndex];
                }
                return _currentRaceOption;
            }
        }

        private readonly IRogueGender _gender;

        [System.NonSerialized] private float _weight;
        [System.NonSerialized] private IReadOnlyNodeBone nodeBone;
        [System.NonSerialized] private AppearanceBoneSpriteTable characterBoneSpriteTable;

        public IKeyword Category => CurrentRaceOption.Category;

		public int MaxHP => CurrentRaceOption.MaxHP;
        public int MaxMP => CurrentRaceOption.MaxMP;
        public int ATK => CurrentRaceOption.ATK;
        public int DEF => CurrentRaceOption.DEF;
        public float Weight
        {
            get
            {
                if (nodeBone == null) { Reload(); }

                return _weight;
            }
        }
        public float LoadCapacity => CurrentRaceOption.LoadCapacity;
        public ISerializableKeyword Faction => CurrentRaceOption.Faction;
        public Spanning<ISerializableKeyword> TargetFactions => CurrentRaceOption.TargetFactions;

        public MainInfoSetAbility Ability => CurrentRaceOption.Ability;
        public IRogueMaterial Material => CurrentRaceOption.Material;
        public IRogueGender Gender => Data.Race.Gender ?? _gender;
        public string HPName => Data.Race.HPName;
        public string MPName => Data.Race.MPName;
        public float Cost => Data.Cost;
        public bool CostIsUnknown => Data.CostIsUnknown;
        public Spanning<IWeightedRogueObjGeneratorList> LootTable => CurrentRaceOption.LootTable;

        public IActiveRogueMethod Walk => CurrentRaceOption.Walk;
        public IActiveRogueMethod Wait => CurrentRaceOption.Wait;
        public ISkill Attack => CurrentRaceOption.Attack;
        public ISkill Throw => CurrentRaceOption.Throw;
        public IActiveRogueMethod PickUp => CurrentRaceOption.PickUp;
        public IActiveRogueMethod Put => CurrentRaceOption.Put;
        public IEatActiveRogueMethod Eat => CurrentRaceOption.Eat;

        public IAffectRogueMethod Hit => CurrentRaceOption.Hit;
        public IAffectRogueMethod BeDefeated => CurrentRaceOption.BeDefeated;
        public IChangeStateRogueMethod Locate => CurrentRaceOption.Locate;
        public IChangeStateRogueMethod Polymorph => CurrentRaceOption.Polymorph;

        public IApplyRogueMethod BeApplied => CurrentRaceOption.BeApplied;
        public IApplyRogueMethod BeThrown => CurrentRaceOption.BeThrown;
        public IApplyRogueMethod BeEaten => CurrentRaceOption.BeEaten;

        [Objforming.CreateInstance]
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
        private void Reload()
        {
            _currentRaceOption = null;
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
                nodeBone = mainNode;
            }
        }

        public IMainInfoSet Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base)
        {
            if (nodeBone == null) { Reload(); }

            var newRaceOption = CurrentRaceOption.Open(self, infoSetType, polymorph2Base, CurrentRaceOption, Data);
            characterBoneSpriteTable.AddEffectFromInfoSet(self);
            Data.SortedIntrinsics.Open(self, infoSetType, polymorph2Base);

            if (newRaceOption == CurrentRaceOption) return this;
            else if (Data.TryGetGrowingInfoSet(newRaceOption, _gender, out var growingInfoSet)) return growingInfoSet;

            Debug.LogError($"予期しない {nameof(IRaceOption)} を取得しました。");
            return this;
        }

        public void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph)
        {
            if (nodeBone == null) { Reload(); }

            CurrentRaceOption.Close(self, infoSetType, base2Polymorph, CurrentRaceOption, Data);
            characterBoneSpriteTable.Remove(self);
            Data.SortedIntrinsics.Close(self, infoSetType, base2Polymorph);
        }

        public IMainInfoSet Reopen(RogueObj self, MainInfoSetType infoSetType, int deltaLv)
        {
            var newRaceOption = CurrentRaceOption.Reopen(self, infoSetType, CurrentRaceOption, Data);
            Data.SortedIntrinsics.Reopen(self, infoSetType, deltaLv);

            if (newRaceOption == CurrentRaceOption) return this;
            else if (Data.TryGetGrowingInfoSet(newRaceOption, _gender, out var growingInfoSet)) return growingInfoSet;

            Debug.LogError($"予期しない {nameof(IRaceOption)} を取得しました。");
            return this;
        }

        public IEquipmentState GetEquipmentState(RogueObj self)
        {
            return CurrentRaceOption.GetEquipmentState(self, CurrentRaceOption, Data);
        }

        public IEquipmentInfo GetEquipmentInfo(RogueObj self)
        {
            return CurrentRaceOption.GetEquipmentInfo(self, CurrentRaceOption, Data);
        }

        public void GetObjSprite(RogueObj self, out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            if (nodeBone == null) { Reload(); }

            CurrentRaceOption.GetObjSprite(CurrentRaceOption, Data, _gender, self, nodeBone, out objSprite, out motionSet);
        }

        public RogueObj CreateObj(RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var obj = CreateObj(this, location, position, Data.StartingItemTable, random, stackOption, Data.Race.Lv);
            CurrentRaceOption.InitializeObj(obj, CurrentRaceOption, Data);
            return obj;
        }

        public static RogueObj CreateObj(
            IMainInfoSet infoSet, RogueObj location, Vector2Int position, StackOption stackOption = StackOption.Default, int initialLv = 0)
        {
            return CreateObj(infoSet, location, position, Spanning<IWeightedRogueObjGeneratorList>.Empty, null, stackOption, initialLv);
        }

        public static RogueObj CreateObj(
            IMainInfoSet infoSet, RogueObj location, Vector2Int position, Spanning<IWeightedRogueObjGeneratorList> startingItems, IRogueRandom random,
            StackOption stackOption = StackOption.Default, int initialLv = 0)
        {
            var obj = new RogueObj();
            obj.Main = new MainRogueObjInfo();
            obj.Main.SetBaseInfoSet(obj, infoSet);
            RoguegardCharacterCreationSettings.LevelInfoInitializer.InitializeLv(obj, initialLv);
            var stats = obj.Main.Stats;
            // キャラは下向き　アイテムは矢の向きを考慮して左下向き
            stats.Direction = infoSet.Ability.HasFlag(MainInfoSetAbility.HasCollider) ? RogueDirection.Down : RogueDirection.LowerLeft;
            stats.Reset(obj);
            if (startingItems.Count >= 1) { WeightedRogueObjGeneratorUtility.CreateObjs(startingItems, obj, random); }
            if (!SpaceUtility.TryLocate(obj, location, position, stackOption)) throw new RogueException("生成したオブジェクトの移動に失敗しました。");

            return obj;
        }

        public bool Equals(IMainInfoSet other)
        {
            return other is CharacterCreationInfoSet c && Data == c.Data && currentRaceOptionIndex == c.currentRaceOptionIndex && _gender == c._gender;
        }

        public override bool Equals(object obj)
        {
            return obj is CharacterCreationInfoSet c && Data == c.Data && currentRaceOptionIndex == c.currentRaceOptionIndex && _gender == c._gender;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
