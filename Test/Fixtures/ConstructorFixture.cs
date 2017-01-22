using System.Collections.Generic;
using Interfacer;
using Interfacer.Attributes;
using NUnit.Framework;
using Test.TestClasses;

namespace Test.Fixtures
{
    [ApplyToStatic(typeof(TestObject))]
    public interface ITestObjectFactory : ITestObjectBase
    {
        [Constructor]
        ITestObject Create();

        [Constructor]
        ITestObject Create(int value);
    }

    [ApplyToInstance(typeof(TestObjectWithGenericTypes<,>))]
    public interface ITestObjectWithGenericTypes<T1, T2>
    {
        T1 Value1 { get; }
        T2 Value2 { get; }
    }

    [ApplyToStatic(typeof(TestObjectWithGenericTypes<,>))]
    public interface ITestObjectWithGenericTypesFactory<T1>
    {
        [Constructor]
        ITestObjectWithGenericTypes<T1, T2> Create<T2>(T1 v1, T2 v2);
    }

    [TestFixture]
    public class ConstructorFixture
    {
        [Test]
        public void ShouldCallConstructorWithNoArgs()
        {
            var factory = InterfacerFactory.Create<ITestObjectFactory>();
            var obj = factory.Create();

            Assert.That(obj.DoIt, Throws.Nothing);
        }

        [Test]
        public void ShouldCallConstructorWithArgs()
        {
            var expectedValue = 123;
            var factory = InterfacerFactory.Create<ITestObjectFactory>();
            var obj = factory.Create(expectedValue);

            Assert.That(obj.Value, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ShouldCallConstructorWithGenericTypes()
        {
            var expectedInt = 123;
            var expectedList = new List<int> {1,2,3};
            var factory = InterfacerFactory.Create<ITestObjectWithGenericTypesFactory<int>>();
            var obj = factory.Create(expectedInt, expectedList);

            Assert.That(obj.Value1, Is.EqualTo(expectedInt));
            Assert.That(obj.Value2, Is.EquivalentTo(expectedList));
        }
    }
}
