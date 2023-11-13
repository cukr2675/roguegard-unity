using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Roguegard
{
    // CharacterCreation で bodyColor からスプライトの種類を変えることがあるため BaseColoredSprite は実装しない。
    public class BoneRogueSprite : RogueObjSprite
    {
        private IBoneNode boneRoot;
        private BoneRogueSpriteNode root;
        private Sprite _iconSprite;
        private Color _iconColor;

        public override Sprite IconSprite => _iconSprite;
        public override Color IconColor => _iconColor;

        private bool wasChangedEquipments;
        private BoneOrder enabledOrder;
        private int normalBonesCount;
        private int backBonesCount;
        private BonePose enabledImmutablePose;

        private BoneRogueSprite()
        {
        }

        /// <summary>
        /// 引数の値が <paramref name="sprite"/> と一致するなら <paramref name="sprite"/> を取得し、違うなら新しく生成する。
        /// </summary>
        public static BoneRogueSprite CreateOrReuse(RogueObj self, IBoneNode boneNode, Sprite iconSprite, Color iconColor)
        {
            var sprite = self.Main.Sprite.Sprite;
            if (sprite is BoneRogueSprite objSprite && boneNode == objSprite.boneRoot &&
                iconSprite == objSprite.IconSprite && iconColor == objSprite.IconColor)
            {
                return objSprite;
            }
            else
            {
                var instance = CreateInstance<BoneRogueSprite>();
                instance.boneRoot = boneNode;
                instance.root = new BoneRogueSpriteNode(boneNode);
                instance._iconSprite = iconSprite;
                instance._iconColor = iconColor;
                return instance;
            }
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = IconSprite;
            tileData.color = IconColor;
        }

        public override void SetBoneSpriteEffects(Spanning<IBoneSpriteEffect> effects)
        {
            root.SetEquipmentSprites(boneRoot, effects);
            wasChangedEquipments = true;
        }

        private void UpdateIndex(BoneOrder boneOrder)
        {
            // 装備が変更されておらず、引数のオーダーが前回のオーダーと同じであれば、再ソートする必要はない。
            if (!wasChangedEquipments && BoneOrder.Equals(enabledOrder, boneOrder)) return;

            normalBonesCount = BoneSorter.SetIndexAndGetCount(root, boneOrder, false);
            backBonesCount = BoneSorter.SetIndexAndGetCount(root, boneOrder, true);
            wasChangedEquipments = false;
            enabledOrder = boneOrder;
        }

        public override void SetTo(IRogueObjSpriteRenderController renderController, BonePose pose, RogueDirection direction)
        {
            // 装備が変更されておらず、引数のポーズが前回のポーズと同じかつ不変であれば、更新する必要はない。
            if (!wasChangedEquipments && enabledImmutablePose == pose) return;

            UpdateIndex(pose.BoneOrder);

            var bonesCount = pose.Back ? backBonesCount : normalBonesCount;
            renderController.AdjustBones(bonesCount);
            renderController.ClearBoneSprites();

            root.SetTo(
                renderController, pose.BoneTransforms, pose.Back, Vector2.zero, Quaternion.identity, Vector3.one, false, false);

            if (pose.IsImmutable) enabledImmutablePose = pose;
            else enabledImmutablePose = null;
        }
    }
}
