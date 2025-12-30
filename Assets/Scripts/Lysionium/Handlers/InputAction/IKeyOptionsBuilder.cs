namespace Lysionium
{
    public interface IKeyOptionsBuilder<TMgr, TArg, TBuilder>
    {
        TBuilder Option();

        TBuilder Option(IKeyOption<TMgr, TArg> option);
    }
}
