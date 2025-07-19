using System.Collections.Generic;

namespace Roguegard
{
    internal class AspectStack
    {
        private readonly List<Pair> pairs = new();

        public bool ContainsItem => pairs.Count >= 1;

        public void GetPeek(out int index, out int id)
        {
            var pair = pairs[^1];
            index = pair.Index;
            id = pair.Id;
        }

        public void SetPeek(int index, int id)
        {
            var pair = pairs[^1];
            pair.Index = index;
            pair.Id = id;
            pairs[^1] = pair;
        }

        public void Push(int rank, float activationDepth)
        {
            if (pairs.Count >= 1)
            {
                var peek = pairs[^1];
                if (activationDepth < peek.ActivationDepth) throw new RogueException(
                    $"{nameof(IRogueMethod)} の実行順が不正です。" +
                    $"activationDepth{peek.ActivationDepth} から activationDepth{activationDepth} を実行しました。");
                if (activationDepth == peek.ActivationDepth && rank <= peek.Rank) throw new RogueException(
                    $"{nameof(IRogueMethod)} の実行順が不正です。" +
                    $"{GetRankName(peek.Rank)} から {GetRankName(rank)} を実行しました。");
            }

            pairs.Add(new Pair(rank, activationDepth));
        }

        public void Pop()
        {
            pairs.RemoveAt(pairs.Count - 1);
        }

        public void Clear()
        {
            pairs.Clear();
        }

        public static string GetRankName(int rank)
        {
            return rank switch
            {
                0 => "Active",
                1 => "Apply",
                2 => "Affect",
                3 => "ChangeState",
                4 => "ChangeEffect",
                _ => throw new RogueException(),
            };
        }

        private struct Pair
        {
            public readonly int Rank { get; }

            public readonly float ActivationDepth { get; }

            public int Index { get; set; }

            public int Id { get; set; }

            public Pair(int rank, float activationDepth)
            {
                Rank = rank;
                ActivationDepth = activationDepth;
                Index = 0;
                Id = -1;
            }
        }
    }
}
