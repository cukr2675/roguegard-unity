using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class PropertiedCmnReference
    {
        public CmnReference Cmn { get; }
        private readonly Dictionary<string, ICmnProperty> properties;

        public PropertiedCmnReference(PropertiedCmnData data, string envRgpackID, IReadOnlyDictionary<string, ICmnProperty> properties)
        {
            Cmn = new CmnReference(data.Cmn, envRgpackID);
            this.properties = new Dictionary<string, ICmnProperty>(properties);
        }

        public object Invoke()
        {
            return Cmn.Asset.Invoke(properties);
        }
    }
}
