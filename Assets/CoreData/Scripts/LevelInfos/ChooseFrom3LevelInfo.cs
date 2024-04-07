using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private static readonly LevelUpMenu levelUpMenu = new LevelUpMenu();

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
            if (MainCharacterWorkUtility.VisibleAt(self.Location, self.Position))
            {
                RogueDevice.Add(DeviceKw.AppendText, self);
                RogueDevice.Add(DeviceKw.AppendText, "はレベルが上がった！\n");
            }

            var selfIsPlayerPartyMember = RogueDevice.Primary.Player.Main.Stats.Party.Members.Contains(self);
            if (selfIsPlayerPartyMember)
            {
                RogueDevice.Add(DeviceKw.EnqueueSEAndWait, StdKw.LevelUp);
                RogueDevice.Add(DeviceKw.AppendText, DeviceKw.StartTalk);
                RogueDevice.Add(DeviceKw.AppendText, self);
                RogueDevice.Add(DeviceKw.AppendText, "はレベルが上がった！\t\n");
            }

            if (self == RogueDevice.Primary.Player)
            {
                if (selfIsPlayerPartyMember)
                {
                    RogueDevice.Add(DeviceKw.AppendText, DeviceKw.EndTalk);
                }
                RogueDevice.Primary.AddMenu(levelUpMenu, self, null, RogueMethodArgument.Identity);
                RogueDevice.Add(DeviceKw.AppendText, DeviceKw.HorizontalRule);
            }
            else
            {
                // プレイヤーでない場合、HP・MP・最大重量をランダムに選択して上げる。
                switch (RogueRandom.Primary.Next(0, 3))
                {
                    case 0:
                        maxHP += 5;
                        self.Main.Stats.SetHP(self, self.Main.Stats.HP + 5, true);
                        if (selfIsPlayerPartyMember)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, StatsKw.MaxHP);
                            RogueDevice.Add(DeviceKw.AppendText, "が5上がった\t\n");
                        }
                        break;
                    case 1:
                        maxMP += 5;
                        self.Main.Stats.SetMP(self, self.Main.Stats.MP + 5, true);
                        if (selfIsPlayerPartyMember)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, StatsKw.MaxMP);
                            RogueDevice.Add(DeviceKw.AppendText, "が5上がった\t\n");
                        }
                        break;
                    case 2:
                        loadCapacity += 2;
                        if (selfIsPlayerPartyMember)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, StatsKw.LoadCapacity);
                            RogueDevice.Add(DeviceKw.AppendText, "が2上がった\t\n");
                        }
                        break;
                }
                if (selfIsPlayerPartyMember)
                {
                    if (self.Main.Stats.Lv == 10 || self.Main.Stats.Lv == 20)
                    {
                        RogueDevice.Add(DeviceKw.AppendText, StatsKw.ATK);
                        RogueDevice.Add(DeviceKw.AppendText, "が1上がった\t\n");
                    }
                    RogueDevice.Add(DeviceKw.AppendText, DeviceKw.EndTalk);
                }
            }
        }

        public override void LevelDown(RogueObj self)
        {
            if (MainCharacterWorkUtility.VisibleAt(self.Location, self.Position))
            {
                RogueDevice.Add(DeviceKw.EnqueueSE, StdKw.LevelDown);
                RogueDevice.Add(DeviceKw.AppendText, self);
                RogueDevice.Add(DeviceKw.AppendText, "はレベルが下がった！\n");
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

        void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
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

        private class LevelUpMenu : IModelsMenu
        {
            private static IModelsMenuChoice[] choices = new IModelsMenuChoice[] { new HPPlusChoice(), new MPPlusChoice(), new WeightLiftingChoice() };
            private static readonly SubmitMenu submitMenu = new SubmitMenu();

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.Get(DeviceKw.MenuScroll).OpenView(ChoiceListPresenter.Instance, choices, root, self, user, arg);
            }

            private class HPPlusChoice : BaseModelsMenuChoice
            {
                public override string Name => "最大HP +5";

                public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var commandArg = new RogueMethodArgument(count: 0);
                    root.OpenMenuAsDialog(submitMenu, self, user, commandArg);
                }
            }

            private class MPPlusChoice : BaseModelsMenuChoice
            {
                public override string Name => "最大MP +5";

                public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var commandArg = new RogueMethodArgument(count: 1);
                    root.OpenMenuAsDialog(submitMenu, self, user, commandArg);
                }
            }

            private class WeightLiftingChoice : BaseModelsMenuChoice
            {
                public override string Name => "最大重量 +2";

                public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var commandArg = new RogueMethodArgument(count: 2);
                    root.OpenMenuAsDialog(submitMenu, self, user, commandArg);
                }
            }
        }

        private class SubmitMenu : IModelsMenu
        {
            private static IModelsMenuChoice[] choices = new IModelsMenuChoice[] { new SubmitChoice(), ExitModelsMenuChoice.Instance };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.Get(DeviceKw.MenuCommand).OpenView(ChoiceListPresenter.Instance, choices, root, self, user, arg);
            }

            private class SubmitChoice : BaseModelsMenuChoice
            {
                public override string Name => "決定";

                public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.Done();
                    root.AddInt(DeviceKw.StartTalk, 0);
                    var levelInfo = (ChooseFrom3LevelInfo)self.Main.GetLevelInfo(self);
                    switch (arg.Count)
                    {
                        case 0:
                            levelInfo.maxHP += 5;
                            self.Main.Stats.SetHP(self, self.Main.Stats.HP + 5, true);
                            root.AddObject(DeviceKw.AppendText, StatsKw.MaxHP.Name);
                            root.AddObject(DeviceKw.AppendText, "が5上がった\t\n");
                            break;
                        case 1:
                            levelInfo.maxMP += 5;
                            self.Main.Stats.SetMP(self, self.Main.Stats.MP + 5, true);
                            root.AddObject(DeviceKw.AppendText, StatsKw.MaxMP.Name);
                            root.AddObject(DeviceKw.AppendText, "が5上がった\t\n");
                            break;
                        case 2:
                            levelInfo.loadCapacity += 2;
                            root.AddObject(DeviceKw.AppendText, StatsKw.LoadCapacity.Name);
                            root.AddObject(DeviceKw.AppendText, "が2上がった\t\n");
                            break;
                    }

                    if (self.Main.Stats.Lv == 10 || self.Main.Stats.Lv == 20)
                    {
                        root.AddObject(DeviceKw.AppendText, StatsKw.ATK.Name);
                        root.AddObject(DeviceKw.AppendText, "が1上がった\t\n");
                    }
                }
            }
        }
    }
}
