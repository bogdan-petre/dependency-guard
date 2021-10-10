using System;
using System.Reflection;
using Xunit;

namespace DependencyGuard.Tests.Unit
{
    public class DynamicTypeBuilderTests
    {
        // Just for debugging purposes.
        [Fact]
        public void Test()
        {
            var builder = new DynamicTypeBuilder();

            Type firstType = builder.CreateType(Assembly.GetExecutingAssembly().FullName, "MyDynamicType", TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public)
                                        .Build();
            Type anotherType = builder.CreateType(Assembly.GetExecutingAssembly().FullName, "AnotherType")
                                        .Build();
            Type secondType = builder.CreateType(Assembly.GetExecutingAssembly().FullName, "SecondType")
                                        .WithConstructorParameters(firstType)
                                        .WithConstructorParameters(anotherType)
                                        .Build();
        }
    }
}
