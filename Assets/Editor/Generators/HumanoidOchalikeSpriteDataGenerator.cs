using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OchalikeSprites;

namespace Roguegard.Editor
{
    [CreateAssetMenu(menuName = "RoguegardData/Sprite/HumanoidOchalikeSpriteGenerator")]
    public class HumanoidOchalikeSpriteDataGenerator : ScriptableObjectGenerator<OchalikeSpriteData>
    {
        protected override int Start => 1;
        protected override int Length => 9;

        protected override bool TrySetObject(OchalikeSpriteData data, int index)
        {
            var size = index;
            data.ClearBones();
            data.Add(Body.Create(size, size));
            data.Add(UpperBody.Create(size));
            data.Add(LeftArm.Create(size, size));
            data.Add(LeftHand.Create(size));
            data.Add(RightArm.Create(size, size));
            data.Add(RightHand.Create(size));
            data.Add(LeftLeg.Create(size));
            data.Add(LeftFoot.Create(size));
            data.Add(RightLeg.Create(size));
            data.Add(RightFoot.Create(size));
            data.Add(Head.Create(size));
            data.Add(Hair.Create());
            data.Add(LeftEar.Create());
            data.Add(RightEar.Create());
            data.Add(Mouth.Create());
            data.Add(LeftEye.Create());
            data.Add(RightEye.Create());
            data.Add(Wings.Create(size));
            data.Add(Tail.Create());
            data.Add(BodyEffect.Create());
            data.Add(HeadEffect.Create());
            return true;
        }

        private static ColorRangedBoneSprite GetArmSprite(int armStature)
        {
            if (armStature >= 9) return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Arm9{0}");
            if (armStature >= 7) return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Arm7{0}");
            if (armStature >= 5) return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Arm5{0}");
            if (armStature >= 3) return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Arm3{0}");
            return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Arm1{0}");
        }

        private static ColorRangedBoneSprite GetLegSprite(int legStature)
        {
            if (legStature >= 8) return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Leg8{0}");
            if (legStature >= 6) return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Leg6{0}");
            if (legStature >= 4) return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Leg4{0}");
            if (legStature >= 2) return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Leg2{0}");
            return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Leg0{0}");
        }

        private static int GetUpperBodyStatureRank(int bodyStature)
        {
            if (bodyStature >= 9) return 9;
            if (bodyStature >= 7) return 8;
            if (bodyStature >= 5) return 7;
            if (bodyStature >= 3) return 6;
            return 5;
        }

        private static int GetHandStatureRank(int armStature)
        {
            if (armStature >= 9) return -9;
            if (armStature >= 7) return -8;
            if (armStature >= 5) return -7;
            return -6;
        }

        private static int GetFootStatureRank(int legStature)
        {
            if (legStature >= 8) return -10;
            if (legStature >= 6) return -9;
            if (legStature >= 4) return -8;
            if (legStature >= 2) return -7;
            return -6;
        }

        private static int GetBodyStatureRank(int legStature)
        {
            return 1 - GetFootStatureRank(legStature);
        }

        private static int GetHeadStatureRank(int bodyStature)
        {
            return GetUpperBodyStatureRank(bodyStature) + 1;
        }

        private abstract class BaseBone : OchalikeSpriteData.Bone
        {
            public BaseBone()
            {
                OverridesOnDefaultColor = true;
                FlipX = false;
                FlipY = false;
            }
        }

        private static class Body
        {
            public static OchalikeSpriteData.Bone Create(int bodyStature, int legStature)
            {
                var statureRank = GetBodyStatureRank(legStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                bone.Sprite = GetSprite(bodyStature);
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +0f;
                bone.BackOrderInParent = +0f;
                bone.OverridesOnDefaultColor = false;
                return bone;
            }

            private static ColorRangedBoneSprite GetSprite(int bodyStature)
            {
                if (bodyStature >= 9) return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Body9{0}");
                if (bodyStature >= 7) return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Body7{0}");
                if (bodyStature >= 5) return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Body5{0}");
                if (bodyStature >= 3) return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Body3{0}");
                return RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "Body1{0}");
            }
        }

        private static class UpperBody
        {
            public static OchalikeSpriteData.Bone Create(int bodyStature)
            {
                var statureRank = GetUpperBodyStatureRank(bodyStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("UpperBody");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                bone.Sprite = RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "UpperBody{0}");
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +1f;
                bone.BackOrderInParent = +1f;
                bone.OverridesOnDefaultColor = false;
                return bone;
            }
        }

        private static class LeftArm
        {
            public static OchalikeSpriteData.Bone Create(int bodyStature, int armStature)
            {
                var statureRank = GetUpperBodyStatureRank(bodyStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftArm");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                bone.Sprite = GetArmSprite(armStature);
                bone.PixelLocalPosition = new Vector3(3f, statureRank);
                bone.NormalOrderInParent = +2f;
                bone.BackOrderInParent = -3f;
                return bone;
            }
        }

        private static class LeftHand
        {
            public static OchalikeSpriteData.Bone Create(int armStature)
            {
                var statureRank = GetHandStatureRank(armStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftHand");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftArm");
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +1f;
                bone.BackOrderInParent = +1f;
                return bone;
            }
        }

        private static class RightArm
        {
            public static OchalikeSpriteData.Bone Create(int bodyStature, int armStature)
            {
                var statureRank = GetUpperBodyStatureRank(bodyStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("RightArm");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                bone.Sprite = GetArmSprite(armStature);
                bone.FlipX = true;
                bone.PixelLocalPosition = new Vector3(-3f, statureRank);
                bone.NormalOrderInParent = -3f;
                bone.BackOrderInParent = +2f;
                return bone;
            }
        }

        private static class RightHand
        {
            public static OchalikeSpriteData.Bone Create(int armStature)
            {
                var statureRank = GetHandStatureRank(armStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("RightHand");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("RightArm");
                bone.FlipX = true;
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +1f;
                bone.BackOrderInParent = +1f;
                return bone;
            }
        }

        private static class LeftLeg
        {
            public static OchalikeSpriteData.Bone Create(int legStature)
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftLeg");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                bone.Sprite = GetLegSprite(legStature);
                bone.PixelLocalPosition = new Vector3(2f, 0f);
                bone.NormalOrderInParent = -1f;
                bone.BackOrderInParent = -2f;
                return bone;
            }
        }

        private static class LeftFoot
        {
            public static OchalikeSpriteData.Bone Create(int legStature)
            {
                var statureRank = GetFootStatureRank(legStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftFoot");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftLeg");
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +1f;
                bone.BackOrderInParent = +1f;
                return bone;
            }
        }

        private static class RightLeg
        {
            public static OchalikeSpriteData.Bone Create(int legStature)
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("RightLeg");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                bone.Sprite = GetLegSprite(legStature);
                bone.PixelLocalPosition = new Vector3(-1f, 0f);
                bone.NormalOrderInParent = -2f;
                bone.BackOrderInParent = -1f;
                return bone;
            }
        }

        private static class RightFoot
        {
            public static OchalikeSpriteData.Bone Create(int legStature)
            {
                var statureRank = GetFootStatureRank(legStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("RightFoot");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("RightLeg");
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +1f;
                bone.BackOrderInParent = +1f;
                return bone;
            }
        }

        private static class Head
        {
            public static OchalikeSpriteData.Bone Create(int bodyStature)
            {
                var statureRank = GetHeadStatureRank(bodyStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("Head");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                bone.Sprite = RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(true, "Head{0}{1}");
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +3f;
                bone.BackOrderInParent = +3f;
                return bone;
            }
        }

        private static class Hair
        {
            public static OchalikeSpriteData.Bone Create()
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("Hair");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Head");
                bone.NormalOrderInParent = +1f;
                bone.BackOrderInParent = +1f;
                return bone;
            }
        }

        private static class LeftEar
        {
            public static OchalikeSpriteData.Bone Create()
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftEar");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Head");
                bone.PixelLocalPosition = new Vector3(1f, 7f);
                bone.NormalOrderInParent = +3f;
                bone.BackOrderInParent = +3f;
                return bone;
            }
        }

        private static class RightEar
        {
            public static OchalikeSpriteData.Bone Create()
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("RightEar");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Head");
                bone.PixelLocalPosition = new Vector3(-4f, 7f);
                bone.NormalOrderInParent = +2f;
                bone.BackOrderInParent = +2f;
                return bone;
            }
        }

        private static class LeftEye
        {
            public static OchalikeSpriteData.Bone Create()
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftEye");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Head");
                bone.PixelLocalPosition = new Vector3(1f, 3f);
                bone.NormalOrderInParent = +6f;
                bone.BackOrderInParent = +6f;
                return bone;
            }
        }

        private static class RightEye
        {
            public static OchalikeSpriteData.Bone Create()
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("RightEye");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Head");
                bone.PixelLocalPosition = new Vector3(-4f, 3f);
                bone.NormalOrderInParent = +5f;
                bone.BackOrderInParent = +5f;
                return bone;
            }
        }

        private static class Mouth
        {
            public static OchalikeSpriteData.Bone Create()
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("Mouth");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Head");
                var clearSprite = RoguegardAssetDatabase.GetSprite("clear");
                bone.Sprite = new ColorRangedBoneSprite(BoneSprite.CreateNFBR_NRBF(clearSprite, clearSprite));
                bone.PixelLocalPosition = new Vector3(-1f, 1f);
                bone.NormalOrderInParent = +4f;
                bone.BackOrderInParent = +4f;
                return bone;
            }
        }

        private static class Wings
        {
            public static OchalikeSpriteData.Bone Create(int bodyStature)
            {
                var statureRank = GetUpperBodyStatureRank(bodyStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("Wing");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +5f;
                bone.BackOrderInParent = +5f;
                return bone;
            }
        }

        private static class Tail
        {
            public static OchalikeSpriteData.Bone Create()
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("Tail");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                bone.PixelLocalPosition = new Vector3(1f, 2f);
                bone.NormalOrderInParent = +4f;
                bone.BackOrderInParent = +4f;
                return bone;
            }
        }

        private static class BodyEffect
        {
            public static OchalikeSpriteData.Bone Create()
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("BodyEffect");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                var clearSprite = RoguegardAssetDatabase.GetSprite("clear");
                bone.Sprite = new ColorRangedBoneSprite(BoneSprite.CreateNFBR_NRBF(clearSprite, clearSprite));
                bone.NormalOrderInParent = +6f;
                bone.BackOrderInParent = +6f;
                return bone;
            }
        }

        private static class HeadEffect
        {
            public static OchalikeSpriteData.Bone Create()
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = RoguegardAssetDatabase.GetBoneKeyword("HeadEffect");
                bone.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Head");
                var clearSprite = RoguegardAssetDatabase.GetSprite("clear");
                bone.Sprite = new ColorRangedBoneSprite(BoneSprite.CreateNFBR_NRBF(clearSprite, clearSprite));
                bone.NormalOrderInParent = +7f;
                bone.BackOrderInParent = +7f;
                return bone;
            }
        }
    }
}
