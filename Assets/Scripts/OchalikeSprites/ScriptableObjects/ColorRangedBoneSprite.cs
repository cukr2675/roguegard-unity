using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    [System.Serializable]
    public class ColorRangedBoneSprite
    {
        [SerializeField] private bool _isColorRanged;
        [SerializeField] private BoneSprite _spriteOrLightSprite;
        [SerializeField] private BoneSprite _darkSprite;

        public bool IsColorRanged => _isColorRanged;
        public BoneSprite Sprite => !_isColorRanged ? _spriteOrLightSprite : throw new System.Exception();
        public BoneSprite LightSprite => _isColorRanged ? _spriteOrLightSprite : throw new System.Exception();
        public BoneSprite DarkSprite => _isColorRanged ? _darkSprite : throw new System.Exception();
        public BoneSprite Icon => _spriteOrLightSprite;

        public ColorRangedBoneSprite() { }

        public ColorRangedBoneSprite(BoneSprite sprite)
        {
            _spriteOrLightSprite = sprite;
            _isColorRanged = false;
        }

        public ColorRangedBoneSprite(BoneSprite lightSprite, BoneSprite darkSprite)
        {
            _spriteOrLightSprite = lightSprite;
            _darkSprite = darkSprite;
            _isColorRanged = true;
        }

        public BoneSprite GetSprite(bool useDarkOutline)
        {
            if (_isColorRanged && useDarkOutline) return _darkSprite;
            else return _spriteOrLightSprite;
        }

        public void Validate()
        {
            if (!_isColorRanged)
            {
                _darkSprite = null;
            }
        }
    }
}
