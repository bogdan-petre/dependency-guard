using DependencyGuard.Tests.Unit.Interfaces;

namespace DependencyGuard.Tests.Unit
{
    /// <summary>
    /// Test service implementation 2.
    /// Nothing to see here, class used to generate <see cref="ParameterInfo"/> for testing.
    /// </summary>
    internal class Class2 : Interface2
    {
        private readonly Class3 _class3;

        public Class2(Class3 class3)
        {
            _class3 = class3;
        }
    }
}
