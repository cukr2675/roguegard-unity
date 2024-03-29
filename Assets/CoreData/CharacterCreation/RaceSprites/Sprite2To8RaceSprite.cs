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

        [SerializeField] private SpriteMotionSetData _motionSet = null;

        public void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out NodeBone mainNode, out AppearanceBoneSpriteTable boneSpriteTable)
        {
            mainNode = null;
            boneSpriteTable = new AppearanceBoneSpriteTable();
        }

        public void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IReadOnlyNodeBone nodeBone,
            out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            var color = RogueColorUtility.GetColor(self);
            objSprite = Sprite2To8RogueSprite.CreateOrReuse(self, _spriteLowerLeft, _spriteLeft, color);
            motionSet = _motionSet;
        }
    }
}
