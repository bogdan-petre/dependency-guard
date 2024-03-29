﻿using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DependencyGuard.Tests.Unit
{
    /// <summary>
    /// Build types dynamically.
    /// </summary>
    public class DynamicTypeBuilder
    {
        private TypeBuilder _typeBuilder;
        public AssemblyBuilder Assembly { get; private set; }

        private ModuleBuilder _moduleBuilder;

        /// <summary>
        /// Will create the defined type with Public accessor by default.
        /// </summary>
        /// <param name="nameOfAssembly">The name of the assembly</param>
        /// <param name="typeName">The name of the type</param>
        /// <returns></returns>
        public DynamicTypeBuilder CreateType(string nameOfAssembly, string typeName)
        {
            if (_moduleBuilder is null) CreateModuleBuilderInstance(nameOfAssembly);
            _typeBuilder = _moduleBuilder.DefineType(typeName, TypeAttributes.Public);
            return this;
        }

        /// <summary>
        /// Will create the defined type.
        /// </summary>
        /// <param name="nameOfAssembly">The name of the assembly</param>
        /// <param name="typeName">The name of the type</param>
        /// <returns></returns>
        public DynamicTypeBuilder CreateType(string nameOfAssembly, string typeName, TypeAttributes typeAttributes)
        {
            if (_moduleBuilder is null) CreateModuleBuilderInstance(nameOfAssembly);
            _typeBuilder = _moduleBuilder.DefineType(typeName, typeAttributes);
            return this;
        }

        /// <summary>
        /// Adds an attribute to the currently created type.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="constructorParams"></param>
        /// <returns></returns>
        public DynamicTypeBuilder WithAttribute(Type attributeType, Type[] constructorParams)
        {
            ConstructorInfo constructorInfo = attributeType.GetConstructor(constructorParams);

            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(constructorInfo, new object[] { });
            _typeBuilder.SetCustomAttribute(customAttributeBuilder);
            return this;
        }

        public DynamicTypeBuilder CreateAssembly(string assemblyName)
        {
            CreateModuleBuilderInstance(assemblyName);
            return this;
        }

        private void CreateModuleBuilderInstance(string nameOfAssembly)
        {
            AssemblyName assemblyName = new AssemblyName(nameOfAssembly);
            Assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            _moduleBuilder = Assembly.DefineDynamicModule($"{Assembly.GetName().Name}.dll");
        }

        /// <summary>
        /// Build type based on current state.
        /// </summary>
        /// <returns></returns>
        public Type Build()
        {
            return _typeBuilder.CreateType();
        }

        /// <summary>
        /// Add constructor parameters to the type.
        /// </summary>
        /// <param name="parameters">An array of parameter types.</param>
        /// <returns></returns>
        public DynamicTypeBuilder WithConstructorParameters(params Type[] parameters)
        {
            if (_typeBuilder is null) throw new InvalidOperationException($"No assembly name and type name specified. Call {nameof(CreateType)} first.");
            ConstructorBuilder constructorBuilder = _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameters);
            ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            return this;
        }
    }
}