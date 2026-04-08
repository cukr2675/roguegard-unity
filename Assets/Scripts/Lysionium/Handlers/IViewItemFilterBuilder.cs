namespace Lysionium
{
    public interface IViewItemFilterBuilder<TItem, TMgr, TBuilder>
    {
        TBuilder Filter(System.Func<TItem, TMgr, bool> selector);
    }
}
