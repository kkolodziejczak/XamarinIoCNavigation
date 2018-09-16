#addin "nuget:?package=Cake.MiniCover&version=0.29.0-next20180721071547&prerelease"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

const string SOLUTION = "./TestDI.sln";
SetMiniCoverToolsProject("./minicover/minicover.csproj");

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    DotNetCoreClean(SOLUTION);
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore(SOLUTION);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
	DotNetCoreBuild("./TestDI.Tests/TestDI.Tests.csproj", new DotNetCoreBuildSettings {
        Configuration = configuration,
        NoRestore = true
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() => 
{
    MiniCover(tool => 
        {
			DotNetCoreTest("./TestDI.Tests/TestDI.Tests.csproj", new DotNetCoreTestSettings
			{
				Configuration = configuration,
				NoRestore = true,
				NoBuild = true
			});
        },
        new MiniCoverSettings()
            .WithAssembliesMatching("./TestDI.Tests/**/*.dll")
            .WithSourcesMatching("./TestDI/**/*.cs")
            .WithoutSourcesMatching("./TestDI/**/*.g.cs")
            .WithoutSourcesMatching("./TestDI/**/*.xaml.cs")
            .WithNonFatalThreshold()
            .GenerateReport(ReportType.CONSOLE)
    );
});

Task("Coveralls")
    .IsDependentOn("Test")
    .Does(() => 
{
    if (!TravisCI.IsRunningOnTravisCI)
    {
        Warning("Not running on travis, cannot publish coverage");
        return;
    }

    MiniCoverReport(new MiniCoverSettings()
        .WithCoverallsSettings(c => c.UseTravisDefaults())
        .GenerateReport(ReportType.COVERALLS)
    );
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);