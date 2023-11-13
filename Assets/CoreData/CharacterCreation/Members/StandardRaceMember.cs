using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
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

        public static IReadOnlyStandardRaceMember GetMember(IMemberable race)
        {
            return (IReadOnlyStandardRaceMember)race.GetMember(SourceInstance);
        }

        public IMember Clone()
        {
            var clone = new StandardRaceMember();
            clone.Size = _size;
            clone.TypeIndex = _typeIndex;
            clone.MotionSetIndex = _motionSetIndex;
            return clone;
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
