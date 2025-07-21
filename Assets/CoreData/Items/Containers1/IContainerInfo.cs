namespace Roguegard
{
    public interface IContainerInfo
    {
        IApplyRogueMethod BeOpened { get; }
        IApplyRogueMethod TakeIn { get; }
        IApplyRogueMethod PutOut { get; }
    }
}
