using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Interfacer.Proxies
{
    internal interface ICanGetWrappedObject
    {
        // ReSharper disable once InconsistentNaming
        object __InterfacerWrappedObject__ { get; }
    }
}
