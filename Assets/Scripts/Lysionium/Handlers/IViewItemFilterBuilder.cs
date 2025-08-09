namespace Lysionium
{
    public interface IViewItemFilterBuilder<TItem, TMgr, TArg, TBuilder>
    {
        TBuilder Filter(System.Func<TItem, TMgr, TArg, bool> selector);
    }
}
