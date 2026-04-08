namespace Lysionium
{
    public static class FlickableOption
    {
        public static IFlickableOption<TMgr> Create<TMgr>(
            string name,
            ClickOptionHandler<TMgr> onKeyDown, ClickOptionHandler<TMgr> onExpand, ClickOptionHandler<TMgr> onKeyUp,
            string style)
            where TMgr : IListuiManager
        {
            var instance = new Implement<TMgr>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.KeyDown = onKeyDown;
            instance.Expand = onExpand;
            instance.KeyUp = onKeyUp;
            return instance;
        }

        public static IFlickableOption<TMgr> Create<TMgr>(
            System.Func<TMgr, string> getName,
            ClickOptionHandler<TMgr> onKeyDown, ClickOptionHandler<TMgr> onExpand, ClickOptionHandler<TMgr> onKeyUp,
            string style)
            where TMgr : IListuiManager
        {
            var instance = new Implement<TMgr>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.KeyDown = onKeyDown;
            instance.Expand = onExpand;
            instance.KeyUp = onKeyUp;
            return instance;
        }

        public static IFlickableOption<TMgr> Create<TMgr>(
            string name,
            ClickOptionHandler<TMgr> onKeyDown, ClickOptionHandler<TMgr> onExpand, ClickOptionHandler<TMgr> onKeyUp,
            System.Func<TMgr, string> style)
            where TMgr : IListuiManager
        {
            var instance = new Implement<TMgr>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.KeyDown = onKeyDown;
            instance.Expand = onExpand;
            instance.KeyUp = onKeyUp;
            return instance;
        }

        public static IFlickableOption<TMgr> Create<TMgr>(
            System.Func<TMgr, string> getName,
            ClickOptionHandler<TMgr> onKeyDown, ClickOptionHandler<TMgr> onExpand, ClickOptionHandler<TMgr> onKeyUp,
            System.Func<TMgr, string> style)
            where TMgr : IListuiManager
        {
            var instance = new Implement<TMgr>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.KeyDown = onKeyDown;
            instance.Expand = onExpand;
            instance.KeyUp = onKeyUp;
            return instance;
        }

        private class Implement<TMgr> : IFlickableOption<TMgr>
            where TMgr : IListuiManager
        {
            private string name;
            private System.Func<TMgr, string> getName;

            private string style;
            private System.Func<TMgr, string> getStyle;

            public ClickOptionHandler<TMgr> KeyDown { get; set; }
            public ClickOptionHandler<TMgr> Expand { get; set; }
            public ClickOptionHandler<TMgr> KeyUp { get; set; }

            public void SetName(string name)
            {
                this.name = name ?? throw new System.ArgumentNullException(nameof(name));
                getName = null;
            }

            public void SetName(System.Func<TMgr, string> selector)
            {
                getName = selector ?? throw new System.ArgumentNullException(nameof(selector));
                name = null;
            }

            public void SetStyle(string style)
            {
                this.style = style;
                getStyle = null;
            }

            public void SetStyle(System.Func<TMgr, string> selector)
            {
                getStyle = selector;
                style = null;
            }

            string IFlickableOption<TMgr>.GetName(TMgr manager)
            {
                if (getName != null)
                {
                    if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return null;

                    return getName(tMgr);
                }
                else return name;
            }

            string IFlickableOption<TMgr>.GetStyle(TMgr manager)
            {
                if (getStyle != null)
                {
                    if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return null;

                    return getStyle(tMgr);
                }
                else return style;
            }

            void IFlickableOption<TMgr>.KeyDown(TMgr manager)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

                KeyDown?.Invoke(tMgr);
            }

            void IFlickableOption<TMgr>.Expand(TMgr manager)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

                Expand?.Invoke(tMgr);
            }

            void IFlickableOption<TMgr>.KeyUp(TMgr manager)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

                KeyUp?.Invoke(tMgr);
            }
        }
    }
}
