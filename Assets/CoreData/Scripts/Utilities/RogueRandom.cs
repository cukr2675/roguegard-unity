namespace Roguegard
{
    [Objforming.Formable]
    public class RogueRandom : IRogueRandom
    {
        [System.NonSerialized] private System.Random random;

        private int seed;
        private int position;

        public static IRogueRandom Primary { get; set; }

        private const int maxPosition = 100000;

        public RogueRandom()
        {
            var random = new System.Random();
            seed = random.Next(int.MinValue, int.MaxValue);
            position = 0;
        }

        public RogueRandom(int seed)
        {
            this.seed = seed;
            position = 0;
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            if (random == null) { Initialize(); }

            IncrementPosition();

            return random.Next(minInclusive, maxExclusive);
        }

        public float NextFloat(float min, float max)
        {
            if (random == null) { Initialize(); }

            IncrementPosition();

            var size = (double)max - min; // float.MaxValue を超えないように double にキャストしてから計算する
            return (float)(min + random.NextDouble() * size);
        }

        private void Initialize()
        {
            random = new System.Random(seed);
            for (int i = 0; i < position; i++) // シリアル化前の乱数位置を修復する
            {
                random.Next();
            }
        }

        private void IncrementPosition()
        {
            if (position >= maxPosition) // 一定回数以上は乱数位置の修復に時間がかかることを考慮して別シードで最初からにする
            {
                seed = random.Next();
                position = 0;
            }
            else
            {
                position++;
            }
        }
    }
}
