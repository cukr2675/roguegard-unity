using System.Collections.Generic;

namespace Lysionium
{
    public static class TreeOptionsBuilderExtensions
    {
        public static TBuilder Node<TMgr, TBuilder>(
            this ITreeOptionsBuilder<TMgr, TBuilder> builder,
            string name, System.Action<SelectOptionTree<TMgr>> nodeInitializeAction)
            where TMgr : IListuiManager
        {
            var children = new SelectOptionTree<TMgr>(nodeInitializeAction);
            return builder.Option(new Option<TMgr> { name = name, style = null, children = children });
        }

        public static TBuilder Node<TMgr, TBuilder>(
            this ITreeOptionsBuilder<TMgr, TBuilder> builder,
            string name, string style, System.Action<SelectOptionTree<TMgr>> nodeInitializeAction)
            where TMgr : IListuiManager
        {
            var children = new SelectOptionTree<TMgr>(nodeInitializeAction);
            return builder.Option(new Option<TMgr> { name = name, style = style, children = children });
        }

        private class Option<TMgr> : ITreeOption<TMgr>
        {
            public string name;
            public string style;
            public IReadOnlyList<object> children;

            public string GetName(TMgr manager) => name;
            public string GetStyle(TMgr manager) => style;
            public IReadOnlyList<object> GetChildren(TMgr manager) => children;
        }
    }
}
