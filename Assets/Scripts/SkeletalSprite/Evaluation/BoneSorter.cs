using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    public static class BoneSorter
    {
        public static int SetIndexAndGetCount<T>(T bone, BoneOrder boneOrder, bool back)
            where T : ISortableBone<T>
        {
            Stacka<T>.boneStack.Clear();
            var bonesCount = SetChildIndex(bone, boneOrder, BoneBack.Type.ForPose, back, 0);
            while (Stacka<T>.boneStack.Count >= 1)
            {
                var childBone = Stacka<T>.boneStack.Pop();
                int frontSpriteCount;
                if (back)
                {
                    childBone.BackPoseFrontSpriteIndex = bonesCount;
                    frontSpriteCount = childBone.BackFrontSpriteCount;
                }
                else
                {
                    childBone.NormalPoseFrontSpriteIndex = bonesCount;
                    frontSpriteCount = childBone.NormalFrontSpriteCount;
                }
                bonesCount += frontSpriteCount;
            }
            return bonesCount;
        }

        private static int SetChildIndex<T>(T bone, BoneOrder boneOrder, BoneBack.Type back, bool poseBack, int bonesCount)
            where T : ISortableBone<T>
        {
            for (int i = 0; i < boneOrder.LocalBacks.Count; i++)
            {
                var boneBack = boneOrder.LocalBacks[i];
                if (boneBack.Name == bone.Name)
                {
                    back = boneBack.LocalBack;
                    break;
                }
            }
            bone.LocalBack = back;
            var backValue = back switch
            {
                BoneBack.Type.ForPose => poseBack,
                BoneBack.Type.InversePose => !poseBack,
                BoneBack.Type.ForcedNormal => false,
                BoneBack.Type.ForcedBack => true,
                _ => throw new System.Exception()
            };

            var boneChildren = bone.Children;
            IReadOnlyList<T> frontChildren, rearChildren;
            if (backValue)
            {
                frontChildren = boneChildren.BackFrontChildren;
                rearChildren = boneChildren.BackRearChildren;
            }
            else
            {
                frontChildren = boneChildren.NormalFrontChildren;
                rearChildren = boneChildren.NormalRearChildren;
            }

            // リセット
            {
                for (int j = 0; j < frontChildren.Count; j++)
                {
                    var child = frontChildren[j];
                    SetRearIndex(child, -1);
                }
                for (int j = 0; j < rearChildren.Count; j++)
                {
                    var child = rearChildren[j];
                    SetRearIndex(child, -1);
                }
            }

            // 強制的に前（フラグを挟まずに即設定）か後ろ（フラグ設定）に移動させる
            var namesReorderFront = boneOrder.Reorders;
            for (int i = 0; i < namesReorderFront.Count; i++)
            {
                var boneReorder = namesReorderFront[i];
                var name = boneReorder.Name;
                for (int j = 0; j < frontChildren.Count; j++)
                {
                    var child = frontChildren[j];
                    if (child.Name == name)
                    {
                        if (boneReorder.Reorder == BoneReorder.Type.Front)
                        {
                            bonesCount = SetChildIndex(child, boneOrder, back, poseBack, bonesCount);
                        }
                        else if (boneReorder.Reorder == BoneReorder.Type.Rear)
                        {
                            SetRearIndex(child, -2);
                        }
                    }
                }
                for (int j = 0; j < rearChildren.Count; j++)
                {
                    var child = rearChildren[j];
                    if (child.Name == name)
                    {
                        if (boneReorder.Reorder == BoneReorder.Type.Front)
                        {
                            bonesCount = SetChildIndex(child, boneOrder, back, poseBack, bonesCount);
                        }
                        else if (boneReorder.Reorder == BoneReorder.Type.Rear)
                        {
                            SetRearIndex(child, -2);
                        }
                    }
                }
            }

            // デフォルト（フラグが未設定ならここで設定する）
            {
                for (int j = 0; j < frontChildren.Count; j++)
                {
                    var child = frontChildren[j];
                    if (GetRearIndex(child) == -1)
                    {
                        bonesCount = SetChildIndex(child, boneOrder, back, poseBack, bonesCount);
                    }
                }
                SetRearIndex(bone, bonesCount);
                Stacka<T>.boneStack.Push(bone);
                var rearSpriteCount = backValue ? bone.BackRearSpriteCount : bone.NormalRearSpriteCount;
                bonesCount += rearSpriteCount;
                for (int j = 0; j < rearChildren.Count; j++)
                {
                    var child = rearChildren[j];
                    if (GetRearIndex(child) == -1)
                    {
                        bonesCount = SetChildIndex(child, boneOrder, back, poseBack, bonesCount);
                    }
                }
            }

            // 強制的に後ろ（フラグ適用）
            for (int i = 0; i < namesReorderFront.Count; i++)
            {
                // 後ろにする順番を適用するため二重ループにする。
                var boneReorder = namesReorderFront[i];
                if (boneReorder.Reorder != BoneReorder.Type.Rear) continue;

                var name = boneReorder.Name;
                for (int j = 0; j < frontChildren.Count; j++)
                {
                    var child = frontChildren[j];
                    if (child.Name == name)
                    {
                        bonesCount = SetChildIndex(child, boneOrder, back, poseBack, bonesCount);
                    }
                }
                for (int j = 0; j < rearChildren.Count; j++)
                {
                    var child = rearChildren[j];
                    if (child.Name == name)
                    {
                        bonesCount = SetChildIndex(child, boneOrder, back, poseBack, bonesCount);
                    }
                }
            }
            return bonesCount;

            int GetRearIndex(T bone)
            {
                if (backValue) return bone.BackPoseRearSpriteIndex;
                else return bone.NormalPoseRearSpriteIndex;
            }

            void SetRearIndex(T bone, int index)
            {
                if (backValue) bone.BackPoseRearSpriteIndex = index;
                else bone.NormalPoseRearSpriteIndex = index;
            }
        }

        private static class Stacka<T>
            where T : ISortableBone<T>
        {
            public static readonly Stack<T> boneStack = new Stack<T>();
        }
    }
}
