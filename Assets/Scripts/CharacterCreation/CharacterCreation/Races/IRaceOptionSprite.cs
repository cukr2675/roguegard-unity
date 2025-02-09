using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OchalikeSprites;

namespace Roguegard.CharacterCreation
{
    public interface IRaceOptionSprite
    {
        void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out OchalikeBone mainBone, out AppearanceBoneSpriteTable boneSpriteTable);

        void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IReadOnlyOchalikeBone mainBone,
            out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet);
    }
}
