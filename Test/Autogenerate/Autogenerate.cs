using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Interfacer.Attributes;
using Test.Fixtures;
using Test.TestClasses;

namespace Test.Autogenerate
{
    public interface IAutogenerate { }

    [ApplyToInstance(typeof(Process), Autogenerate = true)]
    public partial interface IProcess
    {
    }

    [ApplyToInstance(typeof(TestObject), Autogenerate = true)]
    public partial interface ITestObjectAuto
    {
    }

    [ApplyToInstance(typeof(TestObjectWithGenericTypes<,>), Autogenerate = true)]
    public partial interface ITestObjectWithGenericTypesAuto<T1,T2>
    {
    }
}
