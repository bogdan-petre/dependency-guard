namespace DependencyGuard.Tests.Unit.Classes
{
    /// <summary>
    /// Test service with multiple constructors combination.
    /// Nothing to see here, class used to generate <see cref="ParameterInfo"/> for testing.
    /// </summary>
    internal class ClassWithMultipleConstructors
    {
        private readonly Class1 _class1;
        private readonly Class2 _class2;
        private readonly ClassWithAttribute _classWithAttribute;

        public ClassWithMultipleConstructors(Class1 class1)
        {
            _class1 = class1;
        }

        public ClassWithMultipleConstructors(Class2 class2)
        {
            _class2 = class2;
        }

        public ClassWithMultipleConstructors(Class1 class1, Class2 class2) : this(class1)
        {
            _class2 = class2;
        }

        public ClassWithMultipleConstructors(ClassWithAttribute classWithAttribute)
        {
            _classWithAttribute = classWithAttribute;
        }
    }
}
