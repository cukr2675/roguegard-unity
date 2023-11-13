using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class ColoredRogueTile : IRogueTile
    {
        public IRogueTileInfo Info { get; }

        [field: System.NonSerialized] public TileBase Tile { get; }

        public Color Color { get; }

        [ObjectFormer.CreateInstance]
        private ColoredRogueTile() { }

        public ColoredRogueTile(IRogueTileInfo info, Color color)
        {
            Info = info;
            Color = color;

            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = info.Sprite;
            tile.color = color;
            Tile = tile;
        }

        public bool Equals(IRogueTile other)
        {
            return other is ColoredRogueTile coloredOther && coloredOther.Info.Equals(Info) && coloredOther.Color == Color;
        }

        public override bool Equals(object obj)
        {
            return obj is IRogueTile other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Color.GetHashCode();
        }
    }
}
