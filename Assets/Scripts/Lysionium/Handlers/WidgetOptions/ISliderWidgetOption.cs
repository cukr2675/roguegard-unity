namespace Lysionium
{
    public interface ISliderWidgetOption
    {
        string Name { get; }

        float MinValue { get; }

        float MaxValue { get; }

        delegate float SliderEventHandler<in TMgr, in TArg>(TMgr manager, TArg arg, float value);

        float GetValue(IListuiManager manager, IListuiArg arg);

        float HandleValueChanged(IListuiManager manager, IListuiArg arg, float value);
    }
}
