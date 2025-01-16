using System;
using System.Collections.Concurrent;

namespace XMediator.Core.Util
{
    internal class XMediatorMultipleDebouncer<T>
    {
        private ConcurrentDictionary<string, XMediatorDebouncer<T>> debouncers = new ConcurrentDictionary<string, XMediatorDebouncer<T>>();
        
        public T Invoke(String key, Func<T> action)
        {
            if (!debouncers.ContainsKey(key))
            {
                debouncers[key] = new XMediatorDebouncer<T>();
            }
            return debouncers[key].Invoke(action);
        }
        
    }
}