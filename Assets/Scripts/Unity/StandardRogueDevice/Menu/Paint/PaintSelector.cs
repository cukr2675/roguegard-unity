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

        public bool SelectsSplitBone => bone == Bone.Body || bone == Bone.Arm || bone == Bone.Leg;

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
            if (bone == Bone.Hair)
            {
                if (pose == Pose.NormalFront)
                {
                    return "”¯:‘O";
                }
                if (pose == Pose.BackFront)
                {
                    return "”¯:Œã";
                }
            }
            return null;
        }

        public RoguePaintData Get(SewedEquipmentDataItemTable items)
        {
            var item = GetItem(items);
            if (item.FirstSprite == null && item.EquipmentSprite == null)
            {
                var frontPaint = new RoguePaintData();
                var rearPaint = new RoguePaintData();
                item.EquipmentSprite = new RoguePaintBoneSprite(frontPaint, rearPaint, rearPaint, frontPaint);
            }

            switch (pose)
            {
                case Pose.NormalFront:
                    return (item.FirstSprite ?? item.EquipmentSprite).NormalFront;
                case Pose.BackFront:
                    return (item.FirstSprite ?? item.EquipmentSprite).BackFront;
            }
            throw new RogueException();
        }

        public SewedEquipmentDataItem GetItem(SewedEquipmentDataItemTable items)
        {
            switch (bone)
            {
                case Bone.Body:
                    return items.BodyItem ??= new SewedEquipmentDataItem();
                case Bone.Arm:
                    return items.LeftArmItem = items.RightArmItem ??= new SewedEquipmentDataItem();
                case Bone.Leg:
                    return items.LeftLegItem = items.RightLegItem ??= new SewedEquipmentDataItem();
                case Bone.Hair:
                    return items.HairItem ??= new SewedEquipmentDataItem();
            }
            throw new RogueException();
        }

        public enum Bone
        {
            Body,
            Arm,
            Leg,
            Hair
        }

        public enum Pose
        {
            NormalFront,
            BackFront
        }
    }
}
