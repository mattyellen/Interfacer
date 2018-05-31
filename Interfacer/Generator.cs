using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Interfacer.Attributes;
using Interfacer.Generators;

namespace Interfacer
{
    public class Generator
    {
        public Generator()
        {
        }

        public Generator(string frameworkVersion, string profileDir = null)
        {
            UseReferenceAssembly = new ReferenceAssemblyOptions(frameworkVersion)
            {
                ProfileDir = profileDir
            };
        }

        public ReferenceAssemblyOptions UseReferenceAssembly { get; set; }

        public string GenerateAll<T>()
        {
            return GenerateAll(typeof(T));
        }

        public string GenerateAll(Type rootType)
        {
            var stringWriter = new StringWriter();
            foreach (var interfacerType in
                from t in rootType.Assembly.GetExportedTypes()
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
            if (UseReferenceAssembly == null)
            {
                return @class;
            }

            var referenceAssemblyPath = Path.Combine(UseReferenceAssembly.RootDir, UseReferenceAssembly.Version);
            if (UseReferenceAssembly.ProfileDir != null)
            {
                referenceAssemblyPath = Path.Combine(referenceAssemblyPath, UseReferenceAssembly.ProfileDir);
            }

            var systemRoot = Environment.GetEnvironmentVariable("SystemRoot") ?? @"C:\Windows";
            var frameworkDir = Path.Combine(systemRoot, @"Microsoft.NET\Framework");
            var versionDirs = 
                from d in Directory.GetDirectories(frameworkDir, "v*")
                let dirName = Path.GetFileName(d)
                where dirName.StartsWith(UseReferenceAssembly.Version) ||
                      dirName.CompareTo(UseReferenceAssembly.Version) <= 0
                orderby d descending 
                select d;

            var file = Path.GetFileName(@class.Assembly.Location);
            var checkPaths = new[] {referenceAssemblyPath}.Concat(versionDirs);

            foreach (var path in checkPaths)
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

    public class ReferenceAssemblyOptions
    {
        private string _rootDir;

        public ReferenceAssemblyOptions(string version)
        {
            Version = version;
            RootDir = @"Reference Assemblies\Microsoft\Framework\.NETFramework";
        }

        public string RootDir
        {
            get => _rootDir;
            set => _rootDir = Path.Combine(ProgramFilesx86, value);
        }

        public string Version { get; set; }
        public string ProfileDir { get; set; }

        private static string ProgramFilesx86
        {
            get
            {
                if (IntPtr.Size == 8 ||
                    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")))
                {
                    return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                }

                return Environment.GetEnvironmentVariable("ProgramFiles");
            }
        }
    }
}
