using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class PropertiedCmnReference
    {
        public CmnReference Cmn { get; }
        private readonly PropertiedCmnData data;
        private readonly string envRgpackID;
        private IReadOnlyDictionary<string, ICmnProperty> properties;

        public PropertiedCmnReference(PropertiedCmnData data, string envRgpackID)
        {
            Cmn = new CmnReference(data.Cmn, envRgpackID);
            this.data = new PropertiedCmnData(data);
            this.envRgpackID = envRgpackID;
        }

        public object Invoke()
        {
            if (properties == null)
            {
                properties = data.GetProperties(envRgpackID);
            }

            return Cmn.Asset.Invoke(properties);
        }
    }
}
