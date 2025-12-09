namespace Lysionium
{
    public interface ISelectOptionListBuilder<TMgr, TArg, TBuilder>
    {
        TBuilder Option(ISelectOption<TMgr, TArg> option);
    }
}
