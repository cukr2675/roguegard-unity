using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
using SkeletalSprite;

namespace Roguegard
{
    // CharacterCreation で bodyColor からスプライトの種類を変えることがあるため BaseColoredSprite は実装しない。
    public class BoneRogueSprite : IRogueObjSprite
    {
        private IReadOnlyNodeBone rootNode;
        private SkeletalSpriteNode root;

        private TileObject _tile;
        public TileBase Tile => _tile;

        public Color EffectedColor { get; private set; }

        private bool wasChangedEquipments;
        private BoneOrder enabledOrder;
        private int normalBonesCount;
        private int backBonesCount;
        private SpritePose enabledImmutablePose;

        private static readonly AffectableBoneSpriteTable boneSpriteTable = new AffectableBoneSpriteTable();

        private BoneRogueSprite()
        {
        }

        /// <summary>
        /// 引数の値が <paramref name="sprite"/> と一致するなら <paramref name="sprite"/> を取得し、違うなら新しく生成する。
        /// </summary>
        public static BoneRogueSprite CreateOrReuse(RogueObj self, IReadOnlyNodeBone nodeBone, Sprite sprite, Color effectedColor)
        {
            if (self.Main.Sprite.Sprite is BoneRogueSprite objSprite && nodeBone == objSprite.rootNode &&
                sprite == objSprite._tile.sprite && effectedColor == objSprite.EffectedColor)
            {
                return objSprite;
            }
            else
            {
                var instance = new BoneRogueSprite();
                instance.rootNode = nodeBone;
                instance.root = new SkeletalSpriteNode(nodeBone);
                instance._tile = ScriptableObject.CreateInstance<TileObject>();
                instance._tile.sprite = sprite;
                instance.EffectedColor = effectedColor;
                return instance;
            }
        }

        public void SetBoneSpriteEffects(RogueObj self, Spanning<IBoneSpriteEffect> effects)
        {
            // IBoneSpriteEffect と IRogueObjSprite の実装をできるだけ切り離すため、テーブルは空の状態で開始する。（バージョンで変更できる？）
            boneSpriteTable.Clear();

            for (int i = 0; i < effects.Count; i++)
            {
                var effect = effects[i];
                effect.AffectSprite(self, rootNode, boneSpriteTable);
            }
            root.ApplyTable(boneSpriteTable);
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

        public void SetTo(ISkeletalSpriteRenderController renderController, SpritePose pose, SpriteDirection direction)
        {
            // 装備が変更されておらず、引数のポーズが前回のポーズと同じかつ不変であれば、更新する必要はない。
            if (!wasChangedEquipments && enabledImmutablePose == pose) return;

            UpdateIndex(pose.BoneOrder);

            var bonesCount = pose.Back ? backBonesCount : normalBonesCount;
            renderController.AdjustBones(bonesCount);
            renderController.ClearBoneSprites();

            root.SetTo(
                renderController, pose.BoneTransforms, pose.Back, Vector2.zero, Quaternion.identity, Vector3.one, false, false,
                RoguegardSettings.BoneSpriteBaseColor);

            if (pose.IsImmutable) enabledImmutablePose = pose;
            else enabledImmutablePose = null;
        }

        private class TileObject : TileBase
        {
            public Sprite sprite;

            public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
            {
                tileData.sprite = sprite;
            }
        }
    }
}
