using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Roguegard
{
    public abstract class ScriptableRogueTile : ScriptableObject, IRogueTile
    {
        public abstract IRogueTileInfo Info { get; }

        public abstract TileBase Tile { get; }

        public abstract bool Equals(IRogueTile other);
    }
}
