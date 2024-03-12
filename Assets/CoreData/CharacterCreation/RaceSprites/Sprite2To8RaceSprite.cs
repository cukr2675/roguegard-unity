using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    public class Sprite2To8RaceSprite : ReferableScript, IRaceOptionSprite
    {
        [SerializeField] private Sprite _spriteLowerLeft = null;
        [SerializeField] private Sprite _spriteLeft = null;

        [SerializeField] private MotionSetData _motionSet = null;

        public void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out BoneNodeBuilder mainNode, out AppearanceBoneSpriteTable boneSpriteTable)
        {
            mainNode = null;
            boneSpriteTable = new AppearanceBoneSpriteTable();
        }

        public void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IBoneNode boneNode,
            out RogueObjSprite objSprite, out IMotionSet motionSet)
        {
            var color = RogueColorUtility.GetColor(self);
            objSprite = Sprite2To8RogueSprite.CreateOrReuse(self, _spriteLowerLeft, _spriteLeft, color);
            motionSet = _motionSet;
        }
    }
}
