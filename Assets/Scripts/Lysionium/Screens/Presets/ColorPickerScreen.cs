using UnityEngine;

namespace Lysionium
{
    /// <inheritdoc/>
    public class ColorPickerScreen<TMgr> : ColorPickerScreen<TMgr, IListuiArg>
        where TMgr : IListuiManager
    {
        public ColorPickerScreen(
            System.Func<TMgr, IListuiArg, Color> getColor, System.Action<TMgr, IListuiArg, Color> onClose,
            System.Func<TMgr, IColorPickerSubview> colorPickerSubviewSelector = null)
            : base(getColor, onClose, colorPickerSubviewSelector)
        { }
    }

    public class ColorPickerScreen<TMgr, TArg> : IListuiScreen<TMgr, TArg>
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        private readonly System.Func<TMgr, TArg, Color> getColor;
        private readonly System.Action<TMgr, TArg, Color> handleClose;
        private readonly ViewData view;

        public bool IsIncremental => true;

        public ColorPickerScreen(
            System.Func<TMgr, TArg, Color> getColor, System.Action<TMgr, TArg, Color> onClose,
            System.Func<TMgr, IColorPickerSubview> colorPickerSubviewSelector = null)
        {
            this.getColor = getColor;
            handleClose = onClose;
            handleClose += (manager, arg, color) =>
            {
                if (manager is IBackOptionProviderListuiManager<TMgr, TArg> backOptionProvider)
                {
                    backOptionProvider.BackOption.Click(manager, arg);
                }
            };

            view = new()
            {
                colorPickerSubviewSelector = colorPickerSubviewSelector ?? (m => (m as IDefaultSubviewTable)?.ColorPicker),
            };
        }

        public void OpenScreen(TMgr manager, TArg arg)
        {
            var color = getColor(manager, arg);

            view.Show(color, manager, arg)
                ?
                .OnClose((manager, arg, color) =>
                {
                    if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                        LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

                    handleClose(tMgr, tArg, color);
                })
                .Build();
        }

        public void CloseScreenView(TMgr manager, bool back)
        {
            view.Hide(manager, back);
        }

        private class ViewData : ViewData<TMgr, TArg>
        {
            public System.Func<TMgr, IColorPickerSubview> colorPickerSubviewSelector;
            private ISubviewStateProvider colorPickerSubviewStateProvider;
            private Color color;
            private event IColorPickerSubview.ColorPickerEventHandler HandleClose;

            public Builder Show(Color color, TMgr manager, TArg arg)
            {
                if (manager == null) throw new System.ArgumentNullException(nameof(manager));

                this.color = color;

                if (TryShowSubviews(manager, arg)) return null;
                else return new Builder(this, manager, arg);
            }

            protected override void ShowSubviews(TMgr manager, TArg arg)
            {
                var colorPickerSubview = colorPickerSubviewSelector?.Invoke(manager);
                if (colorPickerSubview == null) return;

                colorPickerSubview.SetupColorPicker(color, HandleClose, manager, arg, ref colorPickerSubviewStateProvider);
                colorPickerSubview.Show(onHide: OnHide);
            }

            public void Hide(TMgr manager, bool back)
            {
                colorPickerSubviewSelector?.Invoke(manager)?.Hide(back);
            }

            public class Builder : BaseBuilder<ViewData, Builder>
            {
                public Builder(ViewData parent, TMgr manager, TArg arg)
                    : base(parent, manager, arg)
                {
                }

                public Builder OnClose(IColorPickerSubview.ColorPickerEventHandler onClose)
                {
                    AssertNotBuilt();

                    Parent.HandleClose += onClose;
                    return this;
                }
            }
        }
    }
}
