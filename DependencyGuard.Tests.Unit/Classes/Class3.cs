using DependencyGuard.Tests.Unit.Interfaces;

namespace DependencyGuard.Tests.Unit
{
    /// <summary>
    /// Test service implementation 3.
    /// Nothing to see here, class used to generate <see cref="ParameterInfo"/> for testing.
    /// </summary>
    class Class3 : Interface3
    {
        private readonly Class1 _class1;

        public Class3(Class1 class1)
        {
            _class1 = class1;
        }
    }
}
