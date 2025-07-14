using System.Collections;
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
        private readonly int _count;

        public T this[int index]
        {
            get
            {
                if (index >= _count) { Debug.LogWarning("添え字の範囲外です。"); }

                return _list[index];
            }
        }

        public int Count => _count;

        public static Spanning<T> Empty => _empty;
        private static readonly T[] _empty = new T[0];

        private Spanning(T[] array)
        {
            if (array == null) throw new System.ArgumentNullException(nameof(array));

            _list = array;
            _count = array.Length;
        }

        private Spanning(IReadOnlyList<T> list)
        {
            if (list == null) throw new System.ArgumentNullException(nameof(list));
#if UNITY_EDITOR
            var type = list.GetType();
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>)) throw new System.ArgumentException(
                $"{typeof(List<>)} でないインスタンスで {typeof(Spanning<>)} を生成しようとしました。");
#endif

            _list = list;
            _count = list.Count;
        }

        public T[] ToArray()
        {
            var result = new T[_count];
            for (int i = 0; i < _count; i++)
            {
                result[i] = _list[i];
            }
            return result;
        }

        internal static Spanning<T> Get(IReadOnlyList<T> list) => new(list);
        public static implicit operator Spanning<T>(T[] array) => new(array);
        public static implicit operator System.ReadOnlySpan<T>(Spanning<T> spanning) => new(spanning.ToArray(), 0, spanning._count);
        public Enumerator GetEnumerator() => new Enumerator(_list, _count);

        public ref struct Enumerator
        {
            private readonly IReadOnlyList<T> list;
            private readonly int count;
            private int index;

            public T Current => list[index];

            public Enumerator(IReadOnlyList<T> list, int count)
            {
                this.list = list;
                this.count = count;
                index = -1;
            }

            public bool MoveNext()
            {
                index++;
                return index < count;
            }
        }
    }

    //    public readonly ref struct Spanning<T>
    //    {
    //        private readonly T[] _array;
    //        private readonly int _count;

    //        public T this[int index] => _array[index];

    //        public int Count => _count;

    //        public static Spanning<T> Empty => _empty;
    //        private static readonly T[] _empty = new T[0];

    //        private Spanning(T[] array)
    //        {
    //            if (array == null) throw new System.ArgumentNullException(nameof(array));

    //            _array = array;
    //            _count = array.Length;
    //        }

    //        private Spanning(IReadOnlyList<T> list)
    //        {
    //            if (list == null) throw new System.ArgumentNullException(nameof(list));
    //#if UNITY_EDITOR
    //            var type = list.GetType();
    //            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>)) throw new System.ArgumentException(
    //                $"{typeof(List<>)} でないインスタンスで {typeof(Spanning<>)} を生成しようとしました。");
    //#endif

    //            _array = System.Runtime.CompilerServices.Unsafe.As<System.Runtime.CompilerServices.StrongBox<T[]>>(list).Value;
    //            _count = list.Count;
    //        }

    //        public T[] ToArray()
    //        {
    //            var result = new T[_count];
    //            for (int i = 0; i < _count; i++)
    //            {
    //                result[i] = _array[i];
    //            }
    //            return result;
    //        }

    //        internal static Spanning<T> Get(IReadOnlyList<T> list) => new(list);
    //        public static implicit operator Spanning<T>(T[] array) => new(array);
    //        public static implicit operator System.ReadOnlySpan<T>(Spanning<T> spanning) => new(spanning._array, 0, spanning._count);
    //        public Enumerator GetEnumerator() => new Enumerator(_array, _count);

    //        public ref struct Enumerator
    //        {
    //            private readonly T[] array;
    //            private readonly int count;
    //            private int index;

    //            public T Current => array[index];

    //            public Enumerator(T[] array, int count)
    //            {
    //                this.array = array;
    //                this.count = count;
    //                index = -1;
    //            }

    //            public bool MoveNext()
    //            {
    //                index++;
    //                return index < count;
    //            }
    //        }
    //    }
}
