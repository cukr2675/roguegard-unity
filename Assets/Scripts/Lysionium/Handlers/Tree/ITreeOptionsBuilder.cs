namespace Lysionium
{
    public interface ITreeOptionsBuilder<TMgr, TArg, TBuilder>
    {
        TBuilder Option();

        TBuilder Option(ITreeOption<TMgr, TArg> option);
    }
}
