using MoonSharp.Interpreter;
using System.Diagnostics.CodeAnalysis;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    [SuppressMessage("Style", "IDE1006")]
    public class RogueDirectionUserData
    {
        public RogueDirection Direction { get; }

        private static readonly RogueDirectionUserData _right = new(RogueDirection.Right);
        public RogueDirectionUserData right => _right;
        public static RogueDirectionUserData upperRight { get; } = new RogueDirectionUserData(RogueDirection.UpperRight);
        public static RogueDirectionUserData up { get; } = new RogueDirectionUserData(RogueDirection.Up);
        public static RogueDirectionUserData upperLeft { get; } = new RogueDirectionUserData(RogueDirection.UpperLeft);
        public static RogueDirectionUserData left { get; } = new RogueDirectionUserData(RogueDirection.Left);
        public static RogueDirectionUserData lowerRight { get; } = new RogueDirectionUserData(RogueDirection.LowerLeft);
        public static RogueDirectionUserData down { get; } = new RogueDirectionUserData(RogueDirection.Down);
        public static RogueDirectionUserData lowerLeft { get; } = new RogueDirectionUserData(RogueDirection.LowerRight);

        public RogueDirectionUserData(RogueDirection direction)
        {
            Direction = direction;
        }
    }
}
