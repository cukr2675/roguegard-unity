using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Roguegard
{
    [Objforming.Formable]
    public class UserRogueTile : IRogueTile
    {
        private readonly IRogueTileInfo _info;
        public IRogueTileInfo Info => _info;

        TileBase IRogueSprite.Tile => _info.Tile;
        Color IRogueSprite.EffectedColor => _info.Color;

        public RogueObj User { get; }

        private UserRogueTile() { }

        public UserRogueTile(IRogueTileInfo info, RogueObj user)
        {
            _info = info;
            User = user;
        }

        public bool Equals(IRogueTile other)
        {
            return other is UserRogueTile userOther && userOther.Info.Equals(Info) && userOther.User == User;
        }

        public override bool Equals(object obj)
        {
            return obj is IRogueTile other && Equals(other);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
