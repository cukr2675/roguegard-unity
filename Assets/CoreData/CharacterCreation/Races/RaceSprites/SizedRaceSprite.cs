using OchalikeSprites;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class SizedRaceSprite : ReferableScript, IRaceSprite
    {
        [SerializeField] private Item[] _items = null;

        public void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out OchalikeBone mainBone, out AppearanceMorph morph)
        {
            var standardMember = StandardRaceMember.GetMember(characterCreationData.Race);
            var item = _items[standardMember.Size];

            var bodyColor = characterCreationData.Race.BodyColor;
            var hairColor = RogueColorUtility.GetHairColor(characterCreationData);
            var useDarkOutline = OchalikeSpritesUtility.IsSimilarToLightOutline(hairColor);
            mainBone = item.Bone.CreateOchalikeSprite(bodyColor, useDarkOutline);

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

            var standardMember = StandardRaceMember.GetMember(characterCreationData.Race);
            var item = _items[standardMember.Size];
            motionSet = item.MotionSet;
        }

        [System.Serializable]
        public class Item
        {
            [SerializeField] private OchalikeSpriteAsset _bone = null;
            public OchalikeSpriteAsset Bone => _bone;

            [SerializeField] private SpriteMotionSetAsset _motionSet = null;
            public SpriteMotionSetAsset MotionSet => _motionSet;
        }
    }
}
