using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfacer;
using Interfacer.Attributes;
using Interfacer.Exceptions;
using NUnit.Framework;
using Test.TestClasses;

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
