namespace Roguegard
{
    public class RogueException : System.InvalidOperationException
    {
        public RogueException() { }

        public RogueException(string message)
            : base(message)
        {
        }

        public RogueException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
