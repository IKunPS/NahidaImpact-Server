using Microsoft.VisualStudio.TestTools.UnitTesting;
using NahidaImpact.Util;

[assembly: TestDataSourceDiscovery(TestDataSourceDiscoveryOption.DuringExecution)]

namespace NahidaImpact.GameServer.Tests;

[TestClass]
public class TestSetup
{
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext context)
    {
        var baseDir = AppContext.BaseDirectory;
        var solutionRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));

        // Find resource path: try Release first, then Debug
        var resourcePath = Path.Combine(solutionRoot, "NahidaImpact", "bin", "Release", "net10.0", "Resources");
        if (!Directory.Exists(resourcePath))
            resourcePath = Path.Combine(solutionRoot, "NahidaImpact", "bin", "Debug", "net10.0", "Resources");

        ConfigManager.Config.Path.ResourcePath = resourcePath;
        ConfigManager.Config.Path.DatabasePath = Path.Combine(baseDir, "TestDatabase");
        ConfigManager.Config.Path.LogPath = Path.Combine(baseDir, "TestLogs");
        ConfigManager.InitDirectories();

        // Suppress verbose logging during test runs
        Console.SetOut(System.IO.TextWriter.Null);
    }
}
