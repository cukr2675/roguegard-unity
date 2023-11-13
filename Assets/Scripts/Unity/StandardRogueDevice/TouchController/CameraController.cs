using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Roguegard;

namespace RoguegardUnity
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera = null;
        [SerializeField] private RawImage _image = null;

        private RenderTexture renderTexture;

        private RectTransform imageTransform;

        public bool IsCameraMode { get; private set; }

        public Vector2Int ScreenSize { get; private set; }

        private Vector3 pressCameraLocalPosition;

        private bool warningIsEnabled = true;

        public void Initialize(Vector2Int screenSize)
        {
            ScreenSize = screenSize;
            if (renderTexture != null) { RenderTexture.ReleaseTemporary(renderTexture); }
            renderTexture = RenderTexture.GetTemporary(screenSize.x, screenSize.y);
            renderTexture.autoGenerateMips = false;
            renderTexture.filterMode = FilterMode.Point;
            _mainCamera.targetTexture = renderTexture;
            _image.texture = renderTexture;
            imageTransform = _image.rectTransform;
            imageTransform.sizeDelta = screenSize;
        }

        private void OnDestroy()
        {
            RenderTexture.ReleaseTemporary(renderTexture);
        }

        public void UpdateCamera(
            RogueObj player, Vector3 playerPosition, bool drag, bool startsDrag, Vector3 dragRelativePosition, float zoom, Vector3 deltaPosition)
        {
            // ドラッグでカメラ移動
            if (drag)
            {
                if (startsDrag)
                {
                    // 新しくドラッグを始めたとき、開始カメラ位置とドラッグ開始位置を記憶し、カメラモードに切り替える。
                    pressCameraLocalPosition = _mainCamera.transform.localPosition;
                    IsCameraMode = true;
                }

                var position = pressCameraLocalPosition - dragRelativePosition / RoguegardSettings.PixelPerUnit;
                position = new Vector3(AdjustPixel(position.x), AdjustPixel(position.y), position.z);
                _mainCamera.transform.localPosition = position;
            }

            // カメラモードでなければ、プレイヤーキャラクターを追跡する。
            if (!IsCameraMode)
            {
                var position = playerPosition;
                position.y += .5f;
                position.z = -10f;
                position = new Vector3(AdjustPixel(position.x), AdjustPixel(position.y), position.z);
                _mainCamera.transform.localPosition = position;
            }
            else
            {
                // カメラ強制移動
                _mainCamera.transform.localPosition += deltaPosition;
                pressCameraLocalPosition += deltaPosition;
            }

            // カメラの移動範囲を制限する。
            {
                // 制限範囲を広げて、ゲーム内からはタイルマップの位置がわからないようにする
                // （ダンジョンの生成範囲を予測できないようにする）
                var margin = RoguegardSettings.MaxTilemapSize * 16;

                var position = _mainCamera.transform.localPosition;
                if (float.IsNaN(position.x) || float.IsNaN(position.y))
                {
                    // NaN になったらリセット
                    position = playerPosition;
                }
                else
                {
                    position.x = Mathf.Clamp(position.x, -margin.x, margin.x);
                    position.y = Mathf.Clamp(position.y, -margin.y, margin.x);
                }
                _mainCamera.transform.localPosition = position;
            }

            // マウスホイールかピンチイン・ピンチアウトでカメラズーム
            if (zoom >= 1f)
            {
                // ズーム倍率が 1 以上なら RenderTexture を拡大表示させる。
                imageTransform.localScale = new Vector3(zoom, zoom, 1f);
                _mainCamera.orthographicSize = (float)ScreenSize.y / RoguegardSettings.PixelPerUnit * .5f;
            }
            else
            {
                // ズーム倍率が 1 未満なら Camera をズームアウトさせる。
                imageTransform.localScale = Vector3.one;
                _mainCamera.orthographicSize = (float)ScreenSize.y / RoguegardSettings.PixelPerUnit * (.5f / zoom);
            }

            // ドットが崩れないようにカメラ位置を補正する。（カメラをドット単位で移動させる）
            static float AdjustPixel(float value) => Mathf.Round(value * RoguegardSettings.PixelPerUnit) / RoguegardSettings.PixelPerUnit;
        }

        public Vector3 ScreenPointToWorldPoint(Vector2 position)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                imageTransform, position, null, out var localPoint) && warningIsEnabled)
            {
                Debug.LogWarning($"{nameof(RectTransformUtility.ScreenPointToLocalPointInRectangle)} が false を返しました。");
                warningIsEnabled = false;
            }

            var imageSize = imageTransform.rect.size;
            var viewport = (imageSize / 2f + localPoint) / imageSize;
            var worldPoint = _mainCamera.ViewportToWorldPoint(viewport);
            return worldPoint;
        }

        public void TerminateCameraMode()
        {
            IsCameraMode = false;
        }
    }
}
