using DependencyGuard.Exceptions;
using DependencyGuard.Tests.Unit.Classes;
using DependencyGuard.Tests.Unit.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DependencyGuard.Tests.Unit
{
    public class ServiceCollectionValidatorTests
    {
        [Fact]
        public void ValidateAtStartup_AllServicesRegisteredAsInterfaces_NoExceptionThrown()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.AddScoped<Interface1, Class1>();
            collection.AddScoped<Interface2, Class2>();
            collection.AddScoped<Interface3, Class3>();

            collection.ValidateAtStartup();
        }

        [Fact]
        public void ValidateAtStartup_AllServicesRegisteredAsImplementations_NoExceptionThrown()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.AddScoped<Class1, Class1>();
            collection.AddScoped<Class2, Class2>();
            collection.AddScoped<Class3, Class3>();

            collection.ValidateAtStartup();
        }

        [Fact]
        public void ValidateAtStartup_ClassWithMultipleConstructors_NoExceptionThrown()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.AddScoped<Interface1, Class1>();
            collection.AddScoped<Interface2, Class2>();
            collection.AddScoped<Interface3, Class3>();
            collection.AddScoped<ClassWithMultipleConstructors, ClassWithMultipleConstructors>();
            collection.ValidateAtStartup();
        }

        [Fact]
        public void ValidateAtStartup_ImplementationTypeIsNull_NoExceptionThrown()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.AddScoped<Interface1, Class1>();
            collection.AddScoped<Interface2, Class2>();
            collection.AddScoped<Interface3, Class3>();
            collection.AddScoped<ClassWithMultipleConstructors, ClassWithMultipleConstructors>();
            collection.AddScoped<Interface1, Interface1>();

            collection.ValidateAtStartup();
        }

        [Fact]
        public void ValidateAtStartup_UnregisteredService_ExceptionThrown()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.AddScoped<Interface1, Class1>();
            Assert.Throws<ServiceNotRegisteredException>(() => collection.ValidateAtStartup());
        }
    }
}
