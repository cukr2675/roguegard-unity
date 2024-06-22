using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class MainRogueObjInfo
    {
        public IKeyword Category => InfoSet.Category;

        public IMainInfoSet BaseInfoSet { get; private set; }

        public IMainInfoSet PolymorphInfoSet { get; private set; }

        public IMainInfoSet InfoSet
        {
            get
            {
                if (PolymorphInfoSet != null) return PolymorphInfoSet;
                else return BaseInfoSet;
            }
            private set
            {
                if (PolymorphInfoSet != null) { PolymorphInfoSet = value; }
                else { BaseInfoSet = value; }
            }
        }

        public MainInfoSetType InfoSetState => PolymorphInfoSet != null ? MainInfoSetType.Polymorph : MainInfoSetType.Base;

        public bool IsTicked { get; set; }
        
        public MainStats Stats { get; private set; }

        // 並び替えを記憶するためシリアルに含める。
        public LearnedSkillList Skills { get; private set; }

        [field: System.NonSerialized]
        public RogueCalculatorInfo Calculators { get; }

        [field: System.NonSerialized]
        public MainSpriteInfo Sprite { get; }

        [System.NonSerialized] private IEquipmentInfo equipmentInfo;
        [System.NonSerialized] private bool equipmentInfoIsDirty;

        public RogueEffectState RogueEffects { get; private set; }

        /// <summary>
        /// <see cref="OpenRogueEffects(RogueObj)"/> の進行状況を取得する。
        /// </summary>
        [field: System.NonSerialized]
        public RogueEffectOpenState RogueEffectOpenState { get; private set; }

        [System.NonSerialized] private IPlayerLeaderInfo playerLeaderInfo;
        [System.NonSerialized] private ILevelInfo levelInfo;
        [System.NonSerialized] private IEquipmentState equipmentState;
        [System.NonSerialized] private StatusEffectState statusEffectState;
        [System.NonSerialized] private ValueEffectState valueEffectState;
        [System.NonSerialized] private RogueObjUpdaterState rogueObjUpdaterState;
        [System.NonSerialized] private RogueMethodAspectState rogueMethodAspectState;
        [System.NonSerialized] private BoneSpriteEffectState boneSpriteEffectState;
        [System.NonSerialized] private SpriteMotionEffectState spriteMotionEffectState;

        private static readonly StaticInitializable<bool> recursion = new StaticInitializable<bool>(() => false);

        [Objforming.CreateInstance]
        private MainRogueObjInfo(bool dummy)
        {
            Sprite = new MainSpriteInfo();
            Calculators = new RogueCalculatorInfo();

            // 逆シリアル化時に Dirty に設定しておく
            equipmentInfoIsDirty = true;
            Sprite.SetDirty();
        }

        public MainRogueObjInfo()
        {
            Stats = new MainStats();
            Skills = new LearnedSkillList();
            RogueEffects = new RogueEffectState();
            Sprite = new MainSpriteInfo();
            Calculators = new RogueCalculatorInfo();
            equipmentInfoIsDirty = true;
        }

        public void SetBaseInfoSet(RogueObj self, IMainInfoSet infoSet)
        {
            if (infoSet == null)
            {
                Debug.LogError($"引数 {nameof(infoSet)} が null です。");
                return;
            }
            if (InfoSetState == MainInfoSetType.Polymorph)
            {
                Debug.LogError($"変化中に {nameof(SetBaseInfoSet)} を実行することはできません。");
                return;
            }
            if (equipmentInfo != null && equipmentInfo.EquipIndex >= 0)
            {
                Debug.LogError("装備中の装備品を変化させることはできません。変化前に解除してください。");
                return;
            }
            if (recursion.Value)
            {
                Debug.LogError($"{nameof(SetBaseInfoSet)} の再帰呼び出しは禁止です。");
                return;
            }
            recursion.Value = true;

            switch (RogueEffectOpenState)
            {
                case RogueEffectOpenState.NotStarted:
                    BaseInfoSet = infoSet;
                    break;
                case RogueEffectOpenState.Finished:
                    InfoSet.Close(self, InfoSetState, false);
                    BaseInfoSet = infoSet;
                    BaseInfoSet = infoSet.Open(self, InfoSetState, false);
                    ReopenInfoSet(self);
                    break;
                default:
                    throw new RogueException();
            }
            SpeedCalculator.SetDirty(self);
            MovementCalculator.SetDirty(self);
            WeightCalculator.SetDirty(self);
            Sprite.SetDirty();
            equipmentInfoIsDirty = true;
            recursion.Value = false;
        }

        public void Polymorph(RogueObj self, IMainInfoSet infoSet)
        {
            Polymorph(self, infoSet, 0);
        }

        internal void Polymorph(RogueObj self, IMainInfoSet infoSet, int deltaLv)
        {
            if (infoSet == null)
            {
                Debug.LogError($"引数 {nameof(infoSet)} が null です。");
                return;
            }
            if (equipmentInfo != null && equipmentInfo.EquipIndex >= 0)
            {
                Debug.LogError("装備中の装備品を変化させることはできません。変化前に解除してください。");
                return;
            }
            if (recursion.Value)
            {
                Debug.LogError($"{nameof(Polymorph)} の再帰呼び出しは禁止です。");
                return;
            }
            recursion.Value = true;

            if (infoSet.Equals(InfoSet))
            {
                // 再変化
                switch (RogueEffectOpenState)
                {
                    case RogueEffectOpenState.NotStarted:
                        break;
                    case RogueEffectOpenState.Finished:
                        BaseInfoSet = BaseInfoSet.Reopen(self, MainInfoSetType.Base, deltaLv);
                        PolymorphInfoSet = PolymorphInfoSet?.Reopen(self, MainInfoSetType.Polymorph, deltaLv);
                        ReopenInfoSet(self);
                        break;
                    default:
                        throw new RogueException();
                }
            }
            else if (infoSet.Equals(BaseInfoSet))
            {
                // 変化解除
                switch (RogueEffectOpenState)
                {
                    case RogueEffectOpenState.NotStarted:
                        PolymorphInfoSet = null;
                        break;
                    case RogueEffectOpenState.Finished:
                        InfoSet.Close(self, InfoSetState, false);
                        PolymorphInfoSet = null;
                        BaseInfoSet = infoSet.Open(self, InfoSetState, true);
                        ReopenInfoSet(self);
                        break;
                    default:
                        throw new RogueException();
                }
            }
            else
            {
                // 変化
                switch (RogueEffectOpenState)
                {
                    case RogueEffectOpenState.NotStarted:
                        PolymorphInfoSet = infoSet;
                        break;
                    case RogueEffectOpenState.Finished:
                        InfoSet.Close(self, InfoSetState, InfoSetState == MainInfoSetType.Base);
                        PolymorphInfoSet = infoSet; // 下の行の Open で使用する可能性があるため、いったんここで設定しておく。
                        PolymorphInfoSet = infoSet.Open(self, InfoSetState, false);
                        ReopenInfoSet(self);
                        break;
                    default:
                        throw new RogueException();
                }
            }
            SpeedCalculator.SetDirty(self);
            MovementCalculator.SetDirty(self);
            WeightCalculator.SetDirty(self);
            Sprite.SetDirty();
            equipmentInfoIsDirty = true;
            recursion.Value = false;
        }

        public void UpdatePlayerLeaderInfo(RogueObj self, IPlayerLeaderInfo playerLeaderInfo)
        {
            this.playerLeaderInfo?.RemoveClose(self);
            this.playerLeaderInfo = playerLeaderInfo;
        }

        /// <summary>
        /// このインスタンスに設定済みの <see cref="ILevelInfo.RemoveClose"/> を実行して <paramref name="levelInfo"/> を設定する。
        /// </summary>
        public void UpdateLevelInfo(RogueObj self, ILevelInfo levelInfo)
        {
            this.levelInfo?.RemoveClose(self);
            this.levelInfo = levelInfo;
        }

        public IPlayerLeaderInfo GetPlayerLeaderInfo(RogueObj self)
        {
            if (RogueEffectOpenState == RogueEffectOpenState.NotStarted) OpenRogueEffects(self);
            return playerLeaderInfo;
        }

        public ILevelInfo GetLevelInfo(RogueObj self)
        {
            if (RogueEffectOpenState == RogueEffectOpenState.NotStarted) OpenRogueEffects(self);
            return levelInfo;
        }

        public IEquipmentInfo GetEquipmentInfo(RogueObj self)
        {
            if (equipmentInfoIsDirty)
            {
                equipmentInfo = InfoSet.GetEquipmentInfo(self);

                // この装備品の装備状態を取得するために、所持者のエフェクト準備を挟んでから値を返す
                var owner = self.Location;
                if (owner != null && owner.Main.RogueEffectOpenState == RogueEffectOpenState.NotStarted)
                {
                    owner.Main.OpenRogueEffects(owner);
                }

                equipmentInfoIsDirty = false;
            }
            return equipmentInfo;
        }

        public IEquipmentState GetEquipmentState(RogueObj self)
        {
            if (RogueEffectOpenState == RogueEffectOpenState.NotStarted) OpenRogueEffects(self);
            return equipmentState;
        }

        public StatusEffectState GetStatusEffectState(RogueObj self)
        {
            if (RogueEffectOpenState == RogueEffectOpenState.NotStarted) OpenRogueEffects(self);
            return statusEffectState;
        }

        public ValueEffectState GetValueEffectState(RogueObj self)
        {
            if (RogueEffectOpenState == RogueEffectOpenState.NotStarted) OpenRogueEffects(self);
            return valueEffectState;
        }

        public RogueObjUpdaterState GetRogueObjUpdaterState(RogueObj self)
        {
            if (RogueEffectOpenState == RogueEffectOpenState.NotStarted) OpenRogueEffects(self);
            return rogueObjUpdaterState;
        }

        public RogueMethodAspectState GetRogueMethodAspectState(RogueObj self)
        {
            if (RogueEffectOpenState == RogueEffectOpenState.NotStarted) OpenRogueEffects(self);
            return rogueMethodAspectState;
        }

        public BoneSpriteEffectState GetBoneSpriteEffectState(RogueObj self)
        {
            if (RogueEffectOpenState == RogueEffectOpenState.NotStarted) OpenRogueEffects(self);
            return boneSpriteEffectState;
        }

        public SpriteMotionEffectState GetSpriteMotionEffectState(RogueObj self)
        {
            if (RogueEffectOpenState == RogueEffectOpenState.NotStarted) OpenRogueEffects(self);
            return spriteMotionEffectState;
        }

        public void TryOpenRogueEffects(RogueObj self)
        {
            if (RogueEffectState.openingObj.Value != null) throw new RogueException(
                $"{self} のエフェクト準備に失敗しました。 " +
                $"いずれかの {nameof(RogueObj)} ({RogueEffectState.openingObj.Value}) " +
                $"のエフェクト追加・準備中に新しいエフェクト準備を開始することはできません。");

            if (RogueEffectOpenState == RogueEffectOpenState.NotStarted) OpenRogueEffects(self);
        }

        private void OpenRogueEffects(RogueObj self)
        {
            if (RogueEffectOpenState != RogueEffectOpenState.NotStarted) throw new RogueException();

            statusEffectState = new StatusEffectState();
            valueEffectState = new ValueEffectState();
            rogueObjUpdaterState = new RogueObjUpdaterState();
            rogueMethodAspectState = new RogueMethodAspectState();
            boneSpriteEffectState = new BoneSpriteEffectState();
            spriteMotionEffectState = new SpriteMotionEffectState();

            RogueEffectOpenState = RogueEffectOpenState.OpeningInfoSet;
            BaseInfoSet = BaseInfoSet.Open(self, MainInfoSetType.Base, false);
            if (InfoSetState == MainInfoSetType.Polymorph)
            {
                BaseInfoSet.Close(self, MainInfoSetType.Base, true);
                PolymorphInfoSet = PolymorphInfoSet.Open(self, InfoSetState, false);
            }
            equipmentState = InfoSet.GetEquipmentState(self);

            RogueEffectOpenState = RogueEffectOpenState.OpeningEffects;
            RogueEffects.Open(self);

            RogueEffectOpenState = RogueEffectOpenState.Finished;
        }

        private void ReopenInfoSet(RogueObj self)
        {
            var newEquipmentState = InfoSet.GetEquipmentState(self);
            if (newEquipmentState != equipmentState)
            {
                ReequipEquipments(equipmentState, newEquipmentState);
                equipmentState = newEquipmentState;
            }
            equipmentInfo = null;
        }

        /// <summary>
        /// <paramref name="from"/> から <paramref name="to"/> へ装備品を移動させる。
        /// </summary>
        private static void ReequipEquipments(IEquipmentState from, IEquipmentState to)
        {
            if (from == null) return;

            for (int i = 0; i < from.Parts.Count; i++)
            {
                var fromPart = from.Parts[i];
                if (to == null || !Contains(to.Parts, fromPart))
                {
                    // 移動先に同一の部位が存在しないときは解除する。
                    Unequip(fromPart);
                }
                else
                {
                    Reequip(fromPart);
                }
            }

            bool Contains(Spanning<IKeyword> list, IKeyword item)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == item) return true;
                }
                return false;
            }

            void Unequip(IKeyword fromPart)
            {
                var fromLength = from.GetLength(fromPart);
                for (int i = 0; i < fromLength; i++)
                {
                    var fromEquipment = from.GetEquipment(fromPart, i);
                    var fromEquipmentInfo = fromEquipment.Main.GetEquipmentInfo(fromEquipment);
                    fromEquipmentInfo.RemoveClose(fromEquipment);
                }
            }

            void Reequip(IKeyword fromPart)
            {
                var fromLength = from.GetLength(fromPart);
                var toLength = to.GetLength(fromPart);
                var reequipLength = Mathf.Min(fromLength, toLength);
                for (int i = 0; i < reequipLength; i++)
                {
                    var reequipEquipment = from.GetEquipment(fromPart, i);
                    to.SetEquipment(fromPart, i, reequipEquipment);
                }
                for (int i = reequipLength; i < fromLength; i++)
                {
                    // 移動しきれないぶんは解除する。
                    var fromEquipment = from.GetEquipment(fromPart, i);
                    var fromEquipmentInfo = fromEquipment.Main.GetEquipmentInfo(fromEquipment);
                    fromEquipmentInfo.RemoveClose(fromEquipment);
                }
            }
        }

        public bool CanStack(RogueObj self, RogueObj other)
        {
            if (!BaseInfoSet.Equals(other.Main.BaseInfoSet)) return false;
            if (!(PolymorphInfoSet?.Equals(other.Main.PolymorphInfoSet) ?? other.Main.PolymorphInfoSet == null)) return false;

            var selfEquipmentInfo = GetEquipmentInfo(self);
            var otherEquipmentInfo = other.Main.GetEquipmentInfo(other);
            if (selfEquipmentInfo != null || otherEquipmentInfo != null)
            {
                if (selfEquipmentInfo == null || otherEquipmentInfo == null) return false;
                if (otherEquipmentInfo.EquipIndex != -1) return false;
                if (selfEquipmentInfo.EquipIndex != -1 && !selfEquipmentInfo.CanStackWhileEquipped) return false;
            }

            if (!Stats.CanStack(other.Main.Stats)) return false;
            if (Skills.Count >= 1 || other.Main.Skills.Count >= 1) return false;
            if (!RogueEffects.CanStack(self, other)) return false;

            return true;
        }

        public MainRogueObjInfo Clone(RogueObj self, RogueObj clonedSelf)
        {
            var clone = new MainRogueObjInfo();
            clonedSelf.Main = clone;
            clone.Stats = Stats.Clone(self, clonedSelf);
            clone.RogueEffects = RogueEffects.Clone(self, clonedSelf);
            clone.SetBaseInfoSet(clonedSelf, BaseInfoSet);
            if (PolymorphInfoSet != null) { clone.Polymorph(clonedSelf, PolymorphInfoSet); }
            return clone;
        }

        public void ReplaceCloned(RogueObj obj, RogueObj clonedObj)
        {
            Stats.ReplaceCloned(obj, clonedObj);
            RogueEffects.ReplaceCloned(obj, clonedObj);
        }
    }
}
