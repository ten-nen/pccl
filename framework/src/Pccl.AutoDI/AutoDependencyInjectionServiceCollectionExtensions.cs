using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Pccl.AutoDI
{
    public static class AutoDependencyInjectionServiceCollectionExtensions
    {
        /// <summary>
        /// 根据接口，特性自动注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection AddAutoRegisterServices(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (assemblies != null)
            {
                var types = assemblies
                            .SelectMany(s => s.GetTypes())
                            .Select(p => IntrospectionExtensions.GetTypeInfo(p));

                foreach (var type in types)
                {
                    var attribute = type.GetCustomAttribute<AutoInjectionAttribute>();
                    if (attribute != null)
                    {
                        foreach (var serviceType in type.GetInterfaces().Where(p => IsBasicAllowedType(p, type)))
                        {
                            Register(services, serviceType, type, attribute.Lifetime, null, attribute.TryAdd, attribute.Replace);
                        }
                        continue;
                    }

                    ServiceLifetime? lifetime = null;
                    if (typeof(ITransientService).GetTypeInfo().IsAssignableFrom(type))
                    {
                        lifetime = ServiceLifetime.Transient;
                    }

                    if (typeof(IScopedService).GetTypeInfo().IsAssignableFrom(type))
                    {
                        lifetime = ServiceLifetime.Scoped;
                    }

                    if (typeof(ISingletonService).GetTypeInfo().IsAssignableFrom(type))
                    {
                        lifetime = ServiceLifetime.Singleton;
                    }
                    if (lifetime == null)
                        continue;
                    foreach (var serviceType in type.GetInterfaces().Where(p => IsBasicAllowedType(p, type)))
                    {
                        Register(services, serviceType, type, lifetime.Value, null, false, false);
                    }
                }
            }
            return services;
        }

        private static bool IsBasicAllowedType(Type serviceType, Type implementationType = null)
        {
            if (serviceType == null || implementationType == null)
                throw new ArgumentNullException();

            if (!implementationType.IsClass && implementationType.IsAbstract)
                return false;

            if (implementationType.Equals(typeof(ITransientService)) || implementationType.Equals(typeof(IScopedService)) && implementationType.Equals(typeof(ISingletonService)))
                return false;
            if (serviceType.Equals(typeof(ITransientService)) || serviceType.Equals(typeof(IScopedService)) && serviceType.Equals(typeof(ISingletonService)))
                return false;

            if (!serviceType.IsAssignableFrom(implementationType) && !implementationType.GetInterfaces()
                         .Any(baseInterface => serviceType.IsAssignableFrom(baseInterface) || (baseInterface.IsGenericType && baseInterface.GetGenericTypeDefinition() == serviceType)))
                return false;

            return true;
        }

        private static IServiceCollection AddService(IServiceCollection services, Type serviceType, Type implementationType = null, Func<IServiceProvider, object> factory = null, ServiceLifetime lifeTime = ServiceLifetime.Scoped, bool tryAdd = false, bool replace = false)
        {
            if (!IsBasicAllowedType(serviceType, implementationType))
                return services;

            if (serviceType.IsGenericTypeDefinition && !implementationType.IsGenericTypeDefinition)
                return services;

            if (services.Any(x => x.ServiceType == serviceType && x.Lifetime == lifeTime && (x.ImplementationType == implementationType || x.ImplementationFactory != null)))
                return services;

            var serviceDescriptor = factory == null ? new ServiceDescriptor(serviceType, implementationType, lifeTime) : new ServiceDescriptor(serviceType, factory, lifeTime);
            if (replace)
            {
                services.Replace(serviceDescriptor);
            }
            else if (tryAdd)
            {
                services.TryAdd(serviceDescriptor);
            }
            else
            {
                services.Add(serviceDescriptor);
            }
            return services;
        }

        private static IServiceCollection Register(IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifeTime, Func<IServiceProvider, Type, object> factory = null, bool tryAdd = false, bool replace = false)
        {
            //注册父级到serviceType
            foreach (var interfaceType in implementationType.GetInterfaces())
            {
                if (factory == null)
                    AddService(services, serviceType, interfaceType, null, lifeTime, tryAdd);
                else
                    AddService(services, serviceType, interfaceType, serviceProvider => factory.Invoke(serviceProvider, implementationType), lifeTime, tryAdd);

            }

            //注册自身到serviceType
            if (factory == null)
                AddService(services, serviceType, implementationType, null, lifeTime, tryAdd);
            else
                AddService(services, serviceType, implementationType, serviceProvider => factory.Invoke(serviceProvider, implementationType), lifeTime, tryAdd);

            //自测自身到父级
            foreach (var interfaceType in implementationType.GetInterfaces())
            {
                if (factory == null)
                    AddService(services, interfaceType, implementationType, null, lifeTime, tryAdd);
                else
                    AddService(services, interfaceType, implementationType, serviceProvider => factory.Invoke(serviceProvider, implementationType), lifeTime, tryAdd);

            }
            return services;
        }


        public static IServiceCollection AddServices(this IServiceCollection services, Type serviceType, ServiceLifetime lifeTime, Func<IServiceProvider, Type, object> factory = null, bool tryAdd = false, params Assembly[] assemblies)
        {
            if (serviceType == null || assemblies == null || assemblies.Length <= 0)
                return services;
            var types = assemblies
                          .SelectMany(s => s.GetTypes())
                          .Where(p => IsBasicAllowedType(serviceType, p));

            foreach (var type in types)
            {
                Register(services, serviceType, type, lifeTime, factory, false, false);
            }
            return services;
        }

        public static IServiceCollection AddServices<ServiceType>(this IServiceCollection services, ServiceLifetime lifeTime, Func<IServiceProvider, Type, object> factory = null, bool tryAdd = false, params Assembly[] assemblies) => AddServices(services, typeof(ServiceType), lifeTime, factory, tryAdd, assemblies);

    }
}
