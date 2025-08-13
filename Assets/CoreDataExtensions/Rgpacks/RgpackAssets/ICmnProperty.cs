namespace Roguegard.Rgpacks
{
    [Objforming.RequireRelationalComponent]
    public interface ICmnProperty
    {
        ICmnPropertySource Source { get; }

        ICmnProperty Clone();
    }
}
