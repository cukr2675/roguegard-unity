using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IRaceOptionSprite
    {
        void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out BoneNodeBuilder mainNode, out AffectableBoneSpriteTable boneSpriteTable);

        void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IBoneNode boneNode,
            out RogueObjSprite objSprite, out IMotionSet motionSet);
    }
}
