using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class StartingItemCmnProperty : ICmnProperty
    {
        public static ICmnPropertySource SourceInstance { get; } = new SourceType();

        ICmnPropertySource ICmnProperty.Source => SourceInstance;

        public StartingItem Value { get; set; }

        public static StartingItemCmnProperty Default { get; } = new StartingItemCmnProperty()
        {
            Value = new StartingItem()
            {
                Option = RoguegardSettings.CharacterCreationDatabase.StartingItemOptions[0]
            }
        };

        private StartingItemCmnProperty() { }

        public ICmnProperty Clone()
        {
            return new StartingItemCmnProperty() { Value = Value };
        }

        private class SourceType : ICmnPropertySource
        {
            public ICmnProperty CreateProperty()
            {
                return new StartingItemCmnProperty();
            }
        }
    }
}
