using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    [Objforming.Formable]
    public class StandardLevelInfo : BaseLevelInfo, IValueEffect, ILevelInfoInitializer
    {
        public override Spanning<int> NextTotalExps => _nextTotalExps;
        private static readonly int[] _nextTotalExps;

        float IValueEffect.Order => -100f;

        private static readonly Dictionary<int, StandardLevelInfo> instances = new Dictionary<int, StandardLevelInfo>();

        private readonly int initialLv;

        static StandardLevelInfo()
        {
            var nextTotalExps = new int[30];
            nextTotalExps[0] = 0;
            for (int i = 1; i < nextTotalExps.Length; i++)
            {
                nextTotalExps[i] = nextTotalExps[i - 1] + i * 10;
            }
            _nextTotalExps = nextTotalExps;
        }

        [Objforming.CreateInstance]
        private StandardLevelInfo() { }

        private StandardLevelInfo(int initialLv)
        {
            // 進化・変化で IMainInfoSet.InitialLv が変化する可能性があるため、設定時のレベルを記憶する。
            this.initialLv = initialLv;
        }

        public static void InitializeLv(RogueObj obj, int initialLv)
        {
            if (!instances.TryGetValue(initialLv, out var info))
            {
                info = new StandardLevelInfo(initialLv);
                instances.Add(initialLv, info);
            }

            Initialize(obj, info, initialLv);
        }

        void ILevelInfoInitializer.InitializeLv(RogueObj obj, int initialLv)
        {
            InitializeLv(obj, initialLv);
        }

        public override void LevelUp(RogueObj self)
        {
            //if (MainCharacterWorkUtility.VisibleAt(self.Location, self.Position))
            //{
            //    RogueDevice.Add(DeviceKw.AppendText, self);
            //    RogueDevice.Add(DeviceKw.AppendText, "はレベルが上がった！\n");
            //}

            var selfIsPlayerPartyMember = RogueDevice.Primary.Player.Main.Stats.Party.Members.Contains(self);
            if (selfIsPlayerPartyMember)
            {
                RogueDevice.Add(DeviceKw.EnqueueSEAndWait, StdKw.LevelUp);
                RogueDevice.Add(DeviceKw.AppendText, DeviceKw.StartTalk);
                RogueDevice.Add(DeviceKw.AppendText, self);
                RogueDevice.Add(DeviceKw.AppendText, "はレベルが上がった！\t\n");
            }

            // 偶数Lvに上がった時HPを、奇数Lvに上がった時MPを上げる。
            if (self.Main.Stats.Lv % 2 == 0)
            {
                self.Main.Stats.SetHP(self, self.Main.Stats.HP + 2, true);
                if (selfIsPlayerPartyMember)
                {
                    RogueDevice.Add(DeviceKw.AppendText, StatsKw.MaxHP);
                    RogueDevice.Add(DeviceKw.AppendText, "が2上がった\t\n");
                }
            }
            else
            {
                self.Main.Stats.SetMP(self, self.Main.Stats.MP + 2, true);
                if (selfIsPlayerPartyMember)
                {
                    RogueDevice.Add(DeviceKw.AppendText, StatsKw.MaxMP);
                    RogueDevice.Add(DeviceKw.AppendText, "が2上がった\t\n");
                }
            }

            if (self.Main.Stats.Lv == 10 || self.Main.Stats.Lv == 20)
            {
                if (selfIsPlayerPartyMember)
                {
                    RogueDevice.Add(DeviceKw.AppendText, StatsKw.ATK);
                    RogueDevice.Add(DeviceKw.AppendText, "が1上がった\t\n");
                }
            }

            if (selfIsPlayerPartyMember)
            {
                RogueDevice.Add(DeviceKw.AppendText, DeviceKw.EndTalk);
            }
        }

        public override void LevelDown(RogueObj self)
        {
            if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;
                handler.EnqueueSE(StdKw.LevelDown);
                handler.AppendText(self).AppendText("はレベルが下がった！\n");
            }

            if (self.Main.Stats.Lv < initialLv)
            {
                // 初期レベルより下がっても能力値は変わらない。
                return;
            }

            // 奇数Lvに下がった時HPを、偶数Lvに下がった時MPを下げる。
            if (self.Main.Stats.Lv % 2 == 1)
            {
                self.Main.Stats.SetHP(self, self.Main.Stats.HP - 2);
                if (self.Main.Stats.HP <= 0) { self.Main.Stats.SetHP(self, 1); } // レベルダウンによって倒れることはない
            }
            else
            {
                self.Main.Stats.SetMP(self, self.Main.Stats.MP - 2);
            }
        }

        void IValueEffect.AffectValue(IKeyword keyword, EffectableValue value, RogueObj self)
        {
            if (keyword == StatsKw.MaxHP)
            {
                var rank = Mathf.Max(self.Main.Stats.Lv - initialLv + 1, 0) / 2;
                value.MainValue += rank * 2;
            }
            else if (keyword == StatsKw.MaxMP)
            {
                var rank = Mathf.Max(self.Main.Stats.Lv - initialLv, 0) / 2;
                value.MainValue += rank * 2;
            }
            else if (keyword == StatsKw.ATK)
            {
                // 10Lv ごとに基礎攻撃力+1（2まで）
                var rank = Mathf.Max(Mathf.Clamp(self.Main.Stats.Lv / 10, 0, 2) - Mathf.Clamp(initialLv / 10, 0, 2), 0);
                value.BaseMainValue += rank;
                value.MainValue += rank;
            }
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => Equals(other);
        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => this;

        public override bool Equals(object obj)
        {
            return obj is StandardLevelInfo other && other.initialLv == initialLv;
        }
        public override int GetHashCode()
        {
            return initialLv.GetHashCode();
        }
    }
}
