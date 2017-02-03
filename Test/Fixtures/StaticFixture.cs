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
