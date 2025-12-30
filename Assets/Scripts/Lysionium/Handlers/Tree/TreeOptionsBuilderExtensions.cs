using System.Collections.Generic;

namespace Lysionium
{
    public static class TreeOptionsBuilderExtensions
    {
        public static TBuilder Node<TMgr, TArg, TBuilder>(
            this ITreeOptionsBuilder<TMgr, TArg, TBuilder> builder,
            string name, System.Action<SelectOptionTree<TMgr, TArg>> nodeInitializeAction)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            var children = new SelectOptionTree<TMgr, TArg>(nodeInitializeAction);
            return builder.Option(new Option<TMgr, TArg> { name = name, style = null, children = children });
        }

        public static TBuilder Node<TMgr, TArg, TBuilder>(
            this ITreeOptionsBuilder<TMgr, TArg, TBuilder> builder,
            string name, string style, System.Action<SelectOptionTree<TMgr, TArg>> nodeInitializeAction)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            var children = new SelectOptionTree<TMgr, TArg>(nodeInitializeAction);
            return builder.Option(new Option<TMgr, TArg> { name = name, style = style, children = children });
        }

        private class Option<TMgr, TArg> : ITreeOption<TMgr, TArg>
        {
            public string name;
            public string style;
            public IReadOnlyList<object> children;

            public string GetName(TMgr manager, TArg arg) => name;
            public string GetStyle(TMgr manager, TArg arg) => style;
            public IReadOnlyList<object> GetChildren(TMgr manager, TArg arg) => children;
        }
    }
}
