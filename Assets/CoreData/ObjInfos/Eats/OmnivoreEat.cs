using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class OmnivoreEat : BaseEatRogueMethod, IEatActiveRogueMethod
    {
        public override string Name => "雑食";
        public override Spanning<IKeyword> Edibles => new IKeyword[] { MaterialKw.Flesh, MaterialKw.Veggy };
    }
}
