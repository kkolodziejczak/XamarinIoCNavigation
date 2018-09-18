#addin Cake.Coveralls
#tool "nuget:?package=OpenCover"

#tool coveralls.io

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

const string TEST_PROJECT = "./TestDI.Tests/TestDI.Tests.csproj";

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    DeleteDirectories(GetDirectories("./**/bin") , 
        new DeleteDirectorySettings 
        {
            Force = true,
            Recursive = true,
        });

    DeleteDirectories(GetDirectories("./**/obj") , 
        new DeleteDirectorySettings 
        {
            Force = true,
            Recursive = true,
        });
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore(TEST_PROJECT);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetCoreBuild(TEST_PROJECT,
            new DotNetCoreBuildSettings 
            {
                Configuration = configuration,
                ArgumentCustomization = arg => arg.AppendSwitch("/p:DebugType","=","Full")
            });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => 
    {
        OpenCover(tool => 
            {
                tool.DotNetCoreTest(TEST_PROJECT,
                    new DotNetCoreTestSettings(){
                        Configuration = configuration,
                        NoBuild = true,
                    });
            },
            new FilePath("./OpenCover.xml"),
            new OpenCoverSettings
            {
                SkipAutoProps = true,
                ArgumentCustomization = arg => arg.Append("-hideskipped:All"),
                MergeByHash = true,
                OldStyle = true, // This fixes issue with dotnet core
            }
            .WithFilter("+[TestDI]TestDI.Navigation.*"));
    });

Task("Coveralls")
    .IsDependentOn("Test")
    .Does(() => 
    {
        CoverallsIo("./OpenCover.xml", new CoverallsIoSettings()
        {
            RepoToken = EnvironmentVariable("Coveralls_API_Key") ?? null,
        });
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