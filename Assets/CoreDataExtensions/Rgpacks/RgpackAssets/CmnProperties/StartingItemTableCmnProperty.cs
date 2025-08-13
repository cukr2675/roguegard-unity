using Roguegard.CharacterCreation;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class StartingItemTableCmnProperty : ICmnProperty
    {
        public static ICmnPropertySource SourceInstance { get; } = new SourceType();

        ICmnPropertySource ICmnProperty.Source => SourceInstance;

        public StartingItemTable Value { get; set; }

        public static StartingItemTableCmnProperty Default { get; } = new StartingItemTableCmnProperty()
        {
            Value = new StartingItemTable()
        };

        private StartingItemTableCmnProperty() { }

        public ICmnProperty Clone()
        {
            return new StartingItemTableCmnProperty() { Value = Value };
        }

        private class SourceType : ICmnPropertySource
        {
            public ICmnProperty CreateProperty()
            {
                return new StartingItemTableCmnProperty();
            }
        }
    }
}
