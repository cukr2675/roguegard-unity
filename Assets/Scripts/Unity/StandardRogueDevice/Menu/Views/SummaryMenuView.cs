using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using UnityEngine.UI;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;
using TMPro;
using Roguegard.CharacterCreation;

namespace RoguegardUnity
{
    public class SummaryMenuView : ModelsMenuView, IResultMenuView, IDungeonQuestMenuView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private TMP_Text _topText = null;
        [SerializeField] private TMP_Text _textLeftL = null;
        [SerializeField] private TMP_Text _textLeftR = null;
        [SerializeField] private TMP_Text _textRightL = null;
        [SerializeField] private TMP_Text _textRightR = null;
        [SerializeField] private ModelsMenuViewItemButton _exitButton = null;
        [SerializeField] private ModelsMenuViewItemButton _submitButton = null;

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private StringBuilder topBuilder;
        private StringBuilder leftLBuilder;
        private StringBuilder leftRBuilder;
        private StringBuilder rightLBuilder;
        private StringBuilder rightRBuilder;
        private RogueNameBuilder nameBuilder;

        private static readonly SubmitChoice submitChoice = new SubmitChoice();
        private static readonly StartQuestChoice startQuestChoice = new StartQuestChoice();

        public void Initialize()
        {
            topBuilder = new StringBuilder();
            leftLBuilder = new StringBuilder();
            leftRBuilder = new StringBuilder();
            rightLBuilder = new StringBuilder();
            rightRBuilder = new StringBuilder();
            nameBuilder = new RogueNameBuilder();

            _exitButton.Initialize(this);
            _submitButton.Initialize(this);
        }

        public override void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models,
            IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            SetArg(root, self, user, arg);
            MenuController.Show(_exitButton.CanvasGroup, false);
            MenuController.Show(_submitButton.CanvasGroup, false);
            MenuController.Show(_canvasGroup, true);
        }

        public override float GetPosition()
        {
            // 後からアイテムが増えたときのため、スクロール位置を変換したものを返す
            var offset = _scrollRect.content.rect.height * (1f - _scrollRect.verticalNormalizedPosition);
            return offset;
        }

        public override void SetPosition(float position)
        {
            _scrollRect.verticalNormalizedPosition = position;
        }

        public void SetObj(RogueObj obj)
        {
            SetObj(obj, null);
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

        void IResultMenuView.SetResult(RogueObj player, RogueObj dungeon)
        {
            var arg = new RogueMethodArgument(targetObj: dungeon);
            SetArg(Root, Self, User, arg);

            SetObj(player, dungeon);
            ShowSubmitButton(submitChoice);
        }

        void IResultMenuView.SetGameOver(RogueObj player, RogueObj dungeon)
        {
            var arg = new RogueMethodArgument(targetObj: dungeon);
            SetArg(Root, Self, User, arg);

            SetObj(player, null);
            ShowExitButton(ExitModelsMenuChoice.Instance);
            ShowSubmitButton(submitChoice);
        }

        void IDungeonQuestMenuView.SetQuest(RogueObj player, DungeonQuest quest, bool showSubmitButton)
        {
            SetArg(Root, player, User, new(other: quest));

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

            ShowExitButton(ExitModelsMenuChoice.Instance);
            if (showSubmitButton) { ShowSubmitButton(startQuestChoice); }
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
            using var genderValue = AffectableValue.Get();
            StatsEffectedValues.GetGender(obj, genderValue);
            var objIsMale = genderValue.SubValues.Is(StatsKw.Male);
            var objIsFemale = genderValue.SubValues.Is(StatsKw.Female);
            if (objIsMale && !objIsFemale) return "♂";
            if (objIsFemale && !objIsMale) return "♀";
            else return "-";
        }

        private static string GetATKText(RogueObj obj)
        {
            using var atkValue = AffectableValue.Get();
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

            AppendSkill(obj.Main.InfoSet.Attack, MainInfoKw.Attack.Name);

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

        public void ShowExitButton(IModelsMenuChoice choice)
        {
            _exitButton.SetItem(ChoicesModelsMenuItemController.Instance, choice);
            MenuController.Show(_exitButton.CanvasGroup, true);
        }

        private void ShowSubmitButton(IModelsMenuChoice choice)
        {
            _submitButton.SetItem(ChoicesModelsMenuItemController.Instance, choice);
            MenuController.Show(_submitButton.CanvasGroup, true);
        }

        private class SubmitChoice : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "OK";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var dungeon = DungeonInfo.GetLargestDungeon(arg.TargetObj);

                if (!default(IActiveRogueMethodCaller).LocateSavePoint(self, null, 0f, RogueWorld.SavePointInfo, true)) return;

                if (dungeon?.Stack >= 1)
                {
                    dungeon.TrySetStack(0);
                }
                var memberInfo = LobbyMembers.GetMemberInfo(self);
                memberInfo.SavePoint = RogueWorld.SavePointInfo;

                RogueDevice.Add(DeviceKw.AutoSave, 0);

                root.Done();
            }
        }

        private class StartQuestChoice : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "出発";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var quest = (DungeonQuest)arg.Other;
                quest.Start(self);

                // BackToLobby で階層表示させるため、ここでは終了させない。
                root.Done();
            }
        }
    }
}
