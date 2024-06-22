using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class CmnReference : RgpackReference<ICmnAssset>
    {
        public new ICmnAssset Asset => base.Asset;

        public CmnReference(string id, string envRgpackID)
            : base(id, envRgpackID)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is CmnReference reference && reference.FullID == FullID;
        }

        public override int GetHashCode()
        {
            return FullID.GetHashCode();
        }
    }
}
