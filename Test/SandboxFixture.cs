using System.Diagnostics;
using System.Dynamic;
using Interfacer;
using Interfacer.Attributes;
using NUnit.Framework;
using Test.Fixtures;

namespace Test
{
    [TestFixture]
    public class SandboxFixture
    {
        [ApplyToInstance(typeof(Process))]
        public interface IProcess
        {
            ProcessStartInfo StartInfo { get; }
        }

        [Test]
        public void Test()
        {
            var process = InterfacerFactory.Create<IProcess>();
            var info = process.StartInfo;
            Assert.That(info, Is.Not.Null);
        }

        [Test]
        public void VTest()
        {
            InterfacerFactory.Verify<IProcess>();
            InterfacerFactory.Verify<ITestObject>();
            InterfacerFactory.Verify<ITestStaticClass>();
            InterfacerFactory.Verify<ITestObjectFactory>();
            InterfacerFactory.Verify(typeof(ITestObjectWithGenericTypes<,>));
        }
    }
}
