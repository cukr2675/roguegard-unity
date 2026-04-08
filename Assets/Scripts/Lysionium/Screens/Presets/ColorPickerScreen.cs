using UnityEngine;

namespace Lysionium
{
    public class ColorPickerScreen<TMgr> : IListuiScreen<TMgr>
        where TMgr : IListuiManager
    {
        private readonly System.Func<TMgr, Color> getColor;
        private readonly System.Action<Color, TMgr> handleClose;
        private readonly ViewData view;

        public bool IsIncremental => true;

        public ColorPickerScreen(
            System.Func<TMgr, Color> getColor, System.Action<Color, TMgr> onClose,
            System.Func<TMgr, IColorPickerSubview> colorPickerSubviewSelector = null)
        {
            this.getColor = getColor;
            handleClose = onClose;
            handleClose += (color, manager) =>
            {
                if (manager is IBackOptionProviderListuiManager<TMgr> backOptionProvider)
                {
                    backOptionProvider.BackOption.Click(manager);
                }
            };

            view = new()
            {
                colorPickerSubviewSelector = colorPickerSubviewSelector ?? (m => (m as IDefaultSubviewTable)?.ColorPicker),
            };
        }

        public void OpenScreen(TMgr manager)
        {
            var color = getColor(manager);

            view.Show(color, manager)
                ?
                .OnClose((color, manager) =>
                {
                    if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

                    handleClose(color, tMgr);
                })
                .Build();
        }

        public void CloseScreenView(TMgr manager, bool back)
        {
            view.Hide(manager, back);
        }

        public ISelectOption<T> ToSelectOption<T>()
            where T : IListuiScreenManager<T>, TMgr
        {
            return SelectOption.Create<T>(
                (manager) =>
                {
                    var color = getColor(manager);
                    var rgba = ColorUtility.ToHtmlStringRGBA(color);
                    return $"<#{rgba}>■";
                },
                (manager) =>
                {
                    manager.PushScreen((IListuiScreen<T>)this);
                });
        }

        private class ViewData : ViewData<TMgr>
        {
            public System.Func<TMgr, IColorPickerSubview> colorPickerSubviewSelector;
            private ISubviewStateProvider colorPickerSubviewStateProvider;
            private Color color;
            private event IColorPickerSubview.ColorPickerEventHandler HandleClose;

            public Builder Show(Color color, TMgr manager)
            {
                if (manager == null) throw new System.ArgumentNullException(nameof(manager));

                this.color = color;

                if (TryShowSubviews(manager)) return null;
                else return new Builder(this, manager);
            }

            protected override void ShowSubviews(TMgr manager)
            {
                var colorPickerSubview = colorPickerSubviewSelector?.Invoke(manager);
                if (colorPickerSubview == null) return;

                colorPickerSubview.SetupColorPicker(color, HandleClose, manager, ref colorPickerSubviewStateProvider);
                colorPickerSubview.Show(onHide: OnHide);
            }

            public void Hide(TMgr manager, bool back)
            {
                colorPickerSubviewSelector?.Invoke(manager)?.Hide(back);
            }

            public class Builder : BaseBuilder<ViewData, Builder>
            {
                public Builder(ViewData parent, TMgr manager)
                    : base(parent, manager)
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
