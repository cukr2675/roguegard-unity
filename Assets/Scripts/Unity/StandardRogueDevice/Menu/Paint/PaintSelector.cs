using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    internal struct PaintSelector
    {
        private readonly Bone bone;
        private readonly Pose pose;

        public PaintSelector(Bone bone, Pose pose)
        {
            this.bone = bone;
            this.pose = pose;
        }

        public string GetName()
        {
            if (bone == Bone.Body)
            {
                if (pose == Pose.NormalFront)
                {
                    return "‘Ì:‘O";
                }
                if (pose == Pose.BackFront)
                {
                    return "‘Ì:Œã";
                }
            }
            if (bone == Bone.Arm)
            {
                if (pose == Pose.NormalFront)
                {
                    return "˜r:‘O";
                }
                if (pose == Pose.BackFront)
                {
                    return "˜r:Œã";
                }
            }
            if (bone == Bone.Leg)
            {
                if (pose == Pose.NormalFront)
                {
                    return "‹r:‘O";
                }
                if (pose == Pose.BackFront)
                {
                    return "‹r:Œã";
                }
            }
            return null;
        }

        public RoguePaintData Get(SewedEquipmentDataItemTable items)
        {
            switch (bone)
            {
                case Bone.Body:
                    return Get(items, items.BodyItem);
                case Bone.Arm:
                    return Get(items, items.LeftArmItem);
                case Bone.Leg:
                    return Get(items, items.LeftLegItem);
            }
            throw new RogueException();
        }

        private RoguePaintData Get(SewedEquipmentDataItemTable items, SewedEquipmentDataItem item)
        {
            if (item == null)
            {
                switch (bone)
                {
                    case Bone.Body:
                        item = items.BodyItem = new SewedEquipmentDataItem();
                        break;
                    case Bone.Arm:
                        item = items.LeftArmItem = items.RightArmItem = new SewedEquipmentDataItem();
                        break;
                    case Bone.Leg:
                        item = items.LeftLegItem = items.RightLegItem = new SewedEquipmentDataItem();
                        break;
                }
            }

            if (item.EquipmentSprite == null)
            {
                var frontPaint = new RoguePaintData();
                var rearPaint = new RoguePaintData();
                item.EquipmentSprite = new RoguePaintBoneSprite(frontPaint, rearPaint, rearPaint, frontPaint);
            }

            switch (pose)
            {
                case Pose.NormalFront:
                    return item.EquipmentSprite.NormalFront;
                case Pose.BackFront:
                    return item.EquipmentSprite.BackFront;
            }
            throw new RogueException();
        }

        public enum Bone
        {
            Body,
            Arm,
            Leg
        }

        public enum Pose
        {
            NormalFront,
            BackFront
        }
    }
}
