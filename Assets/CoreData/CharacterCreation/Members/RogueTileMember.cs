using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class RogueTileMember : IMember, IReadOnlyRogueTileMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField, Objforming.IgnoreMember] private ScriptableRogueTile _tile;
        public IRogueTile Tile => _tile;

        private RogueTileMember() { }

        public static IReadOnlyRogueTileMember GetMember(IReadOnlyStartingItem startingItem)
        {
            return (IReadOnlyRogueTileMember)startingItem.GetMember(SourceInstance);
        }

        public IMember Clone()
        {
            return new RogueTileMember
            {
                _tile = _tile
            };
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
