namespace Objforming
{
    public interface IObjformingLogger
    {
        void Log(string text);

        void LogWarning(string text);

        void LogError(string text);
    }
}
