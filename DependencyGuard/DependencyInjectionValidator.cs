using DependencyGuard.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyGuard
{
    public static class DependencyInjectionValidator
    {

        /// <summary>
        /// Validates service collection at startup.
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void ValidateAtStartup(this IServiceCollection serviceCollection, Assembly assembly, Action<GuardOptions> configuration)
        {
            List<ParameterInfo> dependencies = DependencyExtractor.ExtractConstructorParameters(assembly);
            foreach (var dependency in dependencies)
            {
                bool isRegistered = serviceCollection.Any(s => dependency.ParameterType.IsAssignableFrom(s.ServiceType));
                GuardOptions options = new GuardOptions();
                configuration(options);
                if (!isRegistered && options.WithException)
                    throw new ServiceNotRegisteredException($"Service is not registered : {dependency.ParameterType}");
            }
        }

        /// <summary>
        /// Will validate service collection and call the callback method once it's done.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="assembly">Assembly to be analyzed, usually current executing assembly</param>
        /// <param name="callback">Method to be executed when validation finishes.</param>
        public static void ValidateAtStartup(this IServiceCollection serviceCollection, Assembly assembly, Action callback)
        {
            bool isValid = IsValid(serviceCollection, assembly);
            if (!isValid)
                callback();
        }

        /// <summary>
        /// Will validate service collection and call the callback method with once it's done, with an enumeration of <see cref="Type"/> as argument.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="assembly">Assembly to be analyzed, usually current executing assembly</param>
        /// <param name="callback">Method to be executed when validation finishes.</param>
        public static void ValidateAtStartup(this IServiceCollection serviceCollection, Assembly assembly, Action<IEnumerable<Type>> callback)
        {
            callback(GetTypesNotRegistered(serviceCollection, assembly));
        }

        /// <summary>
        /// Validates service collection and logs with warnings all services that are not registered.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="assembly">Assembly to be analyzed, usually current executing assembly</param>
        /// <param name="logger">Logger object</param>
        public static void ValidateAtStartup(this IServiceCollection serviceCollection, Assembly assembly, ILogger logger)
        {
            List<Type> typesNotRegistered = GetTypesNotRegistered(serviceCollection, assembly);
            typesNotRegistered.ForEach(t => logger.LogWarning($"[DependencyGuard] Type {t} is not registered."));
        }

        private static bool IsValid(IServiceCollection serviceCollection, Assembly assembly)
        {
            bool isValid = true;
            List<ParameterInfo> dependencies = DependencyExtractor.ExtractConstructorParameters(assembly);
            foreach (var dependency in dependencies)
            {
                isValid = serviceCollection.Any(s => dependency.ParameterType.IsAssignableFrom(s.ServiceType));
            }
            return isValid;
        }

        private static List<Type> GetTypesNotRegistered(IServiceCollection serviceCollection, Assembly assembly)
        {
            List<Type> typesNotRegistered = new List<Type>();
            List<ParameterInfo> dependencies = DependencyExtractor.ExtractConstructorParameters(assembly);
            foreach (var dependency in dependencies)
            {
                dependencies.Where(d => !serviceCollection
                    .Any(s => d.ParameterType.IsAssignableFrom(s.ServiceType)))
                    .ToList()
                    .ForEach(p => typesNotRegistered.Add(p.ParameterType));
            }
            return typesNotRegistered;
        }
    }
}