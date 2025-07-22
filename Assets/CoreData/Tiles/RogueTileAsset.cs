using UnityEngine;
using UnityEngine.Tilemaps;

namespace Roguegard
{
    public abstract class RogueTileAsset : ScriptableObject, IRogueTile
    {
        public abstract IRogueTileInfo Info { get; }

        public abstract TileBase Tile { get; }

        public abstract Color EffectedColor { get; }

        public abstract bool Equals(IRogueTile other);
    }
}
