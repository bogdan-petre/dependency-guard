using DependencyGuard.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Reflection;
using Xunit;

namespace DependencyGuard.Tests.Unit
{
    public class ServiceCollectionValidatorTests
    {
        [Fact]
        public void No_exception_thrown_when_all_types_are_registered_as_concrete_abstractions()
        {
            ServiceCollection collection = new ServiceCollection();
            var builder = new DynamicTypeBuilder();
            string currentAssembly = Assembly.GetExecutingAssembly().GetName().Name;

            Type interface1 = builder.CreateType(currentAssembly, "Interface1", TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public).Build();
            Type interface2 = builder.CreateType(currentAssembly, "Interface2", TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public).Build();

            Type class1 = builder.CreateType(currentAssembly, "Class1").WithConstructorParameters(interface1).Build();
            Type class2 = builder.CreateType(currentAssembly, "Class2").WithConstructorParameters(interface2).Build();

            collection.AddScoped(interface1, class1);
            collection.AddScoped(interface2, class2);

            collection.ValidateAtStartup(builder.Assembly, cfg => cfg.WithException = true);
        }

        [Fact]
        public void No_exception_thrown_when_all_types_are_registered_as_concrete_classes()
        {
            ServiceCollection collection = new ServiceCollection();
            var builder = new DynamicTypeBuilder();

            Type class1 = builder.CreateType("Assembly", "Class1").Build();
            Type class2 = builder.CreateType("Assembly", "Class2").WithConstructorParameters(class1).Build();
            Type class3 = builder.CreateType("Assembly", "Class3").WithConstructorParameters(class2).Build();

            collection.AddScoped(class1, class1);
            collection.AddScoped(class2, class2);
            collection.AddScoped(class3, class3);

            collection.ValidateAtStartup(builder.Assembly, cfg => cfg.WithException = true);
        }

        [Fact]
        public void Throws_exception_when_service_is_not_registered()
        {
            ServiceCollection collection = new ServiceCollection();
            var builder = new DynamicTypeBuilder();

            Type class1 = builder.CreateType("Assembly", "Class1").Build();
            Type class2 = builder.CreateType("Assembly", "Class2").WithConstructorParameters(class1).Build();

            collection.AddScoped(class2, class2);

            Assert.Throws<ServiceNotRegisteredException>(() => collection.ValidateAtStartup(builder.Assembly, cfg => cfg.WithException = true));
        }

        [Fact]
        public void No_exception_thrown_when_type_is_not_registered_but_is_annotated()
        {
            ServiceCollection collection = new ServiceCollection();
            var builder = new DynamicTypeBuilder();

            Type class1 = builder.CreateType("Assembly", "Class1").WithAttribute(typeof(IgnoreGuardAttribute), Type.EmptyTypes).Build();
            Type class2 = builder.CreateType("Assembly", "Class2").WithConstructorParameters(class1).Build();

            collection.AddScoped(class2, class2);

            collection.ValidateAtStartup(builder.Assembly, cfg => cfg.WithException = true);
        }        

        [Fact]
        public void No_exception_thrown_when_dependency_is_in_another_assembly()
        {
            ServiceCollection collection = new ServiceCollection();
            var builder = new DynamicTypeBuilder();

            Type class1 = builder.CreateType("Assembly1", "Class1").Build();
            Type class2 = builder.CreateType("Assembly2", "Class2").WithConstructorParameters(class1).Build();

            collection.AddScoped(class1, class1);
            collection.AddScoped(class2, class2);

            collection.ValidateAtStartup(builder.Assembly, cfg => cfg.WithException = true);
        }

        [Fact]
        public void Invokes_action_when_service_not_defined()
        {
            ServiceCollection collection = new ServiceCollection();
            var builder = new DynamicTypeBuilder();
            Type class1 = builder.CreateType("Assembly1", "Class1").Build();
            Type class2 = builder.CreateType("Assembly2", "Class2").WithConstructorParameters(class1).Build();
            collection.AddScoped(class2, class2);

            var actionMock = new Mock<Action>();

            collection.ValidateAtStartup(builder.Assembly, actionMock.Object);
            actionMock.Verify(a => a.Invoke(), Times.Once);
        }

        [Fact]
        public void Action_not_invoked_when_service_is_valid()
        {
            ServiceCollection collection = new ServiceCollection();
            var builder = new DynamicTypeBuilder();
            Type class1 = builder.CreateType("Assembly1", "Class1").Build();
            Type class2 = builder.CreateType("Assembly2", "Class2").WithConstructorParameters(class1).Build();
            collection.AddScoped(class1, class1);
            collection.AddScoped(class2, class2);

            var actionMock = new Mock<Action>();

            collection.ValidateAtStartup(builder.Assembly, actionMock.Object);
            actionMock.Verify(a => a.Invoke(), Times.Never);
        }

        [Fact]
        public void Not_registered_services_are_returned_in_the_callback()
        {
            ServiceCollection collection = new ServiceCollection();
            var builder = new DynamicTypeBuilder();
            Type class1 = builder.CreateType("Assembly1", "Class1").Build();
            Type class2 = builder.CreateType("Assembly2", "Class2").WithConstructorParameters(class1).Build();
            collection.AddScoped(class2, class2);

            collection.ValidateAtStartup(builder.Assembly, (types) =>
            {
                Assert.NotEmpty(types);
            });
        }

        [Fact]
        public void No_services_are_returned_in_the_callback()
        {
            ServiceCollection collection = new ServiceCollection();
            var builder = new DynamicTypeBuilder();
            Type class1 = builder.CreateType("Assembly1", "Class1").Build();
            Type class2 = builder.CreateType("Assembly2", "Class2").WithConstructorParameters(class1).Build();
            collection.AddScoped(class1, class1);
            collection.AddScoped(class2, class2);

            collection.ValidateAtStartup(builder.Assembly, (types) =>
            {
                Assert.Empty(types);
            });
        }

        [Fact]
        public void All_services_not_registered_are_logged()
        {
            ServiceCollection collection = new ServiceCollection();
            var builder = new DynamicTypeBuilder();
            Type class1 = builder.CreateType("Assembly1", "Class1").Build();
            Type class2 = builder.CreateType("Assembly2", "Class2").WithConstructorParameters(class1).Build();
            collection.AddScoped(class2, class2);

            var loggerMock = new Mock<ILogger>();

            collection.ValidateAtStartup(builder.Assembly, loggerMock.Object);

            loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                    Times.Once);
        }

        [Fact]
        public void No_exception_thrown_when_setting_is_not_true()
        {
            ServiceCollection collection = new ServiceCollection();
            var builder = new DynamicTypeBuilder();
            Type class1 = builder.CreateType("Assembly1", "Class1").Build();
            Type class2 = builder.CreateType("Assembly2", "Class2").WithConstructorParameters(class1).Build();
            collection.AddScoped(class2, class2);

            Assert.Throws<ServiceNotRegisteredException>(() => collection.ValidateAtStartup(builder.Assembly, cfg => cfg.WithException = true));
        }

        [Fact]
        public void Logs_once_for_every_service_not_registered()
        {
            ServiceCollection collection = new ServiceCollection();
            var builder = new DynamicTypeBuilder();
            Type class1 = builder.CreateType("Assembly1", "Class1").Build();
            Type class2 = builder.CreateType("Assembly2", "Class2").WithConstructorParameters(class1).Build();
            Type class3 = builder.CreateType("Assembly1", "Class3").WithConstructorParameters(class1).Build();
            collection.AddScoped(class2, class2);

            var loggerMock = new Mock<ILogger>();

            collection.ValidateAtStartup(builder.Assembly, loggerMock.Object);

            loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                    Times.Once);
        }
        
    }
}