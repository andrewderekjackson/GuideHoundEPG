using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.UI.Services {
    
    public class ServiceContainer {
        private readonly object lockObject = new object();
        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
        public static readonly ServiceContainer Instance = new ServiceContainer();
        public bool IsEmpty
        {
            get
            {
                bool result;
                lock (this.lockObject)
                {
                    result = (this.services.Count == 0);
                }
                return result;
            }
        }
        private ServiceContainer()
        {
        }
        public void AddService<T>(T service) where T : class
        {
            lock (this.lockObject)
            {
                this.services[typeof(T)] = service;
            }
        }
        public T GetService<T>() where T : class
        {
            object obj2;
            lock (this.lockObject)
            {
                this.services.TryGetValue(typeof(T), out obj2);
            }
            return obj2 as T;
        }
    }
    
}
