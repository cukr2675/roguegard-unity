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

        private static readonly object[] rogueMethodArguments = new object[4];

        public PropertiedCmnReference(PropertiedCmnData data, string envRgpackID, IReadOnlyDictionary<string, ICmnProperty> properties)
        {
            Cmn = new CmnReference(data.Cmn, envRgpackID);
            this.properties = new Dictionary<string, ICmnProperty>(properties);
        }

        public object Invoke()
        {
            return Cmn.Asset.Invoke(properties, System.Array.Empty<object>());
        }

        public object Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            rogueMethodArguments[0] = self;
            rogueMethodArguments[1] = user;
            rogueMethodArguments[2] = activationDepth;
            rogueMethodArguments[3] = arg;
            return Cmn.Asset.Invoke(properties, rogueMethodArguments);
        }
    }
}
