using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// ê´ï Ç…ÇÊÇ¡Çƒå©ÇΩñ⁄ÇïœÇ¶ÇÈ <see cref="IRaceOptionSprite"/>
    /// </summary>
    public class GenderDifferenceRaceSprite : ReferableScript, IRaceOptionSprite
    {
        [SerializeField] private Item[] _items = null;

        public void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out NodeBone mainNode, out AppearanceBoneSpriteTable boneSpriteTable)
        {
            var item = GetItem(gender);

            var bodyColor = characterCreationData.Race.BodyColor;
            var hairColor = RogueColorUtility.GetHairColor(characterCreationData);
            mainNode = item.Bone.CreateNodeBone(bodyColor, hairColor.maxColorComponent);

            boneSpriteTable = new AppearanceBoneSpriteTable();

            for (int i = 0; i < item.Appearances.Count; i++)
            {
                var appearance = item.Appearances[i];
                appearance.Option.Affect(mainNode, boneSpriteTable, appearance, characterCreationData);
            }
        }

        public void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IReadOnlyNodeBone nodeBone,
            out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            var infoSet = self.Main.InfoSet;
            if (nodeBone != null)
            {
                objSprite = BoneRogueSprite.CreateOrReuse(self, nodeBone, infoSet.Icon, infoSet.Color);
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
            [SerializeField] private RogueGender _gender = null;
            public IRogueGender Gender => _gender;

            [SerializeField] private SkeletalSpriteData _bone = null;
            public SkeletalSpriteData Bone => _bone;

            [SerializeField] private SpriteMotionSetData _motionSet = null;
            public SpriteMotionSetData MotionSet => _motionSet;

            [SerializeField] private ScriptableAppearance[] _appearances = null;
            public Spanning<ScriptableAppearance> Appearances => _appearances;
        }
    }
}
