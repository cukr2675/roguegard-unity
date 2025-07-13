using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace OchalikeSprites.Editor
{
    [CreateAssetMenu(menuName = "Ochalike Sprites/Editor/Humanoid Ochalike Sprite Generator")]
    public class HumanoidOchalikeSpriteDataGenerator : ScriptableObjectGenerator<OchalikeSpriteData>
    {
        [SerializeField] private DefaultAsset _spritesFolder = null;

        protected override int Start => 1;
        protected override int Length => 9;

        protected override bool TrySetObject(OchalikeSpriteData data, int index)
        {
            var searchInFolders = new string[] { AssetDatabase.GetAssetPath(_spritesFolder) };

            var size = index;
            data.ClearBones();
            data.Add(Body.Create(size, size, searchInFolders));
            data.Add(Chest.Create(size, searchInFolders));
            data.Add(LeftArm.Create(size, size, searchInFolders));
            data.Add(LeftHand.Create(size, searchInFolders));
            data.Add(RightArm.Create(size, size, searchInFolders));
            data.Add(RightHand.Create(size, searchInFolders));
            data.Add(LeftLeg.Create(size, searchInFolders));
            data.Add(LeftFoot.Create(size, searchInFolders));
            data.Add(RightLeg.Create(size, searchInFolders));
            data.Add(RightFoot.Create(size, searchInFolders));
            data.Add(Head.Create(size, searchInFolders));
            data.Add(Hair.Create());
            data.Add(LeftEar.Create());
            data.Add(RightEar.Create());
            data.Add(Mouth.Create(searchInFolders));
            data.Add(LeftEye.Create());
            data.Add(RightEye.Create());
            data.Add(Wings.Create(size));
            data.Add(LongHair.Create());
            data.Add(Tail.Create());
            data.Add(BodyEffect.Create(searchInFolders));
            data.Add(HeadEffect.Create(searchInFolders));
            return true;
        }

        private static ColorRangedBoneSprite GetArmSprite(int armStature, string[] searchInFolders)
        {
            if (armStature >= 9) return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Arm9{0}", searchInFolders);
            if (armStature >= 7) return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Arm7{0}", searchInFolders);
            if (armStature >= 5) return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Arm5{0}", searchInFolders);
            if (armStature >= 3) return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Arm3{0}", searchInFolders);
            return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Arm1{0}", searchInFolders);
        }

        private static ColorRangedBoneSprite GetLegSprite(int legStature, string[] searchInFolders)
        {
            if (legStature >= 8) return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Leg8{0}", searchInFolders);
            if (legStature >= 6) return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Leg6{0}", searchInFolders);
            if (legStature >= 4) return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Leg4{0}", searchInFolders);
            if (legStature >= 2) return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Leg2{0}", searchInFolders);
            return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Leg0{0}", searchInFolders);
        }

        private static int GetChestStatureRank(int bodyStature)
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
            return GetChestStatureRank(bodyStature) + 1;
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
            public static OchalikeSpriteData.Bone Create(int bodyStature, int legStature, string[] searchInFolders)
            {
                var statureRank = GetBodyStatureRank(legStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body");
                bone.BareSprite = GetSprite(bodyStature, searchInFolders);
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +0f;
                bone.BackOrderInParent = +0f;
                bone.OverridesOnDefaultColor = false;
                return bone;
            }

            private static ColorRangedBoneSprite GetSprite(int bodyStature, string[] searchInFolders)
            {
                if (bodyStature >= 9) return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Body9{0}", searchInFolders);
                if (bodyStature >= 7) return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Body7{0}", searchInFolders);
                if (bodyStature >= 5) return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Body5{0}", searchInFolders);
                if (bodyStature >= 3) return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Body3{0}", searchInFolders);
                return OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Body1{0}", searchInFolders);
            }
        }

        private static class Chest
        {
            public static OchalikeSpriteData.Bone Create(int bodyStature, string[] searchInFolders)
            {
                var statureRank = GetChestStatureRank(bodyStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Chest");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body");
                bone.BareSprite = OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Chest{0}", searchInFolders);
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +1f;
                bone.BackOrderInParent = +1f;
                bone.OverridesOnDefaultColor = false;
                return bone;
            }
        }

        private static class LeftArm
        {
            public static OchalikeSpriteData.Bone Create(int bodyStature, int armStature, string[] searchInFolders)
            {
                var statureRank = GetChestStatureRank(bodyStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftArm");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body");
                bone.BareSprite = GetArmSprite(armStature, searchInFolders);
                bone.PixelLocalPosition = new Vector3(3f, statureRank);
                bone.NormalOrderInParent = +2f;
                bone.BackOrderInParent = -3f;
                return bone;
            }
        }

        private static class LeftHand
        {
            public static OchalikeSpriteData.Bone Create(int armStature, string[] searchInFolders)
            {
                var statureRank = GetHandStatureRank(armStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftHand");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftArm");
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +1f;
                bone.BackOrderInParent = +1f;
                return bone;
            }
        }

        private static class RightArm
        {
            public static OchalikeSpriteData.Bone Create(int bodyStature, int armStature, string[] searchInFolders)
            {
                var statureRank = GetChestStatureRank(bodyStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightArm");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body");
                bone.BareSprite = GetArmSprite(armStature, searchInFolders);
                bone.FlipX = true;
                bone.PixelLocalPosition = new Vector3(-3f, statureRank);
                bone.NormalOrderInParent = -3f;
                bone.BackOrderInParent = +2f;
                return bone;
            }
        }

        private static class RightHand
        {
            public static OchalikeSpriteData.Bone Create(int armStature, string[] searchInFolders)
            {
                var statureRank = GetHandStatureRank(armStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightHand");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightArm");
                bone.FlipX = true;
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +1f;
                bone.BackOrderInParent = +1f;
                return bone;
            }
        }

        private static class LeftLeg
        {
            public static OchalikeSpriteData.Bone Create(int legStature, string[] searchInFolders)
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftLeg");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body");
                bone.BareSprite = GetLegSprite(legStature, searchInFolders);
                bone.PixelLocalPosition = new Vector3(2f, 0f);
                bone.NormalOrderInParent = -1f;
                bone.BackOrderInParent = -2f;
                return bone;
            }
        }

        private static class LeftFoot
        {
            public static OchalikeSpriteData.Bone Create(int legStature, string[] searchInFolders)
            {
                var statureRank = GetFootStatureRank(legStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftFoot");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftLeg");
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +1f;
                bone.BackOrderInParent = +1f;
                return bone;
            }
        }

        private static class RightLeg
        {
            public static OchalikeSpriteData.Bone Create(int legStature, string[] searchInFolders)
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightLeg");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body");
                bone.BareSprite = GetLegSprite(legStature, searchInFolders);
                bone.PixelLocalPosition = new Vector3(-1f, 0f);
                bone.NormalOrderInParent = -2f;
                bone.BackOrderInParent = -1f;
                return bone;
            }
        }

        private static class RightFoot
        {
            public static OchalikeSpriteData.Bone Create(int legStature, string[] searchInFolders)
            {
                var statureRank = GetFootStatureRank(legStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightFoot");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightLeg");
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +1f;
                bone.BackOrderInParent = +1f;
                return bone;
            }
        }

        private static class Head
        {
            public static OchalikeSpriteData.Bone Create(int bodyStature, string[] searchInFolders)
            {
                var statureRank = GetHeadStatureRank(bodyStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body");
                bone.BareSprite = OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(true, "OSpR_Humanoid_Head{0}{1}", searchInFolders);
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
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Hair");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head");
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
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftEar");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head");
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
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightEar");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head");
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
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftEye");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head");
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
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightEye");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head");
                bone.PixelLocalPosition = new Vector3(-4f, 3f);
                bone.NormalOrderInParent = +5f;
                bone.BackOrderInParent = +5f;
                return bone;
            }
        }

        private static class Mouth
        {
            public static OchalikeSpriteData.Bone Create(string[] searchInFolders)
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Mouth");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head");
                var clearSprite = OchalikeSpritesAssetDatabase.GetSprite("OSpR_Humanoid_Clear", searchInFolders);
                bone.BareSprite = new ColorRangedBoneSprite(BoneSprite.CreateNFBR_NRBF(clearSprite, clearSprite));
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
                var statureRank = GetChestStatureRank(bodyStature);
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Wing");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body");
                bone.PixelLocalPosition = new Vector3(0f, statureRank);
                bone.NormalOrderInParent = +5f;
                bone.BackOrderInParent = +5f;
                return bone;
            }
        }

        private static class LongHair
        {
            public static OchalikeSpriteData.Bone Create()
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LongHair");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Wings");
                bone.NormalOrderInParent = +1f;
                bone.BackOrderInParent = +1f;
                return bone;
            }
        }

        private static class Tail
        {
            public static OchalikeSpriteData.Bone Create()
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Tail");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body");
                bone.PixelLocalPosition = new Vector3(1f, 2f);
                bone.NormalOrderInParent = +4f;
                bone.BackOrderInParent = +4f;
                return bone;
            }
        }

        private static class BodyEffect
        {
            public static OchalikeSpriteData.Bone Create(string[] searchInFolders)
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("BodyEffect");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body");
                var clearSprite = OchalikeSpritesAssetDatabase.GetSprite("OSpR_Humanoid_Clear", searchInFolders);
                bone.BareSprite = new ColorRangedBoneSprite(BoneSprite.CreateNFBR_NRBF(clearSprite, clearSprite));
                bone.NormalOrderInParent = +6f;
                bone.BackOrderInParent = +6f;
                return bone;
            }
        }

        private static class HeadEffect
        {
            public static OchalikeSpriteData.Bone Create(string[] searchInFolders)
            {
                var bone = new OchalikeSpriteData.Bone();
                bone.BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("HeadEffect");
                bone.ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head");
                var clearSprite = OchalikeSpritesAssetDatabase.GetSprite("OSpR_Humanoid_Clear", searchInFolders);
                bone.BareSprite = new ColorRangedBoneSprite(BoneSprite.CreateNFBR_NRBF(clearSprite, clearSprite));
                bone.NormalOrderInParent = +7f;
                bone.BackOrderInParent = +7f;
                return bone;
            }
        }
    }
}
