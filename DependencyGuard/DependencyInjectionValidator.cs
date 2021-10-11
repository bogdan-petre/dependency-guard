using DependencyGuard.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyGuard
{
    public static class DependencyInjectionValidator
    {
        /// <summary>
        /// Validates dependency injection container at startup.
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void ValidateAtStartup(this IServiceCollection serviceCollection, Assembly assembly)
        {
            if (assembly is null) throw new ArgumentException("No assembly specified", nameof(assembly));
            List<ParameterInfo> dependencies = DependencyExtractor.ExtractConstructorParameters(assembly);
            foreach (var dependency in dependencies)
            {
                bool isRegistered = serviceCollection.Any(s => dependency.ParameterType.IsAssignableFrom(s.ServiceType));
                if (!isRegistered)
                    throw new ServiceNotRegisteredException($"Service is not registered : {dependency.ParameterType}");
            }
        }
    }
}