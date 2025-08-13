using System.Collections.Generic;

namespace Roguegard.Rgpacks
{
    public interface ICmnAssset
    {
        IReadOnlyDictionary<string, ICmnPropertySource> PropertySources { get; }

        object Invoke(IReadOnlyDictionary<string, ICmnProperty> properties, Spanning<object> arguments);
    }
}
