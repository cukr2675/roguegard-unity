using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class MainStats
    {
        public int HP { get; private set; }
        public int MP { get; private set; }
        public int Nutrition { get; private set; }
        public int Lv { get; private set; } = 1;
        public int TotalExp { get; private set; }

        public RogueParty Party { get; private set; }
        public RogueObj TargetObj { get; set; }
        public RogueDirection Direction { get; set; }
        public int ChargedSpeed { get; set; }

        public int RegenerationHPPermille { get; private set; }
        public int RegenerationMPPermille { get; private set; }

        public void Reset(RogueObj self)
        {
            // オーバーヒールをリセットするために MaxHP を設定
            SetHP(self, StatsEffectedValues.GetMaxHP(self));
            SetMP(self, StatsEffectedValues.GetMaxMP(self));
            SetNutrition(self, StatsEffectedValues.GetMaxNutrition(self));
        }

        public void SetHP(RogueObj self, int hp, bool over = false)
        {
            if (over || hp <= HP)
            {
                // HP 上限を超えて回復できる　減少する場合は最大値を気にする必要がないためそのまま設定
                HP = hp;
            }
            else
            {
                // HP 上限または上限を突破した HP を超えないようにする
                var maxHP = StatsEffectedValues.GetMaxHP(self);
                maxHP = Mathf.Max(maxHP, HP);
                HP = Mathf.Min(hp, maxHP);
            }
        }

        public void SetMP(RogueObj self, int mp, bool over = false)
        {
            if (over || mp <= MP)
            {
                // MP 上限を超えて回復できる　減少する場合は最大値を気にする必要がないためそのまま設定
                MP = mp;
            }
            else
            {
                // MP 上限または上限を突破した MP を超えないようにする
                var maxMP = StatsEffectedValues.GetMaxMP(self);
                maxMP = Mathf.Max(maxMP, MP);
                MP = Mathf.Min(mp, maxMP);
            }
        }

        public void SetNutrition(RogueObj self, int nutrition, bool over = false)
        {
            if (over || nutrition <= Nutrition)
            {
                // 満腹度上限を超えて回復できる　減少する場合は最大値を気にする必要がないためそのまま設定
                Nutrition = nutrition;
            }
            else
            {
                // 満腹度上限または上限を突破した満腹度を超えないようにする
                var maxNutrition = StatsEffectedValues.GetMaxNutrition(self);
                maxNutrition = Mathf.Max(maxNutrition, Nutrition);
                Nutrition = Mathf.Min(nutrition, maxNutrition);
            }
        }

        public void SetLv(RogueObj self, int lv)
        {
            if (lv == Lv)
            {
                var levelInfo = self.Main.GetLevelInfo(self);
                TotalExp = levelInfo?.NextTotalExps[Lv - 1] ?? 0;
                return;
            }

            // 変化前 InfoSet と変化後 InfoSet でスキルの習得順を維持するため１レベルごとに処理する。
            while (Lv < lv)
            {
                Lv++;
                if (self.Main.InfoSet != null) { self.Main.Polymorph(self, self.Main.InfoSet); }

                var levelInfo = self.Main.GetLevelInfo(self);
                if (levelInfo != null)
                {
                    levelInfo.LevelUp(self);
                    if (Lv >= levelInfo.NextTotalExps.Count) { TotalExp = levelInfo.NextTotalExps[levelInfo.NextTotalExps.Count - 1]; }
                    else if (Lv <= 0) { TotalExp = 0; }
                    else { TotalExp = levelInfo.NextTotalExps[Lv - 1]; }
                }
                else
                {
                    TotalExp = 0;
                }
            }
            while (Lv > lv)
            {
                Lv--;
                if (self.Main.InfoSet != null) { self.Main.Polymorph(self, self.Main.InfoSet); }

                var levelInfo = self.Main.GetLevelInfo(self);
                if (levelInfo != null)
                {
                    levelInfo.LevelDown(self);
                    if (Lv >= levelInfo.NextTotalExps.Count) { TotalExp = levelInfo.NextTotalExps[levelInfo.NextTotalExps.Count - 1]; }
                    else if (Lv <= 0) { TotalExp = 0; }
                    else { TotalExp = levelInfo.NextTotalExps[Lv - 1]; }
                }
                else
                {
                    TotalExp = 0;
                }
            }
        }

        public void AddExp(RogueObj self, int deltaExp)
        {
            var levelInfo = self.Main.GetLevelInfo(self);
            if (deltaExp >= 1)
            {
                var totalExp = TotalExp + deltaExp;
                while (Lv < levelInfo.NextTotalExps.Count && totalExp >= levelInfo.NextTotalExps[Lv])
                {
                    SetLv(self, Lv + 1);
                }
                if (Lv < levelInfo.NextTotalExps.Count) { TotalExp = totalExp; }
            }
            else if (deltaExp <= -1)
            {
                var totalExp = TotalExp + deltaExp;
                while (Lv >= 2 && totalExp < levelInfo.NextTotalExps[Lv - 1])
                {
                    SetLv(self, Lv - 1);
                }
                TotalExp = Mathf.Max(totalExp, 0);
            }
        }

        public bool TryAssignParty(RogueObj self, RogueParty party)
        {
            if (Party != null && !UnassignParty(self, Party)) return false;
            if (!party.TryAddMember(self)) return false;

            Party = party;
            return true;
        }

        public bool UnassignParty(RogueObj self, RogueParty party)
        {
            Party = null;
            return party.RemoveMember(self);
        }

        public void Regenerate(RogueObj self)
        {
            if (HP >= StatsEffectedValues.GetMaxHP(self))
            {
                // 最大の場合は自然回復ターン数をリセット
                RegenerationHPPermille = 0;
            }
            else
            {
                var regeneration = StatsEffectedValues.GetRegenerationHPPermille(self);
                RegenerationHPPermille += regeneration;
                if (RegenerationHPPermille >= 1000)
                {
                    SetHP(self, HP + 1);
                    RegenerationHPPermille = 0;
                }
            }

            if (MP >= StatsEffectedValues.GetMaxMP(self))
            {
                // 最大の場合は自然回復ターン数をリセット
                RegenerationMPPermille = 0;
            }
            else
            {
                var regeneration = StatsEffectedValues.GetRegenerationMPPermille(self);
                RegenerationMPPermille += regeneration;
                if (RegenerationMPPermille >= 1000)
                {
                    SetMP(self, MP + 1);
                    RegenerationMPPermille = 0;
                }
            }
        }

        internal bool CanStack(MainStats other)
        {
            if (Party != null || other.Party != null) return false;
            if (TargetObj != null || other.TargetObj != null) return false;

            return HP == other.HP && MP == other.MP && Nutrition == other.Nutrition && Lv == other.Lv && TotalExp == other.TotalExp;
        }

        internal MainStats Clone(RogueObj self, RogueObj clonedSelf)
        {
            var clone = new MainStats();
            clone.HP = HP;
            clone.MP = MP;
            clone.Nutrition = Nutrition;
            clone.Lv = Lv;
            clone.TotalExp = TotalExp;
            clone.TargetObj = TargetObj == self ? clonedSelf : TargetObj;
            clone.Direction = Direction;
            clone.ChargedSpeed = ChargedSpeed;
            clone.RegenerationHPPermille = RegenerationHPPermille;
            clone.RegenerationMPPermille = RegenerationMPPermille;
            return clone;
        }

        internal void ReplaceCloned(RogueObj obj, RogueObj clonedObj)
        {
            if (obj == TargetObj) { TargetObj = clonedObj; }
        }
    }
}
