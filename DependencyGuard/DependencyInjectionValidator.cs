using DependencyGuard.Exceptions;
using Microsoft.Extensions.DependencyInjection;
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
        public static void ValidateAtStartup(this IServiceCollection serviceCollection)
        {
            foreach (ServiceDescriptor descriptor in serviceCollection)
            {
                if (descriptor.ImplementationType != null)
                {
                    List<ParameterInfo> dependencies = DependencyExtractor.ExtractConstructorParameters(descriptor.ImplementationType);
                    foreach (var dependency in dependencies)
                    {
                        bool isRegistered = serviceCollection.Any(s => s.ImplementationType.Equals(dependency.ParameterType));
                        if (!isRegistered)
                            throw new ServiceNotRegisteredException($"Type is not registered: {dependency.ParameterType}");
                    }
                }
            }
        }
    }
}