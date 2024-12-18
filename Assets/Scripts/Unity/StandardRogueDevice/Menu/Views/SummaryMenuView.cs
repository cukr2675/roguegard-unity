using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using UnityEngine.UI;
using ListingMF;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;
using TMPro;
using Roguegard.CharacterCreation;

namespace RoguegardUnity
{
    public class SummaryMenuView : ElementsSubView
    {
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private TMP_Text _topText = null;
        [SerializeField] private TMP_Text _textLeftL = null;
        [SerializeField] private TMP_Text _textLeftR = null;
        [SerializeField] private TMP_Text _textRightL = null;
        [SerializeField] private TMP_Text _textRightR = null;

        private StringBuilder topBuilder;
        private StringBuilder leftLBuilder;
        private StringBuilder leftRBuilder;
        private StringBuilder rightLBuilder;
        private StringBuilder rightRBuilder;
        private RogueNameBuilder nameBuilder;

        private static readonly ISelectOption[] backSelectOption = new[]
        {
            BackSelectOption.Instance
        };

        private static readonly ISelectOption[] submitSelectOption = new[]
        {
            SelectOption.Create<MMgr, MArg>("OK", (manager, arg) =>
            {
                var dungeon = DungeonInfo.GetLargestDungeon(arg.Arg.TargetObj);

                if (!default(IActiveRogueMethodCaller).LocateSavePoint(arg.Self, null, 0f, RogueWorldSavePointInfo.Instance, true)) return;

                if (dungeon?.Stack >= 1)
                {
                    dungeon.TrySetStack(0);
                }
                var memberInfo = LobbyMemberList.GetMemberInfo(arg.Self);
                memberInfo.SavePoint = RogueWorldSavePointInfo.Instance;

                RogueDevice.Add(DeviceKw.AutoSave, 0);
            })
        };

        private static readonly ISelectOption[] startQuestSelectOption = new[]
        {
            SelectOption.Create<MMgr, MArg>("出発", (manager, arg) =>
            {
                var quest = (DungeonQuest)arg.Arg.Other;
                quest.Start(arg.Self);

                // BackToLobby で階層表示させるため、ここでは終了させない。
                manager.Done();
            })
        };

        public void Initialize()
        {
            topBuilder = new StringBuilder();
            leftLBuilder = new StringBuilder();
            leftRBuilder = new StringBuilder();
            rightLBuilder = new StringBuilder();
            rightRBuilder = new StringBuilder();
            nameBuilder = new RogueNameBuilder();
        }

        public override void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider)
        {
            SetArg(manager, arg);
            SetStatusCode(0);
        }

        public void SetObj(MMgr manager, object obj)
        {
            if (obj is RogueObj rogueObj)
            {
                SetObj(rogueObj, null);
            }
            else if (obj is IRogueTile tile)
            {
                SetTile(tile);
            }

            IElementsSubViewStateProvider stateProvider = null;
            manager
                .GetSubView(StandardSubViewTable.BackAnchorName)
                .Show(backSelectOption, SelectOptionHandler.Instance, manager, Arg, ref stateProvider);
        }

        private void SetObj(RogueObj obj, RogueObj resultDungeon)
        {
            if (resultDungeon != null)
            {
                // ダンジョンクリア表示
                SetResultTop(obj, resultDungeon);
            }
            else
            {
                _topText.text = null;
            }

            SetHeader(obj);
            AppendSkills(obj);
            //AppendLoots(obj);

            _textLeftL.SetText(leftLBuilder);
            _textLeftR.SetText(leftRBuilder);
            _textRightL.SetText(rightLBuilder);
            _textRightR.SetText(rightRBuilder);
        }

        public void SetResult(MMgr manager, RogueObj player, RogueObj dungeon)
        {
            SetArg(manager, new MArg.Builder(self: player, arg: new(targetObj: dungeon)).ReadOnly);

            SetObj(player, dungeon);

            IElementsSubViewStateProvider stateProvider = null;
            manager
                .GetSubView(StandardSubViewTable.ForwardAnchorName)
                .Show(submitSelectOption, SelectOptionHandler.Instance, manager, Arg, ref stateProvider);
        }

        public void SetGameOver(MMgr manager, RogueObj player, RogueObj dungeon)
        {
            SetArg(manager, new MArg.Builder(self: player, arg: new(targetObj: dungeon)).ReadOnly);

            SetObj(player, null);

            IElementsSubViewStateProvider stateProvider = null;
            manager
                .GetSubView(StandardSubViewTable.BackAnchorName)
                .Show(backSelectOption, SelectOptionHandler.Instance, manager, Arg, ref stateProvider);
            manager
                .GetSubView(StandardSubViewTable.ForwardAnchorName)
                .Show(submitSelectOption, SelectOptionHandler.Instance, manager, Arg, ref stateProvider);
        }

        public void SetQuest(MMgr manager, RogueObj player, DungeonQuest quest, bool showSubmitButton)
        {
            SetArg(manager, new MArg.Builder(self: player, arg: new(other: quest)).ReadOnly);

            _topText.text = null;
            leftLBuilder.Clear();
            leftRBuilder.Clear();
            rightLBuilder.Clear();
            rightRBuilder.Clear();

            leftLBuilder.AppendLine(quest.Name);
            leftLBuilder.AppendLine("場所：");
            leftLBuilder.AppendLine(quest.Dungeon.DescriptionName);
            leftLBuilder.AppendLine("達成条件：");
            for (int i = 0; i < quest.Objectives.Count; i++)
            {
                leftLBuilder.AppendLine(quest.Objectives[i].Caption);
            }
            leftLBuilder.AppendLine("環境：");
            for (int i = 0; i < quest.Environments.Count; i++)
            {
                leftLBuilder.AppendLine(quest.Environments[i].Name);
            }
            leftLBuilder.AppendLine("報酬：");
            var any = false;
            var plusAlpha = false;
            for (int i = 0; i < quest.LootTable.Count; i++)
            {
                var loots = quest.LootTable[i].Spanning;
                if (loots.Count == 1)
                {
                    var loot = loots[0];
                    leftLBuilder.Append(loot.InfoSet.Name);
                    if (loot.Stack != 1)
                    {
                        leftLBuilder.Append(" x");
                        leftLBuilder.Append(loot.Stack);
                    }
                    leftLBuilder.AppendLine();
                    any = true;
                }
                else if (loots.Count >= 2)
                {
                    plusAlpha = true;
                    any = true;
                }
            }
            if (plusAlpha)
            {
                leftLBuilder.AppendLine("？？？");
            }
            if (!any)
            {
                leftLBuilder.AppendLine("なし");
            }

            _textLeftL.SetText(leftLBuilder);
            _textLeftR.SetText(leftRBuilder);
            _textRightL.SetText(rightLBuilder);
            _textRightR.SetText(rightRBuilder);

            IElementsSubViewStateProvider stateProvider = null;
            manager
                .GetSubView(StandardSubViewTable.BackAnchorName)
                .Show(backSelectOption, SelectOptionHandler.Instance, manager, Arg, ref stateProvider);
            if (showSubmitButton)
            {
                manager
                    .GetSubView(StandardSubViewTable.ForwardAnchorName)
                    .Show(startQuestSelectOption, SelectOptionHandler.Instance, manager, Arg, ref stateProvider);
            }
        }

        private void SetHeader(RogueObj obj)
        {
            leftLBuilder.Clear();
            leftRBuilder.Clear();
            rightLBuilder.Clear();
            rightRBuilder.Clear();

            // 名前, Lv
            obj.GetName(nameBuilder);
            var gender = GetGenderText(obj);
            var stats = obj.Main.Stats;
            leftLBuilder.AppendLine(nameBuilder.ToString());
            leftRBuilder.AppendLine(gender);
            rightLBuilder.AppendLine("Lv：");
            rightRBuilder.Append(stats.Lv).AppendLine();

            // Exp, 必要Exp
            var levelInfo = obj.Main.GetLevelInfo(obj);
            leftLBuilder.AppendLine("　Exp：");
            leftRBuilder.Append(stats.TotalExp).AppendLine();
            rightLBuilder.AppendLine("次のLvまで：");
            rightRBuilder.Append(levelInfo.NextTotalExps[stats.Lv] - stats.TotalExp).AppendLine();

            leftLBuilder.AppendLine("　HP：");
            leftRBuilder.Append(stats.HP).Append(" / ").Append(StatsEffectedValues.GetMaxHP(obj)).AppendLine();
            rightLBuilder.AppendLine("　MP：");
            rightRBuilder.Append(stats.MP).Append(" / ").Append(StatsEffectedValues.GetMaxMP(obj)).AppendLine();

            var atk = GetATKText(obj);
            leftLBuilder.AppendLine("攻撃：");
            leftRBuilder.AppendLine(atk);
            rightLBuilder.AppendLine("防御：");
            rightRBuilder.Append(StatsEffectedValues.GetDEF(obj)).AppendLine();

            var weight = WeightCalculator.Get(obj);
            leftLBuilder.AppendLine("重量：");
            leftRBuilder.Append(weight.SpaceWeight).Append(" / ").Append(StatsEffectedValues.GetLoadCapacity(obj)).AppendLine();
            rightLBuilder.AppendLine();
            rightRBuilder.AppendLine();

            leftLBuilder.AppendLine("ステータス：");
            leftRBuilder.AppendLine();
            rightLBuilder.AppendLine();
            rightRBuilder.AppendLine();
        }

        private static string GetGenderText(RogueObj obj)
        {
            using var genderValue = EffectableValue.Get();
            StatsEffectedValues.GetGender(obj, genderValue);
            var objIsMale = genderValue.SubValues.Is(StatsKw.Male);
            var objIsFemale = genderValue.SubValues.Is(StatsKw.Female);
            if (objIsMale && !objIsFemale) return "♂";
            if (objIsFemale && !objIsMale) return "♀";
            else return "-";
        }

        private static string GetATKText(RogueObj obj)
        {
            using var atkValue = EffectableValue.Get();
            StatsEffectedValues.GetATK(obj, atkValue);
            if (atkValue.BaseMainValue == atkValue.MainValue)
            {
                return atkValue.MainValue.ToString();
            }
            else
            {
                return $"{atkValue.MainValue} ({atkValue.BaseMainValue})";
            }
        }

        private void AppendSkills(RogueObj obj)
        {
            leftLBuilder.AppendLine("スキル：");
            leftRBuilder.AppendLine();
            rightLBuilder.AppendLine();
            rightRBuilder.AppendLine("攻撃力");

            var normalAttack = AttackUtility.GetNormalAttackSkill(obj);
            AppendSkill(normalAttack, MainInfoKw.Attack.Name);

            var skills = obj.Main.Skills;
            for (int i = 0; i < skills.Count; i++)
            {
                SkillNameEffectStateInfo.GetEffectedName(nameBuilder, obj, skills[i]);
                StandardRogueDeviceUtility.Localize(nameBuilder);
                AppendSkill(skills[i], nameBuilder.ToString());
            }

            void AppendSkill(ISkill skill, string name)
            {
                var atk = skill.GetATK(obj, out var additionalEffect);
                var additionalEffectText = additionalEffect ? "+α" : "";
                leftLBuilder.AppendLine(name);
                leftRBuilder.AppendLine();
                rightLBuilder.AppendLine();
                rightRBuilder.Append(atk).AppendLine(additionalEffectText);
            }
        }

        private void AppendLoots(RogueObj obj)
        {
            leftLBuilder.AppendLine("ドロップアイテム：");
            leftRBuilder.AppendLine();
            rightLBuilder.AppendLine();
            rightRBuilder.AppendLine();

            var loots = obj.Main.InfoSet.LootTable;
            for (int i = 0; i < loots.Count; i++)
            {
                var lootList = loots[i];
            }
        }

        public void SetTile(IRogueTile tile)
        {
            leftLBuilder.Clear();
            leftRBuilder.Clear();
            rightLBuilder.Clear();
            rightRBuilder.Clear();

            leftLBuilder.AppendLine(tile.Info.Name);
            leftRBuilder.AppendLine();
            rightLBuilder.AppendLine();
            rightRBuilder.AppendLine();

            _textLeftL.SetText(leftLBuilder);
            _textLeftR.SetText(leftRBuilder);
            _textRightL.SetText(rightLBuilder);
            _textRightR.SetText(rightRBuilder);
        }

        public void SetOther(in RogueMethodArgument arg)
        {
            leftLBuilder.Clear();
            leftRBuilder.Clear();
            rightLBuilder.Clear();
            rightRBuilder.Clear();

            leftLBuilder.AppendLine("不明");

            _textLeftL.SetText(leftLBuilder);
            _textLeftR.SetText(leftRBuilder);
            _textRightL.SetText(rightLBuilder);
            _textRightR.SetText(rightRBuilder);
        }

        private void SetResultTop(RogueObj player, RogueObj dungeon)
        {
            topBuilder.Clear();

            player.GetName(nameBuilder);
            topBuilder.Append(nameBuilder.ToString()).AppendLine(" は");

            dungeon.GetName(nameBuilder);
            topBuilder.Append(nameBuilder.ToString()).AppendLine(" を突破した");

            _topText.SetText(topBuilder);
        }
    }
}
