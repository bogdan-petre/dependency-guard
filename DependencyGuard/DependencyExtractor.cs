using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyGuard
{
    public static class DependencyExtractor
    {
        /// <summary>
        /// Will extract all the types from all the public constructors of a type.
        /// </summary>
        /// <param name="type">Type to extract constructors arguments types.</param>
        /// <returns></returns>
        public static List<ParameterInfo> ExtractConstructorParameters(Assembly assembly)
        {
            List<ParameterInfo> parameters = new List<ParameterInfo>();
            foreach(var type in assembly.GetTypes())
            {
                ConstructorInfo[] constructors = type.GetConstructors();                
                foreach (var constructor in constructors)
                {
                    parameters.AddRange(constructor.GetParameters().Where(c =>
                        c.ParameterType.GetCustomAttributes(typeof(IgnoreGuardAttribute), true).Length == 0).ToList());
                }
            }
            return parameters;
        }
    }
}
