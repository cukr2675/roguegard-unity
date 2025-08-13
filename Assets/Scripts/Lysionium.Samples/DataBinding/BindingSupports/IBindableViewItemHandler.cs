namespace Lysionium.Samples
{
    public interface IBindableViewItemHandler : IViewItemHandler
    {
        IDataBinder GetBinder(NotifyItemHandler notify, DataBinderCache binderCache);
    }
}
