using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IScriptingEvaluator
    {
        void Evaluate(string code, RgpackBuilder rgpack);
    }
}
