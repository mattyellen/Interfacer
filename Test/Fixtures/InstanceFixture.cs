using Interfacer.Attributes;
using NUnit.Framework;
using TestClasses;

namespace Test.Fixtures
{
    [ApplyToInstance(typeof(TestObject))]
    public interface ITestObject : ITestObjectBase
    {

    }

    [ApplyToInstance(typeof(TestObject))]
    public interface IValidTestObject : ITestObjectValidBase
    {

    }

    [TestFixture]
    public class InstanceFixture : ProxyFixtureBase<ITestObject, IValidTestObject>
    {
    }
}
