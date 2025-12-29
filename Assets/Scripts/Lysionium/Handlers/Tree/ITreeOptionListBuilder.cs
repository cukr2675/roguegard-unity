namespace Lysionium
{
    public interface ITreeOptionListBuilder<TMgr, TArg, TBuilder>
    {
        TBuilder Option();

        TBuilder Option(ITreeOption<TMgr, TArg> option);
    }
}
