#addin Cake.Coveralls
#addin nuget:?package=Cake.Git
#tool "nuget:?package=OpenCover"
#tool "nuget:?package=GitVersion.CommandLine&prerelease"
#tool "nuget:?package=gitreleasemanager"
#tool "nuget:?package=ReportGenerator"
#tool "nuget:?package=ReportUnit"
#tool coveralls.io

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

readonly string ProjectName = "Xamarin.BetterNavigation";
readonly DirectoryPath OutputDirectoryPath = "./Artifacts";
readonly string CoverResultFileName = "OpenCover.xml";
readonly string ArtifactFileName = "Artifacts.zip";
readonly string CurrentBranchName = AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Repository.Branch : GitBranchCurrent(".").FriendlyName;
readonly GitVersion currentVersion = GitVersion();
readonly string target = Argument("target", "Default");
readonly string buildConfiguration = Argument("configuration", "Release");

readonly string NugetPattern = $"{OutputDirectoryPath}/*.nupkg";
IEnumerable<FilePath> NugetFilePaths => GetFiles(NugetPattern);

//////////////////////////////////////////////////////////////////////
// FLAGS
//////////////////////////////////////////////////////////////////////

readonly bool IsOnMaster = CurrentBranchName == "master";
readonly bool IsOnDevelop = CurrentBranchName == "develop";
readonly bool IsOnRelease = CurrentBranchName.StartsWith("release/");
readonly bool IsLocalBuild = BuildSystem.IsLocalBuild;
readonly bool IsPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Setup(context =>
{
    Information($"Debug information:");
    Information($"~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
    Information($"Is Local Build: {IsLocalBuild}");
    Information($"Is On Release: {IsOnRelease}");
    Information($"Is On Master: {IsOnMaster}");
    Information($"Is On Develop: {IsOnDevelop}");
    Information($"Current Branch Name: {CurrentBranchName}");
    Information($"Build Configuration: {buildConfiguration}");
    Information($"Target: {target}");
    Information($"Current Version: {currentVersion.InformationalVersion}");
    Information($"~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
});

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

Task("BuildTest")
    .Does(() =>
    {
        ExecuteForEachNonTestProject(projectFilePath =>
        {
            DotNetCoreBuild(projectFilePath.FullPath, new DotNetCoreBuildSettings
            {
                Configuration = buildConfiguration,
				ArgumentCustomization = arg => arg.AppendSwitch("/p:DebugType","=","Full"),
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

Task("BuildRelease")
    .Does(() =>
    {
        ExecuteForEachNonTestProject(projectFilePath =>
        {
            DotNetCoreBuild(projectFilePath.FullPath, new DotNetCoreBuildSettings
            {
                Configuration = buildConfiguration,
            });
        }, ProjectFile.csproj);
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
            .WithFilter("+[Xamarin.BetterNavigation.Core]*")
            .WithFilter("+[Xamarin.BetterNavigation.Forms]*"));

            // Generate Human readable coverage raport
            ReportGenerator(CoverResultFileName, $"{OutputDirectoryPath}/CoverageReport");
            ReportUnit(OutputDirectoryPath, $"{OutputDirectoryPath}/TestReport", new ReportUnitSettings());
            DeleteFiles($"{OutputDirectoryPath}/*.trx");
    });

Task("UploadCover")
    .WithCriteria(IsOnMaster || IsOnDevelop)
    .WithCriteria(!IsLocalBuild && !IsPullRequest)
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

Task("UploadNuGet")
    .WithCriteria(IsOnMaster)
    .WithCriteria(!IsLocalBuild && !IsPullRequest)
    .Does(() =>
    {
        NuGetPush(NugetFilePaths, new NuGetPushSettings()
        {
            ApiKey = EnvironmentVariable("Nuget_API_Key"),
            Source = "https://nuget.org/api/v2/package",
        });
    });

Task("CollectArtifacts")
    .Does(() =>
    {
        Zip(OutputDirectoryPath, ArtifactFileName);
    });

Task("GitRelease")
    .WithCriteria(IsOnMaster || IsOnRelease)
    .WithCriteria(!IsLocalBuild && !IsPullRequest)
    .Does(() =>
    {
        GitReleaseManagerCreate(EnvironmentVariable("Git_Bot_Login"),
                                EnvironmentVariable("Git_Bot_Password"),
                                "kkolodziejczak",
                                "https://github.com/kkolodziejczak/XamarinIoCNavigation",
                                new GitReleaseManagerCreateSettings()
                                {
                                    Assets = string.Join(",", NugetFilePaths.Append($"./{ArtifactFileName}")),
                                });
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("BuildTest")
    .IsDependentOn("TestAndCover");

Task("Deploy")
    .IsDependentOn("Test")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("BuildRelease")
    .IsDependentOn("NuGetPack")
    .IsDependentOn("UploadNuGet")
    .IsDependentOn("CollectArtifacts");

Task("Test")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("BuildTest")
    .IsDependentOn("TestAndCover")
    .IsDependentOn("UploadCover");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

//////////////////////////////////////////////////////////////////////
// UTILS
//////////////////////////////////////////////////////////////////////

    private void UpdateNugetSpecVersion(string nugetSpecFilePath)
    {
        var updatedNuspec = System.IO.File.ReadAllText(nugetSpecFilePath)
                    .Replace("@version", currentVersion.NuGetVersion);
            System.IO.File.WriteAllText(nugetSpecFilePath, updatedNuspec);
    }

    private void ExecuteForEachNonTestProject(Action<FilePath> actionToExecute, ProjectFile file)
    {
        foreach(var projectfile in GetFiles($"./**/{ProjectName}.*.{file}"))
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