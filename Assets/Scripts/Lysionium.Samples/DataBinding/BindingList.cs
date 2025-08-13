using System.Collections;
using System.Collections.Generic;

namespace Lysionium.Samples
{
    public class BindingList<T> : IReadOnlyList<T>
    {
        private readonly List<T> list = new();

        public T this[int index]
        {
            get => list[index];
            set
            {
                list[index] = value;
                OnChanged?.Invoke();
            }
        }

        public int Count => list.Count;

        public delegate void OnChangedHandler();

        public event OnChangedHandler OnChanged;

        public void Add(T item)
        {
            list.Add(item);
            OnChanged?.Invoke();
        }

        public void Remove(T item)
        {
            if (list.Remove(item)) { OnChanged?.Invoke(); }
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
            OnChanged?.Invoke();
        }

        public void Clear()
        {
            list.Clear();
            OnChanged?.Invoke();
        }

        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }
}
