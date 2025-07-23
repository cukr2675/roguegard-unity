using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class RogueTileMember : IMember, IReadOnlyRogueTileMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField, Objforming.IgnoreMember] private RogueTileAsset _editorTile;
        public IRogueTile Tile => _editorTile;

        private RogueTileMember() { }

        public static IReadOnlyRogueTileMember GetMember(IReadOnlyStartingItem startingItem)
        {
            return (IReadOnlyRogueTileMember)startingItem.GetMember(SourceInstance);
        }

        public IMember Clone()
        {
            return new RogueTileMember
            {
                _editorTile = _editorTile
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
