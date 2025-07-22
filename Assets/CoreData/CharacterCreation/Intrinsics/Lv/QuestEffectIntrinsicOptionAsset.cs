using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Intrinsic/Quest Effect")]
    [Objforming.Referable]
    public class QuestEffectIntrinsicOptionAsset : ScriptIntrinsicOptionAsset
    {
        private static readonly IMemberSource[] _memberSources = new[] { QuestMember.SourceInstance };
        public override Spanning<IMemberSource> MemberSources => _memberSources;

        public IntrinsicBuilder GenerateEffect(DungeonCreationDataAsset dungeon, ICharacterCreationDatabase database, IRogueRandom random)
        {
            if (ScriptRef is IQuestEffectIntrinsicScript script)
            {
                var builder = script.GenerateEffect(this, dungeon, database, random);
                return builder;
            }
            else
            {
                return new IntrinsicBuilder
                {
                    Option = this
                };
            }
        }
    }
}
