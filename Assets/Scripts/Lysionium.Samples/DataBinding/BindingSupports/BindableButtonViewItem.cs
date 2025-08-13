namespace Lysionium.Samples
{
    public class BindableButtonViewItem : ButtonViewItem
    {
        private DataBinderCache binderCache;
        private NotifyItemHandler notify;

        protected override void Awake()
        {
            base.Awake();

            binderCache = new DataBinderCache();
            notify = path =>
            {
                if (path == "*") { Rebind(); }
            };
        }

        protected override void BindCore(object item, IViewItemHandler handler)
        {
            base.BindCore(item, handler);

            if (handler is IBindableViewItemHandler bindableViewItemHandler)
            {
                bindableViewItemHandler.GetBinder(notify, binderCache)?.Bind(item);

            }
        }

        protected override void UnbindCore(object item, IViewItemHandler handler)
        {
            if (handler is IBindableViewItemHandler bindableViewItemHandler)
            {
                bindableViewItemHandler.GetBinder(notify, binderCache).Unbind(item);
            }

            base.UnbindCore(item, handler);
        }
    }
}
