using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    [System.Serializable]
    public class RogueDirectionalSpriteTable
    {
        [SerializeField] private Sprite _upperLeft = null;
        [SerializeField] private Sprite _up = null;
        [SerializeField] private Sprite _upperRight = null;
        [SerializeField] private Sprite _left = null;
        [SerializeField] private Sprite _right = null;
        [SerializeField] private Sprite _lowerLeft = null;
        [SerializeField] private Sprite _down = null;
        [SerializeField] private Sprite _lowerRight = null;

        public Sprite GetSprite(RogueDirection direction)
        {
            if (direction == RogueDirection.UpperLeft) return _upperLeft;
            if (direction == RogueDirection.Up) return _up;
            if (direction == RogueDirection.UpperRight) return _upperRight;
            if (direction == RogueDirection.Left) return _left;
            if (direction == RogueDirection.Right) return _right;
            if (direction == RogueDirection.LowerLeft) return _lowerLeft;
            if (direction == RogueDirection.Down) return _down;
            if (direction == RogueDirection.LowerRight) return _lowerRight;
            return null;
        }
    }
}
