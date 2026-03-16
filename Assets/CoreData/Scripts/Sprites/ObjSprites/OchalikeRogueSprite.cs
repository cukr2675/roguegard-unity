using OchalikeSprites;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Roguegard
{
    // CharacterCreation で bodyColor からスプライトの種類を変えることがあるため ColoredOchalikeRogueSprite は実装しない。
    public class OchalikeRogueSprite : IRogueObjSprite
    {
        private IReadOnlyOchalikeBone rootBone;
        private OchalikeEvaluatorNode root;

        private TileObject _tile;
        public TileBase Tile => _tile;

        public Color EffectedColor { get; private set; }

        private bool wasChangedEquipments;
        private BoneOrder enabledOrder;
        private int normalBonesCount;
        private int backBonesCount;
        private SpritePose enabledImmutablePose;
        private IOchalikeSpriteRenderController lastRenderController;

        private static readonly OchalikeMorph ochalikeMorph = new();

        private OchalikeRogueSprite()
        {
        }

        /// <summary>
        /// 引数の値が <paramref name="sprite"/> と一致するなら <paramref name="sprite"/> を取得し、違うなら新しく生成する。
        /// </summary>
        public static OchalikeRogueSprite CreateOrReuse(RogueObj self, IReadOnlyOchalikeBone mainBone, Sprite sprite, Color effectedColor)
        {
            if (self.Main.Sprite.Sprite is OchalikeRogueSprite objSprite && mainBone == objSprite.rootBone &&
                sprite == objSprite._tile.sprite && effectedColor == objSprite.EffectedColor)
            {
                return objSprite;
            }
            else
            {
                var tile = ScriptableObject.CreateInstance<TileObject>();
                tile.sprite = sprite;
                return new OchalikeRogueSprite
                {
                    rootBone = mainBone,
                    root = new OchalikeEvaluatorNode(mainBone),
                    _tile = tile,
                    EffectedColor = effectedColor
                };
            }
        }

        public void SetBoneSpriteEffects(RogueObj self, Spanning<IBoneSpriteEffect> effects)
        {
            // IBoneSpriteEffect と IRogueObjSprite の実装をできるだけ切り離すため、テーブルは空の状態で開始する。（バージョンで変更できる？）
            ochalikeMorph.Clear();

            foreach (var effect in effects)
            {
                effect.AffectSprite(self, rootBone, ochalikeMorph);
            }
            root.ApplyMorph(ochalikeMorph);
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

        public void SetTo(IOchalikeSpriteRenderController renderController, SpritePose pose, SpriteDirection direction)
        {
            // 装備が変更されておらず、引数のポーズが前回のポーズと同じかつ不変であれば、更新する必要はない。
            // ただし RenderController が同一の場合に限る。
            if (!wasChangedEquipments && enabledImmutablePose == pose && lastRenderController == renderController) return;

            lastRenderController = renderController;

            UpdateIndex(pose.BoneOrder);

            var bonesCount = pose.Back ? backBonesCount : normalBonesCount;
            renderController.AdjustBones(bonesCount);
            renderController.ClearBoneSprites();

            root.SetTo(
                renderController, pose.BoneTransforms, pose.Back, Vector2.zero, Quaternion.identity, Vector3.one, false, false,
                RoguegardSettings.DefaultColor);

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
