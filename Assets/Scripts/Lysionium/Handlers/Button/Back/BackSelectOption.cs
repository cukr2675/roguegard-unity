using System.Collections.Generic;

namespace Lysionium
{
    public class BackSelectOption<TMgr> : ISelectOption<TMgr>
        where TMgr : IBackOptionProviderListuiManager<TMgr>
    {
        private readonly string name;
        private readonly string style;

        public static BackSelectOption<TMgr> Instance { get; } = new();

        public BackSelectOption(string name = null, string style = null)
        {
            this.name = name;
            this.style = style;
        }

        string ISelectOption<TMgr>.GetName(TMgr manager) => name ?? manager.BackOption.GetName(manager);
        string ISelectOption<TMgr>.GetStyle(TMgr manager) => style ?? manager.BackOption.GetStyle(manager);
        IReadOnlyList<string> ISelectOption<TMgr>.GetCandidateEventGestureNames(TMgr manager)
            => manager.BackOption.GetCandidateEventGestureNames(manager);
        void ISelectOption<TMgr>.EventGestureConfirmed(TMgr manager, string eventGestureName)
            => manager.BackOption.EventGestureConfirmed(manager, eventGestureName);
    }

    /// <summary>
    /// リフレクションで <see cref="BackSelectOption{TMgr}"/> を取得するクラス
    /// </summary>
    internal static class BackSelectOption
    {
        internal static bool TryCreate<TMgr>(
            out ISelectOption<TMgr> backOption, string name = null, string style = null)
            where TMgr : IListuiManager
        {
            // 引数なしで一度取得成功している場合は即キャッシュを返す
            if (name == null && style == null && Cache<TMgr>.isChached)
            {
                backOption = Cache<TMgr>.instance;
                return true;
            }

            // 使用可能な型を判定
            if (!typeof(IBackOptionProviderListuiManager<TMgr>).IsAssignableFrom(typeof(TMgr)))
            {
                backOption = default;
                if (name == null && style == null)
                {
                    Cache<TMgr>.instance = backOption; // 引数未指定時はキャッシュ
                    Cache<TMgr>.isChached = true;
                }
                return false;
            }

            var genericType = typeof(BackSelectOption<>).MakeGenericType(typeof(TMgr));
            if (name == null && style == null)
            {
                var instance = genericType.GetProperty("Instance").GetValue(null);
                backOption = (ISelectOption<TMgr>)instance;
                Cache<TMgr>.instance = backOption; // 引数未指定時はキャッシュ
                Cache<TMgr>.isChached = true;
                return true;
            }
            else
            {
                var instance = System.Activator.CreateInstance(genericType, name, style);
                backOption = (ISelectOption<TMgr>)instance;
                return true;
            }
        }

        private static class Cache<TMgr>
        {
            public static ISelectOption<TMgr> instance;
            public static bool isChached;
        }
    }
}
