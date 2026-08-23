namespace Lysionium
{
    public static class FlickButtonOption
    {
        public static IFlickButtonOption<TMgr> Create<TMgr>(
            string name,
            string style = null,
            SubmitOptionHandler<TMgr> onPress = null,
            System.Action<TMgr> onExpand = null,
            SubmitOptionHandler<TMgr> onRelease = null)
            where TMgr : IListuiManager
        {
            var instance = new Implement<TMgr>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.Press = onPress;
            instance.Expand = onExpand;
            instance.Release = onRelease;
            return instance;
        }

        public static IFlickButtonOption<TMgr> Create<TMgr>(
            System.Func<TMgr, string> getName,
            string style = null,
            SubmitOptionHandler<TMgr> onPress = null,
            System.Action<TMgr> onExpand = null,
            SubmitOptionHandler<TMgr> onRelease = null)
            where TMgr : IListuiManager
        {
            var instance = new Implement<TMgr>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.Press = onPress;
            instance.Expand = onExpand;
            instance.Release = onRelease;
            return instance;
        }

        public static IFlickButtonOption<TMgr> Create<TMgr>(
            string name,
            System.Func<TMgr, string> style,
            SubmitOptionHandler<TMgr> onPress = null,
            System.Action<TMgr> onExpand = null,
            SubmitOptionHandler<TMgr> onRelease = null)
            where TMgr : IListuiManager
        {
            var instance = new Implement<TMgr>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.Press = onPress;
            instance.Expand = onExpand;
            instance.Release = onRelease;
            return instance;
        }

        public static IFlickButtonOption<TMgr> Create<TMgr>(
            System.Func<TMgr, string> getName,
            System.Func<TMgr, string> style,
            SubmitOptionHandler<TMgr> onPress = null,
            System.Action<TMgr> onExpand = null,
            SubmitOptionHandler<TMgr> onRelease = null)
            where TMgr : IListuiManager
        {
            var instance = new Implement<TMgr>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.Press = onPress;
            instance.Expand = onExpand;
            instance.Release = onRelease;
            return instance;
        }

        private class Implement<TMgr> : SelectOption<TMgr>, IFlickButtonOption<TMgr>
            where TMgr : IListuiManager
        {
            public SubmitOptionHandler<TMgr> Press { get; set; }
            public System.Action<TMgr> Expand { get; set; }
            public SubmitOptionHandler<TMgr> Release { get; set; }

            void IFlickButtonOption<TMgr>.Press(TMgr manager)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

                Press?.Invoke(tMgr);
            }

            void IFlickButtonOption<TMgr>.Expand(TMgr manager)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

                Expand?.Invoke(tMgr);
            }

            void IFlickButtonOption<TMgr>.Release(TMgr manager)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

                Release?.Invoke(tMgr);
            }
        }
    }
}
