using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class SewedEquipmentDataItemTable
    {
        public SewedEquipmentDataItem BodyItem { get; set; }
        public SewedEquipmentDataItem LeftArmItem { get; set; }
        public SewedEquipmentDataItem RightArmItem { get; set; }
        public SewedEquipmentDataItem LeftLegItem { get; set; }
        public SewedEquipmentDataItem RightLegItem { get; set; }
        public SewedEquipmentDataItem HairItem { get; set; }

        private static readonly RectInt upperBodyRect = new RectInt(-3, -2, 6, 2);
        private static readonly RectInt bodyRect = new RectInt(-4, 0, 8, 2);

        public SewedEquipmentDataItemTable() { }

        public SewedEquipmentDataItemTable(SewedEquipmentDataItemTable table)
        {
            BodyItem = table.BodyItem;
            LeftArmItem = table.LeftArmItem;
            RightArmItem = table.RightArmItem;
            LeftLegItem = table.LeftLegItem;
            RightLegItem = table.RightLegItem;
            HairItem = table.HairItem;
        }

        public AffectableBoneSpriteTable GetTable(Color32 mainColor, Spanning<RoguePaintColor> palette)
        {
            var table = new AffectableBoneSpriteTable();

            var bodyItemOverridesUpperBaseColor = BodyItemOverridesBaseColor(true, palette);
            var bodyItemOverridesLowerBaseColor = BodyItemOverridesBaseColor(false, palette);
            AddTo(
                table, mainColor, palette, BodyItem, BoneKw.UpperBody, BoneKw.Body,
                bodyItemOverridesUpperBaseColor, bodyItemOverridesLowerBaseColor);

            AddTo(table, mainColor, palette, LeftArmItem, BoneKw.LeftArm, BoneKw.LeftHand);
            AddTo(table, mainColor, palette, RightArmItem, BoneKw.RightArm, BoneKw.RightHand);
            AddTo(table, mainColor, palette, LeftLegItem, BoneKw.LeftLeg, BoneKw.LeftFoot);
            AddTo(table, mainColor, palette, RightLegItem, BoneKw.RightLeg, BoneKw.RightFoot);
            AddTo(table, mainColor, palette, HairItem, BoneKw.Hair);
            return table;
        }

        /// <summary>
        /// ベースカラーを上書き可能かを取得する。必要な範囲が不透明色で塗りつぶされていれば上書き可能。
        /// </summary>
        private bool BodyItemOverridesBaseColor(bool up, Spanning<RoguePaintColor> palette)
        {
            if (BodyItem == null) return false;

            if (up)
            {
                var upperPivot = BodyItem.GetUpperPivot() * RoguePaintData.BoardSize;
                var upperPivotInt = new Vector2Int((int)upperPivot.x, (int)upperPivot.y);
                return
                    OverridesBaseColor(BodyItem.FirstSprite, upperPivotInt, upperBodyRect, palette) ||
                    OverridesBaseColor(BodyItem.EquipmentSprite, upperPivotInt, upperBodyRect, palette);
            }
            else
            {
                var lowerPivot = BodyItem.GetLowerPivot() * RoguePaintData.BoardSize;
                var lowerPivotInt = new Vector2Int((int)lowerPivot.x, (int)lowerPivot.y);
                return
                    OverridesBaseColor(BodyItem.FirstSprite, lowerPivotInt, bodyRect, palette) ||
                    OverridesBaseColor(BodyItem.EquipmentSprite, lowerPivotInt, bodyRect, palette);
            }
        }

        private static bool OverridesBaseColor(
            RoguePaintBoneSprite paintBoneSprite, Vector2Int pivot, RectInt requiredFillRect, Spanning<RoguePaintColor> palette)
        {
            if (paintBoneSprite == null) return false;
            if (paintBoneSprite.NormalFront == null || paintBoneSprite.BackFront == null) return false;

            for (int y = requiredFillRect.yMin; y < requiredFillRect.yMax; y++)
            {
                for (int x = requiredFillRect.xMin; x < requiredFillRect.xMax; x++)
                {
                    var position = new Vector2Int(x, y) + pivot;
                    if (!RoguePaintData.RectInt.Contains(position)) return false;

                    var colorIndex = paintBoneSprite.NormalFront.GetPixel(position);
                    var color = palette[colorIndex];
                    if (color.A < 1f) return false;

                    colorIndex = paintBoneSprite.BackFront.GetPixel(position);
                    color = palette[colorIndex];
                    if (color.A < 1f) return false;
                }
            }
            return true;
        }

        private static void AddTo(
            AffectableBoneSpriteTable table, Color32 mainColor, Spanning<RoguePaintColor> palette, SewedEquipmentDataItem item,
            IKeyword upperName, IKeyword lowerName = null, bool overridesUpperBaseColor = false, bool overridesLowerBaseColor = false)
        {
            if (item == null) return;

            var upperPivot = item.GetUpperPivot();
            var lowerPivot = item.GetLowerPivot();
            if (1 <= item.SplitY && item.SplitY < 32)
            {
                upperPivot.y = (upperPivot.y * RoguePaintData.BoardSize - item.SplitY) / (RoguePaintData.BoardSize - item.SplitY);
                lowerPivot.y = lowerPivot.y * RoguePaintData.BoardSize / item.SplitY;
            }
            if (item.FirstSprite != null)
            {
                if (lowerName != null)
                {
                    item.FirstSprite.ToBoneSprite(palette, item.SplitY, upperPivot, lowerPivot, out var upperBoneSprite, out var lowerBoneSprite);
                    table.SetFirstSprite(upperName, upperBoneSprite, overridesUpperBaseColor);
                    table.SetFirstSprite(lowerName, lowerBoneSprite, overridesLowerBaseColor);
                }
                else
                {
                    var sprite = item.FirstSprite.ToBoneSprite(palette);
                    table.SetFirstSprite(upperName, sprite, overridesUpperBaseColor);
                }
            }
            if (item.EquipmentSprite != null)
            {
                if (lowerName != null)
                {
                    item.EquipmentSprite.ToBoneSprite(palette, item.SplitY, upperPivot, lowerPivot, out var upperBoneSprite, out var lowerBoneSprite);
                    table.AddEquipmentSprite(upperName, upperBoneSprite, mainColor, overridesUpperBaseColor);
                    table.AddEquipmentSprite(lowerName, lowerBoneSprite, mainColor, overridesLowerBaseColor);
                }
                else
                {
                    var sprite = item.EquipmentSprite.ToBoneSprite(palette);
                    table.AddEquipmentSprite(upperName, sprite, mainColor, overridesUpperBaseColor);
                }
            }
        }

        public Sprite GetIcon(Spanning<RoguePaintColor> palette)
        {
            Sprite icon;

            icon = GetSprite(BodyItem, palette);
            if (icon) return icon;

            icon = GetSprite(LeftArmItem, palette);
            if (icon) return icon;

            icon = GetSprite(RightArmItem, palette);
            if (icon) return icon;

            return null;
        }

        private static Sprite GetSprite(SewedEquipmentDataItem item, Spanning<RoguePaintColor> palette)
        {
            if (item == null) return null;

            var paintBoneSprite = item.EquipmentSprite ?? item.FirstSprite;
            if (paintBoneSprite == null) return null;

            var boneSprite = paintBoneSprite.ToBoneSprite(palette);
            if (boneSprite.NormalFront) return boneSprite.NormalFront;
            if (boneSprite.BackFront) return boneSprite.BackFront;
            if (boneSprite.NormalRear) return boneSprite.NormalRear;
            if (boneSprite.BackRear) return boneSprite.BackRear;
            return null;
        }
    }
}
