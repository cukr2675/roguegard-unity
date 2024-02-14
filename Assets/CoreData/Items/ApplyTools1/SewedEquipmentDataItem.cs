using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class SewedEquipmentDataItem
    {
        public RoguePaintBoneSprite FirstSprite { get; set; }
        public RoguePaintBoneSprite EquipmentSprite { get; set; }

        public int SplitY { get; set; }
        public int PivotDistance { get; set; }

        public Vector2 GetUpperPivot()
        {
            // Pivot の距離は下→上→下→上の順で移動させる
            // そのほうがアイコンとなる胴部のペイントが自然になる
            var pivotY = SplitY + PivotDistance / 2;
            return new Vector2(.5f, (float)pivotY / RoguePaintData.BoardSize);
        }

        public Vector2 GetLowerPivot()
        {
            // Pivot の距離は下→上→下→上の順で移動させる
            var pivotY = SplitY - (PivotDistance + 1) / 2;
            return new Vector2(.5f, (float)pivotY / RoguePaintData.BoardSize);
        }
    }
}
