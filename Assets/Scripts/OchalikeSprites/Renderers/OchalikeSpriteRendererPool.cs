using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace OchalikeSprites
{
    [AddComponentMenu("Ochalike Sprites/Ochalike Sprite Renderer Pool")]
    public class OchalikeSpriteRendererPool : MonoBehaviour
    {
        private readonly Stack<SpriteRenderer> pooledSpriteRenderers = new();
        private readonly Stack<SortingGroup> pooledSortingGroups = new();
        private readonly Stack<OchalikeSpriteRenderer> pooledOchalikeSpriteRenderers = new();
        private readonly Stack<Image> pooledImages = new();
        private readonly Stack<OchalikeImageRenderer> pooledOchalikeImageRenderers = new();

        [SerializeField] private SpriteRenderer _spriteRendererPrefab = null;
        [SerializeField] private Image _imagePrefab = null;

        private void Awake()
        {
            if (_spriteRendererPrefab == null) throw new System.InvalidOperationException($"{nameof(_spriteRendererPrefab)} が設定されていません。");
            if (_imagePrefab == null) throw new System.InvalidOperationException($"{nameof(_imagePrefab)} が設定されていません。");
        }

        public void PoolSpriteRenderer(SpriteRenderer spriteRenderer)
        {
            spriteRenderer.transform.SetParent(transform, false);
            spriteRenderer.gameObject.SetActive(false);
            pooledSpriteRenderers.Push(spriteRenderer);
        }

        public void PoolSortingGroup(SortingGroup sortingGroup)
        {
            sortingGroup.transform.SetParent(transform, false);
            sortingGroup.gameObject.SetActive(false);
            pooledSortingGroups.Push(sortingGroup);
        }

        public void PoolOchalikeSpriteRenderer(OchalikeSpriteRenderer ochalikeSpriteRenderer)
        {
            ochalikeSpriteRenderer.AdjustBones(0);
            ochalikeSpriteRenderer.transform.SetParent(transform, false);
            ochalikeSpriteRenderer.gameObject.SetActive(false);
            pooledOchalikeSpriteRenderers.Push(ochalikeSpriteRenderer);
        }

        public void PoolImage(Image image)
        {
            image.transform.SetParent(transform, false);
            image.gameObject.SetActive(false);
            pooledImages.Push(image);
        }

        public void PoolOchalikeImageRenderer(OchalikeImageRenderer ochalikeImageRenderer)
        {
            ochalikeImageRenderer.AdjustBones(0);
            ochalikeImageRenderer.transform.SetParent(transform, false);
            ochalikeImageRenderer.gameObject.SetActive(false);
            pooledOchalikeImageRenderers.Push(ochalikeImageRenderer);
        }

        public SpriteRenderer GetRenderer(Transform parent)
        {
            SpriteRenderer spriteRenderer;
            if (pooledSpriteRenderers.Count >= 1)
            {
                spriteRenderer = pooledSpriteRenderers.Pop();
                spriteRenderer.transform.SetParent(parent, false);
                spriteRenderer.gameObject.SetActive(true);
            }
            else
            {
                spriteRenderer = Instantiate(_spriteRendererPrefab, parent);
            }
            return spriteRenderer;
        }

        public SortingGroup GetSortingGroup(Transform parent)
        {
            SortingGroup sortingGroup;
            if (pooledSortingGroups.Count >= 1)
            {
                sortingGroup = pooledSortingGroups.Pop();
                sortingGroup.transform.SetParent(parent, false);
                sortingGroup.gameObject.SetActive(true);
            }
            else
            {
                var newObject = new GameObject("SortingGroup");
                newObject.transform.SetParent(parent, false);
                sortingGroup = newObject.AddComponent<SortingGroup>();
            }
            return sortingGroup;
        }

        public OchalikeSpriteRenderer GetOchalikeSpriteRenderer(Transform parent)
        {
            OchalikeSpriteRenderer ochalikeSpriteRenderer;
            if (pooledOchalikeSpriteRenderers.Count >= 1)
            {
                ochalikeSpriteRenderer = pooledOchalikeSpriteRenderers.Pop();
                ochalikeSpriteRenderer.transform.SetParent(parent, false);
                ochalikeSpriteRenderer.gameObject.SetActive(true);
            }
            else
            {
                var newObject = new GameObject("Ochalike Sprite Renderer");
                newObject.transform.SetParent(parent, false);
                ochalikeSpriteRenderer = newObject.AddComponent<OchalikeSpriteRenderer>();
                ochalikeSpriteRenderer.Initialize(this);
            }
            return ochalikeSpriteRenderer;
        }

        public Image GetImage(Transform parent)
        {
            Image image;
            if (pooledImages.Count >= 1)
            {
                image = pooledImages.Pop();
                image.transform.SetParent(parent, false);
                image.gameObject.SetActive(true);
            }
            else
            {
                image = Instantiate(_imagePrefab, parent);
            }
            return image;
        }

        public OchalikeImageRenderer GetOchalikeImageRenderer(RectTransform parent)
        {
            OchalikeImageRenderer ochalikeImageRenderer;
            if (pooledOchalikeImageRenderers.Count >= 1)
            {
                ochalikeImageRenderer = pooledOchalikeImageRenderers.Pop();
                ochalikeImageRenderer.transform.SetParent(parent, false);
                ochalikeImageRenderer.gameObject.SetActive(true);
            }
            else
            {
                var newObject = new GameObject("Ochalike Image Renderer");
                newObject.AddComponent<RectTransform>();
                newObject.transform.SetParent(parent, false);
                var canvasGroup = newObject.AddComponent<CanvasGroup>();
                canvasGroup.ignoreParentGroups = true; // 親オブジェクトの CanvasGroup.alpha で服が透けて下が見えてしまうことを防ぐ
                ochalikeImageRenderer = newObject.AddComponent<OchalikeImageRenderer>();
                ochalikeImageRenderer.Initialize(this);
            }
            return ochalikeImageRenderer;
        }
    }
}
