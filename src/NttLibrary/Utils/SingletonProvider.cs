using System;
using System.Collections.Generic;

namespace NttLibrary.Utils
{
    public static class SingletonProvider
    {
        private static readonly Dictionary<Type, object> _singeltons;

        static SingletonProvider()
        {
            _singeltons = new Dictionary<Type, object>();
        }

        public static T GetSingelton<T>() where T : class, new()
        {
            var singeltonType = typeof(T);
            if (!_singeltons.ContainsKey(singeltonType))
            {
                _singeltons[singeltonType] = Activator.CreateInstance(singeltonType);
            }

            return _singeltons[singeltonType] as T;
        }
    }
}
