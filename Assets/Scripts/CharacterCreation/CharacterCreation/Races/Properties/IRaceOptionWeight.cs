namespace Roguegard.CharacterCreation
{
    public interface IRaceOptionWeight
    {
        float GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData);
    }
}
