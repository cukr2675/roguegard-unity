namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyStandardRaceMember : IReadOnlyMember
    {
        public int Size { get; }
        public int TypeIndex { get; }
        public int MotionSetIndex { get; }
    }
}
