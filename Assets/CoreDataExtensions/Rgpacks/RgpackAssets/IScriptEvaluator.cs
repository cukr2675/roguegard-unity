using System.Collections.Generic;

namespace Roguegard.Rgpacks
{
    public interface IScriptEvaluator
    {
        IEnumerable<KeyValuePair<string, object>> Evaluate(string code, string envRgpackId);
    }
}
