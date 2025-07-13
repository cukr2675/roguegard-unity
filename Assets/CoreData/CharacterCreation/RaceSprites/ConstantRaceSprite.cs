using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OchalikeSprites;

namespace Roguegard.CharacterCreation
{
    public class ConstantRaceSprite : ReferableScript, IRaceOptionSprite
    {
        [SerializeField] private OchalikeSpriteData _bone = null;

        [SerializeField] private SpriteMotionSetData _motionSet = null;

        public void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out OchalikeBone mainBone, out AppearanceMorph morph)
        {
            var bodyColor = characterCreationData.Race.BodyColor;
            var hairColor = RogueColorUtility.GetHairColor(characterCreationData);
            var useDarkOutline = OchalikeSpritesUtility.IsSimilarToLightOutline(hairColor);
            if (_bone != null) { mainBone = _bone.CreateOchalikeSprite(bodyColor, useDarkOutline); }
            else { mainBone = null; }
            morph = new AppearanceMorph();
        }

        public void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IReadOnlyOchalikeBone mainBone,
            out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            var infoSet = self.Main.InfoSet;
            if (mainBone != null)
            {
                objSprite = OchalikeRogueSprite.CreateOrReuse(self, mainBone, infoSet.Icon, infoSet.Color);
            }
            else
            {
                var color = RogueColorUtility.GetColor(self);
                objSprite = ColoredRogueSprite.CreateOrReuse(self, infoSet.Icon, color);
            }
            motionSet = _motionSet;
        }
    }
}
