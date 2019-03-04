// load dependencies from source folder to allow bootstrapping
#r "/bin/Plainion.CI/FAKE/FakeLib.dll"
#load "/bin/Plainion.CI/bits/PlainionCI.fsx"

open Fake.Core
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open PlainionCI

Target.create "CreatePackage" (fun _ ->
    !! ( outputPath </> "*.*Specs.*" )
    ++ ( outputPath </> "*nunit*" )
    ++ ( outputPath </> "*Moq*" )
    ++ ( outputPath </> "TestResult.xml" )
    ++ ( outputPath </> "**/*.pdb" )
    |> File.deleteAll
        
    { Program = "dotnet.exe"
      Args = []
      WorkingDir = projectRoot
      CommandLine = @"publish src\Plainion.JekyllLint\Plainion.JekyllLint.fsproj" }
    |> Process.shellExec
    |> ignore

    PZip.PackRelease()
)

Target.create "Deploy" (fun _ ->
    let releaseDir = @"\bin\Plainion.JekyllLint"

    Shell.cleanDir releaseDir

    let zip = PZip.GetReleaseFile()
    Zip.unzip releaseDir zip
)

Target.create "Publish" (fun _ ->
    let zip = PZip.GetReleaseFile()
    PGitHub.Release [ zip ]
)

Target.runOrDefault ""
