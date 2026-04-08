using UnityEngine;

namespace Lysionium
{
    public interface IColorPickerSubview : ISubview
    {
        delegate void ColorPickerEventHandler(Color color, IListuiManager manager);

        void SetupColorPicker(
            Color color, ColorPickerEventHandler onClose, IListuiManager manager, ref ISubviewStateProvider stateProvider);
    }
}
