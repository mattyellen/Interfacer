﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Interfacer.Attributes;
using Interfacer.Generators;

namespace Interfacer
{
    public class Generator
    {
        private readonly List<string> _referenceAssemblyDirectories = new List<string>();

        public Generator WithTargetFramework(TargetFramework.Moniker moniker)
        {
            _referenceAssemblyDirectories.AddRange(TargetFramework.GetReferenceAssemblyDirectories(moniker));
            return this;
        }

		public Generator WithTargetFramework(string monikerName)
        {
            _referenceAssemblyDirectories.AddRange(TargetFramework.GetReferenceAssemblyDirectories(monikerName));
            return this;
        }

        public Generator WithReferenceAssemblyDirectory(string directory)
        {
            _referenceAssemblyDirectories.Add(directory);
            return this;
        }

		public string GenerateAll(Assembly assembly)
        {
            var stringWriter = new StringWriter();
            foreach (var interfacerType in
                from t in assembly.GetExportedTypes()
                where t.IsInterface
                let a = InterfacerFactory.GetInterfacerAttribute(t)
                where a != null && a.Autogenerate
                select t)
            {
                stringWriter.Write(Generate(interfacerType));
            }

            return stringWriter.ToString();
        }

        public string Generate<TInterface>() where TInterface : class
        {
            return Generate(typeof(TInterface));
        }

        public string Generate(Type interfacerType)
        {
            InterfacerFactory.VerifyInterfaceType(interfacerType);

            var attribute = InterfacerFactory.GetInterfacerAttribute(interfacerType);
            var targetClass = LoadClass(attribute.Class);
            if (attribute is ApplyToInstanceAttribute)
            {
                var generator = new InstanceGenerator(interfacerType, targetClass);
                return generator.GetInterface();
            }

            throw new NotImplementedException();
        }

        private Type LoadClass(Type @class)
        {
            if (!_referenceAssemblyDirectories.Any())
            {
                return @class;
            }

            var file = Path.GetFileName(@class.Assembly.Location);
            foreach (var path in _referenceAssemblyDirectories)
            {
                var dllPath = Path.Combine(path, file);
                if (File.Exists(dllPath))
                {
                    var assembly = Assembly.ReflectionOnlyLoadFrom(dllPath);
                    return assembly.GetType(@class.FullName);
                }
            }

            return @class;
        }
    }
}
