namespace Roguegard
{
    public struct StaticId
    {
        private readonly int id;

        public readonly bool IsValid => id == currentId;

        private static int currentId;

        public static StaticId Current => new(false);

        private StaticId(bool _)
        {
            id = currentId;
        }

        public static void Next()
        {
            currentId++;
        }
    }
}
