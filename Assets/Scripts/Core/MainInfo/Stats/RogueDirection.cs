using UnityEngine;

namespace Roguegard
{
    // OchalikeSprites.SpriteDirection と同一の実装だが、依存を弱めるために別定義にする
    // 命名メモ: RogueDirection8 は冗長 (ローグライクなので8方向は連想しやすいはず？)
    [Objforming.Formable]
    public readonly struct RogueDirection : System.IEquatable<RogueDirection>
    {
        private readonly int angle;

        public Vector2Int Forward => forwards[angle];
        public float Degree => angle * 45f;

        private static readonly Vector2Int[] forwards =
        {
            new(+1, +0),
            new(+1, +1),
            new(+0, +1),
            new(-1, +1),
            new(-1, +0),
            new(-1, -1),
            new(+0, -1),
            new(+1, -1),
        };

        private static readonly string[] texts =
        {
            "Right",
            "UpperRight",
            "Up",
            "UpperLeft",
            "Left",
            "LowerLeft",
            "Down",
            "LowerRight",
        };

        public static RogueDirection Right => new(0);
        public static RogueDirection UpperRight => new(1);
        public static RogueDirection Up => new(2);
        public static RogueDirection UpperLeft => new(3);
        public static RogueDirection Left => new(4);
        public static RogueDirection LowerLeft => new(5);
        public static RogueDirection Down => new(6);
        public static RogueDirection LowerRight => new(7);

        public RogueDirection(int angle)
        {
            while (angle < 0)
            {
                angle += 8;
            }
            angle %= 8;

            this.angle = angle;
        }

        public static RogueDirection FromDegree(float degree)
        {
            degree = Mathf.Repeat(degree + 22.5f, 360f);
            if (degree < 45f)
            {
                return Right;
            }
            else if (degree < 90f)
            {
                return UpperRight;
            }
            else if (degree < 135f)
            {
                return Up;
            }
            else if (degree < 180f)
            {
                return UpperLeft;
            }
            else if (degree < 225f)
            {
                return Left;
            }
            else if (degree < 270f)
            {
                return LowerLeft;
            }
            else if (degree < 315f)
            {
                return Down;
            }
            else
            {
                return LowerRight;
            }
        }

        /// <summary>
        /// 指定の <see cref="Vector2Int"/> の符号（-1 or 0 or +1）から <see cref="RogueDirection"/> を取得する。
        /// </summary>
        public static bool TryFromSign(Vector2Int vector, out RogueDirection direction)
        {
            if (vector == Vector2Int.zero)
            {
                direction = default;
                return false;
            }

            var signX = vector.x <= -1 ? -1 : vector.x == 0 ? 0 : +1;
            var signY = vector.y <= -1 ? -1 : vector.y == 0 ? 0 : +1;
            var sign = new Vector2Int(signX, signY);
            var index = System.Array.IndexOf(forwards, sign);
            direction = new RogueDirection(index);
            return true;
        }

        /// <summary>
        /// 指定の <see cref="Vector2Int"/> の符号（-1 or 0 or +1）から <see cref="RogueDirection"/> を取得する。
        /// </summary>
        public static RogueDirection FromSignOrLowerLeft(Vector2Int vector)
        {
            if (vector == Vector2Int.zero) return LowerLeft;

            var signX = vector.x <= -1 ? -1 : vector.x == 0 ? 0 : +1;
            var signY = vector.y <= -1 ? -1 : vector.y == 0 ? 0 : +1;
            var sign = new Vector2Int(signX, signY);
            var index = System.Array.IndexOf(forwards, sign);
            return new RogueDirection(index);
        }

        /// <summary>
        /// 反時計回りに <paramref name="angle"/> * 45 度回転した方向を取得する。
        /// </summary>
        public RogueDirection Rotate(int angle)
        {
            return new RogueDirection(this.angle + angle);
        }

        public override string ToString()
        {
            return texts[angle];
        }

        public bool Equals(RogueDirection other) => other.angle == angle;
        public override bool Equals(object obj) => obj is RogueDirection other && other.angle == angle;
        public override int GetHashCode() => angle.GetHashCode();

        public static bool operator ==(RogueDirection left, RogueDirection right)
        {
            return left.angle == right.angle;
        }

        public static bool operator !=(RogueDirection left, RogueDirection right)
        {
            return left.angle != right.angle;
        }

        public static explicit operator int(RogueDirection direction)
        {
            return direction.angle;
        }

        public static implicit operator OchalikeSprites.SpriteDirection(RogueDirection direction)
        {
            return new OchalikeSprites.SpriteDirection(direction.angle);
        }
    }
}
