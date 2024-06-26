using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Intrinsic/QuestEffect")]
    [Objforming.Referable]
    public class QuestEffectIntrinsicOption : ScriptIntrinsicOption
    {
        private static readonly IMemberSource[] _memberSources = new[] { QuestMember.SourceInstance };
        public override Spanning<IMemberSource> MemberSources => _memberSources;

        public IntrinsicBuilder GenerateEffect(DungeonCreationData dungeon, ICharacterCreationDatabase database, IRogueRandom random)
        {
            if (ScriptRef is IQuestEffectIntrinsicOptionScript script)
            {
                var builder = script.GenerateEffect(this, dungeon, database, random);
                return builder;
            }
            else
            {
                var builder = new IntrinsicBuilder();
                builder.Option = this;
                return builder;
            }
        }
    }
}
