using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class SlayingQuestObjective : AbilityIntrinsicOptionScript, IQuestEffectIntrinsicOptionScript
    {
        [SerializeField] private KeywordData _targetFaction = null;

        private static List<IStartingItemOption> options;

        public IntrinsicBuilder GenerateEffect(
            QuestEffectIntrinsicOption parent, DungeonCreationData dungeon, ICharacterCreationDatabase database, IRogueRandom random)
        {
            if (options == null)
            {
                options = new List<IStartingItemOption>();
                var allOptions = RoguegardSettings.CharacterCreationDatabase.StartingItemOptions;
                for (int i = 0; i < allOptions.Count; i++)
                {
                    var optionValue = allOptions[i];
                    if (ReferenceEquals(optionValue.InfoSet.Faction, _targetFaction))
                    {
                        options.Add(optionValue);
                    }
                }
            }

            var targetOption = random.Choice(options);
            var maxFloor = dungeon.Floors[dungeon.Floors.Count - 1].EndLv - 1;
            var count = random.Next(3, 5);
            var floor = random.Next(1, maxFloor);

            var builder = new IntrinsicBuilder();
            builder.Option = parent;
            builder.OptionName = $"{targetOption.Name}討伐";
            builder.OptionCaption = $"{floor}F で {targetOption.Name} を {count} 体討伐する";
            var member = (QuestMember)builder.GetMember(QuestMember.SourceInstance);
            var target = member.Targets.Add();
            target.Option = targetOption;
            target.Stack = count;
            member.TargetFloor = floor;
            return builder;
        }

        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
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
                        RogueDevice.Add(DeviceKw.AppendText, DeviceKw.StartTalk);
                        RogueDevice.Add(DeviceKw.AppendText, self);
                        RogueDevice.Add(DeviceKw.AppendText, "は");
                        RogueDevice.Add(DeviceKw.AppendText, quest);
                        RogueDevice.Add(DeviceKw.AppendText, "をクリアした！");
                        if (quest.LootTable.Count >= 1)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, "\tその報酬として…");
                        }
                        for (int i = 0; i < quest.LootTable.Count; i++)
                        {
                            var loot = WeightedRogueObjGeneratorUtility.CreateObj(quest.LootTable[i], self, RogueRandom.Primary);
                            RogueDevice.Add(DeviceKw.AppendText, "\t\n");
                            if (loot.Main.InfoSet == RoguegardSettings.MoneyInfoSet)
                            {
                                RogueDevice.Add(DeviceKw.AppendText, loot.Stack);
                                RogueDevice.Add(DeviceKw.AppendText, "G受け取った！");
                            }
                            else
                            {
                                RogueDevice.Add(DeviceKw.AppendText, loot);
                                RogueDevice.Add(DeviceKw.AppendText, "を受け取った！");
                            }
                        }
                        RogueDevice.Add(DeviceKw.AppendText, DeviceKw.EndTalk);

                        RogueDevice.Add(DeviceKw.AppendText, self);
                        RogueDevice.Add(DeviceKw.AppendText, "は");
                        RogueDevice.Add(DeviceKw.AppendText, quest);
                        RogueDevice.Add(DeviceKw.AppendText, "をクリアして 報酬を受け取った");
                    }

                    var clearMethod = new GoalDownStairsBeApplied();
                    RogueMethodAspectState.Invoke(MainInfoKw.BeApplied, clearMethod, null, self, activationDepth, RogueMethodArgument.Identity);
                }
                return default;
            }

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveNext next)
            {
                var generate = keyword == MainInfoKw.Locate && self.Location.Space.Tilemap == null;

                var result = next.Invoke(keyword, method, self, user, activationDepth, arg);
                if (!result) return false;

                if (generate && self.Location.Space.Tilemap != null && DungeonInfo.TryGet(self.Location, out _) &&
                    self.Location.Main.Stats.Lv == member.TargetFloor)
                {
                    if (RogueDevice.Primary.Player == self)
                    {
                        RogueDevice.Add(DeviceKw.AppendText, DeviceKw.StartTalk);
                        RogueDevice.Add(DeviceKw.AppendText, "目標の階に到達しました");
                        RogueDevice.Add(DeviceKw.AppendText, DeviceKw.EndTalk);
                    }

                    // 目標の階層への移動に成功したとき討伐対象を生成する。
                    var targetEffect = new TargetEffect() { parent = this };
                    for (int i = 0; i < member.Targets[0].Stack; i++)
                    {
                        var position = self.Location.Space.GetRandomPositionInRoom(RogueRandom.Primary);
                        var target = member.Targets[0].Option.CreateObj(member.Targets[0], self.Location, position, RogueRandom.Primary);
                        target.TrySetStack(1);
                        target.Main.RogueEffects.AddOpen(target, targetEffect); // 討伐数をカウントするためのエフェクトを付与
                    }
                }
                return true;
            }

            public bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => false;
            public IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueEffect ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
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

            public bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => false;
            public IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => this;
            public IRogueEffect ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveNext next)
            {
                var result = next.Invoke(keyword, method, self, user, activationDepth, arg);
                if (result && keyword == MainInfoKw.BeDefeated)
                {
                    // 倒されたときクエストの撃破数を加算する
                    parent.currentCount++;
                }
                return result;
            }
        }
    }
}
