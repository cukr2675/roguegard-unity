using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class MainStats
    {
        public int Hp { get; private set; }
        public int Mp { get; private set; }
        public int Nutrition { get; private set; }
        public int Lv { get; private set; } = 1;
        public int TotalExp { get; private set; }

        public RogueParty Party { get; private set; }
        public RogueObj TargetObj { get; set; }
        public RogueDirection Direction { get; set; }
        public int ChargedSpeed { get; set; }

        public int RegenerationHpPermille { get; private set; }
        public int RegenerationMpPermille { get; private set; }

        public void Reset(RogueObj self)
        {
            // オーバーヒールをリセットするために MaxHp を設定
            SetHp(self, StatsEffectedValues.GetMaxHp(self));
            SetMp(self, StatsEffectedValues.GetMaxMp(self));
            SetNutrition(self, StatsEffectedValues.GetMaxNutrition(self));
        }

        public void SetHp(RogueObj self, int hp, bool over = false)
        {
            if (over || hp <= Hp)
            {
                // HP 上限を超えて回復できる　減少する場合は最大値を気にする必要がないためそのまま設定
                Hp = hp;
            }
            else
            {
                // HP 上限または上限を突破した HP を超えないようにする
                var maxHp = StatsEffectedValues.GetMaxHp(self);
                maxHp = Mathf.Max(maxHp, Hp);
                Hp = Mathf.Min(hp, maxHp);
            }
        }

        public void SetMp(RogueObj self, int mp, bool over = false)
        {
            if (over || mp <= Mp)
            {
                // MP 上限を超えて回復できる　減少する場合は最大値を気にする必要がないためそのまま設定
                Mp = mp;
            }
            else
            {
                // MP 上限または上限を突破した MP を超えないようにする
                var maxMp = StatsEffectedValues.GetMaxMp(self);
                maxMp = Mathf.Max(maxMp, Mp);
                Mp = Mathf.Min(mp, maxMp);
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
                if (self.Main.InfoSet != null) { self.Main.Polymorph(self, self.Main.InfoSet, +1); }

                var levelInfo = self.Main.GetLevelInfo(self);
                if (levelInfo != null)
                {
                    levelInfo.LevelUp(self);
                    if (Lv >= levelInfo.NextTotalExps.Length) { TotalExp = levelInfo.NextTotalExps[^1]; }
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
                if (self.Main.InfoSet != null) { self.Main.Polymorph(self, self.Main.InfoSet, -1); }

                var levelInfo = self.Main.GetLevelInfo(self);
                if (levelInfo != null)
                {
                    levelInfo.LevelDown(self);
                    if (Lv >= levelInfo.NextTotalExps.Length) { TotalExp = levelInfo.NextTotalExps[^1]; }
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
                while (Lv < levelInfo.NextTotalExps.Length && totalExp >= levelInfo.NextTotalExps[Lv])
                {
                    SetLv(self, Lv + 1);
                }
                if (Lv < levelInfo.NextTotalExps.Length) { TotalExp = totalExp; }
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
            if (Hp >= StatsEffectedValues.GetMaxHp(self))
            {
                // 最大の場合は自然回復ターン数をリセット
                RegenerationHpPermille = 0;
            }
            else
            {
                var regeneration = StatsEffectedValues.GetRegenerationHpPermille(self);
                RegenerationHpPermille += regeneration;
                if (RegenerationHpPermille >= 1000)
                {
                    SetHp(self, Hp + 1);
                    RegenerationHpPermille = 0;
                }
            }

            if (Mp >= StatsEffectedValues.GetMaxMp(self))
            {
                // 最大の場合は自然回復ターン数をリセット
                RegenerationMpPermille = 0;
            }
            else
            {
                var regeneration = StatsEffectedValues.GetRegenerationMpPermille(self);
                RegenerationMpPermille += regeneration;
                if (RegenerationMpPermille >= 1000)
                {
                    SetMp(self, Mp + 1);
                    RegenerationMpPermille = 0;
                }
            }
        }

        internal bool CanStack(MainStats coming)
        {
            if (Party != null || coming.Party != null) return false;
            if (TargetObj != null || coming.TargetObj != null) return false;

            return Hp == coming.Hp && Mp == coming.Mp && Nutrition == coming.Nutrition && Lv == coming.Lv && TotalExp == coming.TotalExp;
        }

        internal MainStats Clone(RogueObj self, RogueObj clonedSelf)
        {
            var clone = new MainStats
            {
                Hp = Hp,
                Mp = Mp,
                Nutrition = Nutrition,
                Lv = Lv,
                TotalExp = TotalExp,
                TargetObj = TargetObj == self ? clonedSelf : TargetObj,
                Direction = Direction,
                ChargedSpeed = ChargedSpeed,
                RegenerationHpPermille = RegenerationHpPermille,
                RegenerationMpPermille = RegenerationMpPermille
            };
            return clone;
        }

        internal void ReplaceObj(RogueObj obj, RogueObj clonedObj)
        {
            if (obj == TargetObj) { TargetObj = clonedObj; }
        }
    }
}
