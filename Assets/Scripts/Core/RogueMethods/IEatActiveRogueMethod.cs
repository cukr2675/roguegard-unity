namespace Roguegard
{
    public interface IEatActiveRogueMethod : IActiveRogueMethod, IRogueDescribable
    {
        Spanning<IKeyword> Edibles { get; }
    }
}
