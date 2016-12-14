using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Interfacer.Generators
{
    internal class InstanceAdapterClass : AdapterClass
    {
        private readonly CodeFieldReferenceExpression _thisWrappedObjectRef;

        internal InstanceAdapterClass(Type @interface, Type wrappedObjectType) : base(@interface, wrappedObjectType)
        {
            _thisWrappedObjectRef = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_wrappedObject");
        }

        protected override void AddConstructor(CodeTypeDeclaration adapterClass)
        {
            adapterClass.Members.Add(new CodeMemberField()
            {
                Attributes = MemberAttributes.Private,
                Name = _thisWrappedObjectRef.FieldName,
                Type = new CodeTypeReference(WrappedObjectType)
            });

            const string constructorArg = "wrappedObject";
            var constructor = new CodeConstructor { Attributes = MemberAttributes.Public | MemberAttributes.Final };
            constructor.Parameters.Add(new CodeParameterDeclarationExpression(
                WrappedObjectType, constructorArg));

            constructor.Statements.Add(new CodeAssignStatement(_thisWrappedObjectRef,
                new CodeArgumentReferenceExpression(constructorArg)));

            adapterClass.Members.Add(constructor);
        }

        protected override void AddPropertyGetter(CodeMemberProperty adapterProperty)
        {
            adapterProperty.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(
                    _thisWrappedObjectRef, adapterProperty.Name)));
        }

        protected override void AddPropertySetter(CodeMemberProperty adapterProperty)
        {
            adapterProperty.SetStatements.Add(new CodeAssignStatement(
                new CodeFieldReferenceExpression(_thisWrappedObjectRef, adapterProperty.Name),
                new CodePropertySetValueReferenceExpression()));
        }

        protected override void AddMethodBody(CodeMemberMethod adapterMethod, MethodInfo method)
        {
            var returnStatement =
                new CodeMethodReturnStatement();

            var args = from a in method.GetParameters()
                select new CodeArgumentReferenceExpression(a.Name);

            returnStatement.Expression =
                new CodeMethodInvokeExpression(
                _thisWrappedObjectRef,
                adapterMethod.Name,
                args.Cast<CodeExpression>().ToArray());

            adapterMethod.Statements.Add(returnStatement);
        }
    }
}
