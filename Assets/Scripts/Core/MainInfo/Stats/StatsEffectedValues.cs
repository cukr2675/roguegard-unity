using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// 注意： <see cref="IValueEffect"/> 内での呼び出し禁止
    /// </summary>
    public static class StatsEffectedValues
    {
        private static readonly AffectableValue value = AffectableValue.Get();

        public static int GetMaxHP(RogueObj self)
        {
            value.Initialize(self.Main.InfoSet.MaxHP);
            ValueEffectState.AffectValue(StatsKw.MaxHP, value, self);
            return Mathf.FloorToInt(value.MainValue);
        }

        public static int GetMaxMP(RogueObj self)
        {
            value.Initialize(self.Main.InfoSet.MaxMP);
            ValueEffectState.AffectValue(StatsKw.MaxMP, value, self);
            return Mathf.FloorToInt(value.MainValue);
        }

        public static int GetRequiredMP(RogueObj self, float baseRequiredMP)
        {
            value.Initialize(baseRequiredMP);
            ValueEffectState.AffectValue(StatsKw.RequiredMP, value, self);
            return Mathf.Max(Mathf.FloorToInt(value.MainValue), 1);
        }

        public static float GetLoadCapacity(RogueObj self)
        {
            var leader = self.Main.GetPlayerLeaderInfo(self) != null;
            var coefficient = leader ? 1f : 0.5f; // リーダー以外の最大重量は 1/2
            value.Initialize(self.Main.InfoSet.LoadCapacity);
            ValueEffectState.AffectValue(StatsKw.LoadCapacity, value, self);
            return value.MainValue * coefficient;
        }

        public static int GetMaxNutrition(RogueObj self)
        {
            var leader = self.Main.GetPlayerLeaderInfo(self) != null;
            var baseMaxNutrition = 1000f; // 基本最大満腹度は 1000
            var coefficient = leader ? 1f : 0.1f; // リーダー以外の最大満腹度は 1/10
            value.Initialize(baseMaxNutrition);
            ValueEffectState.AffectValue(StatsKw.MaxNutrition, value, self);
            return Mathf.FloorToInt(value.MainValue * coefficient);
        }

        public static int GetRegenerationHPPermille(RogueObj self)
        {
            var maxHP = GetMaxHP(self);
            value.Initialize(maxHP);
            value.MainValue = self.Main.Stats.Nutrition >= 1 ? maxHP * 5 : 0; // 基本は最大値の 0.5%
            ValueEffectState.AffectValue(StatsKw.HPRegenerationPermille, value, self);
            return Mathf.FloorToInt(value.MainValue);
        }

        public static int GetRegenerationMPPermille(RogueObj self)
        {
            var maxMP = GetMaxMP(self);
            value.Initialize(maxMP);
            value.MainValue = self.Main.Stats.Nutrition >= 1 ? maxMP * 2 : 0; // 基本は最大値の 0.2%
            ValueEffectState.AffectValue(StatsKw.MPRegenerationPermille, value, self);
            return Mathf.FloorToInt(value.MainValue);
        }

        public static void GetATK(RogueObj self, AffectableValue refATKValue)
        {
            refATKValue.Initialize(self.Main.InfoSet.ATK);
            refATKValue.SubValues[StatsKw.CriticalATK] = 1f; // 基本会心攻撃力は 1
            ValueEffectState.AffectValue(StatsKw.ATK, refATKValue, self);
        }

        public static int GetDEF(RogueObj self)
        {
            value.Initialize(0f);
            value.MainValue -= self.Main.InfoSet.DEF;
            value.SubValues[StatsKw.GuardDEF] += 1f; // 基本ガード防御力は 1
            ValueEffectState.AffectValue(StatsKw.DEF, value, self);
            var def = -Mathf.FloorToInt(value.MainValue);
            return def;
        }

        public static int GetDamage(RogueObj self, AffectableValue refDamageValue, IRogueRandom random, out bool critical, out bool guard)
        {
            refDamageValue.BaseMainValue = refDamageValue.MainValue;
            refDamageValue.MainValue -= self.Main.InfoSet.DEF;
            refDamageValue.SubValues[StatsKw.GuardDEF] += 1f; // 基本ガード防御力は 1
            ValueEffectState.AffectValue(StatsKw.DEF, refDamageValue, self);

            // 会心成功時、会心攻撃力を足す。
            critical = refDamageValue.SubValues[StatsKw.CriticalRate] >= 1f;
            if (!critical) { critical = random.NextFloat(0f, 1f) < refDamageValue.SubValues[StatsKw.CriticalRate]; }
            if (critical)
            {
                refDamageValue.MainValue += refDamageValue.SubValues[StatsKw.CriticalATK];
                refDamageValue.SubValues[StatsKw.Critical] = 1f;
            }

            // ガード成功時、ガード防御力で引く。
            guard = refDamageValue.SubValues[StatsKw.GuardRate] >= 1f;
            if (!guard) { guard = random.NextFloat(0f, 1f) < refDamageValue.SubValues[StatsKw.GuardRate]; }
            if (guard)
            {
                refDamageValue.MainValue -= refDamageValue.SubValues[StatsKw.GuardDEF];
                refDamageValue.SubValues[StatsKw.Guard] = 1f;
            }

            // ダメージは 0 以上の整数。
            var guaranteedDamage = Mathf.FloorToInt(refDamageValue.SubValues[StatsKw.GuaranteedDamage]);
            guaranteedDamage = Mathf.Max(guaranteedDamage, 0);

            var damage = Mathf.FloorToInt(refDamageValue.MainValue);
            damage = Mathf.Max(damage, guaranteedDamage);
            return damage;
        }

        public static int GetExp(RogueObj self)
        {
            value.Initialize(self.Main.Stats.Lv);
            ValueEffectState.AffectValue(StatsKw.Exp, value, self);
            return Mathf.FloorToInt(value.MainValue);
        }

        public static float GetCost(RogueObj self)
        {
            value.Initialize(self.Main.InfoSet.Cost);
            ValueEffectState.AffectValue(StatsKw.Cost, value, self);
            return value.MainValue;
        }

        public static void GetMaterial(RogueObj self, AffectableValue refMaterialValue)
        {
            refMaterialValue.Initialize(0);
            self.Main.InfoSet.Material.AffectValue(refMaterialValue, self);
            ValueEffectState.AffectValue(StatsKw.Material, refMaterialValue, self);
        }

        public static void GetGender(RogueObj self, AffectableValue refGenderValue)
        {
            refGenderValue.Initialize(0);
            self.Main.BaseInfoSet.Gender.AffectValue(refGenderValue, self, MainInfoSetType.Base);
            self.Main.PolymorphInfoSet?.Gender.AffectValue(refGenderValue, self, MainInfoSetType.Polymorph);
            ValueEffectState.AffectValue(StatsKw.Gender, refGenderValue, self);
        }

        private static bool Targeting(RogueObj self, RogueObj obj)
        {
            if (self == null) throw new System.ArgumentNullException(nameof(self));
            if (obj == null) throw new System.ArgumentNullException(nameof(obj));

            var selfStats = self.Main.Stats;
            var objParty = obj.Main.Stats.Party;
            if (selfStats.TargetObj != null) return RogueParty.Equals(selfStats.TargetObj, obj);

            GetFaction(self, selfStats.Party, out _, out var targetFactions);
            GetFaction(obj, objParty, out var objFaction, out _);
            for (int i = 0; i < targetFactions.Count; i++)
            {
                if (targetFactions[i] == objFaction) return true;
            }
            return false;

            static void GetFaction(RogueObj obj, RogueParty party, out ISerializableKeyword faction, out Spanning<ISerializableKeyword> targetFactions)
            {
                if (party != null)
                {
                    faction = party.Faction;
                    targetFactions = party.TargetFactions;
                }
                else
                {
                    faction = obj.Main.InfoSet.Faction;
                    targetFactions = obj.Main.InfoSet.TargetFactions;
                }
            }
        }

        public static bool AreVS(RogueObj obj1, RogueObj obj2)
        {
            return Targeting(obj1, obj2) || Targeting(obj2, obj1);
        }
    }
}
