using UnityEngine;

namespace Lysionium
{
    public class ColorPickerMenuScreen<TMgr, TArg> : MenuScreen<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly System.Func<TMgr, TArg, Color> getColor;
        private readonly System.Action<TMgr, TArg, Color> handleClose;
        private readonly ViewData view;

        public override bool IsIncremental => true;

        public ColorPickerMenuScreen(System.Func<TMgr, TArg, Color> getColor, System.Action<TMgr, TArg, Color> onClose)
        {
            this.getColor = getColor;
            handleClose = onClose;
            handleClose += (manager, arg, color) => manager.BackOption.Click(manager, arg);

            view = new()
            {
            };
        }

        public override void OpenScreen(in TMgr manager, in TArg arg)
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

        public override void CloseScreenView(TMgr manager, bool back)
        {
            view.Hide(manager, back);
        }

        private class ViewData : ViewData<TMgr, TArg>
        {
            private ISubviewStateProvider colorPickerSubviewStateProvider;
            private Color color;
            private event ColorPickerSubview.HandleClose HandleClose;

            public Builder Show(Color color, TMgr manager, TArg arg)
            {
                if (manager == null) throw new System.ArgumentNullException(nameof(manager));

                this.color = color;

                if (TryShowSubviews(manager, arg)) return null;
                else return new Builder(this, manager, arg);
            }

            protected override void ShowSubviews(TMgr manager, TArg arg)
            {
                if (LuiAssert.Type<ColorPickerSubview>(manager.GetSubview(StandardSubviewTable.ColorPickerName), out var colorPickerSubview)) return;

                colorPickerSubview.SetParameters(color, HandleClose, manager, arg, ref colorPickerSubviewStateProvider);
                colorPickerSubview.Show();
            }

            public void Hide(TMgr manager, bool back)
            {
                manager.GetSubview(StandardSubviewTable.ColorPickerName).Hide(back);
            }

            public class Builder : BaseBuilder<Builder>
            {
                private readonly ViewData parent;

                public Builder(ViewData parent, TMgr manager, TArg arg)
                    : base(parent, manager, arg)
                {
                    this.parent = parent;
                }

                public Builder OnClose(ColorPickerSubview.HandleClose onClose)
                {
                    AssertNotBuilt();

                    parent.HandleClose += onClose;
                    return this;
                }
            }
        }
    }
}
