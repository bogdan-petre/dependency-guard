using System;
using System.Reflection;
using Xunit;

namespace DependencyGuard.Tests.Unit
{
    public class DynamicTypeBuilderTests
    {
        [Fact]
        public void Adds_one_parameter_to_constructor()
        {
            var builder = new DynamicTypeBuilder();

            Type firstType = builder.CreateType(Assembly.GetExecutingAssembly().FullName, "MyDynamicType",
                TypeAttributes.Interface |
                TypeAttributes.Abstract |
                TypeAttributes.Public)
                .Build();
            Type anotherType = builder
                .CreateType(Assembly.GetExecutingAssembly().FullName, "AnotherType")
                .WithConstructorParameters(firstType)
                .Build();

            ConstructorInfo constructorInfo = anotherType.GetConstructor(new Type[] { firstType });

            Assert.NotNull(constructorInfo);
        }

        [Fact]
        public void Adds_attribute_to_type()
        {
            var builder = new DynamicTypeBuilder();
            Type dynamicType = builder
                .CreateType(Assembly.GetExecutingAssembly().FullName, "TypwWithAttribute")
                .WithAttribute(typeof(IgnoreGuardAttribute), Type.EmptyTypes)
                .Build();

            bool hasAttribute = dynamicType.GetCustomAttributes(typeof(IgnoreGuardAttribute), true).Length > 0;

            Assert.True(hasAttribute);
        }
    }
}