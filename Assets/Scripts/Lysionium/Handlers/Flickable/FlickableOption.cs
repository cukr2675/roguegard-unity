namespace Lysionium
{
    public static class FlickableOption
    {
        public static IFlickableOption<TMgr, TArg> Create<TMgr, TArg>(
            string name,
            ClickItemHandler<TMgr, TArg> onKeyDown, ClickItemHandler<TMgr, TArg> onExpand, ClickItemHandler<TMgr, TArg> onKeyUp,
            string style)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            var instance = new Implement<TMgr, TArg>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.KeyDown = onKeyDown;
            instance.Expand = onExpand;
            instance.KeyUp = onKeyUp;
            return instance;
        }

        public static IFlickableOption<TMgr, TArg> Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> getName,
            ClickItemHandler<TMgr, TArg> onKeyDown, ClickItemHandler<TMgr, TArg> onExpand, ClickItemHandler<TMgr, TArg> onKeyUp,
            string style)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            var instance = new Implement<TMgr, TArg>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.KeyDown = onKeyDown;
            instance.Expand = onExpand;
            instance.KeyUp = onKeyUp;
            return instance;
        }

        public static IFlickableOption<TMgr, TArg> Create<TMgr, TArg>(
            string name,
            ClickItemHandler<TMgr, TArg> onKeyDown, ClickItemHandler<TMgr, TArg> onExpand, ClickItemHandler<TMgr, TArg> onKeyUp,
            System.Func<TMgr, TArg, string> style)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            var instance = new Implement<TMgr, TArg>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.KeyDown = onKeyDown;
            instance.Expand = onExpand;
            instance.KeyUp = onKeyUp;
            return instance;
        }

        public static IFlickableOption<TMgr, TArg> Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> getName,
            ClickItemHandler<TMgr, TArg> onKeyDown, ClickItemHandler<TMgr, TArg> onExpand, ClickItemHandler<TMgr, TArg> onKeyUp,
            System.Func<TMgr, TArg, string> style)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            var instance = new Implement<TMgr, TArg>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.KeyDown = onKeyDown;
            instance.Expand = onExpand;
            instance.KeyUp = onKeyUp;
            return instance;
        }

        private class Implement<TMgr, TArg> : IFlickableOption<TMgr, TArg>
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            private string name;
            private System.Func<TMgr, TArg, string> getName;

            private string style;
            private System.Func<TMgr, TArg, string> getStyle;

            public ClickItemHandler<TMgr, TArg> KeyDown { get; set; }
            public ClickItemHandler<TMgr, TArg> Expand { get; set; }
            public ClickItemHandler<TMgr, TArg> KeyUp { get; set; }

            public void SetName(string name)
            {
                this.name = name ?? throw new System.ArgumentNullException(nameof(name));
                getName = null;
            }

            public void SetName(System.Func<TMgr, TArg, string> selector)
            {
                getName = selector ?? throw new System.ArgumentNullException(nameof(selector));
                name = null;
            }

            public void SetStyle(string style)
            {
                this.style = style;
                getStyle = null;
            }

            public void SetStyle(System.Func<TMgr, TArg, string> selector)
            {
                getStyle = selector;
                style = null;
            }

            string IFlickableOption<TMgr, TArg>.GetName(TMgr manager, TArg arg)
            {
                if (getName != null)
                {
                    if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                        LuiAssert.Type<TArg>(arg, out var tArg)) return null;

                    return getName(tMgr, tArg);
                }
                else return name;
            }

            string IFlickableOption<TMgr, TArg>.GetStyle(TMgr manager, TArg arg)
            {
                if (getStyle != null)
                {
                    if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                        LuiAssert.Type<TArg>(arg, out var tArg)) return null;

                    return getStyle(tMgr, tArg);
                }
                else return style;
            }

            void IFlickableOption<TMgr, TArg>.KeyDown(TMgr manager, TArg arg)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                    LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

                KeyDown?.Invoke(tMgr, tArg);
            }

            void IFlickableOption<TMgr, TArg>.Expand(TMgr manager, TArg arg)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                    LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

                Expand?.Invoke(tMgr, tArg);
            }

            void IFlickableOption<TMgr, TArg>.KeyUp(TMgr manager, TArg arg)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                    LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

                KeyUp?.Invoke(tMgr, tArg);
            }
        }
    }
}
