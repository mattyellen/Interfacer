using Interfacer.Attributes;
using NUnit.Framework;
using TestClasses;

namespace Test.Fixtures
{
    [ApplyToStatic(typeof(TestStaticClass))]
    public interface ITestStaticClass : ITestObjectBase
    {
    }

    [ApplyToStatic(typeof(TestStaticClass))]
    public interface IValidTestStaticClass : ITestObjectValidBase
    {
    }

    [TestFixture]
    public class StaticFixture : ProxyFixtureBase<ITestStaticClass, IValidTestStaticClass>
    {
    }
}
