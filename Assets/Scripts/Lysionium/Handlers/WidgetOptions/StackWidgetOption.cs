using System.Collections.Generic;

namespace Lysionium
{
    public static class StackWidgetOption
    {
        public static IStackWidgetOption Create(params (string width, object item)[] children)
        {
            return new WidgetOptionImplement()
            {
                Name = LuiUtility.EmitIdentity("StackWidget"),
                Children = children,
            };
        }

        private class WidgetOptionImplement : IStackWidgetOption
        {
            public string Name { get; set; }

            public IReadOnlyList<(string width, object item)> Children { get; set; }
        }
    }
}
