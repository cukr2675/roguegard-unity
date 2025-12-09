namespace Lysionium
{
    public class BackSelectOption<TMgr, TArg> : ISelectOption<TMgr, TArg>
        where TMgr : IBackOptionProviderListMenuManager<TMgr, TArg>
        where TArg : IListMenuArg
    {
        private readonly string name;
        private readonly string style;

        public static BackSelectOption<TMgr, TArg> Instance { get; } = new();

        public BackSelectOption(string name = null, string style = null)
        {
            this.name = name;
            this.style = style;
        }

        string ISelectOption<TMgr, TArg>.GetName(TMgr manager, TArg arg) => name ?? manager.BackOption.GetName(manager, arg);
        string ISelectOption<TMgr, TArg>.GetStyle(TMgr manager, TArg arg) => style ?? manager.BackOption.GetStyle(manager, arg);
        void ISelectOption<TMgr, TArg>.Click(TMgr manager, TArg arg) => manager.BackOption.Click(manager, arg);
    }

    /// <summary>
    /// リフレクションで <see cref="BackSelectOption{TMgr, TArg}"/> を取得するクラス
    /// </summary>
    internal static class BackSelectOption
    {
        internal static bool TryCreate<TMgr, TArg>(out ISelectOption<TMgr, TArg> backOption, string name = null, string style = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            // 引数なしで一度取得成功している場合は即キャッシュを返す
            if (name == null && style == null && Cache<TMgr, TArg>.instance != null)
            {
                backOption = Cache<TMgr, TArg>.instance;
                return true;
            }

            // 使用可能な型を判定
            if (!typeof(IBackOptionProviderListMenuManager<TMgr, TArg>).IsAssignableFrom(typeof(TMgr)))
            {
                backOption = default;
                return false;
            }

            var genericType = typeof(BackSelectOption<,>).MakeGenericType(typeof(TMgr), typeof(TArg));
            if (name == null && style == null)
            {
                var instance = genericType.GetProperty("Instance").GetValue(null);
                backOption = (ISelectOption<TMgr, TArg>)instance;
                Cache<TMgr, TArg>.instance = backOption;
                return true;
            }
            else
            {
                var instance = System.Activator.CreateInstance(genericType, name, style);
                backOption = (ISelectOption<TMgr, TArg>)instance;
                return true;
            }
        }

        private static class Cache<TMgr, TArg>
        {
            public static ISelectOption<TMgr, TArg> instance;
        }
    }
}
