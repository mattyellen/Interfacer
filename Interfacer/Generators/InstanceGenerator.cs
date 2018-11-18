using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Interfacer.Utility;
using Microsoft.CSharp;

namespace Interfacer.Generators
{
    internal class InstanceGenerator
    {
        private readonly Type _targetInterface;
        private readonly Type _targetClass;

        public InstanceGenerator(Type targetInterface, Type targetClass)
        {
            _targetInterface = targetInterface;
            _targetClass = targetClass;
        }

        public string GetInterface()
        {
            var interfaceNamespace = new CodeNamespace(_targetInterface.Namespace);

            var name = _targetInterface.Name;
            if (name.Contains('`'))
            {
                name = name.Remove(name.IndexOf('`'));
            }

            var generatedInterface = new CodeTypeDeclaration(name)
            {
                IsPartial = true,
                IsInterface = true
            };

            generatedInterface.TypeParameters.AddRange(GetTypeParameters(_targetInterface.GetGenericArguments()));

            foreach (var propertyInfo in _targetClass.GetProperties().Where(p => !IsStatic(p)))
            {
                var interfaceProperty = new CodeMemberProperty()
                {
                    Name = propertyInfo.Name,
                    Type = new CodeTypeReference(propertyInfo.PropertyType.FullName ?? propertyInfo.PropertyType.Name),
                    HasGet = propertyInfo.GetGetMethod() != null,
                    HasSet = propertyInfo.GetSetMethod() != null                    
                };
                interfaceProperty.Comments.Add(new CodeCommentStatement(propertyInfo.ToString()));

                generatedInterface.Members.Add(interfaceProperty);
            }

            foreach (var methodInfo in _targetClass.GetMethods().Where(m => !m.IsSpecialName && !m.IsStatic))
            {
                var methodInfoName = methodInfo.Name;
                var interfaceMethod = new CodeMemberMethod()
                {
                    Name = methodInfoName,
                    ReturnType = new CodeTypeReference(methodInfo.ReturnType.ToString())
                };
                interfaceMethod.Comments.Add(new CodeCommentStatement(methodInfo.ToString()));

                interfaceMethod.TypeParameters.AddRange(GetTypeParameters(methodInfo.GetGenericArguments()));

                foreach (var parameterInfo in methodInfo.GetParameters())
                {
                    var parameterType = parameterInfo.ParameterType;
                    var elementType = parameterType.IsByRef ? parameterType.GetElementType() : parameterType;
                    var parameterExpression = new CodeParameterDeclarationExpression(elementType.ToString(), parameterInfo.Name)
                    {
                        Direction = parameterType.IsByRef ?
                            (parameterInfo.IsOut ?
                                FieldDirection.Out :
                                FieldDirection.Ref) :
                            FieldDirection.In
                    };

                    interfaceMethod.Parameters.Add(
                        parameterExpression);
                }
                generatedInterface.Members.Add(interfaceMethod);
            }

            interfaceNamespace.Types.Add(generatedInterface);

            var stringWriter = new StringWriter();
            var csProv = new CSharpCodeProvider();
            csProv.GenerateCodeFromNamespace(interfaceNamespace, stringWriter, new CodeGeneratorOptions());
            return stringWriter.ToString();
        }

        private bool IsStatic(PropertyInfo propertyInfo)
        {
            var methodInfo = propertyInfo.GetGetMethod() ?? propertyInfo.GetSetMethod();
            return methodInfo.IsStatic;
        }

        private CodeTypeParameter[] GetTypeParameters(Type[] genericArgs)
        {
            var typeParameters = new List<CodeTypeParameter>();
            foreach (var genericArgument in genericArgs)
            {
                var typeParam = new CodeTypeParameter(genericArgument.Name);

                if (genericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes
                    .ReferenceTypeConstraint))
                {
                    typeParam.Constraints.Add(" class");
                }
                if (genericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes
                    .NotNullableValueTypeConstraint))
                {
                    typeParam.Constraints.Add(" struct");
                }

                typeParam.Constraints.AddRange(genericArgument.GetGenericParameterConstraints()
                    .Select(c => new CodeTypeReference(c.ToString()))
                    .ToArray());

                typeParam.HasConstructorConstraint =
                    genericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint);

                typeParameters.Add(typeParam);
            }
            return typeParameters.ToArray();
        }
    }
}
