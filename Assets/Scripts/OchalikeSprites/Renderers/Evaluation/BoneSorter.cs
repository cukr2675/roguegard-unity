using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    public static class BoneSorter
    {
        public static int SetIndexAndGetCount<T>(T bone, BoneOrder boneOrder, bool poseBack)
            where T : ISortableBone<T>
        {
            StaticStack<T>.boneStack.Clear();
            var bonesCount = SetChildIndex(bone, boneOrder, BoneBack.Type.ForPose, poseBack, 0);
            while (StaticStack<T>.boneStack.Count >= 1)
            {
                var childBone = StaticStack<T>.boneStack.Pop();
                var isBack = childBone.LocalBack switch
                {
                    BoneBack.Type.ForPose => poseBack,
                    BoneBack.Type.InversePose => !poseBack,
                    BoneBack.Type.ForcedNormal => false,
                    BoneBack.Type.ForcedBack => true,
                    _ => throw new System.Exception()
                };

                if (poseBack) { childBone.BackPoseFrontSpriteIndex = bonesCount; }
                else { childBone.NormalPoseFrontSpriteIndex = bonesCount; }

                var frontSpriteCount = isBack ? childBone.BackFrontSpriteCount : childBone.NormalFrontSpriteCount;
                bonesCount += frontSpriteCount;
            }
            return bonesCount;
        }

        private static int SetChildIndex<T>(T bone, BoneOrder boneOrder, BoneBack.Type backType, bool poseBack, int bonesCount)
            where T : ISortableBone<T>
        {
            for (int i = 0; i < boneOrder.LocalBacks.Count; i++)
            {
                var boneBack = boneOrder.LocalBacks[i];
                if (boneBack.Name == bone.Name)
                {
                    backType = boneBack.LocalBack;
                    break;
                }
            }
            bone.LocalBack = backType;
            var isBack = backType switch
            {
                BoneBack.Type.ForPose => poseBack,
                BoneBack.Type.InversePose => !poseBack,
                BoneBack.Type.ForcedNormal => false,
                BoneBack.Type.ForcedBack => true,
                _ => throw new System.Exception()
            };

            var boneChildren = bone.Children;
            IReadOnlyList<T> frontChildren, rearChildren;
            if (isBack)
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
                            bonesCount = SetChildIndex(child, boneOrder, backType, poseBack, bonesCount);
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
                            bonesCount = SetChildIndex(child, boneOrder, backType, poseBack, bonesCount);
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
                        bonesCount = SetChildIndex(child, boneOrder, backType, poseBack, bonesCount);
                    }
                }
                SetRearIndex(bone, bonesCount);
                StaticStack<T>.boneStack.Push(bone);
                var rearSpriteCount = isBack ? bone.BackRearSpriteCount : bone.NormalRearSpriteCount;
                bonesCount += rearSpriteCount;
                for (int j = 0; j < rearChildren.Count; j++)
                {
                    var child = rearChildren[j];
                    if (GetRearIndex(child) == -1)
                    {
                        bonesCount = SetChildIndex(child, boneOrder, backType, poseBack, bonesCount);
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
                        bonesCount = SetChildIndex(child, boneOrder, backType, poseBack, bonesCount);
                    }
                }
                for (int j = 0; j < rearChildren.Count; j++)
                {
                    var child = rearChildren[j];
                    if (child.Name == name)
                    {
                        bonesCount = SetChildIndex(child, boneOrder, backType, poseBack, bonesCount);
                    }
                }
            }
            return bonesCount;

            int GetRearIndex(T bone)
            {
                if (poseBack) return bone.BackPoseRearSpriteIndex;
                else return bone.NormalPoseRearSpriteIndex;
            }

            void SetRearIndex(T bone, int index)
            {
                if (poseBack) bone.BackPoseRearSpriteIndex = index;
                else bone.NormalPoseRearSpriteIndex = index;
            }
        }

        private static class StaticStack<T>
            where T : ISortableBone<T>
        {
            public static readonly Stack<T> boneStack = new Stack<T>();
        }
    }
}
