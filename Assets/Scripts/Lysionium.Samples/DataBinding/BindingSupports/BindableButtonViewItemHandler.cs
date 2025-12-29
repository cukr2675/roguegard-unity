namespace Lysionium.Samples
{
    public class BindableButtonViewItemHandler<TItem, TMgr, TArg> : ButtonViewItemHandler<TItem, TMgr, TArg>, IBindableViewItemHandler
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        public System.Func<NotifyItemHandler, DataBinderCache, IDataBinder> GetBinder { get; set; }

        IDataBinder IBindableViewItemHandler.GetBinder(NotifyItemHandler notify, DataBinderCache binderCache)
        {
            return GetBinder?.Invoke(notify, binderCache);
        }
    }
}
