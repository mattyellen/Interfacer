using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfacer;
using NUnit.Framework;
using Test.TestClasses;

namespace Test.Fixtures
{
    [Interfacer(WrappedObjectType.Static, typeof(TestStaticClass))]
    public interface ITestStaticInterface : ITestInterface
    {

    }

    [TestFixture]
    public class StaticFixture : ProxyFixtureBase<ITestStaticInterface>
    {
    }
}
