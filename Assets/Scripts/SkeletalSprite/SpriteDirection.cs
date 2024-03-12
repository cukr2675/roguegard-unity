using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    public class SpriteDirection : System.IEquatable<SpriteDirection>
    {
        private readonly int angle;

        public Vector2Int Forward => forwards[angle];
        public float Degree => angle * 45f;

        private static readonly Vector2Int[] forwards = new Vector2Int[]
        {
            new Vector2Int(+1, +0),
            new Vector2Int(+1, +1),
            new Vector2Int(+0, +1),
            new Vector2Int(-1, +1),
            new Vector2Int(-1, +0),
            new Vector2Int(-1, -1),
            new Vector2Int(+0, -1),
            new Vector2Int(+1, -1),
        };

        private static readonly string[] texts = new string[]
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

        public static SpriteDirection Right => new SpriteDirection(0);
        public static SpriteDirection UpperRight => new SpriteDirection(1);
        public static SpriteDirection Up => new SpriteDirection(2);
        public static SpriteDirection UpperLeft => new SpriteDirection(3);
        public static SpriteDirection Left => new SpriteDirection(4);
        public static SpriteDirection LowerLeft => new SpriteDirection(5);
        public static SpriteDirection Down => new SpriteDirection(6);
        public static SpriteDirection LowerRight => new SpriteDirection(7);

        public SpriteDirection(int angle)
        {
            while (angle < 0)
            {
                angle += 8;
            }
            angle %= 8;

            this.angle = angle;
        }

        public static SpriteDirection FromDegree(float degree)
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
        /// 指定の <see cref="Vector2Int"/> の符号（-1 or 0 or +1）から <see cref="SpriteDirection"/> を取得する。
        /// </summary>
        public static bool TryFromSign(Vector2Int vector, out SpriteDirection direction)
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
            direction = new SpriteDirection(index);
            return true;

            //var degree = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
            //direction = FromDegree(degree);
        }

        /// <summary>
        /// 指定の <see cref="Vector2Int"/> の符号（-1 or 0 or +1）から <see cref="SpriteDirection"/> を取得する。
        /// </summary>
        public static SpriteDirection FromSignOrLowerLeft(Vector2Int vector)
        {
            if (vector == Vector2Int.zero) return LowerLeft;

            var signX = vector.x <= -1 ? -1 : vector.x == 0 ? 0 : +1;
            var signY = vector.y <= -1 ? -1 : vector.y == 0 ? 0 : +1;
            var sign = new Vector2Int(signX, signY);
            var index = System.Array.IndexOf(forwards, sign);
            return new SpriteDirection(index);
        }

        /// <summary>
        /// 反時計回りに <paramref name="angle"/> * 45 度回転した方向を取得する。
        /// </summary>
        public SpriteDirection Rotate(int angle)
        {
            return new SpriteDirection(this.angle + angle);
        }

        public override string ToString()
        {
            return texts[angle];
        }

        public bool Equals(SpriteDirection other) => other.angle == angle;
        public override bool Equals(object obj) => obj is SpriteDirection other && other.angle == angle;
        public override int GetHashCode() => angle.GetHashCode();

        public static bool operator ==(SpriteDirection left, SpriteDirection right)
        {
            return left.angle == right.angle;
        }

        public static bool operator !=(SpriteDirection left, SpriteDirection right)
        {
            return left.angle != right.angle;
        }

        public static explicit operator int(SpriteDirection direction)
        {
            return direction.angle;
        }

        //public static explicit operator Direction8(int angleIndex)
        //{
        //    if (angleIndex < 0 || 8 <= angleIndex) throw new System.ArgumentOutOfRangeException(nameof(angleIndex));

        //    return new Direction8(angleIndex);
        //}
    }
}
