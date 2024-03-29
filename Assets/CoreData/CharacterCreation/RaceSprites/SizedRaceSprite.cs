using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    public class SizedRaceSprite : ReferableScript, IRaceOptionSprite
    {
        [SerializeField] private Item[] _items = null;

        public void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out NodeBone mainNode, out AppearanceBoneSpriteTable boneSpriteTable)
        {
            var standardMember = StandardRaceMember.GetMember(characterCreationData.Race);
            var item = _items[standardMember.Size];

            var bodyColor = characterCreationData.Race.BodyColor;
            var hairColor = RogueColorUtility.GetHairColor(characterCreationData);
            mainNode = item.Bone.CreateNodeBone(bodyColor, hairColor.maxColorComponent);

            boneSpriteTable = new AppearanceBoneSpriteTable();
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

            var standardMember = StandardRaceMember.GetMember(characterCreationData.Race);
            var item = _items[standardMember.Size];
            motionSet = item.MotionSet;
        }

        [System.Serializable]
        public class Item
        {
            [SerializeField] private SkeletalSpriteData _bone = null;
            public SkeletalSpriteData Bone => _bone;

            [SerializeField] private SpriteMotionSetData _motionSet = null;
            public SpriteMotionSetData MotionSet => _motionSet;
        }
    }
}
