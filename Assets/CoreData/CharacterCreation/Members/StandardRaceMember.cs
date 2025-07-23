using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class StandardRaceMember : IMember, IReadOnlyStandardRaceMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField] private int _size;
        public int Size { get => _size; set => _size = value; }

        [SerializeField] private int _typeIndex;
        public int TypeIndex { get => _typeIndex; set => _typeIndex = value; }

        [SerializeField] private int _motionSetIndex;
        public int MotionSetIndex { get => _motionSetIndex; set => _motionSetIndex = value; }

        private StandardRaceMember() { }

        public static IReadOnlyStandardRaceMember GetMember(IReadOnlyMemberable race)
        {
            return (IReadOnlyStandardRaceMember)race.GetMember(SourceInstance);
        }

        public IMember Clone()
        {
            return new StandardRaceMember
            {
                Size = _size,
                TypeIndex = _typeIndex,
                MotionSetIndex = _motionSetIndex
            };
        }

        private class SourceType : IMemberSource
        {
            public IMember CreateMember()
            {
                return new StandardRaceMember();
            }
        }
    }
}
