using System.Collections.Generic;
using UnityEngine.UI;

namespace Lysionium.Views
{
    public abstract class EventGestureViewItem : ViewItem, IEventGestureTarget
    {
        protected EventGestureRecognizer EventGestureRecognizer { get; private set; }

        private IReadOnlyList<string> candidateEventGestureNames;
        IReadOnlyList<string> IEventGestureTarget.CandidateEventGestureNames => candidateEventGestureNames;

        protected virtual void Start()
        {
            EventGestureRecognizer = GetComponentInParent<EventGestureRecognizer>();
        }

        protected void ResetEventGesture(IReadOnlyList<string> candidateEventGestureNames)
        {
            this.candidateEventGestureNames = candidateEventGestureNames;

            // イベントジェスチャの判定中に再バインドされたとき、
            // 前回のバインドでのイベントジェスチャが新しいバインドで継続されないようにキャンセルする
            EventGestureRecognizer.CancelTarget(this);
        }

        /// <summary>
        /// 実装時は <see cref="Selectable.IsInteractable"/> で有効性を検証してから処理する必要がある
        /// </summary>
        protected abstract void EventGestureConfirmed(string eventGenstureName);

        void IEventGestureTarget.EventGestureConfirmed(string eventGenstureName)
            => EventGestureConfirmed(eventGenstureName);
    }
}
