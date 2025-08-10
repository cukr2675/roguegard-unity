namespace Objforming
{
    public interface IDependencyModuleTable<T>
        where T : IDependencyModule
    {
        bool TryGetModule(string name, string version, out T module);
    }
}
