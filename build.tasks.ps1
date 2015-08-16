properties {
    # build variables
    $framework = "4.5.1"		# .net framework version
    $configuration = "Release"	# build configuration
    $script:version = "0.0.1"
    $script:nugetVersion = "0.0.1"
    $script:runCoverity = $false
    $script:testOptions = ""

    # directories
    $base_dir = . resolve-path .\
    $build_output_dir = "$base_dir\src\csmacnz.CoverityPublisher\bin\$configuration\"
    $test_results_dir = "$base_dir\TestResults\"
    $package_dir = "$base_dir\Package\"
    $archive_dir = "$package_dir" + "Archive"
    $nuget_pack_dir = "$package_dir" + "Pack"

    # files
    $sln_file = "$base_dir\src\csmacnz.CoverityPublisher.sln"
    $nuspec_filename = "PublishCoverity.nuspec"

    $applicationName = "PublishCoverity"

}

task default

task SetChocolateyPath {
    $script:chocolateyDir = $null
    if ($env:ChocolateyInstall -ne $null) {
        $script:chocolateyDir = $env:ChocolateyInstall;
    } elseif (Test-Path (Join-Path $env:SYSTEMDRIVE Chocolatey)) {
        $script:chocolateyDir = Join-Path $env:SYSTEMDRIVE Chocolatey;
    } elseif (Test-Path (Join-Path ([Environment]::GetFolderPath("CommonApplicationData")) Chocolatey)) {
        $script:chocolateyDir = Join-Path ([Environment]::GetFolderPath("CommonApplicationData")) Chocolatey;
    }

    Write-Output "Chocolatey installed at $script:chocolateyDir";
}

task RestoreNuGetPackages -depends SetChocolateyPath {
    $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
    $NuGetExe = Join-Path $chocolateyBinDir -ChildPath "NuGet.exe";

    exec { & $NuGetExe restore $sln_file }
}

task GitVersion -depends SetChocolateyPath {
    $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
    $gitVersionExe = Join-Path $chocolateyBinDir -ChildPath "GitVersion.exe";

    & $gitVersionExe /output buildserver /updateassemblyinfo
}

task AppVeyorEnvironmentSettings {
    if(Test-Path Env:\GitVersion_ClassicVersion) {
        $script:version = $env:GitVersion_ClassicVersion
        echo "version set to $script:version"
    }
    elseif (Test-Path Env:\APPVEYOR_BUILD_VERSION) {
        $script:version = $env:APPVEYOR_BUILD_VERSION
        echo "version set to $script:version"
    }
    if(Test-Path Env:\GitVersion_NuGetVersionV2) {
        $script:nugetVersion = $env:GitVersion_NuGetVersionV2
        echo "nuget version set to $script:nugetVersion"
    }
    elseif (Test-Path Env:\APPVEYOR_BUILD_VERSION) {
        $script:nugetVersion = $env:APPVEYOR_BUILD_VERSION
        echo "nuget version set to $script:nugetVersion"
    }
    $script:testOptions = "/logger:Appveyor"
}

task clean {
    if (Test-Path $package_dir) {
      Remove-Item $package_dir -r
    }
    if (Test-Path $test_results_dir) {
      Remove-Item $test_results_dir -r
    }
    $archive_filename = "$applicationName.*.zip"
    if (Test-Path $archive_filename) {
      Remove-Item $archive_filename
    }
    $nupkg_filename = "$applicationName.*.nupkg"
    if (Test-Path $nupkg_filename) {
      Remove-Item $nupkg_filename
    }
    exec { msbuild "/t:Clean" "/p:Configuration=$configuration" $sln_file }
}

task build {
    exec { msbuild "/t:Clean;Build" "/p:Configuration=$configuration" $sln_file }
}

task setup-coverity-local {
  $env:APPVEYOR_BUILD_FOLDER = "."
  $env:APPVEYOR_BUILD_VERSION = $script:version
  $env:APPVEYOR_REPO_NAME = "csMACnz/coveritypublisher"
  "You should have set the COVERITY_TOKEN and COVERITY_EMAIL environment variables already"
  $env:APPVEYOR_SCHEDULED_BUILD = "True"
}

task test-coverity -depends setup-coverity-local, coverity

task coverity -precondition { return $env:APPVEYOR_SCHEDULED_BUILD -eq "True" } {

  & cov-build --dir cov-int msbuild "/t:Clean;Build" "/p:Configuration=$configuration" $sln_file

  $coverityFileName = "$applicationName.coverity.$script:nugetVersion.zip"

  $coverity = "$build_output_dir\PublishCoverity"

  & $coverity compress -o $coverityFileName

  & $coverity publish -t $env:COVERITY_TOKEN -e $env:COVERITY_EMAIL -z $coverityFileName -d "AppVeyor scheduled build ($env:APPVEYOR_BUILD_VERSION)." --codeVersion $script:nugetVersion
}

task ResolveCoverallsPath {
    $script:coveralls = (Resolve-Path "src/packages/coveralls.net.*/tools/csmacnz.coveralls.exe").ToString()
}

task coverage -depends build, coverage-only

task coverage-only {
    vstest.console.exe $script:testOptions /inIsolation /Enablecodecoverage /Settings:CodeCoverage.runsettings /TestAdapterPath:".\src\packages\xunit.runner.visualstudio.2.0.0-rc3-build1046\build\_common\" ".\src\csmacnz.CoverityPublisher.Unit.Tests\bin\$configuration\csmacnz.CoverityPublisher.Unit.Tests.dll" ".\src\csmacnz.CoverityPublisher.Integration.Tests\bin\$configuration\csmacnz.CoverityPublisher.Integration.Tests.dll"
    $coverageFilePath = Resolve-Path -path "TestResults\*\*.coverage"

    $coverageFilePath = $coverageFilePath.ToString()

    if(Test-Path .\coverage.coveragexml){ rm .\coverage.coveragexml }

    ."C:\Program Files (x86)\Microsoft Visual Studio 12.0\Team Tools\Dynamic Code Coverage Tools\CodeCoverage.exe" analyze /output:coverage.coveragexml "$coverageFilePath"
}

task test-coveralls -depends coverage, ResolveCoverallsPath {
    exec { & $script:coveralls --dynamiccodecoverage -i coverage.coveragexml --dryrun -o coverallsTestOutput.json --repoToken "NOTAREALTOKEN" --useRelativePaths }
}

task coveralls -depends ResolveCoverallsPath -precondition { return -not $env:APPVEYOR_PULL_REQUEST_NUMBER } {
    exec { & $script:coveralls --dynamiccodecoverage -i coverage.coveragexml --repoToken $env:COVERALLS_REPO_TOKEN --useRelativePaths --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_JOB_ID }
}

task archive -depends build, archive-only

task archive-only {
    $archive_filename = "$applicationName.$script:nugetVersion.zip"

    mkdir $archive_dir

    cp "$build_output_dir\*.*" "$archive_dir"

    Add-Type -assembly "system.io.compression.filesystem"

    [io.compression.zipfile]::CreateFromDirectory("$archive_dir", $archive_filename)
}

task pack -depends build, pack-only

task pack-only -depends SetChocolateyPath {

    $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";

    $NuGetExe = Join-Path $chocolateyBinDir -ChildPath "NuGet.exe";

    exec { & $NuGetExe pack .\src\csmacnz.CoverityPublisher\csmacnz.CoverityPublisher.nuspec -Version $script:nugetVersion -Properties Configuration=Release }
}

task postbuild -depends pack, archive, coverage-only, coveralls

task appveyor-install -depends GitVersion, RestoreNuGetPackages

task appveyor-build -depends build

task appveyor-test -depends AppVeyorEnvironmentSettings, postbuild, coverity
