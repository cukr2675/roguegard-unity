namespace Lysionium
{
    public interface ISliderWidgetOption
    {
        string Name { get; }

        float MinValue { get; }

        float MaxValue { get; }

        delegate float SliderEventHandler<in TMgr>(TMgr manager, float value);

        float GetValue(IListuiManager manager);

        float HandleValueChanged(IListuiManager manager, float value);
    }
}
