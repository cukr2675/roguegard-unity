namespace Roguegard.CharacterCreation
{
    public interface IQuestEffectIntrinsicScript
    {
        Intrinsic GenerateEffect(
            QuestEffectIntrinsicOptionAsset parent, DungeonCreationDataAsset dungeon, ICharacterCreationDatabase database, IRogueRandom random);
    }
}
