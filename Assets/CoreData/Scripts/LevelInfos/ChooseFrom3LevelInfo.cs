using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    /// <summary>
    /// HP・MP・最大重量から一つ選ぶレベルアップボーナス。
    /// </summary>
    [Objforming.Formable]
    public class ChooseFrom3LevelInfo : BaseLevelInfo, IValueEffect, ILevelInfoInitializer
    {
        public override Spanning<int> NextTotalExps => _nextTotalExps;
        private static readonly int[] _nextTotalExps;

        float IValueEffect.Order => -100f;

        private int maxHP;
        private int maxMP;
        private int loadCapacity;

        private static readonly LevelUpBonusScreen levelUpBonusScreen = new();
        private static readonly ResultScreen resultScreen = new();

        static ChooseFrom3LevelInfo()
        {
            var nextTotalExps = new int[30];
            nextTotalExps[0] = 0;
            for (int i = 1; i < nextTotalExps.Length; i++)
            {
                nextTotalExps[i] = nextTotalExps[i - 1] + i * 10;
            }
            _nextTotalExps = nextTotalExps;
        }

        private ChooseFrom3LevelInfo() { }

        public static void InitializeLv(RogueObj obj, int initialLv)
        {
            var levelInfo = new ChooseFrom3LevelInfo();
            Initialize(obj, levelInfo, initialLv);
        }

        void ILevelInfoInitializer.InitializeLv(RogueObj obj, int initialLv)
        {
            InitializeLv(obj, initialLv);
        }

        public override void LevelUp(RogueObj self)
        {
            if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(self).AppendText("はレベルが上がった！\n");
            }

            var selfIsPlayerPartyMember = RogueDevice.Primary.Player.Main.Stats.Party.Members.Contains(self);
            if (selfIsPlayerPartyMember)
            {
                RogueDevice.Add(DeviceKw.EnqueueSEAndWait, StdKw.LevelUp);
            }

            if (self == RogueDevice.Primary.Player)
            {
                RogueDevice.Primary.AddMenu(levelUpBonusScreen, self, null, RogueMethodArgument.Identity);
                RogueDevice.Add(DeviceKw.AppendText, DeviceKw.HorizontalRule);
            }
            else
            {
                if (selfIsPlayerPartyMember)
                {
                    RogueDevice.Primary.AddMenu(resultScreen, self, null, RogueMethodArgument.Identity);
                }
                resultScreen.message = "";

                // プレイヤーでない場合、HP・MP・最大重量をランダムに選択して上げる。
                switch (RogueRandom.Primary.Next(0, 3))
                {
                    case 0:
                        maxHP += 5;
                        self.Main.Stats.SetHP(self, self.Main.Stats.HP + 5, true);
                        if (selfIsPlayerPartyMember)
                        {
                            resultScreen.message = $"{StatsKw.MaxHP.Name}が5上がった\n";
                        }
                        break;
                    case 1:
                        maxMP += 5;
                        self.Main.Stats.SetMP(self, self.Main.Stats.MP + 5, true);
                        if (selfIsPlayerPartyMember)
                        {
                            resultScreen.message = $"{StatsKw.MaxMP.Name}が5上がった\n";
                        }
                        break;
                    case 2:
                        loadCapacity += 2;
                        if (selfIsPlayerPartyMember)
                        {
                            resultScreen.message = $"{StatsKw.LoadCapacity.Name}が2上がった\n";
                        }
                        break;
                }
                if (selfIsPlayerPartyMember)
                {
                    if (self.Main.Stats.Lv == 10 || self.Main.Stats.Lv == 20)
                    {
                        resultScreen.message += $"{StatsKw.ATK.Name}が1上がった\n";
                    }
                    RogueDevice.Add(DeviceKw.AppendText, DeviceKw.EndTalk);
                }
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

            // 上がった能力をランダムで選んで下げる。
            var count = 0;
            count += maxHP >= 1 ? 1 : 0;
            count += maxMP >= 1 ? 1 : 0;
            count += loadCapacity >= 1 ? 1 : 0;
            var random = RogueRandom.Primary.Next(0, count);
            if (maxHP >= 1)
            {
                if (random == 0)
                {
                    maxHP -= 5;
                    self.Main.Stats.SetHP(self, self.Main.Stats.HP - 5);
                    if (self.Main.Stats.HP <= 0) { self.Main.Stats.SetHP(self, 1); } // レベルダウンによって倒れることはない
                    return;
                }
                random--;
            }
            if (maxMP >= 1)
            {
                if (random == 0)
                {
                    maxMP -= 5;
                    self.Main.Stats.SetMP(self, self.Main.Stats.MP - 5);
                    return;
                }
                random--;
            }
            if (loadCapacity >= 1)
            {
                if (random == 0)
                {
                    loadCapacity -= 2;
                    return;
                }
            }
            Debug.LogError("下げる能力が見つかりません。");
        }

        void IValueEffect.AffectValue(IKeyword keyword, EffectableValue value, RogueObj self)
        {
            if (keyword == StatsKw.MaxHP)
            {
                value.MainValue += maxHP;
            }
            else if (keyword == StatsKw.MaxMP)
            {
                value.MainValue += maxMP;
            }
            else if (keyword == StatsKw.LoadCapacity)
            {
                value.MainValue += loadCapacity;
            }
            else if (keyword == StatsKw.ATK)
            {
                // 10Lv ごとに基礎攻撃力+1（2まで）
                var rank = Mathf.Min(self.Main.Stats.Lv / 10, 2);
                value.BaseMainValue += rank;
                value.MainValue += rank;
            }
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return Equals(other);
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            var clone = new ChooseFrom3LevelInfo();
            clone.maxHP = maxHP;
            clone.maxMP = maxMP;
            clone.loadCapacity = loadCapacity;
            return clone;
        }

        private class LevelUpBonusScreen : RogueMenuScreen
        {
            private readonly SpeechBoxViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                view.ShowTemplate(arg.Self.GetName() + "はレベルが上がった！<link=\"VerticalArrow\"></link>", manager, arg)
                    ?
                    .OnCompleted(new SelectScreen())

                    .Build();
            }

            private class SelectScreen : RogueMenuScreen
            {
                private readonly MainMenuViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
                {
                    PrimaryCommandSubViewName = StandardSubViewTable.ScrollName,
                };

                public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
                {
                    view.ShowTemplate(manager, arg)
                        ?
                        .VariableOnce(out var nextScreen, new ConfirmScreen())

                        .Option("最大HP +5", (manager, arg) =>
                        {
                            manager.PushMenuScreen(nextScreen, arg.Self, arg.User, count: 0);
                        })

                        .Option("最大MP +5", (manager, arg) =>
                        {
                            manager.PushMenuScreen(nextScreen, arg.Self, arg.User, count: 1);
                        })

                        .Option("最大重量 +2", (manager, arg) =>
                        {
                            manager.PushMenuScreen(nextScreen, arg.Self, arg.User, count: 2);
                        })

                        .Build();
                }
            }

            private class ConfirmScreen : RogueMenuScreen
            {
                private readonly ResultScreen nextScreen = new();

                private readonly MainMenuViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
                {
                    PrimaryCommandSubViewName = StandardSubViewTable.SecondaryCommandName,
                };

                public override bool IsIncremental => true;

                public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
                {
                    view.ShowTemplate(manager, arg)
                        ?
                        .Option("決定", (manager, arg) =>
                        {
                            var self = arg.Self;
                            var levelInfo = (ChooseFrom3LevelInfo)self.Main.GetLevelInfo(self);
                            if (self.Main.Stats.Lv == 10 || self.Main.Stats.Lv == 20)
                            {
                                nextScreen.message = $"{StatsKw.ATK.Name}が1上がった<link=\"HorizontalArrow\"></link>\n";
                            }
                            else
                            {
                                nextScreen.message = "";
                            }
                            switch (arg.Arg.Count)
                            {
                                case 0:
                                    levelInfo.maxHP += 5;
                                    self.Main.Stats.SetHP(self, self.Main.Stats.HP + 5, true);
                                    nextScreen.message += $"{StatsKw.MaxHP.Name}が5上がった";
                                    manager.PushMenuScreen(nextScreen);
                                    break;
                                case 1:
                                    levelInfo.maxMP += 5;
                                    self.Main.Stats.SetMP(self, self.Main.Stats.MP + 5, true);
                                    nextScreen.message += $"{StatsKw.MaxMP.Name}が5上がった";
                                    manager.PushMenuScreen(nextScreen);
                                    break;
                                case 2:
                                    levelInfo.loadCapacity += 2;
                                    nextScreen.message += $"{StatsKw.LoadCapacity.Name}が2上がった";
                                    manager.PushMenuScreen(nextScreen);
                                    break;
                            }
                        })

                        .Back()

                        .Build();
                }

                public override void CloseScreen(RogueMenuManager manager, bool back)
                {
                    view.HideTemplate(manager, back);
                }
            }
        }

        private class ResultScreen : RogueMenuScreen
        {
            public string message;

            private readonly SpeechBoxViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                view.ShowTemplate(message + view.VA, manager, arg)
                    ?
                    .OnCompleted((manager, arg) => manager.Done())

                    .Build();
            }
        }
    }
}
