namespace Lysionium
{
    public interface IViewItemHandlerBuilder<TItem, TMgr, TBuilder>
    {
        TBuilder Init(System.Func<System.IDisposable> func);

        TBuilder NameFrom(System.Func<TItem, TMgr, string> selector);

        TBuilder StyleFrom(System.Func<TItem, TMgr, string> selector);
    }
}
