using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;
using Roguegard;

namespace RoguegardUnity
{
    public class RogueObjSpriteRenderer : MonoBehaviour, IRogueObjSpriteRenderController
    {
        /// <summary>
        /// このインスタンスを生成・プーリングする親
        /// </summary>
        private RogueSpriteRendererPool parentPool;

        private List<Bone> bones;

        private List<SortingGroup> sortingGroups;

        public int Count => bones.Count;

        private const int sortingGroupMemberCount = 30;

        public void Initialize(RogueSpriteRendererPool parentPool)
        {
            this.parentPool = parentPool;
            bones = new List<Bone>();
            sortingGroups = new List<SortingGroup>();
        }

        public void BePooledParent()
        {
            parentPool.PoolRogueSpriteRenderer(this);
        }

        public void AdjustBones(int count)
        {
            if (count < 0) throw new System.ArgumentOutOfRangeException(nameof(count));

            if (bones.Count != count)
            {
                while (bones.Count < count)
                {
                    // 追加
                    Transform parent;
                    var sortingGroupIndex = bones.Count / sortingGroupMemberCount;
                    if (sortingGroupIndex < sortingGroups.Count)
                    {
                        parent = sortingGroups[sortingGroupIndex].transform;
                    }
                    else
                    {
                        // SortingGroup が足りない場合は追加する。
                        var sortingGroup = parentPool.GetSortingGroup(transform);
                        sortingGroups.Add(sortingGroup);
                        parent = sortingGroup.transform;
                    }
                    var renderer = parentPool.GetRenderer(parent);
                    bones.Add(new Bone(renderer));
                }
                if (bones.Count > count)
                {
                    // 削除
                    var sortingGroupsCount = 1 + ((count - 1) / sortingGroupMemberCount);
                    for (int i = count; i < bones.Count; i++)
                    {
                        parentPool.PoolSpriteRenderer(bones[i].spriteRenderer);
                    }
                    for (int i = sortingGroupsCount; i < sortingGroups.Count; i++)
                    {
                        parentPool.PoolSortingGroup(sortingGroups[i]);
                    }
                    bones.RemoveRange(count, bones.Count - count);
                    sortingGroups.RemoveRange(sortingGroupsCount, sortingGroups.Count - sortingGroupsCount);
                }
            }
        }

        public void SetBoneSprite(
            int index, string name, Sprite sprite, Color color, bool flipX, bool flipY,
            Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
        {
            bones[index].SetSprite(name, sprite, color, flipX, flipY, localPosition, localRotation, localScale);
        }

        public void ClearBoneSprites()
        {
            foreach (var bone in bones)
            {
                bone.ClearSprite();
            }
        }

        private struct Bone
        {
            public readonly SpriteRenderer spriteRenderer;

            public Bone(SpriteRenderer spriteRenderer)
            {
                this.spriteRenderer = spriteRenderer;
            }

            public void SetSprite(
                string name, Sprite sprite, Color color, bool flipX, bool flipY,
                Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
            {
                spriteRenderer.gameObject.name = name;
                spriteRenderer.sprite = sprite;
                spriteRenderer.color = color;
                spriteRenderer.flipX = flipX;
                spriteRenderer.flipY = flipY;
                var transform = spriteRenderer.transform;
                transform.localPosition = localPosition;
                transform.localRotation = localRotation;
                transform.localScale = localScale;
            }

            public void ClearSprite()
            {
                spriteRenderer.gameObject.name = "(Empty)";
                spriteRenderer.sprite = null;
            }

            private static Color RGB2HSV(Color rgb)
            {
                var K = new Vector4(0f, -1f / 3f, 2f / 3f, -1f);
                var p = Vector4.Lerp(
                    new Vector4(rgb.b, rgb.g, K.w, K.z), new Vector4(rgb.g, rgb.b, K.x, K.y), Step(rgb.b, rgb.g));
                var q = Vector4.Lerp(
                    new Vector4(p.x, p.y, p.w, rgb.r), new Vector4(rgb.r, p.y, p.z, p.x), Step(p.x, rgb.r));

                float d = q.x - Mathf.Min(q.w, q.y);
                float e = 1.0e-10f;
                return new Color(Mathf.Abs(q.z + (q.w - q.y) / (6f * d + e)), d / (q.x + e), q.x);

                static float Step(float left, float right)
                {
                    return left <= right ? 1f : 0f;
                }
            }
        }
    }
}
