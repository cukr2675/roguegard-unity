using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public interface IScriptEvaluator
    {
        IEnumerable<KeyValuePair<string, object>> Evaluate(string code);
    }
}
