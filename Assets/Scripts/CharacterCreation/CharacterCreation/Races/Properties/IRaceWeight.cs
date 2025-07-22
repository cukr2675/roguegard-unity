namespace Roguegard.CharacterCreation
{
    public interface IRaceWeight
    {
        float GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData);
    }
}
