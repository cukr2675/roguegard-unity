namespace Lysionium
{
    public interface IViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder>
    {
        TBuilder Init(System.Func<System.IDisposable> func);

        TBuilder NameFrom(System.Func<TItem, TMgr, TArg, string> selector);

        TBuilder StyleFrom(System.Func<TItem, TMgr, TArg, string> selector);
    }
}
