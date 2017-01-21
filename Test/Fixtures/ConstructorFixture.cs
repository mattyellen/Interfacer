using Interfacer;
using Interfacer.Attributes;
using NUnit.Framework;
using Test.TestClasses;

namespace Test.Fixtures
{
    [Interfacer(WrappedObjectType.Static, typeof(TestObject))]
    public interface ITestConstructorInterface : ITestInterface
    {
        [Constructor]
        ITestInstanceInterface Create();

        [Constructor]
        ITestInstanceInterface Create(int value);
    }

    [TestFixture]
    public class ConstructorFixture
    {
        [Test]
        public void ShouldCallConstructorWithNoArgs()
        {
            var factory = InterfacerFactory.Create<ITestConstructorInterface>();
            var obj = factory.Create();

            Assert.That(obj.DoIt, Throws.Nothing);
        }

        [Test]
        public void ShouldCallConstructorWithArgs()
        {
            var expectedValue = 123;
            var factory = InterfacerFactory.Create<ITestConstructorInterface>();
            var obj = factory.Create(expectedValue);

            Assert.That(obj.Value, Is.EqualTo(expectedValue));
        }
    }
}
