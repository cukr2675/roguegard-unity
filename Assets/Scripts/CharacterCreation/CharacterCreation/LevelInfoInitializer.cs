namespace Roguegard.CharacterCreation
{
    public interface ILevelInfoInitializer : ILevelInfo
    {
        void InitializeLv(RogueObj obj, int initialLv);
    }
}
