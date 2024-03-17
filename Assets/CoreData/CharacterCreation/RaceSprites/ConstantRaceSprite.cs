using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    public class ConstantRaceSprite : ReferableScript, IRaceOptionSprite
    {
        [SerializeField] private SkeletalSpriteData _bone = null;

        [SerializeField] private SpriteMotionSetData _motionSet = null;

        public void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out NodeBone mainNode, out AppearanceBoneSpriteTable boneSpriteTable)
        {
            var bodyColor = characterCreationData.Race.BodyColor;
            var hairColor = RogueColorUtility.GetHairColor(characterCreationData);
            if (_bone != null) { mainNode = _bone.CreateNodeBone(bodyColor, hairColor.maxColorComponent); }
            else { mainNode = null; }
            boneSpriteTable = new AppearanceBoneSpriteTable();
        }

        public void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IReadOnlyNodeBone nodeBone,
            out RogueObjSprite objSprite, out ISpriteMotionSet motionSet)
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
            motionSet = _motionSet;
        }
    }
}
