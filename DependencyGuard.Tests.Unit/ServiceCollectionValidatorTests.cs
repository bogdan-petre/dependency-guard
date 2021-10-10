using DependencyGuard.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using Xunit;

namespace DependencyGuard.Tests.Unit
{
    public class ServiceCollectionValidatorTests
    {
        [Fact]
        public void ValidateAtStartup_AllServicesRegisteredAsInterfaces_NoExceptionThrown()
        {
            ServiceCollection collection = new ServiceCollection();
            var builder = new DynamicTypeBuilder();
            string currentAssembly = Assembly.GetExecutingAssembly().GetName().Name;

            Type interface1 = builder.CreateType(currentAssembly, "Interface1", TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public)
                                    .Build();
            Type interface2 = builder.CreateType(currentAssembly, "Interface2", TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public)
                                    .Build();
            Type class1 = builder.CreateType(currentAssembly, "Class1")
                                .WithConstructorParameters(interface1)
                                .Build();
            Type class2 = builder.CreateType(currentAssembly, "Class2")
                                .WithConstructorParameters(interface2)
                                .Build();


            collection.AddScoped(interface1, class1);
            collection.AddScoped(interface2, class2);

            collection.ValidateAtStartup(builder.Assembly);
        }

        [Fact]
        public void ValidateAtStartup_AllServicesRegisteredAsImplementations_NoExceptionThrown()
        {
            ServiceCollection collection = new ServiceCollection();
            var builder = new DynamicTypeBuilder();
            Type class1 = builder.CreateType("Assembly", "Class1")
                                .Build();
            Type class2 = builder.CreateType("Assembly", "Class2")
                                .WithConstructorParameters(class1)
                                .Build();
            Type class3 = builder.CreateType("Assembly", "Class3")
                                .WithConstructorParameters(class2)
                                .Build();
            collection.AddScoped(class1, class1);
            collection.AddScoped(class2, class2);
            collection.AddScoped(class3, class3);


            collection.ValidateAtStartup(builder.Assembly);
        }


        //TODO: fix test when DynamicTypeBuilder is capable of creating multiple constructor classes.
        //[Fact]
        //public void ValidateAtStartup_ClassWithMultipleConstructors_NoExceptionThrown()
        //{
        //    ServiceCollection collection = new ServiceCollection();


        //    collection.ValidateAtStartup(Assembly.GetExecutingAssembly());
        //}

     
        [Fact]
        public void ValidateAtStartup_UnregisteredService_ExceptionThrown()
        {
            ServiceCollection collection = new ServiceCollection();            
            var builder = new DynamicTypeBuilder();
            Type class1 = builder.CreateType("Assembly", "Class1")
                                .Build();
            Type class2 = builder.CreateType("Assembly", "Class2")
                                .WithConstructorParameters(class1)
                                .Build();
            collection.AddScoped(class2, class2);
            Assert.Throws<ServiceNotRegisteredException>(() => collection.ValidateAtStartup(builder.Assembly));
        }     
    }
}
