using Lysionium;
using Roguegard.Device;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class SlayingQuestObjective : AbilityIntrinsicScript, IQuestEffectIntrinsicScript
    {
        [SerializeField] private KeywordAsset _targetFaction = null;

        private static List<IStartingItemOption> options;

        public Intrinsic GenerateEffect(
            QuestEffectIntrinsicOptionAsset parent, DungeonCreationDataAsset dungeon, ICharacterCreationDatabase database, IRogueRandom random)
        {
            if (options == null)
            {
                options = new List<IStartingItemOption>();
                foreach (var optionValue in RoguegardSettings.CharacterCreationDatabase.StartingItemOptions)
                {
                    if (ReferenceEquals(optionValue.InfoSet.Faction, _targetFaction))
                    {
                        options.Add(optionValue);
                    }
                }
            }

            var targetOption = random.Choice(options);
            var maxFloor = dungeon.Floors[^1].EndLv - 1;
            var count = random.Next(3, 5);
            var floor = random.Next(1, maxFloor);

            var intrinsic = new Intrinsic
            {
                Option = parent,
                CustomName = $"{targetOption.Name}討伐",
                CustomCaption = $"{floor}F で {targetOption.Name} を {count} 体討伐する"
            };
            var member = (QuestMember)intrinsic.GetMember(QuestMember.SourceInstance);
            var target = member.Targets.Add();
            target.Option = targetOption;
            target.Stack = count;
            member.TargetFloor = floor;
            return intrinsic;
        }

        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOptionAsset parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            var member = (QuestMember)QuestMember.GetMember(intrinsic);
            return new SortedIntrinsic(lv) { member = member };
        }

        private class SortedIntrinsic : AbilitySortedIntrinsic
        {
            public QuestMember member;

            public SortedIntrinsic(int lv) : base(lv) { }

            protected override void LevelUpToLv(RogueObj self, MainInfoSetType infoSetType)
            {
                if (self.Main.RogueEffects.TryGetEffect<Effect>(out _)) return;

                self.Main.RogueEffects.AddOpen(self, new Effect() { member = member });
            }

            protected override void LevelDownFromLv(RogueObj self, MainInfoSetType infoSetType)
            {
                if (self.Main.RogueEffects.TryGetEffect<Effect>(out var effect))
                {
                    RogueEffectUtility.RemoveClose(self, effect);
                }
            }
        }

        [Objforming.Formable]
        private class Effect : IRogueEffect, IRogueObjUpdater, IRogueMethodPassiveAspect
        {
            public QuestMember member;
            public int currentCount;

            [System.NonSerialized] private readonly RewardsScreen rewardsScreen = new();
            [System.NonSerialized] private readonly NotifyScreen notifyScreen = new();

            float IRogueObjUpdater.Order => -100f;
            float IRogueMethodPassiveAspect.Order => 0f;

            public void Open(RogueObj self)
            {
                RogueEffectUtility.AddFromRogueEffect(self, this);
            }

            RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                if (currentCount >= member.Targets[0].Stack)
                {
                    // 討伐数が目標に到達したときクリア

                    // 報酬を受け取る
                    if (RogueDevice.Primary.Player == self && DungeonQuestInfo.TryGetQuest(self, out var quest))
                    {
                        RogueDevice.Add(DeviceKw.AppendText, self);
                        RogueDevice.Add(DeviceKw.AppendText, "は");
                        RogueDevice.Add(DeviceKw.AppendText, quest);
                        RogueDevice.Add(DeviceKw.AppendText, "をクリアして 報酬を受け取った");
                        RogueDevice.Primary.AddScreen(rewardsScreen, self, null, new(other: quest));
                    }

                    var clearMethod = new GoalDownStairsBeApplied();
                    RogueMethodAspectState.Invoke(MainInfoKw.BeApplied, clearMethod, null, self, activationDepth, RogueMethodArgument.Identity);
                }
                return default;
            }

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveChain chain)
            {
                var generate = keyword == MainInfoKw.Locate && self.Location.Space.Tilemap == null;

                var result = chain.Invoke(keyword, method, self, user, activationDepth, arg);
                if (!result) return false;

                if (generate && self.Location.Space.Tilemap != null && DungeonInfo.TryGet(self.Location, out _) &&
                    self.Location.Main.Stats.Lv == member.TargetFloor)
                {
                    if (RogueDevice.Primary.Player == self)
                    {
                        RogueDevice.Primary.AddScreen(notifyScreen, self, null, RogueMethodArgument.Identity);
                    }

                    // 目標の階層への移動に成功したとき討伐対象を生成する。
                    var targetEffect = new TargetEffect() { parent = this };
                    for (int i = 0; i < member.Targets[0].Stack; i++)
                    {
                        if (!self.Location.Space.TryGetRandomPositionInRoom(RogueRandom.Primary, out var position))
                        {
                            // 生成に失敗したらそのぶんは討伐扱いとしてカウントする
                            currentCount++;
                            continue;
                        }

                        var target = member.Targets[0].Option.CreateObj(member.Targets[0], self.Location, position, RogueRandom.Primary);
                        target.TrySetStack(1);
                        target.Main.RogueEffects.AddOpen(target, targetEffect); // 討伐数をカウントするためのエフェクトを付与
                    }
                }
                return true;
            }

            public bool CanStack(RogueObj self, RogueObj comingObj, IRogueEffect coming) => false;
            public IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueEffect ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;
        }

        [Objforming.Formable]
        private class TargetEffect : IRogueEffect, IRogueMethodPassiveAspect
        {
            public Effect parent;

            float IRogueMethodPassiveAspect.Order => 0f;

            public void Open(RogueObj self)
            {
                RogueEffectUtility.AddFromRogueEffect(self, this);
            }

            public bool CanStack(RogueObj self, RogueObj comingObj, IRogueEffect coming) => false;
            public IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => this;
            public IRogueEffect ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveChain chain)
            {
                var result = chain.Invoke(keyword, method, self, user, activationDepth, arg);
                if (result && keyword == MainInfoKw.BeDefeated)
                {
                    // 倒されたときクエストの撃破数を加算する
                    parent.currentCount++;
                }
                return result;
            }
        }

        private class RewardsScreen : RogueListuiScreen
        {
            private readonly SpeechBoxViewData<MMgr> view = new()
            {
            };

            public RewardsScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    var self = Arg.Self;
                    var quest = (DungeonQuest)Arg.Arg.Other;

                    var message = new StringBuilder();
                    message.Append(Arg.Self.GetName()).Append("は").Append(quest).Append("をクリアした！");
                    if (quest.LootTable.Length >= 1)
                    {
                        message.Append("{v}その報酬として…");
                    }
                    foreach (var lootTableRow in quest.LootTable)
                    {
                        var loot = WeightedRogueObjGeneratorUtility.CreateObj(lootTableRow, self, RogueRandom.Primary);
                        message.Append("{v}").AppendLine();
                        if (loot.Main.InfoSet.Equals(RoguegardSettings.MoneyInfoSet))
                        {
                            message.Append(loot.Stack).Append("G受け取った！");
                        }
                        else
                        {
                            message.Append(loot).Append("を受け取った！");
                        }
                    }

                    view.Show(message.ToString(), manager)
                    ?
                    .OnCompleted(m => m.Done())

                    .Build();
                };
            }
        }

        private class NotifyScreen : RogueListuiScreen
        {
            private readonly SpeechBoxViewData<MMgr> view = new()
            {
            };

            public NotifyScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show($"目標の階に到達しました{{v}}", manager)
                    ?
                    .OnCompleted(m => m.Done())

                    .Build();
                };
            }
        }
    }
}
