using OchalikeSprites;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class Sprite2To8RaceSprite : ReferableScript, IRaceOptionSprite
    {
        [SerializeField] private Sprite _spriteLowerLeft = null;
        [SerializeField] private Sprite _spriteLeft = null;

        [SerializeField] private SpriteMotionSetData _motionSet = null;

        public void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out OchalikeBone mainBone, out AppearanceMorph morph)
        {
            mainBone = null;
            morph = new AppearanceMorph();
        }

        public void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IReadOnlyOchalikeBone mainBone,
            out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            var color = RogueColorUtility.GetColor(self);
            objSprite = Sprite2To8RogueSprite.CreateOrReuse(self, _spriteLowerLeft, _spriteLeft, color);
            motionSet = _motionSet;
        }
    }
}
