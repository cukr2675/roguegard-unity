using OchalikeSprites;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// 性別によって見た目を変える <see cref="IRaceSprite"/>
    /// </summary>
    public class GenderDifferenceRaceSprite : ReferableScript, IRaceSprite
    {
        [SerializeField] private Item[] _items = null;

        public void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out OchalikeBone mainBone, out AppearanceMorph morph)
        {
            var item = GetItem(gender);

            var bodyColor = characterCreationData.Race.BodyColor;
            var hairColor = RogueColorUtility.GetHairColor(characterCreationData);
            var useDarkOutline = OchalikeSpritesUtility.IsSimilarToLightOutline(hairColor);
            mainBone = item.Bone.CreateOchalikeSprite(bodyColor, useDarkOutline);

            morph = new AppearanceMorph();

            foreach (var appearance in item.Appearances)
            {
                appearance.Option.Affect(mainBone, morph, appearance, characterCreationData);
            }
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

            var item = GetItem(gender);
            motionSet = item.MotionSet;
        }

        private Item GetItem(IRogueGender gender)
        {
            foreach (var item in _items)
            {
                if (item.Gender == gender) return item;
            }
            return _items[0];
        }

        [System.Serializable]
        public class Item
        {
            [SerializeField] private RogueGenderAsset _gender = null;
            public IRogueGender Gender => _gender;

            [SerializeField] private OchalikeSpriteData _bone = null;
            public OchalikeSpriteData Bone => _bone;

            [SerializeField] private SpriteMotionSetAsset _motionSet = null;
            public SpriteMotionSetAsset MotionSet => _motionSet;

            [SerializeField, ElementDescription("_option")] private ScriptableAppearance[] _appearances = null;
            public Spanning<ScriptableAppearance> Appearances => _appearances;
        }
    }
}
