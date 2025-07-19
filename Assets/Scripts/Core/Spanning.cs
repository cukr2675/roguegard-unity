using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class Spanning
    {
        public static Spanning<T> Get<T>(IReadOnlyList<T> list)
        {
            return Spanning<T>.Get(list);
            //return new System.ReadOnlySpan<T>(System.Runtime.CompilerServices.Unsafe.As<System.Runtime.CompilerServices.StrongBox<T[]>>(list).Value, 0, list.Count);
        }
    }

    public readonly ref struct Spanning<T>
    {
        private readonly IReadOnlyList<T> _list;

        /// <summary>
        /// <see cref="List{T}"/> を扱う場合、後から要素数が変化する可能性があるため記憶する
        /// </summary>
        private readonly int _length;

        public T this[int index]
        {
            get
            {
                if (index >= _length) { Debug.LogWarning("添え字の範囲外です。"); }

                return _list[index];
            }
        }

        public int Length => _length;

        public static Spanning<T> Empty => _empty;
        private static readonly T[] _empty = new T[0];

        private Spanning(T[] array)
        {
            _list = array;
            _length = array.Length;
        }

        private Spanning(IReadOnlyList<T> list)
        {
#if UNITY_EDITOR
            var type = list.GetType();
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>)) throw new System.ArgumentException(
                $"{typeof(List<>)} でないインスタンスで {typeof(Spanning<>)} を生成しようとしました。");
#endif

            _list = list;
            _length = list.Count;
        }

        public T[] ToArray()
        {
            var result = new T[_length];
            for (int i = 0; i < _length; i++)
            {
                result[i] = _list[i];
            }
            return result;
        }

        internal static Spanning<T> Get(IReadOnlyList<T> list) => list != null ? new(list) : Empty;
        public static implicit operator Spanning<T>(T[] array) => array != null ? new(array) : Empty;
        public static implicit operator System.ReadOnlySpan<T>(Spanning<T> spanning) => new(spanning.ToArray(), 0, spanning._length);
        public Enumerator GetEnumerator() => new(_list, _length);

        public ref struct Enumerator
        {
            private readonly IReadOnlyList<T> list;
            private readonly int length;
            private int index;

            public readonly T Current => list[index];

            public Enumerator(IReadOnlyList<T> list, int length)
            {
                this.list = list;
                this.length = length;
                index = -1;
            }

            public bool MoveNext()
            {
                if (list.Count != length) throw new System.InvalidOperationException("ループ中のリストサイズが変更されました。");

                index++;
                return index < length;
            }
        }
    }

    //    public readonly ref struct Spanning<T>
    //    {
    //        private readonly T[] _array;
    //        private readonly int _length;

    //        public T this[int index] => _array[index];

    //        public int Length => _length;

    //        public static Spanning<T> Empty => _empty;
    //        private static readonly T[] _empty = new T[0];

    //        private Spanning(T[] array)
    //        {
    //            _array = array;
    //            _length = array.Length;
    //        }

    //        private Spanning(IReadOnlyList<T> list)
    //        {
    //#if UNITY_EDITOR
    //            var type = list.GetType();
    //            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>)) throw new System.ArgumentException(
    //                $"{typeof(List<>)} でないインスタンスで {typeof(Spanning<>)} を生成しようとしました。");
    //#endif

    //            _array = System.Runtime.CompilerServices.Unsafe.As<System.Runtime.CompilerServices.StrongBox<T[]>>(list).Value;
    //            _length = list.Count;
    //        }

    //        public T[] ToArray()
    //        {
    //            var result = new T[_length];
    //            for (int i = 0; i < _length; i++)
    //            {
    //                result[i] = _array[i];
    //            }
    //            return result;
    //        }

    //        internal static Spanning<T> Get(IReadOnlyList<T> list) => list != null ? new(list) : Empty;
    //        public static implicit operator Spanning<T>(T[] array) => array != null ? new(array) : Empty;
    //        public static implicit operator System.ReadOnlySpan<T>(Spanning<T> spanning) => new(spanning._array, 0, spanning._length);
    //        public Enumerator GetEnumerator() => new(_array, _length);

    //        public ref struct Enumerator
    //        {
    //            private readonly T[] array;
    //            private readonly int length;
    //            private int index;

    //            public readonly T Current => array[index];

    //            public Enumerator(T[] array, int length)
    //            {
    //                this.array = array;
    //                this.length = length;
    //                index = -1;
    //            }

    //            public bool MoveNext()
    //            {
    //                index++;
    //                return index < length;
    //            }
    //        }
    //    }
}
