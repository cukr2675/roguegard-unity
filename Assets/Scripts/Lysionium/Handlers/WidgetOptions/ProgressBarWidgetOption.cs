namespace Lysionium
{
    public static class ProgressBarWidgetOption
    {
        public static IProgressBarWidgetOption Create<TMgr>(System.Func<TMgr, float> progress, string name = null)
        {
            return new WidgetOptionImplement<TMgr>()
            {
                Name = name ?? LuiUtility.EmitIdentity("ProgressBarViewWidget"),
                GetProgress = progress
            };
        }

        private class WidgetOptionImplement<TMgr> : IProgressBarWidgetOption
        {
            public string Name { get; set; }
            public System.Func<TMgr, float> GetProgress { get; set; }

            float IProgressBarWidgetOption.GetProgress(IListuiManager manager)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return 0f;

                return GetProgress(tMgr);
            }
        }
    }
}
