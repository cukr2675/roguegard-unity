namespace Roguegard.CharacterCreation
{
    public interface IQuestEffectIntrinsicScript
    {
        IntrinsicBuilder GenerateEffect(
            QuestEffectIntrinsicOptionAsset parent, DungeonCreationDataAsset dungeon, ICharacterCreationDatabase database, IRogueRandom random);
    }
}
