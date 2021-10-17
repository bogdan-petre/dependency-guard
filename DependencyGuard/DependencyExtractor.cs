using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyGuard
{
    public static class DependencyExtractor
    {
        /// <summary>
        /// Will extract all the types from all the public constructors of all types in an assembly.
        /// </summary>
        /// <param name="type">Type to extract constructors arguments types.</param>
        /// <returns></returns>
        public static List<ParameterInfo> ExtractConstructorParameters(Assembly assembly)
        {
            if (assembly is null) throw new ArgumentException("No assembly specified", nameof(assembly));
            List<ParameterInfo> parameters = new List<ParameterInfo>();
            foreach (var type in assembly.GetTypes())
            {
                ConstructorInfo[] constructors = type.GetConstructors();
                foreach (var constructor in constructors)
                {
                    parameters.AddRange(constructor.GetParameters().Where(param =>
                        param.ParameterType.GetCustomAttributes(typeof(IgnoreGuardAttribute), true).Length == 0 &&
                        !parameters.Any(p => p.ParameterType.IsAssignableFrom(param.ParameterType))).ToList());
                }
            }
            return parameters;
        }
    }
}