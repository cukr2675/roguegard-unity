using Lysionium;
using Lysionium.Views;
using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoguegardUnity
{
    public class SummarySubview : Subview, ISummaryElementsSubview
    {
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private TMP_Text _topText = null;
        [SerializeField] private TMP_Text _textLeftL = null;
        [SerializeField] private TMP_Text _textLeftR = null;
        [SerializeField] private TMP_Text _textRightL = null;
        [SerializeField] private TMP_Text _textRightR = null;

        private MArg arg;
        private StringBuilder topBuilder;
        private StringBuilder leftLBuilder;
        private StringBuilder leftRBuilder;
        private StringBuilder rightLBuilder;
        private StringBuilder rightRBuilder;
        private RogueNameBuilder nameBuilder;

        private static readonly ISelectOption<MMgr>[] backSelectOption = new[]
        {
            BackSelectOption<MMgr>.Instance
        };

        private ISelectOption<MMgr>[] submitSelectOption;
        private ISelectOption<MMgr>[] startQuestSelectOption;

        public void Initialize()
        {
            topBuilder = new StringBuilder();
            leftLBuilder = new StringBuilder();
            leftRBuilder = new StringBuilder();
            rightLBuilder = new StringBuilder();
            rightRBuilder = new StringBuilder();
            nameBuilder = new RogueNameBuilder();

            submitSelectOption = new[]
            {
                SelectOption.Create<MMgr>("OK", (manager) =>
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
            
            startQuestSelectOption = new[]
            {
                SelectOption.Create<MMgr>("出発", (manager) =>
                {
                    var quest = (DungeonQuest)arg.Arg.Other;
                    quest.Start(arg.Self);

                    // BackToLobby で階層表示させるため、ここでは終了させない。
                    manager.Done();
                })
            };
        }

        public void SetObj(object obj, MMgr manager)
        {
            if (obj is RogueObj rogueObj)
            {
                SetObj(rogueObj, null);
            }
            else if (obj is IRogueTile tile)
            {
                SetTile(tile);
            }

            ISubviewStateProvider stateProvider = null;
            manager.BackAnchor.Show(backSelectOption, manager, ref stateProvider);
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

        public void SetResult(RogueObj player, RogueObj dungeon, MMgr manager)
        {
            Manager = manager;
            arg = new MArg.Builder(self: player, arg: new(targetObj: dungeon)).ReadOnly;

            SetObj(player, dungeon);

            ISubviewStateProvider stateProvider = null;
            manager.ForwardAnchor.Show(submitSelectOption, manager, ref stateProvider);
        }

        public void SetGameOver(RogueObj player, RogueObj dungeon, MMgr manager)
        {
            Manager = manager;
            arg = new MArg.Builder(self: player, arg: new(targetObj: dungeon)).ReadOnly;

            SetObj(player, null);

            ISubviewStateProvider stateProvider = null;
            manager.BackAnchor.Show(backSelectOption, manager, ref stateProvider);
            manager.ForwardAnchor.Show(submitSelectOption, manager, ref stateProvider);
        }

        public void SetQuest(RogueObj player, DungeonQuest quest, bool showSubmitButton, MMgr manager)
        {
            Manager = manager;
            arg = new MArg.Builder(self: player, arg: new(other: quest)).ReadOnly;

            _topText.text = null;
            leftLBuilder.Clear();
            leftRBuilder.Clear();
            rightLBuilder.Clear();
            rightRBuilder.Clear();

            leftLBuilder.AppendLine(quest.Name);
            leftLBuilder.AppendLine("場所：");
            leftLBuilder.AppendLine(quest.Dungeon.DescriptionName);
            leftLBuilder.AppendLine("達成条件：");
            foreach (var objective in quest.Objectives)
            {
                leftLBuilder.AppendLine(objective.Caption);
            }
            leftLBuilder.AppendLine("環境：");
            foreach (var environment in quest.Environments)
            {
                leftLBuilder.AppendLine(environment.Name);
            }
            leftLBuilder.AppendLine("報酬：");
            var any = false;
            var plusAlpha = false;
            foreach (var lootTableRow in quest.LootTable)
            {
                var loots = lootTableRow.Span;
                if (loots.Length == 1)
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
                else if (loots.Length >= 2)
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

            ISubviewStateProvider stateProvider = null;
            manager.BackAnchor.Show(backSelectOption, manager, ref stateProvider);
            if (showSubmitButton)
            {
                manager.ForwardAnchor.Show(startQuestSelectOption, manager, ref stateProvider);
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
            leftRBuilder.Append(stats.Hp).Append(" / ").Append(StatsEffectedValues.GetMaxHp(obj)).AppendLine();
            rightLBuilder.AppendLine("　MP：");
            rightRBuilder.Append(stats.Mp).Append(" / ").Append(StatsEffectedValues.GetMaxMp(obj)).AppendLine();

            var atk = GetAtkText(obj);
            leftLBuilder.AppendLine("攻撃：");
            leftRBuilder.AppendLine(atk);
            rightLBuilder.AppendLine("防御：");
            rightRBuilder.Append(StatsEffectedValues.GetDef(obj)).AppendLine();

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

        private static string GetAtkText(RogueObj obj)
        {
            using var atkValue = EffectableValue.Get();
            StatsEffectedValues.GetAtk(obj, atkValue);
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
                var atk = skill.GetAtk(obj, out var additionalEffect);
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

            foreach (var lootList in obj.Main.InfoSet.LootTable)
            {
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

        public void SetOther()
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
