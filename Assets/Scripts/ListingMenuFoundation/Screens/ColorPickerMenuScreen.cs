using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class ColorPickerMenuScreen<TMgr, TArg> : MenuScreen<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly System.Func<TMgr, TArg, Color> getColor;
        private readonly System.Action<TMgr, TArg, Color> handleClose;
        private readonly ViewTemplate view;

        public override bool IsIncremental => true;

        public ColorPickerMenuScreen(System.Func<TMgr, TArg, Color> getColor, System.Action<TMgr, TArg, Color> handleClose)
        {
            this.getColor = getColor;
            this.handleClose = handleClose;
            this.handleClose += (manager, arg, color) => manager.BackOption.HandleClick(manager, arg);

            view = new()
            {
            };
        }

        public override void OpenScreen(in TMgr manager, in TArg arg)
        {
            var color = getColor(manager, arg);

            view.ShowTemplate(color, manager, arg)
                ?
                .OnClose((manager, arg, color) =>
                {
                    if (LMFAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                        LMFAssert.Type<TArg>(arg, out var tArg, manager)) return;

                    handleClose(tMgr, tArg, color);
                })
                .Build();
        }

        public override void CloseScreen(TMgr manager, bool back)
        {
            view.HideTemplate(manager, back);
        }

        private class ViewTemplate : ViewTemplate<TMgr, TArg>
        {
            private IElementsSubViewStateProvider colorPickerSubViewStateProvider;
            private Color color;
            private event ColorPickerSubView.HandleClose OnClose;

            public Builder ShowTemplate(Color color, TMgr manager, TArg arg)
            {
                if (manager == null) throw new System.ArgumentNullException(nameof(manager));

                this.color = color;

                if (TryShowSubViews(manager, arg)) return null;
                else return new Builder(this, manager, arg);
            }

            protected override void ShowSubViews(TMgr manager, TArg arg)
            {
                if (LMFAssert.Type<ColorPickerSubView>(manager.GetSubView(StandardSubViewTable.ColorPickerName), out var colorPickerSubView)) return;

                colorPickerSubView.SetParameters(color, OnClose, manager, arg, ref colorPickerSubViewStateProvider);
                colorPickerSubView.Show();
            }

            public void HideTemplate(TMgr manager, bool back)
            {
                manager.GetSubView(StandardSubViewTable.ColorPickerName).Hide(back);
            }

            public class Builder : BaseBuilder<Builder>
            {
                private readonly ViewTemplate parent;

                public Builder(ViewTemplate parent, TMgr manager, TArg arg)
                    : base(parent, manager, arg)
                {
                    this.parent = parent;
                }

                public Builder OnClose(ColorPickerSubView.HandleClose handleClose)
                {
                    AssertNotBuilded();

                    parent.OnClose += handleClose;
                    return this;
                }
            }
        }
    }
}
