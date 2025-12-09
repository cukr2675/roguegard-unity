namespace Lysionium
{
    public interface IKeyOptionListBuilder<TMgr, TArg, TBuilder>
    {
        TBuilder Option(IKeyOption<TMgr, TArg> option);
    }
}
