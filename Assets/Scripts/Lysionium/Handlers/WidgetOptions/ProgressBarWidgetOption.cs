namespace Lysionium
{
    public static class ProgressBarWidgetOption
    {
        public static IProgressBarWidgetOption Create<TMgr, TArg>(System.Func<TMgr, TArg, float> progress, string name = null)
        {
            return new WidgetOptionImplement<TMgr, TArg>()
            {
                Name = name ?? LuiUtility.EmitIdentity("ProgressBarViewWidget"),
                GetProgress = progress
            };
        }

        private class WidgetOptionImplement<TMgr, TArg> : IProgressBarWidgetOption
        {
            public string Name { get; set; }
            public System.Func<TMgr, TArg, float> GetProgress { get; set; }

            float IProgressBarWidgetOption.GetProgress(IListMenuManager manager, IListMenuArg arg)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return 0f;

                return GetProgress(tMgr, tArg);
            }
        }
    }
}
