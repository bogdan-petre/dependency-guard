using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace DependencyGuard.Tests.Unit
{
    public class DependencyExtractorTests
    {
        [Fact]
        public void Extracts_a_type_only_once()
        {
            var builder = new DynamicTypeBuilder();
            Type type1 = builder.CreateType("Assembly1", "Class1").Build();
            builder.CreateType("Assembly2", "Class2").WithConstructorParameters(type1).Build();
            builder.CreateType("Assembly2", "Class3").WithConstructorParameters(type1).Build();

            var constructorParameters = DependencyExtractor.ExtractConstructorParameters(builder.Assembly);

            Assert.Single(constructorParameters);
        }

        [Fact]
        public void Throws_argument_exception_when_assembly_is_null()
        {
            ServiceCollection collection = new ServiceCollection();
            Assert.Throws<ArgumentException>(() => collection.ValidateAtStartup(null, cfg => cfg.WithException = true));
        }
    }
}
