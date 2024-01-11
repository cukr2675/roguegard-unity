using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    internal static class AutoAction
    {
        private static readonly List<IObjCommand> choices = new List<IObjCommand>();
        private static readonly List<RogueObj> items = new List<RogueObj>();

        public static bool TryOtherAction(RogueObj self, float activationDepth, float visibleRadius, RectInt room, IRogueRandom random)
        {
            var skills = self.Main.Skills;
            var items = self.Space.Objs;
            var actionLength = skills.Count + items.Count;
            var actionIndex = random.Next(0, actionLength);
            if (actionIndex < skills.Count)
            {
                var skill = skills[actionIndex];
                var skillResult = AutoSkill(MainInfoKw.Skill, skill, self, self, activationDepth, null, visibleRadius, room, random, true);
                return skillResult;
            }
            actionIndex -= skills.Count;
            if (actionIndex < items.Count)
            {
                var item = items[actionIndex];
                return AutoItem(item, self, self, activationDepth, visibleRadius, room, random);
            }
            return false;
        }

        public static bool AutoSkill(
            IKeyword keyword, ISkill skill, RogueObj self, RogueObj user, float activationDepth,
            RogueObj tool, float visibleRadius, RectInt room, IRogueRandom random, bool enqueueMessageRule = false)
        {
            using var predicator = skill.Target?.GetPredicator(self, 0f, tool);
            if (predicator == null) return false;

            skill.Range?.Predicate(predicator, self, 0f, tool, visibleRadius, room);
            predicator.EndPredicate();
            if (predicator.Positions.Count == 0) return false;

            if (enqueueMessageRule && !RoguegardSettings.KeywordsNotEnqueueMessageRule.Contains(keyword))
            {
                RogueDevice.Add(DeviceKw.AppendText, DeviceKw.HorizontalRule);
            }

            var positionIndex = random.Next(0, predicator.Positions.Count);
            var position = predicator.Positions[positionIndex];
            var arg = new RogueMethodArgument(targetPosition: position);
            return RogueMethodAspectState.Invoke(keyword, skill, self, user, activationDepth, arg);
        }

        private static bool AutoItem(RogueObj item, RogueObj self, RogueObj user, float activationDepth, float visibleRadius, RectInt room, IRogueRandom random)
        {
            RoguegardSettings.ObjCommandTable.GetCommands(self, item, choices);
            foreach (var choice in choices)
            {
                var skillDescription = choice.GetSkillDescription(self, item);
                if (skillDescription == null) continue;

                using var predicator = skillDescription.Target?.GetPredicator(self, 0f, item);
                if (predicator == null) continue;

                skillDescription.Range?.Predicate(predicator, self, 0f, item, visibleRadius, room);
                predicator.EndPredicate();
                if (predicator.Positions.Count >= 1)
                {
                    var positionIndex = random.Next(0, predicator.Positions.Count);
                    var arg = new RogueMethodArgument(tool: item, targetPosition: predicator.Positions[positionIndex]);
                    var result = choice.CommandInvoke(self, user, activationDepth, arg);
                    if (result) return true;
                }
            }
            return false;
        }

        public static bool TryHypnosisOtherAction(RogueObj self, float activationDepth, float visibleRadius, RectInt room, IRogueRandom random)
        {
            if (self.Space == null) return false;

            var spaceObjs = self.Space.Objs;
            items.Clear();
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var spaceObj = spaceObjs[i];
                if (spaceObj == null) continue;

                if (!SpaceUtility.ObjIsGlued(spaceObj))
                {
                    items.Add(spaceObj);
                }
            }
            if (items.Count == 0) return false;

            var actionIndex = random.Next(0, items.Count);
            var item = items[actionIndex];
            return HypnosisAutoItem(item, self, self, activationDepth, visibleRadius, room, random);
        }

        private static bool HypnosisAutoItem(
            RogueObj item, RogueObj self, RogueObj user, float activationDepth, float visibleRadius, RectInt room, IRogueRandom random)
        {
            RoguegardSettings.ObjCommandTable.GetCommands(self, item, choices);
            foreach (var choice in choices)
            {
                var skillDescription = choice.GetSkillDescription(self, item);
                if (skillDescription == null) continue;

                var target = GetTarget(skillDescription.Target);
                using var predicator = target?.GetPredicator(self, 0f, item);
                if (predicator == null) continue;

                skillDescription.Range?.Predicate(predicator, self, 0f, item, visibleRadius, room);
                predicator.EndPredicate();
                if (predicator.Positions.Count >= 1)
                {
                    var positionIndex = random.Next(0, predicator.Positions.Count);
                    var arg = new RogueMethodArgument(tool: item, targetPosition: predicator.Positions[positionIndex]);
                    var result = choice.CommandInvoke(self, user, activationDepth, arg);
                    if (result) return true;
                }
            }
            return false;
        }

        private static IRogueMethodTarget GetTarget(IRogueMethodTarget target)
        {
            if (target == ForEnemyRogueMethodTarget.Instance) return ForPartyMemberRogueMethodTarget.Instance;
            if (target == ForPartyMemberRogueMethodTarget.Instance) return ForEnemyRogueMethodTarget.Instance;
            if (target == WoundedPartyMemberRogueMethodTarget.Instance) return ForEnemyRogueMethodTarget.Instance;
            return null;
        }
    }
}
