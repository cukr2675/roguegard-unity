namespace Roguegard.Rgpacks
{
    [Objforming.Formable(Objforming.FormerMode.Wrapper)]
    public sealed class CmnReference : RgpackReference<ICmnAssset>
    {
        public new ICmnAssset Asset => base.Asset;

        public CmnReference(string id, string envRgpackId)
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
