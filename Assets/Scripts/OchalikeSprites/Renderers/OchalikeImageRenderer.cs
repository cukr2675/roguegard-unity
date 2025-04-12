using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace OchalikeSprites
{
    [AddComponentMenu("Ochalike Sprites/Ochalike Image Renderer")]
    public class OchalikeImageRenderer : MonoBehaviour, IOchalikeSpriteRenderController
    {
        [SerializeField] private int _pixelsPerUnit = OchalikeSpritesUtility.DefaultPixelsPerUnit;

        private OchalikeSpriteRendererPool parentPool;

        private List<Bone> bones;

        public int Count => bones.Count;

        public void Initialize(OchalikeSpriteRendererPool parentPool)
        {
            this.parentPool = parentPool;
            bones = new List<Bone>();
        }

        public void AdjustBones(int count)
        {
            if (count < 0) throw new System.ArgumentOutOfRangeException(nameof(count));

            if (bones.Count != count)
            {
                while (bones.Count < count)
                {
                    // 追加
                    var image = parentPool.GetImage(transform);
                    bones.Add(new Bone(this, image));
                }
                if (bones.Count > count)
                {
                    // 削除
                    for (int i = count; i < bones.Count; i++)
                    {
                        parentPool.PoolImage(bones[i].image);
                    }
                    bones.RemoveRange(count, bones.Count - count);
                }
            }
        }

        public void ClearBoneSprites()
        {
            foreach (var bone in bones)
            {
                bone.ClearSprite();
            }
        }

        public void SetBoneSprite(
            int index, string name, Sprite sprite, Color color, bool flipX, bool flipY,
            Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
        {
            bones[index].SetSprite(name, sprite, color, flipX, flipY, localPosition, localRotation, localScale);
        }

        private struct Bone
        {
            private readonly OchalikeImageRenderer parent;
            public readonly Image image;

            public Bone(OchalikeImageRenderer parent, Image image)
            {
                this.parent = parent;
                this.image = image;
            }

            public void SetSprite(
                string name, Sprite sprite, Color color, bool flipX, bool flipY,
                Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
            {
                image.gameObject.name = name;
                image.sprite = sprite;
                image.color = color;
                image.SetNativeSize();
                var transform = (RectTransform)image.transform;
                transform.anchorMin = transform.anchorMax = Vector2.zero;
                transform.pivot = sprite.pivot / sprite.rect.size;
                transform.localPosition = localPosition * parent._pixelsPerUnit;
                transform.localRotation = localRotation;
                transform.localScale = Vector3.Scale(localScale, new Vector3(flipX ? -1f : 1f, flipY ? -1f : 1f, 1f)) / 2f;
            }

            public void ClearSprite()
            {
                image.gameObject.name = "(Empty)";
                image.sprite = null;
            }
        }
    }
}
