namespace Roguegard
{
    public static class SpanningExtensions
    {
        public static bool Contains<T>(this Spanning<T> spanning, T value)
        {
            var index = spanning.IndexOf(value);
            return index != -1;
        }

        public static int IndexOf<T>(this Spanning<T> spanning, T value)
        {
            for (int i = 0; i < spanning.Length; i++)
            {
                if (spanning[i]?.Equals(value) ?? value == null) return i;
            }
            return -1;
        }
    }
}
