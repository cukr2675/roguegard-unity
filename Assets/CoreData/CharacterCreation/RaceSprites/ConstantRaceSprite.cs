using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    public class ConstantRaceSprite : ReferableScript, IRaceOptionSprite
    {
        [SerializeField] private BoneData _bone = null;

        [SerializeField] private MotionSetData _motionSet = null;

        public void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out BoneNodeBuilder mainNode, out AppearanceBoneSpriteTable boneSpriteTable)
        {
            var bodyColor = characterCreationData.Race.BodyColor;
            if (_bone != null) { mainNode = BoneNodeBuilder.Create(_bone, bodyColor); }
            else { mainNode = null; }
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
            motionSet = _motionSet;
        }
    }
}
