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

        private static readonly RectInt upperBodyRect = new RectInt(13, 17, 6, 2);
        private static readonly RectInt bodyRect = new RectInt(13, 10, 6, 2);

        public SewedEquipmentDataItemTable() { }

        public SewedEquipmentDataItemTable(SewedEquipmentDataItemTable table)
        {
            BodyItem = table.BodyItem;
            LeftArmItem = table.LeftArmItem;
            RightArmItem = table.RightArmItem;
            LeftLegItem = table.LeftLegItem;
            RightLegItem = table.RightLegItem;
        }

        public AffectableBoneSpriteTable GetTable(Color32 mainColor, Spanning<RoguePaintColor> palette)
        {
            var table = new AffectableBoneSpriteTable();
            var bodyItemOverridesUpperBaseColor = BodyItemOverridesBaseColor(upperBodyRect, palette);
            var bodyItemOverridesLowerBaseColor = BodyItemOverridesBaseColor(bodyRect, palette);

            AddTo(
                table, mainColor, palette, BodyItem,
                BoneKw.UpperBody, new Vector2(.5f, 6f / RoguegardSettings.PixelPerUnit),
                BoneKw.Body, new Vector2(.5f, 20f / RoguegardSettings.PixelPerUnit),
                bodyItemOverridesUpperBaseColor, bodyItemOverridesLowerBaseColor);
            AddTo(
                table, mainColor, palette, LeftArmItem,
                BoneKw.LeftArm, new Vector2(.5f, 6f / RoguegardSettings.PixelPerUnit),
                BoneKw.LeftHand, new Vector2(.5f, 20f / RoguegardSettings.PixelPerUnit));
            AddTo(
                table, mainColor, palette, RightArmItem,
                BoneKw.RightArm, new Vector2(.5f, 6f / RoguegardSettings.PixelPerUnit),
                BoneKw.RightHand, new Vector2(.5f, 20f / RoguegardSettings.PixelPerUnit));
            AddTo(
                table, mainColor, palette, LeftLegItem,
                BoneKw.LeftLeg, new Vector2(.5f, 6f / RoguegardSettings.PixelPerUnit),
                BoneKw.LeftFoot, new Vector2(.5f, 20f / RoguegardSettings.PixelPerUnit));
            AddTo(
                table, mainColor, palette, RightLegItem,
                BoneKw.RightLeg, new Vector2(.5f, 6f / RoguegardSettings.PixelPerUnit),
                BoneKw.RightFoot, new Vector2(.5f, 20f / RoguegardSettings.PixelPerUnit));
            return table;
        }

        /// <summary>
        /// ベースカラーを上書き可能かを取得する。必要な範囲が不透明色で塗りつぶされていれば上書き可能。
        /// </summary>
        private bool BodyItemOverridesBaseColor(RectInt requiredFillRect, Spanning<RoguePaintColor> palette)
        {
            if (BodyItem == null) return false;

            return
                OverridesBaseColor(BodyItem.FirstSprite, requiredFillRect, palette) ||
                OverridesBaseColor(BodyItem.EquipmentSprite, requiredFillRect, palette);
        }

        private static bool OverridesBaseColor(RoguePaintBoneSprite paintBoneSprite, RectInt requiredFillRect, Spanning<RoguePaintColor> palette)
        {
            if (paintBoneSprite == null) return false;
            if (paintBoneSprite.NormalFront == null || paintBoneSprite.BackFront == null) return false;

            for (int y = requiredFillRect.yMin; y < requiredFillRect.yMax; y++)
            {
                for (int x = requiredFillRect.xMin; x < requiredFillRect.xMax; x++)
                {
                    var colorIndex = paintBoneSprite.NormalFront.GetPixel(new Vector2Int(x, y));
                    var color = palette[colorIndex];
                    if (color.A < 1f) return false;

                    colorIndex = paintBoneSprite.BackFront.GetPixel(new Vector2Int(x, y));
                    color = palette[colorIndex];
                    if (color.A < 1f) return false;
                }
            }
            return true;
        }

        private static void AddTo(
            AffectableBoneSpriteTable table, Color32 mainColor, Spanning<RoguePaintColor> palette, SewedEquipmentDataItem item,
            IKeyword upperName, Vector2 upperPivot, IKeyword lowerName, Vector2 lowerPivot,
            bool overridesUpperBaseColor = false, bool overridesLowerBaseColor = false)
        {
            if (item == null) return;

            if (item.FirstSprite != null)
            {
                item.FirstSprite.ToBoneSprite(palette, upperPivot, lowerPivot, out var upperBoneSprite, out var lowerBoneSprite);
                table.SetFirstSprite(upperName, upperBoneSprite, overridesUpperBaseColor);
                table.SetFirstSprite(lowerName, lowerBoneSprite, overridesLowerBaseColor);
            }
            if (item.EquipmentSprite != null)
            {
                item.EquipmentSprite.ToBoneSprite(palette, upperPivot, lowerPivot, out var upperBoneSprite, out var lowerBoneSprite);
                table.AddEquipmentSprite(upperName, upperBoneSprite, mainColor, overridesUpperBaseColor);
                table.AddEquipmentSprite(lowerName, lowerBoneSprite, mainColor, overridesLowerBaseColor);
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
