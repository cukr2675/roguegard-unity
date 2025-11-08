using UnityEngine;

namespace Lysionium
{
    public interface IColorPickerSubview : ISubview
    {
        delegate void ColorPickerEventHandler(IListMenuManager manager, IListMenuArg arg, Color color);

        void SetParameters(
            Color color, ColorPickerEventHandler onClose, IListMenuManager manager, IListMenuArg arg,
            ref ISubviewStateProvider stateProvider);
    }
}
