namespace Lysionium
{
    public interface IViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder>
    {
        TBuilder Init(System.Func<System.IDisposable> func);

        TBuilder NameFrom(ItemNameSelector<TItem, TMgr, TArg> selector);

        TBuilder StyleFrom(ItemStyleSelector<TItem, TMgr, TArg> selector);
    }
}
