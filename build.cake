#addin Cake.Coveralls
#addin nuget:?package=Cake.Git
#tool "nuget:?package=OpenCover"
#tool "nuget:?package=GitVersion.CommandLine&prerelease"
#tool "nuget:?package=ReportGenerator"
#tool "nuget:?package=ReportUnit"
#tool coveralls.io

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

readonly string ProjectName = "Xamarin.BetterNavigation";
readonly DirectoryPath OutputDirectoryPath = "./Artifacts/";
readonly string CoverResultFileName = "OpenCover.xml";
readonly string ArtifactFileName = "Artifacts.zip";
readonly GitVersion currentVersion = GitVersion();
readonly string target = Argument("target", "Default");
readonly string buildConfiguration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// FLAGS
//////////////////////////////////////////////////////////////////////

readonly bool IsOnMaster = GitBranchCurrent(".").FriendlyName == "master";
readonly bool IsOnDevelop = GitBranchCurrent(".").FriendlyName == "develop";
readonly bool IsOnRelease = GitBranchCurrent(".").FriendlyName.StartsWith("release/");
readonly bool IsLocalBuild = BuildSystem.IsLocalBuild;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .ContinueOnError()
    .Does(() =>
{
    var directoriesToDelete = GetDirectories("./**/bin")
        .Concat(GetDirectories("./**/obj"))
        .Append(OutputDirectoryPath);

    DeleteDirectories(directoriesToDelete,
        new DeleteDirectorySettings
        {
            Force = true,
            Recursive = true,
        });
    if (FileExists(CoverResultFileName))
        DeleteFile(CoverResultFileName);
    if (FileExists(ArtifactFileName))
        DeleteFile(ArtifactFileName);
});

Task("Restore")
    .Does(() =>
    {
        DotNetCoreRestore(".");
    });

Task("Build")
    .Does(() =>
    {
        ExecuteForEachNonTestProject(projectFilePath =>
        {
            DotNetCoreBuild(projectFilePath.FullPath, new DotNetCoreBuildSettings
            {
                Configuration = buildConfiguration,
            });
        }, ProjectFile.csproj);
        ExecuteForEachTestProject(testProjectFilePath =>
        {
            DotNetCoreBuild(testProjectFilePath.FullPath, new DotNetCoreBuildSettings
            {
                Configuration = buildConfiguration,
                ArgumentCustomization = arg => arg.AppendSwitch("/p:DebugType","=","Full"),
            });
        });
    });

Task("TestAndCover")
    .Does(() =>
    {
        EnsureDirectoryExists(OutputDirectoryPath);
        OpenCover(tool =>
            {
                ExecuteForEachTestProject(testProjectFilePath =>
                {
                    tool.DotNetCoreTest(testProjectFilePath.FullPath, new DotNetCoreTestSettings()
                        {
                            NoBuild = true,
                            Configuration = buildConfiguration,
                            Logger = "trx",
                            ResultsDirectory = OutputDirectoryPath,
                        });
                });
            },
            new FilePath(CoverResultFileName),
            new OpenCoverSettings
            {
                SkipAutoProps = true,
                ArgumentCustomization = arg => arg.Append("-hideskipped:All"),
                MergeByHash = true,
                OldStyle = true, // This fixes issue with dotnet core
            }
            .WithFilter("+[Xamarin.BetterNavigation.UnitTests]Xamarin.BetterNavigation.UnitTests.Navigation.*"));

            // Generate Human readable coverage raport
            ReportGenerator(CoverResultFileName, $"{OutputDirectoryPath}/CoverageReport");
            ReportUnit(OutputDirectoryPath, $"{OutputDirectoryPath}/TestReport", new ReportUnitSettings());
            DeleteFiles($"{OutputDirectoryPath}/*.trx");
    });

Task("UploadCover")
    .WithCriteria(IsOnMaster || IsOnDevelop || IsOnRelease)
    .WithCriteria(!IsLocalBuild)
    .Does(() =>
    {
        CoverallsIo(CoverResultFileName, new CoverallsIoSettings()
        {
            RepoToken = EnvironmentVariable("Coveralls_API_Key") ?? null,
        });
    });

Task("NuGetPack")
    .WithCriteria(IsOnMaster || IsOnDevelop || IsOnRelease)
    .Does(() =>
    {
        ExecuteForEachNonTestProject(projectFilePath => 
        {
            UpdateNugetSpecVersion(projectFilePath.FullPath);
            NuGetPack(projectFilePath.FullPath, new NuGetPackSettings()
            {
                OutputDirectory = OutputDirectoryPath,
                BasePath = ".",
            });

            // For local testing only
            if(IsLocalBuild)
            {
                var updatedNuspec = System.IO.File.ReadAllText(projectFilePath.FullPath)
                    .Replace(currentVersion.NuGetVersion, "@version");
                System.IO.File.WriteAllText(projectFilePath.FullPath, updatedNuspec);
            }

        }, ProjectFile.nuspec);
    });

    private void UpdateNugetSpecVersion(string nugetSpecFilePath)
    {
        var updatedNuspec = System.IO.File.ReadAllText(nugetSpecFilePath)
                    .Replace("@version", currentVersion.NuGetVersion);
            System.IO.File.WriteAllText(nugetSpecFilePath, updatedNuspec);
    }

    private void ExecuteForEachNonTestProject(Action<FilePath> actionToExecute, ProjectFile file)
    {
        var str = $"./**/{ProjectName}.*.{file.ToString()}";
        var files = GetFiles($"./**/{ProjectName}.*.{file.ToString()}");
        foreach(var projectfile in files)
        {
            if(projectfile.FullPath.Contains("Tests"))
                continue;

            actionToExecute?.Invoke(projectfile);
        }
    }

    private void ExecuteForEachTestProject(Action<FilePath> actionToExecute)
    {
        foreach(var projectfile in GetFiles($"./**/{ProjectName}.*Tests.csproj"))
        {
            actionToExecute?.Invoke(projectfile);
        }
    }

public enum ProjectFile
{
    csproj,
    nuspec,
}

Task("UploadNuGet")
    .WithCriteria(IsOnMaster || IsOnRelease)
    .WithCriteria(!IsLocalBuild)
    .Does(() =>
    {
        // NuGetPack("dir", nuGetPackSettings);
    });

Task("CollectArtifacts")
    .Does(() =>
    {
        Zip(OutputDirectoryPath, ArtifactFileName);
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("TestAndCover");

Task("Deploy")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("TestAndCover")
    .IsDependentOn("UploadCover")
    .IsDependentOn("NuGetPack")
    .IsDependentOn("UploadNuGet")
    .IsDependentOn("CollectArtifacts");

Task("Coveralls")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("TestAndCover")
    .IsDependentOn("UploadCover");


//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);