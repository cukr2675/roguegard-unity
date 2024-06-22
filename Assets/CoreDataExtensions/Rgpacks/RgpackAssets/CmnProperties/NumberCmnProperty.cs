using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class NumberCmnProperty : ICmnProperty
    {
        public static ICmnPropertySource SourceInstance { get; } = new SourceType();

        ICmnPropertySource ICmnProperty.Source => SourceInstance;

        public float Value { get; set; }

        private NumberCmnProperty() { }

        public ICmnProperty Clone()
        {
            return new NumberCmnProperty();
        }

        private class SourceType : ICmnPropertySource
        {
            public ICmnProperty CreateProperty()
            {
                return new NumberCmnProperty();
            }
        }
    }
}
