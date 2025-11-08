namespace Lysionium
{
    public static class LabelWidgetOption
    {
        public static ILabelWidgetOption Create<TMgr, TArg>(string text, ClickItemHandler<string, TMgr, TArg> onClickLink = null)
        {
            return new WidgetOptionImplement<TMgr, TArg>()
            {
                GetText = delegate { return text; },
                ClickLink = onClickLink
            };
        }

        public static ILabelWidgetOption Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> text, ClickItemHandler<string, TMgr, TArg> onClickLink = null)
        {
            return new WidgetOptionImplement<TMgr, TArg>()
            {
                GetText = text,
                ClickLink = onClickLink
            };
        }

        private class WidgetOptionImplement<TMgr, TArg> : ILabelWidgetOption
        {
            public string WidgetName { get; set; }
            public System.Func<TMgr, TArg, string> GetText { get; set; }
            public ClickItemHandler<string, TMgr, TArg> ClickLink { get; set; }

            string ILabelWidgetOption.GetText(IListMenuManager manager, IListMenuArg arg)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetName(manager, arg);

                return GetText(tMgr, tArg);
            }

            void ILabelWidgetOption.ClickLink(string link, IListMenuManager manager, IListMenuArg arg)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return;

                ClickLink?.Invoke(link, tMgr, tArg);
            }
        }
    }
}
