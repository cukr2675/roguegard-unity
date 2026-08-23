namespace Lysionium.Views
{
    public interface IEventGestureDefinitionRecognizerHandler
    {
        void Handle(string eventName, EventGestureRecognizer recognizer);
    }

    public interface IEventGestureDefinitionRecognizerHandler<T>
    {
        void Handle(string eventName, EventGestureRecognizer recognizer, T eventData);
    }
}
