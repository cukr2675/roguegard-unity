namespace Lysionium
{
    public interface ISelectOptionListBuilder<TMgr, TArg, TBuilder>
    {
        TBuilder Option();

        TBuilder Option(ISelectOption<TMgr, TArg> option);
    }
}
