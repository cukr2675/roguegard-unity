using Roguegard.CharacterCreation;

namespace Roguegard.Rgpacks
{
    public class StartingItemOptionReference : RgpackReference<IStartingItemOption>
    {
        public new IStartingItemOption Asset => base.Asset;

        public StartingItemOptionReference(string id, string envRgpackId)
            : base(id, envRgpackId)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is CmnReference reference && reference.FullId == FullId;
        }

        public override int GetHashCode()
        {
            return FullId.GetHashCode();
        }
    }
}
