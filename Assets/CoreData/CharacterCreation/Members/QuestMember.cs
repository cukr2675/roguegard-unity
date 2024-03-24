using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class QuestMember : IMember, IReadOnlyQuestMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        public StartingItemBuilderList Targets { get; } = new StartingItemBuilderList();
        public int TargetFloor { get; set; }

        Spanning<IWeightedRogueObjGenerator> IReadOnlyQuestMember.Targets => Targets.Spanning;

        public static IReadOnlyQuestMember GetMember(IMemberable memberable)
        {
            return (IReadOnlyQuestMember)memberable.GetMember(SourceInstance);
        }

        public IMember Clone()
        {
            var clone = new QuestMember();
            clone.Targets.AddClones(Targets);
            clone.TargetFloor = TargetFloor;
            return clone;
        }

        private class SourceType : IMemberSource
        {
            public IMember CreateMember()
            {
                return new QuestMember();
            }
        }
    }
}
