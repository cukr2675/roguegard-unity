using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using ListingMF;
using Roguegard.Device;

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
            builder.OptionName = $"{targetOption.Name}����";
            builder.OptionCaption = $"{floor}F �� {targetOption.Name} �� {count} �̓�������";
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
                    // ���������ڕW�ɓ��B�����Ƃ��N���A

                    // ��V���󂯎��
                    if (RogueDevice.Primary.Player == self && DungeonQuestInfo.TryGetQuest(self, out var quest))
                    {
                        RogueDevice.Add(DeviceKw.AppendText, self);
                        RogueDevice.Add(DeviceKw.AppendText, "��");
                        RogueDevice.Add(DeviceKw.AppendText, quest);
                        RogueDevice.Add(DeviceKw.AppendText, "���N���A���� ��V���󂯎����");
                        RogueDevice.Primary.AddMenu(rewardsScreen, self, null, new(other: quest));
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
                        RogueDevice.Primary.AddMenu(notifyScreen, self, null, RogueMethodArgument.Identity);
                    }

                    // �ڕW�̊K�w�ւ̈ړ��ɐ��������Ƃ������Ώۂ𐶐�����B
                    var targetEffect = new TargetEffect() { parent = this };
                    for (int i = 0; i < member.Targets[0].Stack; i++)
                    {
                        if (!self.Location.Space.TryGetRandomPositionInRoom(RogueRandom.Primary, out var position))
                        {
                            // �����Ɏ��s�����炻�̂Ԃ�͓��������Ƃ��ăJ�E���g����
                            currentCount++;
                            continue;
                        }

                        var target = member.Targets[0].Option.CreateObj(member.Targets[0], self.Location, position, RogueRandom.Primary);
                        target.TrySetStack(1);
                        target.Main.RogueEffects.AddOpen(target, targetEffect); // ���������J�E���g���邽�߂̃G�t�F�N�g��t�^
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
                RogueMethodAspectState.PassiveChain chain)
            {
                var result = chain.Invoke(keyword, method, self, user, activationDepth, arg);
                if (result && keyword == MainInfoKw.BeDefeated)
                {
                    // �|���ꂽ�Ƃ��N�G�X�g�̌��j�������Z����
                    parent.currentCount++;
                }
                return result;
            }
        }

        private class RewardsScreen : RogueMenuScreen
        {
            private readonly SpeechBoxViewTemplate<MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var self = arg.Self;
                var quest = (DungeonQuest)arg.Arg.Other;
                
                var message = new StringBuilder();
                message.Append(arg.Self.GetName()).Append("��").Append(quest).Append("���N���A�����I");
                if (quest.LootTable.Count >= 1)
                {
                    message.Append(view.VA).Append("���̕�V�Ƃ��āc");
                }
                for (int i = 0; i < quest.LootTable.Count; i++)
                {
                    var loot = WeightedRogueObjGeneratorUtility.CreateObj(quest.LootTable[i], self, RogueRandom.Primary);
                    message.Append(view.VA).AppendLine();
                    if (loot.Main.InfoSet.Equals(RoguegardSettings.MoneyInfoSet))
                    {
                        message.Append(loot.Stack).Append("G�󂯎�����I");
                    }
                    else
                    {
                        message.Append(loot).Append("���󂯎�����I");
                    }
                }

                view.ShowTemplate(message.ToString(), manager, arg)
                    ?
                    .OnCompleted((manager, arg) => manager.Done())

                    .Build();
            }
        }

        private class NotifyScreen : RogueMenuScreen
        {
            private readonly SpeechBoxViewTemplate<MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate($"�ڕW�̊K�ɓ��B���܂���{view.VA}", manager, arg)
                    ?
                    .OnCompleted((manager, arg) => manager.Done())

                    .Build();
            }
        }
    }
}
