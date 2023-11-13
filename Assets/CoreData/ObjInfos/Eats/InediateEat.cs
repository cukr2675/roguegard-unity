using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class InediateEat : BaseEatRogueMethod, IEatActiveRogueMethod
    {
        public override string Name => "不食";
        public override Spanning<IKeyword> Edibles => Spanning<IKeyword>.Empty;
    }
}
