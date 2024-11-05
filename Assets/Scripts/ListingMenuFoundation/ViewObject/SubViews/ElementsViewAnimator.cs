using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Elements View Animator")]
    public class ElementsViewAnimator : MonoBehaviour
    {
        [SerializeField] private string _visibleBool = "IsVisible";
        public string VisibleBool => _visibleBool;

        [SerializeField] private string _statusCodeInteger = "StatusCode";
        public string StatusCodeInteger => _statusCodeInteger;

        [Space]
        [SerializeField] private PlayStringEvent _onPlayString = null;
        public PlayStringEvent OnPlayString => _onPlayString;

        [Space]
        [SerializeField] private PlayObjectEvent _onPlayObject = null;
        public PlayObjectEvent OnPlayObject => _onPlayObject;

        [Header("ViewElement")]

        [SerializeField] private string _playOnSelect = "Select";

        [SerializeField] private string _playOnSelectOutOfRange = "SelectOutOfRange";

        [SerializeField] private bool _cancelOnSelectOutOfRange = true;

#if UNITY_EDITOR
        [Header("Debug (Editor Only)")]
        [SerializeField] private bool _log = false;

        private static readonly StringBuilder animatorLog = new();
        private static readonly List<AnimatorClipInfo> clipInfos = new();
#endif

        private GameObject prevSelectedGameObject;
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

            void Log(object value, Object sender)
            {
                // Play �����N�G�X�g�����I�u�W�F�N�g���A�j���[�^�[�����ꍇ�A���̏�Ԃ�\������
                animatorLog.Clear();
                if ((sender is GameObject obj && obj.TryGetComponent<Animator>(out var animator)) ||
                    (sender is Component component && component.TryGetComponent(out animator)))
                {
                    animatorLog.Append("[");

                    var firstElement = true;
                    for (int i = 0; i < animator.layerCount; i++)
                    {
                        // �d�݂��[���̃��C���[�͕\�����Ȃ�
                        if (animator.GetLayerWeight(i) == 0f) continue;

                        // ��؂�J���}
                        if (!firstElement) { animatorLog.Append(", "); }

                        // Base Layer �̓��C���[����\�����Ȃ�
                        if (i >= 1) { animatorLog.Append("<color=grey>").Append(animator.GetLayerName(i)).Append(":</color> "); }

                        // ���C���[���Đ����̃X�e�[�g����\��
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

            string SenderToString(Object sender)
            {
                // �r���[�v�f�͗�
                if (sender is ViewElement || sender is GameObject) return $"<color=green>{sender}</color>";

                // �T�u�r���[�͐�
                else if (sender is IElementsSubView) return $"<color=blue>{sender}</color>";

                // ����ȊO�͒ʏ�F
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
            if (queuedCancelSelection)
            {
                EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
                queuedCancelSelection = false;
            }
        }

        public void OnSelect(GameObject gameObject, bool outOfRange)
        {
            // ���ڑI������ Play �����s�i�I�����L�����Z�����͉������Ȃ��j
            if (!queuedCancelSelection)
            {
                if (outOfRange)
                {
                    _onPlayString.Invoke(_playOnSelectOutOfRange, gameObject);
                }
                else
                {
                    _onPlayString.Invoke(_playOnSelect, gameObject);
                }
            }

            // �͈͊O�̍��ڂ�I�������Ƃ��I�����L�����Z������
            if (_cancelOnSelectOutOfRange && outOfRange)
            {
                queuedCancelSelection = true;
            }
            else
            {
                prevSelectedGameObject = gameObject;
            }
        }

        [System.Serializable] public class PlayStringEvent : UnityEvent<string, Object> { }
        [System.Serializable] public class PlayObjectEvent : UnityEvent<Object, Object> { }
    }
}
