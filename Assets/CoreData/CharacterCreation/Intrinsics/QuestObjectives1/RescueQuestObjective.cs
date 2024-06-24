using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class RescueQuestObjective : AbilityIntrinsicOptionScript, IQuestEffectIntrinsicOptionScript
    {
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
                    if (optionValue.InfoSet.Ability.HasFlag(MainInfoSetAbility.HasCollider) &&
                        optionValue.InfoSet.Ability.HasFlag(MainInfoSetAbility.Movable))
                    {
                        options.Add(optionValue);
                    }
                }
            }

            var clientOption = random.Choice(options);
            var maxFloor = dungeon.Floors[dungeon.Floors.Count - 1].EndLv - 1;
            var floor = random.Next(1, maxFloor);

            var builder = new IntrinsicBuilder();
            builder.Option = parent;
            builder.OptionCaption = $"{floor}F で {clientOption.Name} を救助する";
            var member = (QuestMember)builder.GetMember(QuestMember.SourceInstance);
            var client = member.Targets.Add();
            client.Option = clientOption;
            client.Stack = 1;
            member.TargetFloor = floor;
            return builder;
        }

        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            var member = (QuestMember)QuestMember.GetMember(intrinsic);
            return new SortedIntrinsic(lv) { member = member };
        }

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueMethodPassiveAspect
        {
            public QuestMember member;

            float IRogueMethodPassiveAspect.Order => 0f;

            public SortedIntrinsic(int lv) : base(lv) { }

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
                    // 目標の階層への移動に成功したとき NPC を生成する。
                    if (!self.Location.Space.TryGetRandomPositionInRoom(RogueRandom.Primary, out var position))
                    {
                        // 生成に失敗したらメッセージを表示
                        RogueDevice.Add(DeviceKw.AppendText, "目的の階層に到達したが 目標が見つからなかった\n");
                        return true;
                    }

                    var client = member.Targets[0];
                    client.Option.CreateObj(client, self.Location, position, RogueRandom.Primary);
                }
                return true;
            }
        }
    }
}
