using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace ObjectFormer.Unity.RuntimeInspector
{
    [RequireComponent(typeof(Canvas))]
    public class FormInspector : MonoBehaviour
    {
        [SerializeField] private ScrollRect _pageScrollRect = null;
        [SerializeField] private Button _prevButton = null;
        [SerializeField] private Button _nextButton = null;
        [SerializeField] private TMP_Text _pathText = null;
        [SerializeField] private Button _closeButton = null;

        [Header("Open / Close Inspector")]
        [SerializeField] private KeyCode _keyCode = KeyCode.F12;
        [SerializeField] private int _doubleTapFingers = 3;
        [SerializeField] private float _doubleTapInterval = .5f;

        public RectTransform Page => _pageScrollRect.content;
        public object UpdateCoroutineWait => config.UpdateCoroutineWait;

        private Canvas canvas;
        private EventSystem eventSystem;
        private BaseInputModule inputModule;
        private InspectorConfig config;

        private List<object> path;
        private bool prevTripleTap;
        private float doubleTapRemainingTime;

        private static readonly StringBuilder stringBuilder = new StringBuilder();

        public void Initialize(InspectorConfig config)
        {
            canvas = GetComponent<Canvas>();
            eventSystem = FindObjectOfType<EventSystem>();
            inputModule = FindObjectOfType<BaseInputModule>();
            this.config = config;

            path = new List<object>();
            _prevButton.onClick.AddListener(() => Prev());
        }

        private void Update()
        {
            if (Input.GetKeyDown(_keyCode) || GetDoubleTap())
            {
                // インスペクターを開く/閉じる
                canvas.enabled = !canvas.enabled;
            }
        }

        private bool GetDoubleTap()
        {
            var tripleTap = Input.touchCount >= _doubleTapFingers;

            // 3本指タップしたときそのタッチイベントは無効化する
            eventSystem.enabled = !tripleTap;
            if (tripleTap != prevTripleTap)
            {
                if (tripleTap)
                {
                    inputModule.DeactivateModule();
                }
                else
                {
                    inputModule.ActivateModule();
                }
            }

            // 3本指タップが終わったらダブルタップ受付開始
            if (!tripleTap && prevTripleTap)
            {
                doubleTapRemainingTime = _doubleTapInterval;
            }

            // 3本指ダブルタップしたときインスペクターを開く
            if (doubleTapRemainingTime > 0f)
            {
                if (tripleTap)
                {
                    doubleTapRemainingTime = 0f;
                    return true;
                }

                doubleTapRemainingTime -= Time.deltaTime;
            }

            prevTripleTap = tripleTap;
            return false;
        }

        public void SetRoot(object root)
        {
            foreach (Transform child in Page)
            {
                //child.SetParent(null);
                Destroy(child.gameObject);
            }
            //Page.DetachChildren();
            _pageScrollRect.horizontalNormalizedPosition = 0f;
            path.Clear();
            path.Add(root);
            UpdatePathText();
            config.SetFormTo(this, root);
        }

        public void SetTarget(object target)
        {
            foreach (Transform child in Page)
            {
                //child.SetParent(null);
                Destroy(child.gameObject);
            }
            //Page.DetachChildren();
            _pageScrollRect.horizontalNormalizedPosition = 0f;
            path.Add(target);
            UpdatePathText();
            config.SetFormTo(this, target);
        }

        private void Prev()
        {
            if (path.Count <= 1) return;

            foreach (Transform child in Page)
            {
                //child.SetParent(null);
                Destroy(child.gameObject);
            }
            //Page.DetachChildren();
            _pageScrollRect.horizontalNormalizedPosition = 0f;
            path.RemoveAt(path.Count - 1);
            UpdatePathText();
            config.SetFormTo(this, path[path.Count - 1]);
        }

        private void UpdatePathText()
        {
            stringBuilder.Clear();
            //stringBuilder.AppendJoin("/", path);
            stringBuilder.Append(path[path.Count - 1]);
            _pathText.SetText(stringBuilder);
        }

        public void AppendElement(string key, System.Type type, ElementValueGetter getter, ElementValueSetter setter)
        {
            config.AppendElementTo(this, key, type, getter, setter);
        }
    }
}
