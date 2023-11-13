using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Race/StandardRaceOption")]
    public class StandardRaceOption : BaseRaceOption, IStandardRaceOption
    {
        [SerializeField] private int _minSize = 1;
        [SerializeField] private int _maxSize = 1;
        [SerializeField] private int _typeCount = 1;
        [SerializeField] private int _motionSetCount = 1;

        public int MinSize
        {
            get
            {
                return _minSize;
            }
            set
            {
                _minSize = value;
                OnValidate();
            }
        }

        public int MaxSize
        {
            get
            {
                return _maxSize;
            }
            set
            {
                _maxSize = value;
                OnValidate();
            }
        }

        public int TypeCount
        {
            get
            {
                return _typeCount;
            }
            set
            {
                _typeCount = value;
                OnValidate();
            }
        }

        public int MotionSetCount
        {
            get
            {
                return _motionSetCount;
            }
            set
            {
                _motionSetCount = value;
                OnValidate();
            }
        }

        public override Spanning<IMemberSource> MemberSources => _memberSources;
        private static readonly IMemberSource[] _memberSources = new IMemberSource[] { StandardRaceMember.SourceInstance };

        public override void UpdateMemberRange(IMember member, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            if (member is StandardRaceMember standardMember)
            {
                //standardMember.MotionSetIndex
            }
        }

        protected virtual void OnValidate()
        {
            _minSize = Mathf.Max(_minSize, 1);
            _maxSize = Mathf.Max(_maxSize, 1);
            _typeCount = Mathf.Max(_typeCount, 1);
            _motionSetCount = Mathf.Max(_motionSetCount, 1);
        }
    }
}
