using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfacer;
using Interfacer.Attributes;
using NUnit.Framework;
using Test.TestClasses;

namespace Test.Fixtures
{
    [Interfacer(WrappedObjectType.Instance, typeof(TestObject))]
    public interface ITestInstanceInterface : ITestInterface
    {

    }

    [TestFixture]
    public class InstanceFixture : ProxyFixtureBase<ITestInstanceInterface>
    {
    }
}
