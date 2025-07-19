namespace Roguegard
{
    public interface IEatActiveRogueMethod : IActiveRogueMethod, IRogueDescription
    {
        Spanning<IKeyword> Edibles { get; }
    }
}
