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
            var maxFloor = dungeon.Levels[dungeon.Levels.Count - 1].EndLv - 1;
            var floor = random.Next(1, maxFloor);

            var builder = new IntrinsicBuilder();
            builder.Option = parent;
            builder.OptionCaption = $"{floor}F Ç≈ {clientOption.Name} Çã~èïÇ∑ÇÈ";
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
                RogueMethodAspectState.PassiveNext next)
            {
                var generate = keyword == MainInfoKw.Locate && self.Location.Space.Tilemap == null;

                var result = next.Invoke(keyword, method, self, user, activationDepth, arg);
                if (!result) return false;

                if (generate && self.Location.Space.Tilemap != null && DungeonInfo.TryGetLevel(self.Location, 0, out _) &&
                    self.Location.Main.Stats.Lv == member.TargetFloor)
                {
                    // ñ⁄ïWÇÃäKëwÇ÷ÇÃà⁄ìÆÇ…ê¨å˜ÇµÇΩÇ∆Ç´ NPC Çê∂ê¨Ç∑ÇÈÅB
                    var position = self.Location.Space.GetRandomPositionInRoom(RogueRandom.Primary);
                    var client = member.Targets[0];
                    client.Option.CreateObj(client, self.Location, position, RogueRandom.Primary);
                }
                return true;
            }
        }
    }
}
