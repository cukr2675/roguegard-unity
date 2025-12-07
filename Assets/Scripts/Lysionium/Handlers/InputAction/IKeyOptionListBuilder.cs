namespace Lysionium
{
    public interface IKeyOptionListBuilder<TMgr, TArg, TBuilder>
    {
        TBuilder Option(IKeyOption option);
    }
}
