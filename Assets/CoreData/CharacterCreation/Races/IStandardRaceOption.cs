namespace Roguegard.CharacterCreation
{
    public interface IStandardRaceOption : IRaceOption
    {
        int MinSize { get; }
        int MaxSize { get; }
        int TypeCount { get; }
        int MotionSetCount { get; }
    }
}
