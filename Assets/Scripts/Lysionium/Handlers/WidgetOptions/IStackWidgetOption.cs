using System.Collections.Generic;

namespace Lysionium
{
    public interface IStackWidgetOption
    {
        string Name { get; }

        IReadOnlyList<(string width, object item)> Children { get; }
    }
}
