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
            out BoneNodeBuilder mainNode, out AppearanceBoneSpriteTable boneSpriteTable)
        {
            var standardMember = StandardRaceMember.GetMember(characterCreationData.Race);
            var item = _items[standardMember.Size];

            var bodyColor = characterCreationData.Race.BodyColor;
            mainNode = BoneNodeBuilder.Create(item.Bone, bodyColor);

            boneSpriteTable = new AppearanceBoneSpriteTable();
        }

        public void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IBoneNode boneNode,
            out RogueObjSprite objSprite, out IMotionSet motionSet)
        {
            var infoSet = self.Main.InfoSet;
            if (boneNode != null)
            {
                objSprite = BoneRogueSprite.CreateOrReuse(self, boneNode, infoSet.Icon, infoSet.Color);
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
            [SerializeField] private BoneData _bone = null;
            public BoneData Bone => _bone;

            [SerializeField] private MotionSetData _motionSet = null;
            public MotionSetData MotionSet => _motionSet;
        }
    }
}
