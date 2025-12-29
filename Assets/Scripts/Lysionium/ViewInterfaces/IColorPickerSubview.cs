using UnityEngine;

namespace Lysionium
{
    public interface IColorPickerSubview : ISubview
    {
        delegate void ColorPickerEventHandler(IListuiManager manager, IListuiArg arg, Color color);

        void SetupColorPicker(
            Color color, ColorPickerEventHandler onClose, IListuiManager manager, IListuiArg arg,
            ref ISubviewStateProvider stateProvider);
    }
}
