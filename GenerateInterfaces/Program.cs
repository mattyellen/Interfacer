using System.IO;
using Interfacer;

namespace GenerateInterfaces
{
    class Program
    {
        static void Main(string[] args)
        {
	        var code = new Generator("v3.5").GenerateAll<IAutogenerate>();
            File.WriteAllText(args[0], code);
        }
    }
}
