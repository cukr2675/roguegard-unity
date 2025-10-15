using OchalikeSprites;

namespace Roguegard.CharacterCreation
{
    public interface IRaceSprite
    {
        void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out OchalikeBone mainBone, out AppearanceMorph morph);

        void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self,
            IReadOnlyOchalikeBone mainBone, out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet);
    }
}
