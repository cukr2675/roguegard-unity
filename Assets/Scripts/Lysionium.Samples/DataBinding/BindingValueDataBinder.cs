namespace Lysionium.Samples
{
    public class BindingValueDataBinder : IDataBinder
    {
        private readonly BindingValue.OnChangedHandler onChanged;

        public BindingValueDataBinder(NotifyItemHandler notify)
        {
            onChanged = () => notify();
        }

        public void Bind(object data)
        {
            if (data is BindingValue bindingValue)
            {
                bindingValue.OnChanged += onChanged;
            }
        }

        public void Unbind(object data)
        {
            if (data is BindingValue bindingValue)
            {
                bindingValue.OnChanged -= onChanged;
            }
        }
    }
}
