using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Interfacer.Generators
{
    public class AdapterGenerator
    {
        private readonly CodeCompileUnit _targetUnit;
        private readonly CodeNamespace _adapterNamespace;

        private readonly IDictionary<Type, string> _adapterObjectName = new Dictionary<Type, string>();
        private Assembly _adapterObjectAssembly;
        private HashSet<string> _referencedAssemblies = new HashSet<string>();

        public AdapterGenerator()
        {
            _targetUnit = new CodeCompileUnit();
            _adapterNamespace = new CodeNamespace("Interfacer.GeneratedAdapter");
            _adapterNamespace.Imports.Add(new CodeNamespaceImport(nameof(System)));
            _targetUnit.Namespaces.Add(_adapterNamespace);
        }

        public void AddAdapterClass(Type @interface, InterfacerAttribute attribute)
        {
            //_adapterNamespace.Imports.Add(new CodeNamespaceImport(attribute.Class.Assembly.FullName));

            _referencedAssemblies.Add(attribute.Class.Assembly.Location);

            AdapterClass adapterClass;
            if (attribute.Type == WrappedObjectType.Instance)
            {
                adapterClass = new InstanceAdapterClass(@interface, attribute.Class);
            }
            else
            {
                adapterClass = new AdapterClass(@interface, attribute.Class);
            }

            var codeTypeDeclaration = adapterClass.GetTypeDeclaration();
            _adapterNamespace.Types.Add(codeTypeDeclaration);
            _adapterObjectName.Add(@interface, _adapterNamespace.Name + "." + codeTypeDeclaration.Name);
        }

        public void Generate()
        {
            DebugPrintCode();
            CompileTargetUnit();
        }

        public TInterface CreateAdapterObject<TInterface>()
        {
            return (TInterface)_adapterObjectAssembly.CreateInstance(_adapterObjectName[typeof(TInterface)]);
        }

        public TInterface CreateAdapterObject<TInterface>(object wrappedObject)
        {
            foreach (var cls in _adapterObjectAssembly.GetTypes())
            {
                Console.WriteLine(cls.FullName);
            }

            var name = _adapterObjectName[typeof(TInterface)];
            var instance = _adapterObjectAssembly.CreateInstance(name, false, BindingFlags.Default, null,
                new[] {wrappedObject}, null, null);

            return (TInterface)instance;
        }

        private void CompileTargetUnit()
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var compilerParameters = new CompilerParameters()
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                OutputAssembly = "AdapterAssembly"
            };
            compilerParameters.ReferencedAssemblies.AddRange(_referencedAssemblies.ToArray());
            var results = provider.CompileAssemblyFromDom(compilerParameters, _targetUnit);

            if (results.Errors.HasErrors)
            {
                var message = new StringBuilder();
                message.AppendLine("Failed to build adapter:");
                foreach (CompilerError error in results.Errors)
                {
                    message.AppendLine(error.ToString());
                }
                throw new Exception(message.ToString());
            }

            _adapterObjectAssembly = results.CompiledAssembly;
        }

        private void DebugPrintCode()
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions {BracingStyle = "C"};
            using (var sourceWriter = new StringWriter())
            {
                provider.GenerateCodeFromCompileUnit(
                    _targetUnit, sourceWriter, options);

                 Console.Write(sourceWriter.ToString());
            }
        }
    }
}
