namespace Lysionium.Samples
{
    public class BindableEventGestureViewItemHandler<TItem, TMgr> :
        EventGestureViewItemHandler<TItem, TMgr>, IBindableViewItemHandler
        where TMgr : IListuiManager
    {
        public System.Func<NotifyItemHandler, DataBinderCache, IDataBinder> GetBinder { get; set; }

        IDataBinder IBindableViewItemHandler.GetBinder(NotifyItemHandler notify, DataBinderCache binderCache)
        {
            return GetBinder?.Invoke(notify, binderCache);
        }
    }
}
