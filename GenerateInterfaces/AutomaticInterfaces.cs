using System.Diagnostics;
using Interfacer.Attributes;
using TestClasses;

namespace GenerateInterfaces
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
