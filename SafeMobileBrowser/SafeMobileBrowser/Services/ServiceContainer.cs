// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Collections.Generic;
using System.Threading;

namespace SafeMobileBrowser.Services
{
    public static class ServiceContainer
    {
        static readonly Dictionary<Type, Lazy<object>> services = new Dictionary<Type, Lazy<object>>();

        /// <summary>
        /// Register the specified service with an instance
        /// </summary>
        public static void Register<T>(T service)
        {
            services[typeof(T)] = new Lazy<object>(() => service);
        }

        /// <summary>
        /// Register the specified service for a class with a default constructor
        /// </summary>
        public static void Register<T>()
            where T : new()
        {
            services[typeof(T)] = new Lazy<object>(() => new T(), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// Register the specified service with a callback to be invoked when requested
        /// </summary>
        public static void Register<T>(Func<T> function)
        {
            services[typeof(T)] = new Lazy<object>(() => function());
        }

        /// <summary>
        /// Register the specified service with an instance
        /// </summary>
        public static void Register(Type type, object service)
        {
            services[type] = new Lazy<object>(() => service);
        }

        /// <summary>
        /// Register the specified service with a callback to be invoked when requested
        /// </summary>
        public static void Register(Type type, Func<object> function)
        {
            services[type] = new Lazy<object>(function, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// Resolves the type, throwing an exception if not found
        /// </summary>
        public static T Resolve<T>(bool nullIsAcceptable = false)
        {
            return (T)Resolve(typeof(T), nullIsAcceptable);
        }

        /// <summary>
        /// Resolves the type, throwing an exception if not found
        /// </summary>
        public static object Resolve(Type type, bool nullIsAcceptable = false)
        {
            // Non-scoped services
            {
                if (services.TryGetValue(type, out Lazy<object> service))
                    return service.Value;

                if (nullIsAcceptable)
                    return null;

                throw new KeyNotFoundException($"Service not found for type '{type}'");
            }
        }

        /// <summary>
        /// Clears the entire container
        /// </summary>
        public static void Clear()
        {
            services.Clear();
        }
    }
}
