using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.Editor
{
    [CreateAssetMenu(menuName = "RoguegardData/Sprite/HumanoidBoneGenerator")]
    public class HumanoidSkeletalSpriteDataGenerator : ScriptableObjectGenerator<SkeletalSpriteData>
    {
        protected override int Start => 1;
        protected override int Length => 9;

        protected override bool TrySetObject(SkeletalSpriteData data, int index)
        {
            var size = index;
            data.ClearNodes();
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
            data.Add(LeftEye.Create());
            data.Add(RightEye.Create());
            data.Add(LeftWing.Create(size));
            data.Add(RightWing.Create(size));
            data.Add(Tail.Create());
            data.Add(Effect.Create());
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

        private abstract class BaseBone : SkeletalSpriteData.Node
        {
            public BaseBone()
            {
                OverridesBaseColor = true;
                FlipX = false;
                FlipY = false;
            }
        }

        private static class Body
        {
            public static SkeletalSpriteData.Node Create(int bodyStature, int legStature)
            {
                var statureRank = GetBodyStatureRank(legStature);
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                node.Sprite = GetSprite(bodyStature);
                node.PixelLocalPosition = new Vector3(0f, statureRank);
                node.NormalOrderInParent = 0f;
                node.BackOrderInParent = 0f;
                node.OverridesBaseColor = false;
                return node;
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
            public static SkeletalSpriteData.Node Create(int bodyStature)
            {
                var statureRank = GetUpperBodyStatureRank(bodyStature);
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("UpperBody");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                node.Sprite = RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "UpperBody{0}");
                node.PixelLocalPosition = new Vector3(0f, statureRank);
                node.NormalOrderInParent = -1f;
                node.BackOrderInParent = -1f;
                node.OverridesBaseColor = false;
                return node;
            }
        }

        private static class LeftArm
        {
            public static SkeletalSpriteData.Node Create(int bodyStature, int armStature)
            {
                var statureRank = GetUpperBodyStatureRank(bodyStature);
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftArm");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                node.Sprite = GetArmSprite(armStature);
                node.PixelLocalPosition = new Vector3(3f, statureRank);
                node.NormalOrderInParent = -2f;
                node.BackOrderInParent = 3f;
                return node;
            }
        }

        private static class LeftHand
        {
            public static SkeletalSpriteData.Node Create(int armStature)
            {
                var statureRank = GetHandStatureRank(armStature);
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftHand");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftArm");
                node.PixelLocalPosition = new Vector3(0f, statureRank);
                node.NormalOrderInParent = -1f;
                node.BackOrderInParent = -1f;
                return node;
            }
        }

        private static class RightArm
        {
            public static SkeletalSpriteData.Node Create(int bodyStature, int armStature)
            {
                var statureRank = GetUpperBodyStatureRank(bodyStature);
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("RightArm");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                node.Sprite = GetArmSprite(armStature);
                node.FlipX = true;
                node.PixelLocalPosition = new Vector3(-3f, statureRank);
                node.NormalOrderInParent = 3f;
                node.BackOrderInParent = -2f;
                return node;
            }
        }

        private static class RightHand
        {
            public static SkeletalSpriteData.Node Create(int armStature)
            {
                var statureRank = GetHandStatureRank(armStature);
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("RightHand");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("RightArm");
                node.FlipX = true;
                node.PixelLocalPosition = new Vector3(0f, statureRank);
                node.NormalOrderInParent = -1f;
                node.BackOrderInParent = -1f;
                return node;
            }
        }

        private static class LeftLeg
        {
            public static SkeletalSpriteData.Node Create(int legStature)
            {
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftLeg");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                node.Sprite = GetLegSprite(legStature);
                node.PixelLocalPosition = new Vector3(2f, 0f);
                node.NormalOrderInParent = 1f;
                node.BackOrderInParent = 2f;
                return node;
            }
        }

        private static class LeftFoot
        {
            public static SkeletalSpriteData.Node Create(int legStature)
            {
                var statureRank = GetFootStatureRank(legStature);
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftFoot");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftLeg");
                node.PixelLocalPosition = new Vector3(0f, statureRank);
                node.NormalOrderInParent = -1f;
                node.BackOrderInParent = -1f;
                return node;
            }
        }

        private static class RightLeg
        {
            public static SkeletalSpriteData.Node Create(int legStature)
            {
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("RightLeg");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                node.Sprite = GetLegSprite(legStature);
                node.PixelLocalPosition = new Vector3(-1f, 0f);
                node.NormalOrderInParent = 2f;
                node.BackOrderInParent = 1f;
                return node;
            }
        }

        private static class RightFoot
        {
            public static SkeletalSpriteData.Node Create(int legStature)
            {
                var statureRank = GetFootStatureRank(legStature);
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("RightFoot");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("RightLeg");
                node.PixelLocalPosition = new Vector3(0f, statureRank);
                node.NormalOrderInParent = -1f;
                node.BackOrderInParent = -1f;
                return node;
            }
        }

        private static class Head
        {
            public static SkeletalSpriteData.Node Create(int bodyStature)
            {
                var statureRank = GetHeadStatureRank(bodyStature);
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("Head");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                node.Sprite = RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(true, "Head{0}{1}");
                node.PixelLocalPosition = new Vector3(0f, statureRank);
                node.NormalOrderInParent = -3f;
                node.BackOrderInParent = -3f;
                return node;
            }
        }

        private static class Hair
        {
            public static SkeletalSpriteData.Node Create()
            {
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("Hair");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Head");
                node.NormalOrderInParent = -1f;
                node.BackOrderInParent = -1f;
                return node;
            }
        }

        private static class LeftEar
        {
            public static SkeletalSpriteData.Node Create()
            {
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftEar");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Head");
                node.PixelLocalPosition = new Vector3(1f, 7f);
                node.NormalOrderInParent = -3f;
                node.BackOrderInParent = -3f;
                return node;
            }
        }

        private static class RightEar
        {
            public static SkeletalSpriteData.Node Create()
            {
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("RightEar");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Head");
                node.PixelLocalPosition = new Vector3(-4f, 7f);
                node.NormalOrderInParent = -2f;
                node.BackOrderInParent = -2f;
                return node;
            }
        }

        private static class LeftEye
        {
            public static SkeletalSpriteData.Node Create()
            {
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftEye");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Head");
                node.PixelLocalPosition = new Vector3(1f, 3f);
                node.NormalOrderInParent = -5f;
                node.BackOrderInParent = -5f;
                return node;
            }
        }

        private static class RightEye
        {
            public static SkeletalSpriteData.Node Create()
            {
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("RightEye");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Head");
                node.PixelLocalPosition = new Vector3(-4f, 3f);
                node.NormalOrderInParent = -4f;
                node.BackOrderInParent = -4f;
                return node;
            }
        }

        private static class LeftWing
        {
            public static SkeletalSpriteData.Node Create(int bodyStature)
            {
                var statureRank = GetUpperBodyStatureRank(bodyStature);
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("LeftWing");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                node.PixelLocalPosition = new Vector3(3f, statureRank);
                node.NormalOrderInParent = -4f;
                node.BackOrderInParent = -4f;
                return node;
            }
        }

        private static class RightWing
        {
            public static SkeletalSpriteData.Node Create(int bodyStature)
            {
                var statureRank = GetUpperBodyStatureRank(bodyStature);
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("RightWing");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                node.PixelLocalPosition = new Vector3(-1f, statureRank);
                node.NormalOrderInParent = -5f;
                node.BackOrderInParent = -5f;
                return node;
            }
        }

        private static class Tail
        {
            public static SkeletalSpriteData.Node Create()
            {
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("Tail");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                node.PixelLocalPosition = new Vector3(1f, 2f);
                node.NormalOrderInParent = -6f;
                node.BackOrderInParent = -6f;
                return node;
            }
        }

        private static class Effect
        {
            public static SkeletalSpriteData.Node Create()
            {
                var node = new SkeletalSpriteData.Node();
                node.BoneName = RoguegardAssetDatabase.GetBoneKeyword("Effect");
                node.ParentBoneName = RoguegardAssetDatabase.GetBoneKeyword("Body");
                var clearSprite = RoguegardAssetDatabase.GetSprite("clear");
                node.Sprite = new ColorRangedBoneSprite(BoneSprite.CreateNFBR_NRBF(clearSprite, clearSprite));
                node.NormalOrderInParent = -7f;
                node.BackOrderInParent = -7f;
                return node;
            }
        }
    }
}
