using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Interfacer.Generators
{
    internal class AdapterClass 
    {
        protected readonly Type Interface;
        protected readonly Type WrappedObjectType;

        public AdapterClass(Type @interface, Type wrappedObjectType)
        {
            Interface = @interface;
            WrappedObjectType = wrappedObjectType;
        }

        internal CodeTypeDeclaration GetTypeDeclaration()
        {
            var adapterClass = new CodeTypeDeclaration("AdapterFor_" + Interface.Name)
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            adapterClass.BaseTypes.Add(new CodeTypeReference(Interface.FullName));

            AddConstructor(adapterClass);

            foreach(var property in Interface.GetProperties())
            {
                var adapterProperty = new CodeMemberProperty
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Final,
                    Name = property.Name,
                    Type = new CodeTypeReference(property.PropertyType.FullName),
                    HasGet = property.CanRead,
                    HasSet = property.CanWrite
                };

                if (adapterProperty.HasGet)
                {
                    AddPropertyGetter(adapterProperty);
                }

                if (adapterProperty.HasSet)
                {
                    AddPropertySetter(adapterProperty);
                }

                adapterClass.Members.Add(adapterProperty);
            }

            foreach (var method in Interface.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => !m.IsSpecialName))
            {
                var adapterMethod = new CodeMemberMethod()
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Final,
                    Name = method.Name,
                    ReturnType = new CodeTypeReference(method.ReturnType.FullName ?? method.ReturnType.Name)
                };

                if (method.IsGenericMethod)
                {
                    var genericTypes = new List<CodeTypeParameter>();
                    foreach (var t in method.GetGenericArguments())
                    {
                        var typeParam = new CodeTypeParameter(t.Name);
                        foreach (var c in t.GetGenericParameterConstraints())
                        {
                            typeParam.Constraints.Add(new CodeTypeReference(c));
                        }

                        if (t.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
                        {
                            typeParam.HasConstructorConstraint = true;
                        }
                        genericTypes.Add(typeParam);
                    }

                    adapterMethod.TypeParameters.AddRange(genericTypes.ToArray());
                }

                adapterMethod.Parameters.AddRange(
                    (from a in method.GetParameters()
                     select new CodeParameterDeclarationExpression(a.ParameterType, a.Name)
                     ).ToArray());

                AddMethodBody(adapterMethod, method);
                adapterClass.Members.Add(adapterMethod);
            }

            return adapterClass;
        }

        protected virtual void AddPropertyGetter(CodeMemberProperty adapterProperty)
        {
        }

        protected virtual void AddPropertySetter(CodeMemberProperty adapterProperty)
        {
        }

        protected virtual void AddConstructor(CodeTypeDeclaration adapterClass)
        {
        }

        protected virtual void AddMethodBody(CodeMemberMethod adapterMethod, MethodInfo method)
        {
        }
    }
}