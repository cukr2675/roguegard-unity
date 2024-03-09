using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
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

        public System.ReadOnlySpan<T> Span => new System.Span<T>(ToArray(), 0, _count);

        public static Spanning<T> Empty => _empty;
        private static readonly T[] _empty = new T[0];

        private Spanning(T[] array)
        {
            if (array == null) throw new System.ArgumentNullException(nameof(array));

            _list = array;
            _count = array.Length;
        }

        private Spanning(IReadOnlyList<T> list, int count)
        {
            if (list == null) throw new System.ArgumentNullException(nameof(list));
#if UNITY_EDITOR
            var type = list.GetType();
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>)) throw new System.ArgumentException(
                $"{typeof(List<>)} でないインスタンスで {typeof(Spanning<>)} を生成しようとしました。");
#endif

            _list = list;
            _count = count;
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

        private Spanning<TTo> Cast<TTo>()
        {
            return new Spanning<TTo>((IReadOnlyList<TTo>)_list, _count);
        }

        public static Spanning<T> Create<TFrom, TFromElm>(TFrom list)
            where TFrom : List<TFromElm>, IReadOnlyList<T>
        {
            return new Spanning<T>(list, list.Count);
        }

        public static Spanning<T> Create<TFrom>(TFrom list)
            where TFrom : IList, IReadOnlyList<T>
        {
            return new Spanning<T>(list, ((IReadOnlyList<T>)list).Count);
        }

        public static implicit operator Spanning<T>(T[] array)
        {
            return new Spanning<T>(array);
        }

        public static implicit operator Spanning<T>(List<T> list)
        {
            return new Spanning<T>(list, list.Count);
        }
    }

    //    public readonly ref struct Spanning<T>
    //    {
    //        private readonly T[] array;
    //        private readonly int _count;

    //        public T this[int index] => array[index];

    //        public int Count => _count;

    //        public static Spanning<T> Empty => _empty;
    //        private static readonly T[] _empty = new T[0];

    //        private Spanning(T[] array, int count, IReadOnlyList<T> list)
    //        {
    //            this.array = array;
    //            _count = count;
    //        }

    //        private Spanning(IReadOnlyList<T> list)
    //        {
    //#if UNITY_EDITOR
    //            var type = list.GetType();
    //            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>)) throw new System.ArgumentException(
    //                $"{typeof(List<>)} でないインスタンスで {typeof(Spanning<>)} を生成しようとしました。");
    //#endif

    //            array = default;
    //            _count = list.Count;
    //        }

    //        public T[] ToArray()
    //        {
    //            var result = new T[_count];
    //            for (int i = 0; i < _count; i++)
    //            {
    //                result[i] = array[i];
    //            }
    //            return result;
    //        }

    //        public static Spanning<T> Create<TFrom, TFromElm>(TFrom list)
    //            where TFrom : List<TFromElm>, IReadOnlyList<T>
    //        {
    //            return new Spanning<T>(list);
    //        }

    //        public static Spanning<T> Create<TFrom>(TFrom list)
    //            where TFrom : IList, IReadOnlyList<T>
    //        {
    //            return new Spanning<T>(list);
    //        }

    //        public static implicit operator Spanning<T>(T[] array)
    //        {
    //            return new Spanning<T>(array, array.Length, array);
    //        }

    //        public static implicit operator Spanning<T>(List<T> list)
    //        {
    //            return new Spanning<T>();
    //        }
    //    }
}
