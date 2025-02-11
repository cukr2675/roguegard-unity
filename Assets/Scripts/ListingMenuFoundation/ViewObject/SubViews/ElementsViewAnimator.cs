using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ListingMF
{
    /// <summary>
    /// SubView のアニメーターパラメータ名と LMF Play を制御するコンポーネント。このオブジェクトの下の <see cref="ElementsSubView"/> に影響を与える
    /// </summary>
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Elements View Animator")]
    public class ElementsViewAnimator : MonoBehaviour
    {
        [Tooltip("SubView の AnimatorController の表示/非表示パラメータ名")]
        [SerializeField] private string _visibleBool = "IsVisible";
        public string VisibleBool => _visibleBool;

        [Tooltip("SubView の AnimatorController のステータスコードパラメータ名")]
        [SerializeField] private string _statusCodeInteger = "StatusCode";
        public string StatusCodeInteger => _statusCodeInteger;

        [Space]

        [Tooltip("LMF Play イベント (string 引数)")]
        [SerializeField] private PlayStringEvent _onPlayString = null;
        public PlayStringEvent OnPlayString => _onPlayString;

        [Space]

        [Tooltip("LMF Play イベント (object 引数)")]
        [SerializeField] private PlayObjectEvent _onPlayObject = null;
        public PlayObjectEvent OnPlayObject => _onPlayObject;

        [Header("ViewElement")]

        [Tooltip("範囲内でカーソル移動したとき再生")]
        [SerializeField] private string _playOnSelect = "Select";

        [Tooltip("範囲外にカーソル移動しようとしたとき再生")]
        [SerializeField] private string _playOnSelectOutOfRange = "SelectOutOfRange";

#if UNITY_EDITOR
        [Header("Debug (Editor Only)")]
        [SerializeField] private bool _log = false;

        private static readonly StringBuilder animatorLog = new();
        private static readonly List<AnimatorClipInfo> clipInfos = new();
#endif

        /// <summary>
        /// ひとつ前にカーソルで選択されていた項目。選択をはじきたい項目が選択された際この変数の項目に戻す
        /// </summary>
        private GameObject lastSelectedGameObject;

        /// <summary>
        /// カーソル移動キャンセル予約状態
        /// </summary>
        private bool queuedCancelSelection;

        public static ElementsViewAnimator Get(Component obj)
        {
            LMFUtility.TryGetComponentInRecursiveParents<ElementsViewAnimator>(obj.transform, out var viewAnimator);
            return viewAnimator;
        }

        private void Awake()
        {
#if UNITY_EDITOR
            if (_log)
            {
                _onPlayString.AddListener(Log);
                _onPlayObject.AddListener(Log);
            }

            void Log(object value, object sender)
            {
                // Play をリクエストしたオブジェクトがアニメーターを持つ場合、その状態を表示する
                animatorLog.Clear();
                if ((sender is GameObject obj && obj.TryGetComponent<Animator>(out var animator)) ||
                    (sender is Component component && component.TryGetComponent(out animator)))
                {
                    animatorLog.Append("[");

                    var firstElement = true;
                    for (int i = 0; i < animator.layerCount; i++)
                    {
                        // 重みがゼロのレイヤーは表示しない
                        if (animator.GetLayerWeight(i) == 0f) continue;

                        // 区切りカンマ
                        if (!firstElement) { animatorLog.Append(", "); }

                        // Base Layer はレイヤー名を表示しない
                        if (i >= 1) { animatorLog.Append("<color=grey>").Append(animator.GetLayerName(i)).Append(":</color> "); }

                        // レイヤーが再生中のステート名を表示
                        animator.GetCurrentAnimatorClipInfo(i, clipInfos);
                        if (clipInfos.Count >= 1)
                        {
                            animatorLog.Append(clipInfos[0].clip.name);
                            if (clipInfos.Count >= 2) { animatorLog.Append(" (+").Append(clipInfos.Count - 1).Append(")"); }
                            firstElement = false;
                        }
                        else if (i >= 1)
                        {
                            animatorLog.Append("<color=grey><No AnimationClip></color>");
                            firstElement = false;
                        }
                    }

                    animatorLog.Append("]");
                }

                Debug.Log($"<color=grey>Play:</color> {value} <color=grey>Sender:</color> {SenderToString(sender)} {animatorLog}");
            }

            string SenderToString(object sender)
            {
                // ビュー要素は緑
                if (sender is ViewElement || sender is GameObject) return $"<color=green>{sender}</color>";

                // SubView は青
                else if (sender is IElementsSubView) return $"<color=blue>{sender}</color>";

                // それ以外は通常色
                else return $"{sender}";
            }

            //string GetCurrentStateName(Animator animator, int layerIndex, UnityEditor.Animations.AnimatorControllerLayer[] layers)
            //{
            //    var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            //    var layers = ((UnityEditor.Animations.AnimatorController)animator.runtimeAnimatorController).layers;
            //    var layer = layers[layerIndex];
            //    foreach (var state in layer.stateMachine.states)
            //    {
            //        if (stateInfo.IsName(state.state.name)) return state.state.name;
            //    }
            //    return null;
            //}
#endif
        }

        private void Update()
        {
            // 予約されたカーソル移動処理を実行する
            if (queuedCancelSelection)
            {
#if UNITY_EDITOR
                if (_log) { Debug.Log($"Selection was canceled {EventSystem.current.currentSelectedGameObject} -> {lastSelectedGameObject}"); }
#endif

                EventSystem.current.SetSelectedGameObject(lastSelectedGameObject);
                queuedCancelSelection = false;
            }

        }

        private void LateUpdate()
        {
            // 監視されていないカーソル移動を検知する
            var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            if (currentSelectedGameObject != lastSelectedGameObject)
            {
                // カーソル移動時の Play を実行（タッチ操作中は再生しない）
                if (CursorImageSystem.ShowCursor) { _onPlayString.Invoke(_playOnSelect, currentSelectedGameObject); }

                // 選択履歴を更新する
                lastSelectedGameObject = currentSelectedGameObject;
            }
        }

        public void OnSelect(GameObject gameObject, bool outOfRange)
        {
            // カーソル移動時の Play を実行
            if (gameObject != lastSelectedGameObject && // QueueSelect で移動していた場合は再生しない（QueueSelect で再生しない場合は鳴らないようにする）
                CursorImageSystem.ShowCursor) // タッチ操作中は再生しない
            {
                if (outOfRange) { _onPlayString.Invoke(_playOnSelectOutOfRange, gameObject); } // 範囲外にカーソル移動しようとしたとき再生
                else { _onPlayString.Invoke(_playOnSelect, gameObject); } // 範囲内でカーソル移動したとき再生
            }

            if (outOfRange)
            {
                // 範囲外にカーソル移動したときカーソルを戻す
                // 即戻すと無限再帰となるためカーソル移動キャンセル処理を予約
                queuedCancelSelection = true;
            }
            else
            {
                // 範囲内の項目を選択したとき選択履歴を更新する
                lastSelectedGameObject = gameObject;
            }
        }

        public void QueueSelect(GameObject sender, GameObject to, CursorPlay play)
        {
            // カーソル移動時の Play を実行
            if (play == CursorPlay.SelectOutOfRange) { _onPlayString.Invoke(_playOnSelectOutOfRange, sender); }
            else if (play == CursorPlay.Select) { _onPlayString.Invoke(_playOnSelect, sender); }

            // 即移動させると無限再帰となるためカーソル移動処理を予約
            lastSelectedGameObject = to;
            queuedCancelSelection = true;
        }

        public void QueueSelectToLastSelectedObj(GameObject sender, CursorPlay play)
        {
            // カーソル移動時の Play を実行
            if (play == CursorPlay.SelectOutOfRange) { _onPlayString.Invoke(_playOnSelectOutOfRange, sender); }
            else if (play == CursorPlay.Select) { _onPlayString.Invoke(_playOnSelect, sender); }

            // 範囲外にカーソル移動したときなどにカーソルを戻す
            // 即戻すと無限再帰となるためカーソル移動キャンセル処理を予約
            queuedCancelSelection = true;
        }

        [System.Serializable] public class PlayStringEvent : UnityEvent<string, object> { }
        [System.Serializable] public class PlayObjectEvent : UnityEvent<Object, object> { }
    }
}
