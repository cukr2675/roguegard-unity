namespace Lysionium
{
    public static class LabelWidgetOption
    {
        public static ILabelWidgetOption Create<TMgr>(string text, ClickItemHandler<string, TMgr> onClickLink = null)
        {
            return new WidgetOptionImplement<TMgr>()
            {
                GetText = delegate { return text; },
                ClickLink = onClickLink
            };
        }

        public static ILabelWidgetOption Create<TMgr>(
            System.Func<TMgr, string> text, ClickItemHandler<string, TMgr> onClickLink = null)
        {
            return new WidgetOptionImplement<TMgr>()
            {
                GetText = text,
                ClickLink = onClickLink
            };
        }

        private class WidgetOptionImplement<TMgr> : ILabelWidgetOption
        {
            public string WidgetName { get; set; }
            public System.Func<TMgr, string> GetText { get; set; }
            public ClickItemHandler<string, TMgr> ClickLink { get; set; }

            string ILabelWidgetOption.GetText(IListuiManager manager)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetName(manager);

                return GetText(tMgr);
            }

            void ILabelWidgetOption.ClickLink(string link, IListuiManager manager)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return;

                ClickLink?.Invoke(link, tMgr);
            }
        }
    }
}
