namespace Lysionium
{
    public interface IKeyOptionListBuilder<TMgr, TArg, TBuilder>
    {
        TBuilder Option();

        TBuilder Option(IKeyOption<TMgr, TArg> option);
    }
}
