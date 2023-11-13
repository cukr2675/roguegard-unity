using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [System.Serializable]
    public class ColorRangedBoneSprite
    {
        [SerializeField] private bool _isColorRanged;
        [SerializeField] private BoneSprite _spriteOrLightRed;
        [SerializeField] private BoneSprite _darkRed;
        [SerializeField] private BoneSprite _lightOther;
        [SerializeField] private BoneSprite _darkOther;

        public bool IsColorRanged => _isColorRanged;
        public BoneSprite Sprite => !_isColorRanged ? _spriteOrLightRed : throw new RogueException();
        public BoneSprite LightRed => _isColorRanged ? _spriteOrLightRed : throw new RogueException();
        public BoneSprite DarkRed => _isColorRanged ? _darkRed : throw new RogueException();
        public BoneSprite LightOther => _isColorRanged ? _lightOther : throw new RogueException();
        public BoneSprite DarkOther => _isColorRanged ? _darkOther : throw new RogueException();
        public BoneSprite Icon => _spriteOrLightRed;

        public ColorRangedBoneSprite() { }

        public ColorRangedBoneSprite(BoneSprite sprite)
        {
            _spriteOrLightRed = sprite;
            _isColorRanged = false;
        }

        public ColorRangedBoneSprite(BoneSprite lightRed, BoneSprite darkRed, BoneSprite lightOther, BoneSprite darkOther)
        {
            _spriteOrLightRed = lightRed;
            _darkRed = darkRed;
            _lightOther = lightOther;
            _darkOther = darkOther;
            _isColorRanged = true;
        }

        public BoneSprite GetSprite(ColorRange colorRange)
        {
            if (!_isColorRanged) return Sprite;

            return colorRange switch
            {
                ColorRange.LightRed => _spriteOrLightRed,
                ColorRange.DarkRed => _darkRed,
                ColorRange.LightOther => _lightOther,
                ColorRange.DarkOther => _darkOther,
                _ => throw new RogueException(),
            };
        }

        public void Validate()
        {
            if (!_isColorRanged)
            {
                _darkRed = null;
                _lightOther = null;
                _darkOther = null;
            }
        }
    }
}
