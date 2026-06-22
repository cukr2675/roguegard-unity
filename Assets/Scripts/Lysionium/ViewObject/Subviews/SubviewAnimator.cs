using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Lysionium.Views
{
    /// <summary>
    /// Subview のアニメーターパラメータ名と LUI EVTFX を制御するコンポーネント。
    /// このオブジェクトの下の <see cref="Subview"/> に影響を与える
    /// <para>
    /// ビューコンポーネント以外からの参照は非推奨。
    /// <see cref="OnEvtfxString"/> または <see cref="OnEvtfxObject"/> を経由すること
    /// </para>
    /// </summary>
    [AddComponentMenu("UI/Lysionium/LUI Subview Animator")]
    public class SubviewAnimator : MonoBehaviour
    {
        [Tooltip("Subview の AnimatorController の表示/非表示パラメータ名")]
        [SerializeField] private string _visibleBool = "IsVisible";
        public string VisibleBool => _visibleBool;

        [Tooltip("Subview の AnimatorController のメニュー戻り判定パラメータ名")]
        [SerializeField] private string _backBool = "Back";
        public string BackBool => _backBool;

        [Space]

        [Tooltip("LUI EVTFX イベント (string 引数)")]
        [SerializeField] private EvtfxStringEvent _onEvtfxString = null;
        public EvtfxStringEvent OnEvtfxString => _onEvtfxString;

        [Space]

        [Tooltip("LUI EVTFX イベント (object 引数)")]
        [SerializeField] private EvtfxObjectEvent _onEvtfxObject = null;
        public EvtfxObjectEvent OnEvtfxObject => _onEvtfxObject;

        [Header("ViewItem")]

        [Tooltip("範囲内でカーソル移動したとき再生")]
        [SerializeField] private string _evtfxOnSelect = "Select";

        [Tooltip("範囲外にカーソル移動しようとしたとき再生")]
        [SerializeField] private string _evtfxOnSelectOutOfRange = "SelectOutOfRange";

#if UNITY_EDITOR
        [Header("Debug (Editor Only)")]
        [SerializeField] private bool _log = false;

        private static readonly StringBuilder animatorLog = new();
        private static readonly List<AnimatorClipInfo> clipInfos = new();
#endif

        private EventSystem eventSystem;

        /// <summary>
        /// ひとつ前にカーソルで選択されていた項目。選択をはじきたい項目が選択された際この変数の項目に戻す
        /// </summary>
        private GameObject lastSelectedGameObject;

        /// <summary>
        /// カーソル移動キャンセル予約状態
        /// </summary>
        private bool queuedCancelSelection;

        protected virtual void Awake()
        {
#if UNITY_EDITOR
            _onEvtfxString.AddListener(Log);
            _onEvtfxObject.AddListener(Log);

            void Log(object value, object sender)
            {
                if (!_log) return;

                // EVTFX をリクエストしたオブジェクトがアニメーターを持つ場合、その状態を表示する
                animatorLog.Clear();
                if ((sender is GameObject obj && obj.TryGetComponent<Animator>(out var animator)) ||
                    (sender is Component component && component.TryGetComponent(out animator)))
                {
                    animatorLog.Append("[");

                    var firstItem = true;
                    for (int i = 0; i < animator.layerCount; i++)
                    {
                        // 重みがゼロのレイヤーは表示しない
                        if (animator.GetLayerWeight(i) == 0f) continue;

                        // 区切りカンマ
                        if (!firstItem) { animatorLog.Append(", "); }

                        // Base Layer はレイヤー名を表示しない
                        if (i >= 1) { animatorLog.Append("<color=grey>").Append(animator.GetLayerName(i)).Append(":</color> "); }

                        // レイヤーが再生中のステート名を表示
                        animator.GetCurrentAnimatorClipInfo(i, clipInfos);
                        if (clipInfos.Count >= 1)
                        {
                            animatorLog.Append(clipInfos[0].clip.name);
                            if (clipInfos.Count >= 2) { animatorLog.Append(" (+").Append(clipInfos.Count - 1).Append(")"); }
                            firstItem = false;
                        }
                        else if (i >= 1)
                        {
                            animatorLog.Append("<color=grey><No AnimationClip></color>");
                            firstItem = false;
                        }
                    }

                    animatorLog.Append("]");
                }

                Debug.Log($"<color=grey>EVTFX:</color> {value} <color=grey>Sender:</color> {SenderToString(sender)} {animatorLog}");
            }

            string SenderToString(object sender)
            {
                // ビュー要素は緑
                if (sender is ViewItem || sender is GameObject) return $"<color=green>{sender}</color>";

                // Subview は青
                else if (sender is ISubview) return $"<color=blue>{sender}</color>";

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

        protected virtual void Start()
        {
            eventSystem = LuiUtility.GetEventSystem(this);
        }

        protected virtual void Update()
        {
            // 予約されたカーソル移動処理を実行する
            if (queuedCancelSelection)
            {
#if UNITY_EDITOR
                if (_log) { Debug.Log($"Selection was canceled {eventSystem.currentSelectedGameObject} -> {lastSelectedGameObject}"); }
#endif

                eventSystem.SetSelectedGameObject(lastSelectedGameObject);
                queuedCancelSelection = false;
            }

        }

        protected virtual void LateUpdate()
        {
            // 監視されていないカーソル移動を検知する
            var currentSelectedGameObject = eventSystem.currentSelectedGameObject;
            if (currentSelectedGameObject != lastSelectedGameObject)
            {
                // カーソル移動時の EVTFX を実行（タッチ操作中は再生しない）
                if (CursorImageSystem.ShowCursor) { _onEvtfxString.Invoke(_evtfxOnSelect, currentSelectedGameObject); }

                // 選択履歴を更新する
                lastSelectedGameObject = currentSelectedGameObject;
            }
        }

        public void OnSelect(GameObject gameObject, bool outOfRange)
        {
            // カーソル移動時の EVTFX を実行
            if (gameObject != lastSelectedGameObject && // QueueSelect で移動していた場合は再生しない（QueueSelect で再生しない場合は鳴らないようにする）
                CursorImageSystem.ShowCursor) // タッチ操作中は再生しない
            {
                if (outOfRange) { _onEvtfxString.Invoke(_evtfxOnSelectOutOfRange, gameObject); } // 範囲外にカーソル移動しようとしたとき再生
                else { _onEvtfxString.Invoke(_evtfxOnSelect, gameObject); } // 範囲内でカーソル移動したとき再生
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

        public void QueueSelect(GameObject sender, GameObject to, CursorEvtfx evtfx)
        {
            // カーソル移動時の EVTFX を実行
            if (evtfx == CursorEvtfx.SelectOutOfRange) { _onEvtfxString.Invoke(_evtfxOnSelectOutOfRange, sender); }
            else if (evtfx == CursorEvtfx.Select) { _onEvtfxString.Invoke(_evtfxOnSelect, sender); }

            // 即移動させると無限再帰となるためカーソル移動処理を予約
            lastSelectedGameObject = to;
            queuedCancelSelection = true;
        }

        public void QueueSelectToLastSelectedObj(GameObject sender, CursorEvtfx evtfx)
        {
            // カーソル移動時の EVTFX を実行
            if (evtfx == CursorEvtfx.SelectOutOfRange) { _onEvtfxString.Invoke(_evtfxOnSelectOutOfRange, sender); }
            else if (evtfx == CursorEvtfx.Select) { _onEvtfxString.Invoke(_evtfxOnSelect, sender); }

            // 範囲外にカーソル移動したときなどにカーソルを戻す
            // 即戻すと無限再帰となるためカーソル移動キャンセル処理を予約
            queuedCancelSelection = true;
        }

        [System.Serializable] public class EvtfxStringEvent : UnityEvent<string, object> { }
        [System.Serializable] public class EvtfxObjectEvent : UnityEvent<Object, object> { }
    }
}
