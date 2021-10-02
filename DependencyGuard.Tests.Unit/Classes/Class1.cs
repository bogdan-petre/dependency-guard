using DependencyGuard.Tests.Unit.Interfaces;

namespace DependencyGuard.Tests.Unit
{
    /// <summary>
    /// Test service implementation 1.
    /// Nothing to see here, class used to generate <see cref="ParameterInfo"/> for testing.
    /// </summary>
    internal class Class1 : Interface1
    {
        private readonly Class2 _class2;

        public Class1(Class2 class2)
        {
            _class2 = class2;
        }
    }
}
