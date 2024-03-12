using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;
using RuntimeDotter;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class PaintBoneSprite : IPaintBoneSprite
    {
        public DotterBoard NormalFront { get; set; }
        public DotterBoard NormalRear { get; set; }
        public DotterBoard BackFront { get; set; }
        public DotterBoard BackRear { get; set; }
        public int PivotDistance { get; set; }

        public ISerializableKeyword Bone { get; set; }
        public bool Mirroring { get; set; }
        public bool IsFirst { get; set; }
        public bool OverridesSourceColor { get; set; }

        // Pivot の距離は下→上→下→上の順で移動させる
        // そのほうがアイコンとなる胴部のペイントが自然になりやすい
        private Vector2Int UpperRelationalPivot => new Vector2Int(0, PivotDistance / 2);
        private Vector2Int LowerRelationalPivot => new Vector2Int(0, -(PivotDistance + 1) / 2);

        private static readonly RectInt upperBodyRect = new RectInt(-3, -2, 6, 2);
        private static readonly RectInt bodyRect = new RectInt(-4, 0, 8, 2);

        public IPaintBoneSprite Clone()
        {
            var clone = new PaintBoneSprite();
            clone.NormalFront = NormalFront;
            clone.NormalRear = NormalRear;
            clone.BackFront = BackFront;
            clone.BackRear = BackRear;
            clone.PivotDistance = PivotDistance;
            clone.Bone = Bone;
            clone.Mirroring = Mirroring;
            clone.IsFirst = IsFirst;
            clone.OverridesSourceColor = OverridesSourceColor;
            return clone;
        }

        public bool ShowsSplitLine(DotterBoard board, out Vector2 upperPivot, out Vector2 lowerPivot)
        {
            if (Bone == BoneKw.Body || Bone == BoneKw.LeftArm || Bone == BoneKw.RightArm || Bone == BoneKw.LeftLeg || Bone == BoneKw.RightLeg)
            {
                var center = board.Size / 2;
                var size = (Vector2)board.Size;
                upperPivot = (center + UpperRelationalPivot) / size;
                lowerPivot = (center + LowerRelationalPivot) / size;
                return true;
            }
            else
            {
                upperPivot = lowerPivot = Vector2.zero;
                return false;
            }
        }

        public void AddTo(AffectableBoneSpriteTable table, Color mainColor, Spanning<ShiftableColor> palette)
        {
            if (Bone == BoneKw.Body)
            {
                var overridesUpperBaseColor = OverridesBaseColor(true, upperBodyRect, palette);
                var overridesLowerBaseColor = OverridesBaseColor(false, bodyRect, palette);
                ToBoneSprite(palette, out var upperBoneSprite, out var lowerBoneSprite);
                AddTo(table, BoneKw.UpperBody, upperBoneSprite, mainColor, overridesUpperBaseColor);
                AddTo(table, BoneKw.Body, lowerBoneSprite, mainColor, overridesLowerBaseColor);
            }
            else if (Bone == BoneKw.LeftArm)
            {
                ToBoneSprite(palette, out var upperBoneSprite, out var lowerBoneSprite);
                AddTo(table, BoneKw.LeftArm, upperBoneSprite, mainColor);
                AddTo(table, BoneKw.LeftHand, lowerBoneSprite, mainColor);
                if (Mirroring)
                {
                    AddTo(table, BoneKw.RightArm, upperBoneSprite, mainColor);
                    AddTo(table, BoneKw.RightHand, lowerBoneSprite, mainColor);
                }
            }
            else if (Bone == BoneKw.LeftLeg)
            {
                ToBoneSprite(palette, out var upperBoneSprite, out var lowerBoneSprite);
                AddTo(table, BoneKw.LeftLeg, upperBoneSprite, mainColor);
                AddTo(table, BoneKw.LeftFoot, lowerBoneSprite, mainColor);
                if (Mirroring)
                {
                    AddTo(table, BoneKw.RightLeg, upperBoneSprite, mainColor);
                    AddTo(table, BoneKw.RightFoot, lowerBoneSprite, mainColor);
                }
            }
            else
            {
                var sprite = ToBoneSprite(palette);
                AddTo(table, Bone, sprite, mainColor);
            }
        }

        private void AddTo(AffectableBoneSpriteTable table, IKeyword name, BoneSprite sprite, Color color, bool overridesBaseColor = false)
        {
            if (IsFirst)
            {
                if (OverridesSourceColor) { table.SetFirstSprite(new BoneKeyword(name.Name), sprite, color, true); }
                else { table.SetFirstSprite(new BoneKeyword(name.Name), sprite, overridesBaseColor); }
            }
            else
            {
                table.AddEquipmentSprite(new BoneKeyword(name.Name), sprite, color, overridesBaseColor);
            }
        }

        /// <summary>
        /// ベースカラーを上書き可能かを取得する。必要な範囲が不透明色で塗りつぶされていれば上書き可能。
        /// </summary>
        private bool OverridesBaseColor(bool up, RectInt requiredFillRect, Spanning<ShiftableColor> palette)
        {
            if (NormalFront == null || BackFront == null) return false;

            var normalPivot = NormalFront.Size / 2;
            var backPivot = BackFront.Size / 2;
            if (up)
            {
                normalPivot += UpperRelationalPivot;
                backPivot += UpperRelationalPivot;
            }
            else
            {
                normalPivot += LowerRelationalPivot;
                backPivot += LowerRelationalPivot;
            }
            for (int y = requiredFillRect.yMin; y < requiredFillRect.yMax; y++)
            {
                for (int x = requiredFillRect.xMin; x < requiredFillRect.xMax; x++)
                {
                    var normalPosition = new Vector2Int(x, y) + normalPivot;
                    var backPosition = new Vector2Int(x, y) + backPivot;
                    if (!NormalFront.RectInt.Contains(normalPosition)) return false;
                    if (!BackFront.RectInt.Contains(backPosition)) return false;

                    var colorIndex = NormalFront.GetPixel(normalPosition);
                    var color = palette[colorIndex];
                    if (color.APercent < 100) return false;

                    colorIndex = BackFront.GetPixel(backPosition);
                    color = palette[colorIndex];
                    if (color.APercent < 100) return false;
                }
            }
            return true;
        }

        public Sprite GetIcon(Spanning<ShiftableColor> palette)
        {
            var boneSprite = ToBoneSprite(palette);
            if (boneSprite.NormalFront) return boneSprite.NormalFront;
            if (boneSprite.BackFront) return boneSprite.BackFront;
            if (boneSprite.NormalRear) return boneSprite.NormalRear;
            if (boneSprite.BackRear) return boneSprite.BackRear;
            return null;
        }

        private BoneSprite ToBoneSprite(Spanning<ShiftableColor> palette)
        {
            return new BoneSprite(
                ToSprite(NormalFront, palette), ToSprite(NormalRear, palette),
                ToSprite(BackFront, palette), ToSprite(BackRear, palette));
        }

        private void ToBoneSprite(Spanning<ShiftableColor> palette, out BoneSprite upperBoneSprite, out BoneSprite lowerBoneSprite)
        {
            ToSprite(NormalFront, palette, out var upperNormalFront, out var lowerNormalFront);
            ToSprite(NormalRear, palette, out var upperNormalRear, out var lowerNormalRear);
            ToSprite(BackFront, palette, out var upperBackFront, out var lowerBackFront);
            ToSprite(BackRear, palette, out var upperBackRear, out var lowerBackRear);
            upperBoneSprite = new BoneSprite(upperNormalFront, upperNormalRear, upperBackFront, upperBackRear);
            lowerBoneSprite = new BoneSprite(lowerNormalFront, lowerNormalRear, lowerBackFront, lowerBackRear);
        }

        private static Sprite ToSprite(DotterBoard board, Spanning<ShiftableColor> palette)
        {
            // WebGL でドットがつぶれないようミップマップを無効化する
            var texture = new Texture2D(board.Size.x, board.Size.y, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            board.SetPixelsTo(texture, palette.Span);
            texture.Apply();
            var sprite = Sprite.Create(texture, board.Rect, new Vector2(.5f, .5f), RoguegardSettings.PixelsPerUnit);
            return sprite;
        }

        private void ToSprite(DotterBoard board, Spanning<ShiftableColor> palette, out Sprite upperSprite, out Sprite lowerSprite)
        {
            // WebGL でドットがつぶれないようミップマップを無効化する
            var texture = new Texture2D(board.Size.x, board.Size.y, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            board.SetPixelsTo(texture, palette.Span);
            texture.Apply();
            var splittedSize = new Vector2(board.Size.x, board.Size.y / 2);
            var upperPivot = (new Vector2Int(board.Size.x / 2, 0) + UpperRelationalPivot) / splittedSize;
            var upperRect = new Rect(0f, splittedSize.y, board.Size.x, splittedSize.y);
            upperSprite = Sprite.Create(texture, upperRect, upperPivot, RoguegardSettings.PixelsPerUnit);
            var lowerPivot = (board.Size / 2 + LowerRelationalPivot) / splittedSize;
            var lowerRect = new Rect(0f, 0f, board.Size.x, splittedSize.y);
            lowerSprite = Sprite.Create(texture, lowerRect, lowerPivot, RoguegardSettings.PixelsPerUnit);
        }
    }
}
