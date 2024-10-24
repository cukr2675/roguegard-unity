using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Objforming;
using Roguegard;

namespace RoguegardUnity
{
    public class TitleScene : MonoBehaviour
    {
        [SerializeField] private Image _image = null;
        [SerializeField] private float _imageRotationSpeed = 10f;
        [SerializeField] private AssetReferenceT<TitleData> _titleData = null;

        private void Start()
        {
            // Roguegard 全体の初期化処理
            StaticID.Next();
            Application.targetFrameRate = 60;
            ObjformingLogger.Primary = new RoguegardObjformingLogger();
            //RogueMethodAspectState.Logger = new CoreRogueMethodAspectLogger();

            // ゲーム開始
            StartCoroutine(Coroutine());
        }

        private IEnumerator Coroutine()
        {
            // タイトル画面データ読み込み
            var loadTitleData = _titleData.LoadAssetAsync();
            yield return loadTitleData;
            if (loadTitleData.Status != AsyncOperationStatus.Succeeded) throw new RogueException($"{typeof(Sprite)} 読み込み失敗");

            var titleData = loadTitleData.Result;

            // プログレスバースプライト設定
            _image.sprite = titleData.ProgressCircle;
            _image.color = Color.white;
            _image.SetNativeSize();
            Progress(.1f);

            // Roguegard の各要素読み込み
            var coroutines = titleData.Settings.LoadAsync();
            for (int i = 0; i < coroutines.Length; i++)
            {
                yield return coroutines[i];

                var progress = Mathf.Lerp(.1f, 1f, (float)i / (coroutines.Length - 1));
                Progress(progress);
            }

            yield return null;

            // タイトルメニュー表示
            _image.enabled = false;
            titleData.ShowTitleMenu();

            //FadeCanvas.FadeIn();
        }

        /// <summary>
        /// プログレスバー更新処理
        /// </summary>
        private void Progress(float progress)
        {
            _image.fillAmount = progress;
        }

        private void Update()
        {
            // ローディングアニメーション
            if (_image.enabled)
            {
                var z = _image.rectTransform.eulerAngles.z + _imageRotationSpeed * Time.deltaTime;
                _image.rectTransform.localRotation = Quaternion.Euler(0f, 0f, z);
            }
        }
    }
}
