using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lysionium
{
    /// <summary>
    /// &lt;link=SpriteMark:{key}&gt; タグで文字の下にスプライトを敷くコンポーネント
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    [AddComponentMenu("UI/Lysionium/LUI Text Mesh Pro Sprite Marker")]
    public class TextMeshProSpriteMarker : MonoBehaviour
    {
        [SerializeField] private RectTransform _markerGroup;

        [Tooltip("スプライトマーカーを有効化するリンクID")]
        [SerializeField] private string _spriteMarkLinkId = "SpriteMark";

        [SerializeField] private Image _markerImagePrefab;

        [SerializeField] private KeySpritePair[] _markerSprites;

        private TextMeshProUGUI tmp;
        private string spriteMarkLindIdPrefix;
        private bool updateMarkersRequested;
        private readonly List<Image> markers = new();
        private readonly List<(Rect rect, Sprite sprite)> markerRects = new();

        void OnEnable()
        {
            tmp = GetComponent<TextMeshProUGUI>();
            spriteMarkLindIdPrefix = $"{_spriteMarkLinkId}:";
            tmp.OnPreRenderText += OnPreRenderText;
        }

        void OnDisable()
        {
            tmp.OnPreRenderText -= OnPreRenderText;
        }

        void OnPreRenderText(TMP_TextInfo textInfo)
        {
            // link タグを探索
            markerRects.Clear();
            for (int i = 0; i < textInfo.linkCount; i++)
            {
                var link = textInfo.linkInfo[i];
                if (!link.GetLinkID().StartsWith(spriteMarkLindIdPrefix)) continue; // タグ名が一致しない場合スキップ

                // 該当スプライトを取得
                var key = link.GetLinkID()[spriteMarkLindIdPrefix.Length..];
                var markerSprite = GetMarkerSprite(key);

                // 該当矩形を取得
                var startIndex = link.linkTextfirstCharacterIndex;
                var endIndex = startIndex + link.linkTextLength;
                var currentLineNumber = -1;
                var rect = Rect.zero;
                for (int j = startIndex; j < endIndex; j++)
                {
                    var character = textInfo.characterInfo[j];
                    if (!character.isVisible) continue;

                    if (character.lineNumber != currentLineNumber)
                    {
                        currentLineNumber = character.lineNumber;
                        rect.xMin = character.bottomLeft.x;
                        rect.yMin = character.bottomLeft.y;
                        rect.xMax = character.topRight.x;
                        rect.yMax = character.topRight.y;
                    }
                    else
                    {
                        rect.xMin = Mathf.Min(rect.xMin, character.bottomLeft.x);
                        rect.yMin = Mathf.Min(rect.yMin, character.bottomLeft.y);
                        rect.xMax = Mathf.Max(rect.xMax, character.topRight.x);
                        rect.yMax = Mathf.Max(rect.yMax, character.topRight.y);
                    }
                    markerRects.Add((rect, markerSprite));
                }

                // このメソッド内で GameObject を生成するとエラーとなるためリクエストだけにする
                updateMarkersRequested = true;
            }
        }

        protected virtual void Update()
        {
            if (!updateMarkersRequested) return;
            updateMarkersRequested = false;

            // 再利用のために一旦全て非アクティブにする
            foreach (var marker in markers)
            {
                marker.gameObject.SetActive(false);
            }

            for (int i = 0; i < markerRects.Count; i++)
            {
                // マーカー Image 生成/再利用
                Image markerImage;
                if (i < markers.Count)
                {
                    markerImage = markers[i];
                    markerImage.gameObject.SetActive(true);
                }
                else
                {
                    markerImage = Instantiate(_markerImagePrefab, _markerGroup);
                    markers.Add(markerImage);
                }
                markerImage.sprite = markerRects[i].sprite;

                // TextMeshPro のローカル座標から UI 座標に変換し、位置とサイズを計算
                var rect = markerRects[i].rect;
                Vector3 bottomLeft3D = tmp.transform.TransformPoint(new Vector3(rect.xMin, rect.yMin));
                Vector3 topRight3D = tmp.transform.TransformPoint(new Vector3(rect.xMax, rect.yMax));
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    tmp.rectTransform, RectTransformUtility.WorldToScreenPoint(null, bottomLeft3D), null, out var bottomLeft);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    tmp.rectTransform, RectTransformUtility.WorldToScreenPoint(null, topRight3D), null, out var topRight);
                var size = topRight - bottomLeft;

                var markerImageTransform = markerImage.rectTransform;
                markerImageTransform.anchoredPosition = bottomLeft + size / 2;
                markerImageTransform.sizeDelta = size;
            }
        }

        private Sprite GetMarkerSprite(string key)
        {
            foreach (var ks in _markerSprites)
            {
                if (ks.Key == key) return ks.Sprite;
            }
            throw new KeyNotFoundException();
        }

        [System.Serializable]
        private class KeySpritePair
        {
            [SerializeField] private string _key;
            public string Key => _key;

            [SerializeField] private Sprite _sprite;
            public Sprite Sprite => _sprite;
        }
    }
}
