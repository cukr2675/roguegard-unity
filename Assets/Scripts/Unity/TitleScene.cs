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
            StaticID.Next();
            Application.targetFrameRate = 60;
            ObjformingLogger.Primary = new RoguegardObjformingLogger();
            //RogueMethodAspectState.Logger = new CoreRogueMethodAspectLogger();

            StartCoroutine(Coroutine());
        }

        private IEnumerator Coroutine()
        {
            // 読み込み進捗表示スプライト読み込み
            var loadTitleData = _titleData.LoadAssetAsync();
            yield return loadTitleData;
            if (loadTitleData.Status != AsyncOperationStatus.Succeeded) throw new RogueException($"{typeof(Sprite)} 読み込み失敗");

            var titleData = loadTitleData.Result;

            _image.sprite = titleData.ProgressCircle;
            _image.color = Color.white;
            _image.SetNativeSize();
            Progress(.1f);

            // 各要素読み込み
            var coroutines = titleData.Settings.LoadAsync();
            for (int i = 0; i < coroutines.Length; i++)
            {
                yield return coroutines[i];

                var progress = Mathf.Lerp(.1f, 1f, (float)i / (coroutines.Length - 1));
                Progress(progress);
            }

            yield return null;

            // タイトルメニュー生成
            _image.enabled = false;
            titleData.InstantiateTitleMenu();

            FadeCanvas.FadeIn();
        }

        private void Progress(float progress)
        {
            _image.fillAmount = progress;
        }

        private void Update()
        {
            if (_image.enabled)
            {
                var z = _image.rectTransform.eulerAngles.z + _imageRotationSpeed * Time.deltaTime;
                _image.rectTransform.localRotation = Quaternion.Euler(0f, 0f, z);
            }
        }
    }
}
