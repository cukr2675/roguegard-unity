using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Roguegard;

namespace RoguegardUnity
{
    public class FadeCanvas : MonoBehaviour
    {
        [SerializeField] private Image _targetImage = null;
        [SerializeField] private FadeType _fadeType = FadeType.Default;

        private float fadeProgress;

        private float fadeSpeed;

        private float fadeTarget;

        private System.Action onFadeEnd;

        public static FadeCanvas current;

        private static IEnumerator currentCoroutine;

        public static bool FadingNow { get; private set; }

        //private void Awake()
        //{
        //    DontDestroyOnLoad(gameObject);
        //    GetValues(_fadeType, out fadeSpeed);

        //    fadeTarget = 1f;
        //    fadeProgress = fadeTarget;

        //    current = this;

        //    Update();
        //}

        private static void GetValues(
            FadeType fadeType, out float fadeSpeed)
        {
            switch (fadeType)
            {
                case FadeType.Default:
                    fadeSpeed = 2.5f;
                    break;
                default:
                    throw new RogueException();
            }
        }

        //private void Update()
        //{
        //    var deltaFade = fadeSpeed * Time.deltaTime;
        //    fadeProgress = Mathf.MoveTowards(fadeProgress, fadeTarget, deltaFade);

        //    _targetImage.color = new Color(0f, 0f, 0f, fadeProgress);

        //    if (fadeProgress == fadeTarget)
        //    {
        //        // フェードが終了した場合、コンポーネントを無効化 (無駄な Update をなくす) したあとイベントを実行する (enabled をイベントで設定できるようにする)
        //        enabled = false;
        //        onFadeEnd?.Invoke();
        //        _targetImage.StartCoroutine(ResetFadingNow());
        //    }
        //}

        private static IEnumerator ResetFadingNow()
        {
            yield return new WaitForSeconds(.5f);
            FadingNow = false;
        }

        public static void FadeIn(System.Action action = null)
        {
            // 既にフェード中だった場合はエラー
            if (current.enabled)
            {
                //Debug.LogWarning("多重フェード");
                return;
            }

            current._targetImage.StopAllCoroutines();
            FadingNow = true;

            current.fadeTarget = 0f;
            current.onFadeEnd = action;
            current.enabled = true;
        }

        public static void FadeOut(System.Action action = null)
        {
            // 既にフェード中だった場合はエラー
            if (current.enabled)
            {
                //Debug.LogWarning("多重フェード");
                return;
            }

            current._targetImage.StopAllCoroutines();
            FadingNow = true;

            current.fadeTarget = 1f;
            current.onFadeEnd = action;
            current.enabled = true;
        }

        /// <summary>
        /// <see cref="Action"/> 内で <see cref="SceneManager.LoadScene(string)"/> しても
        /// その <see cref="Action"/> 内では読み込まれないので、このメソッドを使用する。
        /// </summary>
        public static void FadeWithLoadScene(string sceneName, System.Action action = null)
        {
            //Debug.Log(sceneName);
            FadeOut(() =>
            {
                var loadSceneOperation = Addressables.LoadSceneAsync(sceneName);
                current.StartCoroutine(FadeInWithLoadScene(loadSceneOperation, action));
            });
        }

        private static IEnumerator FadeInWithLoadScene(AsyncOperationHandle<SceneInstance> loadSceneOperation, System.Action action)
        {
            //while (!loadSceneOperation.isDone)
            //{
            //    yield return null;
            //}
            yield return loadSceneOperation;

            action?.Invoke();
            FadeIn();
        }

        public static void StartCanvasCoroutine(IEnumerator routine)
        {
            if (current == null)
            {
                current = new GameObject().AddComponent<FadeCanvas>();
            }

            currentCoroutine = CreateCoroutine(routine);
            current.StartCoroutine(currentCoroutine);
        }

        private static IEnumerator CreateCoroutine(IEnumerator routine)
        {
            yield return routine;

            currentCoroutine = null;
        }

        private enum FadeType
        {
            Default
        }
    }
}
