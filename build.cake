#tool nuget:?package=NuGet.CommandLine&version=5.9.1
#tool nuget:?package=ReportGenerator&version=4.2.15
// #tool nuget:?package=coverlet.console&version=1.5.3
#tool nuget:?package=coverlet.console&version=3.1.0
// https://github.com/cake-build/cake/issues/2077
#tool nuget:?package=Microsoft.TestPlatform&version=16.2.0

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("Configuration", "Release");
var outputDirectory = Argument<DirectoryPath>("OutputDirectory", "output");
var codeCoverageDirectory = Argument<DirectoryPath>("CodeCoverageDirectory", "output/coverage");
var packageDirectory = Argument<DirectoryPath>("CodeCoverageDirectory", "output/packages");
var solutionFile = Argument("SolutionFile", "magentoapi.authentication.sln");
var versionSuffix = Argument("VersionSuffix", "");
var nugetDeployFeed = Argument("NugetDeployFeed", "https://api.nuget.org/v3/index.json");
var nugetDeployApiKey = Argument("NugetDeployApiKey", "");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

// Target : Clean
// 
// Description
// - Cleans binary directories.
// - Cleans output directory.
// - Cleans the test coverage directory.
Task("Clean")
    .Does(() =>
{
    CleanDirectory(packageDirectory);
    CleanDirectory(codeCoverageDirectory);
    CleanDirectory(outputDirectory);


    // remove all binaries in source files
    var srcBinDirectories = GetDirectories("./src/**/bin");
    foreach(var directory in srcBinDirectories)
    {
        CleanDirectory(directory);
    }

    // remove all intermediates in source files
    var srcObjDirectories = GetDirectories("./src/**/obj");
    foreach(var directory in srcObjDirectories)
    {
        CleanDirectory(directory);
    }

    // remove all binaries in test files
    var testsBinDirectories = GetDirectories("./tests/**/bin");
    foreach(var directory in testsBinDirectories)
    {
        CleanDirectory(directory);
    }
    
    // remove all intermediates in source files
    var testsObjDirectories = GetDirectories("./tests/**/obj");
    foreach(var directory in testsObjDirectories)
    {
        CleanDirectory(directory);
    }    
});

// Target : Restore-NuGet-Packages
// 
// Description
// - Restores all needed NuGet packages for the projects.
Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    // https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore
    //
    // Reload all nuget packages used by the solution
    NuGetRestore(solutionFile);
});

// Target : Build
// 
// Description
// - Builds the artifacts.
Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
    
      // Use MSBuild
      MSBuild(solutionFile, settings => {
        settings.ArgumentCustomization = 
            args => args
                .Append("/p:IncludeSymbols=true")
                .Append("/p:IncludeSource=true")
                .Append($"/p:VersionSuffix={versionSuffix}");
        settings.SetConfiguration(configuration);
      });
    
    } else {
    
      // Use XBuild
      XBuild(solutionFile, settings => {
        settings.ArgumentCustomization = 
            args => args
                .Append("/p:IncludeSymbols=true")
                .Append("/p:IncludeSource=true")
                .Append($"/p:VersionSuffix={versionSuffix}");
        settings.SetConfiguration(configuration);
      });

    }
});

// Target : Test
// 
// Description
// - Executes the test and generates with code coverage files.
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    CreateDirectory(codeCoverageDirectory);

    var includeFilter = "[Compori.MagentoApi.Authentication]*"; 
    var excludeFilter = "[xunit.*]*"; 

    var targetFrameworks = new string[] {"net48", "net5.0"};
    var projectFiles = GetFiles("./tests/**/*Tests.csproj");
    foreach(var projectFile in projectFiles)
    {
        foreach(var targetFramework in targetFrameworks)
        {
            var coverageFile = projectFile.GetFilenameWithoutExtension() + "." + targetFramework + ".cobertura.xml";
            var coveragePath = MakeAbsolute(codeCoverageDirectory).CombineWithFilePath(coverageFile);
            var logFileName = projectFile.GetFilenameWithoutExtension() + "." + targetFramework + ".trx";
            var logFilePath = MakeAbsolute(codeCoverageDirectory).CombineWithFilePath(logFileName);

            // coverlet test via dotnet test
            var settings = new DotNetCoreTestSettings
            {
                Configuration = configuration,
                Framework = targetFramework,
                ArgumentCustomization = args => args
                    .Append("/p:CollectCoverage=true")
                    .Append("/p:CoverletOutputFormat=cobertura")
                    .Append($"/p:Include={includeFilter}")                    
                    .Append($"/p:Exclude={excludeFilter}")                    
                    .Append($"/p:CoverletOutput=\"{coveragePath.FullPath}\"")
                    .Append($"--logger trx;LogFileName=\"{logFilePath}\"")
            };
            DotNetCoreTest(projectFile.FullPath, settings);
        }
    }           

    ReportGenerator( 
        new GlobPattern(MakeAbsolute(codeCoverageDirectory).FullPath + "/*.cobertura.*.xml"), 
        MakeAbsolute(codeCoverageDirectory).FullPath + "/report",
        new ReportGeneratorSettings(){
            ReportTypes = new[] { 
                ReportGeneratorReportType.HtmlInline,
                ReportGeneratorReportType.Badges 
            }
        }
    );    
});

// Target : Deploy
// 
// Description
// - Deploys package to nuget repository.
Task("Deploy")
    .IsDependentOn("Build")
    .Does(() =>
{
    CreateDirectory(packageDirectory);

    var packageFiles = GetFiles("src/**/*.nupkg");
    foreach(var packageFile in packageFiles)
    {
        var packageFilename = packageFile.GetFilename();
        var destionation = MakeAbsolute(packageDirectory).CombineWithFilePath(packageFilename);
        CopyFile(packageFile.FullPath, destionation);
    }

    // DeleteFiles(MakeAbsolute(outputDirectory).FullPath + "/*.symbols.nupkg");
    packageFiles = GetFiles(MakeAbsolute(outputDirectory).FullPath + "/*.nupkg");

    if(string.IsNullOrWhiteSpace(nugetDeployApiKey)) 
    {
        Error("No nuget api key provided.");
        return;
    }

    // Push the package.
    NuGetPush(packageFiles, new NuGetPushSettings {
        Source = nugetDeployFeed,
        ApiKey = nugetDeployApiKey,
        SkipDuplicate = true
    });    
});

// Target : Build
// 
// Description
// - Setup the default task.
Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
