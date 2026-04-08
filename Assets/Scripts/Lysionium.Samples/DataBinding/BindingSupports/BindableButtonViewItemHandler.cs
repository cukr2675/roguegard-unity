namespace Lysionium.Samples
{
    public class BindableButtonViewItemHandler<TItem, TMgr> : ButtonViewItemHandler<TItem, TMgr>, IBindableViewItemHandler
        where TMgr : IListuiManager
    {
        public System.Func<NotifyItemHandler, DataBinderCache, IDataBinder> GetBinder { get; set; }

        IDataBinder IBindableViewItemHandler.GetBinder(NotifyItemHandler notify, DataBinderCache binderCache)
        {
            return GetBinder?.Invoke(notify, binderCache);
        }
    }
}
