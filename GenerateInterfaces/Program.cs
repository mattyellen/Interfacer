using System.IO;
using Interfacer;

namespace GenerateInterfaces
{
    public class Program
    {
        static void Main(string[] args)
        {
            var moniker = args[0];
            var file = args[1];

            var assembly = typeof(IAutogenerate).Assembly;
	        var code = new Generator().WithTargetFramework(moniker).GenerateAll(assembly);
            File.WriteAllText(file, code);
        }
    }
}
