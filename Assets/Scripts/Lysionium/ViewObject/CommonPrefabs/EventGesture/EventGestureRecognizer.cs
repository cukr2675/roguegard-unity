using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/LUI Event Gesture Recognizer")]
    public class EventGestureRecognizer : MonoBehaviour
    {
        [SerializeField] private EventGestureDefinition[] _definitions;

        private readonly Dictionary<string, IEventGestureDefinitionRecognizer> definitionRecognizers = new();
        private readonly List<string> pendingEventGestureNames = new();
        private readonly List<string> confirmingEventGestureNames = new();

        // 設計メモ: ターゲットは基本1つにする
        // - インスタンス数が増大する
        // - 並列判定したいケースは少ない
        private IEventGestureTarget target;

        protected virtual void Awake()
        {
            foreach (var definition in _definitions)
            {
                var definitionRecognizer = definition.CreateDefinitionRecognizer();
                definitionRecognizers.Add(definition.name, definitionRecognizer);
            }
        }

        public float GetGestureProgress(IEventGestureTarget target, string eventGestureName)
        {
            if (target != this.target || !pendingEventGestureNames.Contains(eventGestureName)) return 0f;

            foreach (var pair in definitionRecognizers)
            {
                if (pair.Key == eventGestureName) return pair.Value.GestureProgress;
            }
            return 0f;
        }

        public void SetTarget(IEventGestureTarget target)
        {
            if (target == this.target) return;

            // ターゲット変更前に保留中のイベントジェスチャをすべて非成立として確定する
            ClearAllTargets();

            // ターゲットを変更する
            this.target = target;

            // 新しいターゲットのイベントジェスチャの判定を開始する
            var candidateEventGestureNames = target.CandidateEventGestureNames;
            for (int i = 0; i < candidateEventGestureNames.Count; i++)
            {
                // イベントジェスチャの重複を除いて追加
                if (!pendingEventGestureNames.Contains(candidateEventGestureNames[i]))
                {
                    pendingEventGestureNames.Add(candidateEventGestureNames[i]);
                }
            }
        }

        public void CancelTarget(IEventGestureTarget target)
        {
            if (target == this.target)
            {
                this.target = null;
                pendingEventGestureNames.Clear();
                confirmingEventGestureNames.Clear();
            }
        }

        public void ClearAllTargets()
        {
            // ターゲットクリア前に保留中のイベントジェスチャをすべて非成立として確定する
            pendingEventGestureNames.Clear();
            EvaluateEventGesture();

            target = null;
            confirmingEventGestureNames.Clear();
        }

        public void ConfirmEventGesture(string eventGestureName)
        {
            if (pendingEventGestureNames.Remove(eventGestureName))
            {
                confirmingEventGestureNames.Add(eventGestureName);
            }
        }

        public void DenyEventGesture(string eventGestureName)
        {
            pendingEventGestureNames.Remove(eventGestureName);
        }

        private void EvaluateEventGesture()
        {
            // 保留中のイベントジェスチャがなくなった (すべて処理完了した) ときイベントジェスチャを確定させる
            if (pendingEventGestureNames.Count == 0)
            {
                // 最後に成立したイベントジェスチャで確定する
                target?.EventGestureConfirmed(confirmingEventGestureNames[^1]);

                target = null;
                confirmingEventGestureNames.Clear();
            }
        }

        public void Handle(string eventName)
        {
            foreach (var pair in definitionRecognizers)
            {
                // 保留中ではないイベントジェスチャは更新しない
                if (!pendingEventGestureNames.Contains(pair.Key)) continue;

                if (pair.Value is IEventGestureDefinitionRecognizerHandler definitionRecognizerHandler)
                {
                    definitionRecognizerHandler.Handle(eventName, this);
                }
            }
            EvaluateEventGesture();
        }

        public void Handle<T>(string eventName, T eventData)
        {
            foreach (var pair in definitionRecognizers)
            {
                // 保留中ではないイベントジェスチャは更新しない
                if (!pendingEventGestureNames.Contains(pair.Key)) continue;

                if (pair.Value is IEventGestureDefinitionRecognizerHandler<T> definitionRecognizerHandler)
                {
                    definitionRecognizerHandler.Handle(eventName, this, eventData);
                }
            }
            EvaluateEventGesture();
        }

        protected virtual void Update()
        {
            foreach (var pair in definitionRecognizers)
            {
                // 保留中ではないイベントジェスチャは更新しない
                if (!pendingEventGestureNames.Contains(pair.Key)) continue;

                pair.Value.UpdateRecognizer(this);
            }
            EvaluateEventGesture();
        }
    }
}
