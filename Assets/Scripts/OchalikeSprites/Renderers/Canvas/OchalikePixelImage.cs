using UnityEngine;
using UnityEngine.UI;

namespace OchalikeSprites
{
    [AddComponentMenu("Ochalike Sprites/Ochalike Pixel Image")]
    [RequireComponent(typeof(RawImage))]
    public class OchalikePixelImage : MonoBehaviour
    {
        [SerializeField] private Camera _sourceCamera = null;
        [SerializeField] private Vector2Int _screenSize = new(1920, 1080);
        [SerializeField] private int _pixelsPerUnit = OchalikeSpritesUtility.DefaultPixelsPerUnit;
        [SerializeField] private float _zoom = 4f;

        private RawImage image;
        private RectTransform imageTransform;
        private RenderTexture renderTexture;

        public Vector2Int ScreenSize => _screenSize;

        protected virtual void Awake()
        {
            if (_sourceCamera != null) { SetSourceCamera(_sourceCamera); }
        }

        public void SetSourceCamera(Camera sourceCamera)
        {
            TryGetComponent(out image);
            _sourceCamera = sourceCamera;

            if (renderTexture != null) { RenderTexture.ReleaseTemporary(renderTexture); }
            renderTexture = RenderTexture.GetTemporary(ScreenSize.x, ScreenSize.y, 1);
            renderTexture.autoGenerateMips = false;
            renderTexture.filterMode = FilterMode.Point;
            sourceCamera.targetTexture = renderTexture;
            image.texture = renderTexture;
            imageTransform = image.rectTransform;
            imageTransform.sizeDelta = ScreenSize;
            SetZoom(_zoom);
        }

        protected virtual void OnDestroy()
        {
            RenderTexture.ReleaseTemporary(renderTexture);
        }

        protected virtual void LateUpdate()
        {
            var position = _sourceCamera.transform.localPosition;
            position = new Vector3(AdjustPixel(position.x), AdjustPixel(position.y), position.z);
            _sourceCamera.transform.localPosition = position;

            // ドットが崩れないようにカメラ位置を補正する。（カメラをドット単位で移動させる）
            float AdjustPixel(float value) => Mathf.Round(value * _pixelsPerUnit) / _pixelsPerUnit;
        }

        public void SetZoom(float zoom)
        {
            _zoom = zoom;
            if (zoom >= 1f)
            {
                // ズーム倍率が 1 以上なら RenderTexture を拡大表示させる。
                imageTransform.localScale = new Vector3(zoom, zoom, 1f);
                _sourceCamera.orthographicSize = (float)ScreenSize.y / _pixelsPerUnit * .5f;
            }
            else
            {
                // ズーム倍率が 1 未満なら Camera をズームアウトさせる。
                imageTransform.localScale = Vector3.one;
                _sourceCamera.orthographicSize = (float)ScreenSize.y / _pixelsPerUnit * (.5f / zoom);
            }
        }

        public Vector2 WorldToScreenPoint(Vector3 position)
        {
            var cameraScreenPoint = _sourceCamera.WorldToScreenPoint(position);
            if (_zoom >= 1f)
            {
                // ズーム倍率が 1 以上なら拡大表示ぶんずらす
                var p = (Vector2)cameraScreenPoint;
                p -= ScreenSize / 2;
                p *= imageTransform.localScale;
                p += ScreenSize / 2;
                return p;
            }
            else
            {
                // ズーム倍率が 1 未満ならそのまま返す
                return cameraScreenPoint;
            }
        }
    }
}
