using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Castle.Core.Resource;

namespace Interfacer
{
    public static class TargetFramework
    {
        private static readonly string SystemRoot = Environment.GetEnvironmentVariable("SystemRoot") ?? @"C:\Windows";
        private static readonly string ProgramFiles = Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? 
                                                      Environment.GetEnvironmentVariable("ProgramFiles") ??
                                                      @"C:\Program Files (x86)";


	    private static Dictionary<string, List<string>> _referenceAssemblyDirectories;

	    public static List<string> GetReferenceAssemblyDirectories(Moniker moniker)
	    {
		    return _referenceAssemblyDirectories[GetMonikerName(moniker)];
	    }

	    public static List<string> GetReferenceAssemblyDirectories(string moniker)
	    {
		    if (!_referenceAssemblyDirectories.ContainsKey(moniker))
		    {
			    throw new InvalidMoniker(moniker);
		    }

		    return _referenceAssemblyDirectories[moniker];
	    }

		static TargetFramework()
	    {

#if NETSTANDARD2_0
	    var homePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
		    ? Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%")
		    : Environment.GetEnvironmentVariable("HOME");

			_referenceAssemblyDirectories = GetCoreReferenceAssemblies(homePath);
#else
		    var frameworkAssemblies = GetFrameworkReferenceAssemblies();

		    var homePath = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
		    var coreAssemblies = GetCoreReferenceAssemblies(homePath);

			_referenceAssemblyDirectories = frameworkAssemblies.Concat(coreAssemblies)
			    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
#endif

		}

		private static Dictionary<string, List<string>> GetFrameworkReferenceAssemblies()
	    {
		    var frameworkAssemblies = new Dictionary<string, List<string>>();

		    var root = PathCombine(SystemRoot, ProgramFiles, @"Reference Assemblies\Microsoft\Framework");
		    var referenceAssemblyMap = new Dictionary<Moniker, string>()
		    {
			    [Moniker.Net11] = Path.Combine(root, "v1.1"),
			    [Moniker.Net20] = Path.Combine(root, "v2.0"),
			    [Moniker.Net30] = Path.Combine(root, "v3.0"),
			    [Moniker.Net35] = Path.Combine(root, "v3.5"),
			    [Moniker.Net40] = Path.Combine(root, @".NETFramework\v4.0"),
			    [Moniker.Net403] = Path.Combine(root, @".NETFramework\v4.0.3"),
			    [Moniker.Net45] = Path.Combine(root, @".NETFramework\v4.5"),
			    [Moniker.Net451] = Path.Combine(root, @".NETFramework\v4.5.1"),
			    [Moniker.Net452] = Path.Combine(root, @".NETFramework\v4.5.2"),
			    [Moniker.Net46] = Path.Combine(root, @".NETFramework\v4.6"),
			    [Moniker.Net461] = Path.Combine(root, @".NETFramework\v4.6.1"),
			    [Moniker.Net462] = Path.Combine(root, @".NETFramework\v4.6.2"),
			    [Moniker.Net47] = Path.Combine(root, @".NETFramework\v4.7"),
			    [Moniker.Net471] = Path.Combine(root, @".NETFramework\v4.7.1"),
			    [Moniker.Net472] = Path.Combine(root, @".NETFramework\v4.7.2")
		    };

		    var frameworkFallback = Enum.GetValues(typeof(Moniker)).Cast<Moniker>().Reverse().ToList();
		    var net11Index = frameworkFallback.IndexOf(Moniker.Net11);
		    foreach (var moniker in referenceAssemblyMap.Keys)
		    {
			    var startIndex = frameworkFallback.IndexOf(moniker);
			    var paths =
				    from i in Enumerable.Range(startIndex, net11Index - startIndex + 1)
				    select referenceAssemblyMap[frameworkFallback[i]];

			    frameworkAssemblies.Add(GetMonikerName(moniker), paths.ToList());
		    }

		    return frameworkAssemblies;
	    }

	    public enum Moniker
	    {
		    NetStandard2_0,
		    NetCoreApp2_0,
		    NetCoreApp2_1,
		    Net11,
		    Net20,
		    Net30,
		    Net35,
		    Net40,
		    Net403,
		    Net45,
		    Net451,
		    Net452,
		    Net46,
		    Net461,
		    Net462,
		    Net47,
		    Net471,
		    Net472
	    }

		private static Dictionary<string, List<string>> GetCoreReferenceAssemblies(string homePath)
		{
			var result = new Dictionary<string, List<string>>();
			foreach (var netStandardMoniker in GetMonikersFor("netstandard"))
			{
				result.Add(GetMonikerName(netStandardMoniker),
					GetCoreRefPaths(homePath, "netstandard.library", netStandardMoniker, "build/{0}/ref"));
			}
			
			foreach (var netCoreMoniker in GetMonikersFor("netcoreapp"))
			{
				result.Add(GetMonikerName(netCoreMoniker),
					GetCoreRefPaths(homePath, "microsoft.netcore.app", netCoreMoniker, "ref/{0}"));
			}

			return result;
		}

	    private static IEnumerable<Moniker> GetMonikersFor(string prefix)
	    {
		    return from m in Enum.GetValues(typeof(Moniker)).Cast<Moniker>()
			    where GetMonikerName(m).StartsWith(prefix)
			    select m;
	    }

	    private static List<string> GetCoreRefPaths(string homePath, string package, Moniker moniker,
		    string relativeDirTemplate)
	    {
		    var packageDir = PathCombine(homePath, ".nuget", "packages", package);
		    var versions = new DirectoryInfo(packageDir).GetDirectories().Select(di => di.Name).Reverse();

		    var monikerPaths = from v in versions
			    select PathCombine(packageDir, v, string.Format(relativeDirTemplate, GetMonikerName(moniker)));

		    return (from p in monikerPaths
			    where Directory.Exists(p)
			    select p).ToList();
	    }

	    private static string GetMonikerName(Moniker moniker)
	    {
		    return moniker.ToString().ToLower().Replace("_", ".");
	    }

	    private static string PathCombine(string root, params string[] paths)
	    {
		    return paths.Aggregate(root, Path.Combine);
	    }
    }

	public class InvalidMoniker : Exception
	{
		public string Moniker { get; }

		public InvalidMoniker(string moniker)
		{
			Moniker = moniker;
		}

		public override string Message => $"Invalid target framework moniker: {Moniker}";
	}
}
