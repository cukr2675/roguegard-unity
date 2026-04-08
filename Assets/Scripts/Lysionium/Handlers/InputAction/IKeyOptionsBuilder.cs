namespace Lysionium
{
    public interface IKeyOptionsBuilder<TMgr, TBuilder>
    {
        TBuilder Option();

        TBuilder Option(IKeyOption<TMgr> option);
    }
}
