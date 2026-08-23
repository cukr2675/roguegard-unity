using System.Collections.Generic;

namespace Lysionium.Views
{
    public interface IEventGestureTarget
    {
        IReadOnlyList<string> CandidateEventGestureNames { get; }

        void EventGestureConfirmed(string eventGenstureName);
    }
}
