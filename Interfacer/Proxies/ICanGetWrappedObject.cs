using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Interfacer.Proxies
{
    internal interface ICanGetWrappedObject
    {
        // ReSharper disable once InconsistentNaming
        object __InterfacerWrappedObject__ { get; }
    }
}
