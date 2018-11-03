using System;
using System.Diagnostics;
using Interfacer;
using Interfacer.Generators;
using NUnit.Framework;
using Test.Autogenerate;
using Test.TestClasses;

namespace Test.Fixtures
{
    [TestFixture]
    public class InstanceGeneratorFixture
    {
        [Test]
        public void ShouldGenerateInterface()
        {
            var generator = new InstanceGenerator(typeof(IValidTestObject), typeof(TestObject));
            Debug.WriteLine(generator.GetInterface());
        }

        public interface ITestInterface { }

        [Test]
        public void ShouldGenerateAnotherInterface()
        {
            var generator = new InstanceGenerator(typeof(ITestInterface), typeof(Process));
            Debug.WriteLine(generator.GetInterface());
        }

        [Test]
        public void ShouldGenerateAllInterfaces()
        {
            var p = Process.GetCurrentProcess();
            var pt = Type.GetType("Microsoft.Win32.SafeHandles.SafeProcessHandle");

            Debug.WriteLine(new Generator()
            {
                UseReferenceAssembly = new ReferenceAssemblyOptions("v3.5")
            }.GenerateAll<IAutogenerate>());
        }
    }
}
