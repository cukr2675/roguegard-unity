using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class RogueTileMember : IMember, IReadOnlyRogueTileMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField, ObjectFormer.IgnoreMember] private ScriptableRogueTile _tile;
        public IRogueTile Tile => _tile;

        private RogueTileMember() { }

        public static IReadOnlyRogueTileMember GetMember(IReadOnlyStartingItem startingItem)
        {
            return (IReadOnlyRogueTileMember)startingItem.GetMember(SourceInstance);
        }

        public void SetRandom(ICharacterCreationDatabase database, IRogueRandom random)
        {
            if (_tile != null) throw new RogueException();
        }

        public IMember Clone()
        {
            var clone = new RogueTileMember();
            clone._tile = _tile;
            return clone;
        }

        private class SourceType : IMemberSource
        {
            public IMember CreateMember()
            {
                return new RogueTileMember();
            }
        }
    }
}
