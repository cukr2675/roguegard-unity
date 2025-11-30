namespace Lysionium
{
    public interface ISliderWidgetOption
    {
        string Name { get; }

        float MinValue { get; }

        float MaxValue { get; }

        delegate float SliderEventHandler<in TMgr, in TArg>(TMgr manager, TArg arg, float value);

        float GetValue(IListMenuManager manager, IListMenuArg arg);

        float HandleValueChanged(IListMenuManager manager, IListMenuArg arg, float value);
    }
}
