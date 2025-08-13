using UnityEditor;
using UnityEngine;

namespace OchalikeSprites.Editor
{
    [CreateAssetMenu(menuName = "Ochalike Sprites/Editor/Humanoid Ochalike Sprite Generator")]
    public class HumanoidOchalikeSpriteDataGenerator : ScriptableObjectGenerator<OchalikeSpriteAsset>
    {
        [SerializeField] private DefaultAsset _spritesFolder = null;

        protected override int Start => 1;
        protected override int Length => 9;

        protected override bool TrySetObject(OchalikeSpriteAsset data, int index)
        {
            var searchInFolders = new string[] { AssetDatabase.GetAssetPath(_spritesFolder) };

            var size = index;
            data.ClearBones();
            data.Add(Body.Create(size, size, searchInFolders));
            data.Add(Chest.Create(size, searchInFolders));
            data.Add(LeftArm.Create(size, size, searchInFolders));
            data.Add(LeftHand.Create(size));
            data.Add(RightArm.Create(size, size, searchInFolders));
            data.Add(RightHand.Create(size));
            data.Add(LeftLeg.Create(size, searchInFolders));
            data.Add(LeftFoot.Create(size));
            data.Add(RightLeg.Create(size, searchInFolders));
            data.Add(RightFoot.Create(size));
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

        private abstract class BaseBone : OchalikeSpriteAsset.Bone
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
            public static OchalikeSpriteAsset.Bone Create(int bodyStature, int legStature, string[] searchInFolders)
            {
                var statureRank = GetBodyStatureRank(legStature);
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body"),
                    BareSprite = GetSprite(bodyStature, searchInFolders),
                    PixelLocalPosition = new Vector3(0f, statureRank),
                    NormalOrderInParent = +0f,
                    BackOrderInParent = +0f,
                    OverridesOnDefaultColor = false
                };
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
            public static OchalikeSpriteAsset.Bone Create(int bodyStature, string[] searchInFolders)
            {
                var statureRank = GetChestStatureRank(bodyStature);
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Chest"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body"),
                    BareSprite = OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, "OSpR_Humanoid_Chest{0}", searchInFolders),
                    PixelLocalPosition = new Vector3(0f, statureRank),
                    NormalOrderInParent = +1f,
                    BackOrderInParent = +1f,
                    OverridesOnDefaultColor = false
                };
            }
        }

        private static class LeftArm
        {
            public static OchalikeSpriteAsset.Bone Create(int bodyStature, int armStature, string[] searchInFolders)
            {
                var statureRank = GetChestStatureRank(bodyStature);
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftArm"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body"),
                    BareSprite = GetArmSprite(armStature, searchInFolders),
                    PixelLocalPosition = new Vector3(3f, statureRank),
                    NormalOrderInParent = +2f,
                    BackOrderInParent = -3f
                };
            }
        }

        private static class LeftHand
        {
            public static OchalikeSpriteAsset.Bone Create(int armStature)
            {
                var statureRank = GetHandStatureRank(armStature);
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftHand"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftArm"),
                    PixelLocalPosition = new Vector3(0f, statureRank),
                    NormalOrderInParent = +1f,
                    BackOrderInParent = +1f
                };
            }
        }

        private static class RightArm
        {
            public static OchalikeSpriteAsset.Bone Create(int bodyStature, int armStature, string[] searchInFolders)
            {
                var statureRank = GetChestStatureRank(bodyStature);
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightArm"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body"),
                    BareSprite = GetArmSprite(armStature, searchInFolders),
                    FlipX = true,
                    PixelLocalPosition = new Vector3(-3f, statureRank),
                    NormalOrderInParent = -3f,
                    BackOrderInParent = +2f
                };
            }
        }

        private static class RightHand
        {
            public static OchalikeSpriteAsset.Bone Create(int armStature)
            {
                var statureRank = GetHandStatureRank(armStature);
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightHand"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightArm"),
                    FlipX = true,
                    PixelLocalPosition = new Vector3(0f, statureRank),
                    NormalOrderInParent = +1f,
                    BackOrderInParent = +1f
                };
            }
        }

        private static class LeftLeg
        {
            public static OchalikeSpriteAsset.Bone Create(int legStature, string[] searchInFolders)
            {
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftLeg"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body"),
                    BareSprite = GetLegSprite(legStature, searchInFolders),
                    PixelLocalPosition = new Vector3(2f, 0f),
                    NormalOrderInParent = -1f,
                    BackOrderInParent = -2f
                };
            }
        }

        private static class LeftFoot
        {
            public static OchalikeSpriteAsset.Bone Create(int legStature)
            {
                var statureRank = GetFootStatureRank(legStature);
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftFoot"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftLeg"),
                    PixelLocalPosition = new Vector3(0f, statureRank),
                    NormalOrderInParent = +1f,
                    BackOrderInParent = +1f
                };
            }
        }

        private static class RightLeg
        {
            public static OchalikeSpriteAsset.Bone Create(int legStature, string[] searchInFolders)
            {
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightLeg"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body"),
                    BareSprite = GetLegSprite(legStature, searchInFolders),
                    PixelLocalPosition = new Vector3(-1f, 0f),
                    NormalOrderInParent = -2f,
                    BackOrderInParent = -1f
                };
            }
        }

        private static class RightFoot
        {
            public static OchalikeSpriteAsset.Bone Create(int legStature)
            {
                var statureRank = GetFootStatureRank(legStature);
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightFoot"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightLeg"),
                    PixelLocalPosition = new Vector3(0f, statureRank),
                    NormalOrderInParent = +1f,
                    BackOrderInParent = +1f
                };
            }
        }

        private static class Head
        {
            public static OchalikeSpriteAsset.Bone Create(int bodyStature, string[] searchInFolders)
            {
                var statureRank = GetHeadStatureRank(bodyStature);
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body"),
                    BareSprite = OchalikeSpritesAssetDatabase.CreateColorRangedBoneSpriteOrNull(true, "OSpR_Humanoid_Head{0}{1}", searchInFolders),
                    PixelLocalPosition = new Vector3(0f, statureRank),
                    NormalOrderInParent = +3f,
                    BackOrderInParent = +3f
                };
            }
        }

        private static class Hair
        {
            public static OchalikeSpriteAsset.Bone Create()
            {
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Hair"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head"),
                    NormalOrderInParent = +1f,
                    BackOrderInParent = +1f
                };
            }
        }

        private static class LeftEar
        {
            public static OchalikeSpriteAsset.Bone Create()
            {
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftEar"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head"),
                    PixelLocalPosition = new Vector3(1f, 7f),
                    NormalOrderInParent = +3f,
                    BackOrderInParent = +3f
                };
            }
        }

        private static class RightEar
        {
            public static OchalikeSpriteAsset.Bone Create()
            {
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightEar"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head"),
                    PixelLocalPosition = new Vector3(-4f, 7f),
                    NormalOrderInParent = +2f,
                    BackOrderInParent = +2f
                };
            }
        }

        private static class LeftEye
        {
            public static OchalikeSpriteAsset.Bone Create()
            {
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LeftEye"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head"),
                    PixelLocalPosition = new Vector3(1f, 3f),
                    NormalOrderInParent = +6f,
                    BackOrderInParent = +6f
                };
            }
        }

        private static class RightEye
        {
            public static OchalikeSpriteAsset.Bone Create()
            {
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("RightEye"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head"),
                    PixelLocalPosition = new Vector3(-4f, 3f),
                    NormalOrderInParent = +5f,
                    BackOrderInParent = +5f
                };
            }
        }

        private static class Mouth
        {
            public static OchalikeSpriteAsset.Bone Create(string[] searchInFolders)
            {
                var clearSprite = OchalikeSpritesAssetDatabase.GetSprite("OSpR_Humanoid_Clear", searchInFolders);
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Mouth"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head"),
                    BareSprite = new ColorRangedBoneSprite(BoneSprite.CreateNFBR_NRBF(clearSprite, clearSprite)),
                    PixelLocalPosition = new Vector3(-1f, 1f),
                    NormalOrderInParent = +4f,
                    BackOrderInParent = +4f
                };
            }
        }

        private static class Wings
        {
            public static OchalikeSpriteAsset.Bone Create(int bodyStature)
            {
                var statureRank = GetChestStatureRank(bodyStature);
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Wing"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body"),
                    PixelLocalPosition = new Vector3(0f, statureRank),
                    NormalOrderInParent = +5f,
                    BackOrderInParent = +5f
                };
            }
        }

        private static class LongHair
        {
            public static OchalikeSpriteAsset.Bone Create()
            {
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("LongHair"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Wings"),
                    NormalOrderInParent = +1f,
                    BackOrderInParent = +1f
                };
            }
        }

        private static class Tail
        {
            public static OchalikeSpriteAsset.Bone Create()
            {
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Tail"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body"),
                    PixelLocalPosition = new Vector3(1f, 2f),
                    NormalOrderInParent = +4f,
                    BackOrderInParent = +4f
                };
            }
        }

        private static class BodyEffect
        {
            public static OchalikeSpriteAsset.Bone Create(string[] searchInFolders)
            {
                var clearSprite = OchalikeSpritesAssetDatabase.GetSprite("OSpR_Humanoid_Clear", searchInFolders);
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("BodyEffect"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Body"),
                    BareSprite = new ColorRangedBoneSprite(BoneSprite.CreateNFBR_NRBF(clearSprite, clearSprite)),
                    NormalOrderInParent = +6f,
                    BackOrderInParent = +6f
                };
            }
        }

        private static class HeadEffect
        {
            public static OchalikeSpriteAsset.Bone Create(string[] searchInFolders)
            {
                var clearSprite = OchalikeSpritesAssetDatabase.GetSprite("OSpR_Humanoid_Clear", searchInFolders);
                return new OchalikeSpriteAsset.Bone
                {
                    BoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("HeadEffect"),
                    ParentBoneName = OchalikeSpritesAssetDatabase.GetBoneKeyword("Head"),
                    BareSprite = new ColorRangedBoneSprite(BoneSprite.CreateNFBR_NRBF(clearSprite, clearSprite)),
                    NormalOrderInParent = +7f,
                    BackOrderInParent = +7f
                };
            }
        }
    }
}
