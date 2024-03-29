using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Roguegard
{
    [Objforming.Formable]
    public class ColoredRogueTile : IRogueTile
    {
        public IRogueTileInfo Info { get; }

        TileBase IRogueSprite.Tile => Info.Tile;

        private readonly Color32 _color;
        public Color EffectedColor => _color;

        [Objforming.CreateInstance]
        private ColoredRogueTile() { }

        public ColoredRogueTile(IRogueTileInfo info, Color color)
        {
            Info = info;
            _color = color;
        }

        public bool Equals(IRogueTile other)
        {
            return other is ColoredRogueTile coloredOther && coloredOther.Info.Equals(Info) && coloredOther.EffectedColor == EffectedColor;
        }

        public override bool Equals(object obj)
        {
            return obj is IRogueTile other && Equals(other);
        }

        public override int GetHashCode()
        {
            return EffectedColor.GetHashCode();
        }
    }
}
