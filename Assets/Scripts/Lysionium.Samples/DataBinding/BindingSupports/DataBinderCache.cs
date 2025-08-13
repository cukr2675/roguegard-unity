using System.Collections.Generic;

namespace Lysionium.Samples
{
    public class DataBinderCache
    {
        private readonly Dictionary<System.Type, IDataBinder> map = new();

        public bool TryGet<T>(out T binder)
            where T : IDataBinder
        {
            if (map.TryGetValue(typeof(T), out var pb))
            {
                binder = (T)pb;
                return true;
            }
            else
            {
                binder = default;
                return false;
            }
        }

        public void Add(IDataBinder binder)
        {
            map.Add(binder.GetType(), binder);
        }
    }
}
