namespace Lysionium
{
    public interface ITreeOptionsBuilder<TMgr, TBuilder>
    {
        TBuilder Option();

        TBuilder Option(ITreeOption<TMgr> option);
    }
}
