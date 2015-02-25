properties {
    # build variables
    $framework = "4.5.1"		# .net framework version
    $configuration = "Release"	# build configuration
    $script:version = "1.0.0"
    $script:nugetVersion = "1.0.0"
    $script:runCoverity = $false

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

task RestoreNuGetPackages {
    exec { nuget.exe restore $sln_file }
}

task GitVersion {
    GitVersion /output buildserver /updateassemblyinfo true /assemblyVersionFormat Major
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

task appveyor-checkCoverity {
  if($env:APPVEYOR_SCHEDULED_BUILD -eq "True") {
    #download coverity
    # Invoke-WebRequest -Uri "https://scan.coverity.com/download/cxx/win_64" -Body @{ project = "$env:APPVEYOR_REPO_NAME"; token = "$env:COVERITY_TOKEN" } -OutFile "$env:APPVEYOR_BUILD_FOLDER\coverity.zip"
    Invoke-WebRequest -Uri "https://dl.dropboxusercontent.com/u/19134447/cov-analysis-win64-7.5.0-netonly.zip" -OutFile "$env:APPVEYOR_BUILD_FOLDER\coverity.zip"

    Expand-Archive .\coverity.zip

    $script:runCoverity = $true
    $script:covbuild = (Resolve-Path ".\cov-analysis-win64-*\bin\cov-build.exe").ToString()
  }
}

task setup-coverity-local {
  $script:runCoverity = $true
  $script:covbuild = "cov-build"
  $env:APPVEYOR_BUILD_FOLDER = "."
  $env:APPVEYOR_BUILD_VERSION = $script:version
  $env:APPVEYOR_REPO_NAME = "csmacnz/coveritypublisher"
  "You should have set the COVERITY_TOKEN environment variable already"
}

task test-coverity -depends setup-coverity-local, coverity

task coverity -precondition { return $script:runCoverity }{
  & $script:covbuild --dir cov-int msbuild "/t:Clean;Build" "/p:Configuration=$configuration" $sln_file
  $coverityFileName = "$applicationName.coverity.$script:nugetVersion.zip"
  Write-Zip -Path "cov-int" -OutputPath $coverityFileName
  
  #TODO an app for this:
  Add-Type -AssemblyName "System.Net.Http"
  $client = New-Object Net.Http.HttpClient
  $client.Timeout = [TimeSpan]::FromMinutes(20)
  $form = New-Object Net.Http.MultipartFormDataContent
  [Net.Http.HttpContent]$formField = New-Object Net.Http.StringContent($env:COVERITY_TOKEN)
  $form.Add($formField, "token")
  $formField = New-Object Net.Http.StringContent($env:COVERITY_EMAIL)
  $form.Add($formField, "email")
  $fs = New-Object IO.FileStream("$env:APPVEYOR_BUILD_FOLDER\$coverityFileName", [IO.FileMode]::Open, [IO.FileAccess]::Read)
  $formField = New-Object Net.Http.StreamContent($fs)
  $form.Add($formField, "file", "$coverityFileName")
  $formField = New-Object Net.Http.StringContent($script:nugetVersion)
  $form.Add($formField, "version")
  $formField = New-Object Net.Http.StringContent("AppVeyor scheduled build ($env:APPVEYOR_BUILD_VERSION).")
  $form.Add($formField, "description")
  $url = "https://scan.coverity.com/builds?project=$env:APPVEYOR_REPO_NAME"
  $task = $client.PostAsync($url, $form)
  try {
    $task.Wait()  # throws AggregateException on time-out
  } catch [AggregateException] {
    throw $_.Exception.InnerException
  }
  $task.Result
  $fs.Close()
}

task ResolveCoverallsPath {
    $script:coveralls = (Resolve-Path "src/packages/coveralls.net.*/csmacnz.coveralls.exe").ToString()
}

task coverage -depends build, coverage-only

task coverage-only {
    vstest.console.exe /inIsolation /Enablecodecoverage /Settings:CodeCoverage.runsettings /TestAdapterPath:".\src\packages\xunit.runner.visualstudio.2.0.0-rc3-build1046\build\_common\" .\src\csmacnz.CoverityPublisher.Unit.Tests\bin\Release\csmacnz.CoverityPublisher.Unit.Tests.dll .\src\csmacnz.CoverityPublisher.Integration.Tests\bin\Release\csmacnz.CoverityPublisher.Integration.Tests.dll
    $coverageFilePath = Resolve-Path -path "TestResults\*\*.coverage"
    
    $coverageFilePath = $coverageFilePath.ToString()
    
    if(Test-Path .\coverage.coveragexml){ rm .\coverage.coveragexml }
    
    ."C:\Program Files (x86)\Microsoft Visual Studio 12.0\Team Tools\Dynamic Code Coverage Tools\CodeCoverage.exe" analyze /output:coverage.coveragexml "$coverageFilePath"
}

task test-coveralls -depends coverage, ResolveCoverallsPath {
    exec { & $script:coveralls --vscodecoverage -i coverage.coveragexml --dryrun -o coverallsTestOutput.json --repoToken "NOTAREALTOKEN" }
}

task coveralls -depends ResolveCoverallsPath {
    exec { & $script:coveralls --vscodecoverage -i coverage.coveragexml --repoToken $env:COVERALLS_REPO_TOKEN --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_JOB_ID }
}

task archive -depends build, archive-only

task archive-only {
    $archive_filename = "$applicationName.$script:nugetVersion.zip"

    mkdir $archive_dir

    cp "$build_output_dir\PublishCoverity.exe" "$archive_dir"

    Write-Zip -Path "$archive_dir\*" -OutputPath $archive_filename
}

task pack -depends build, pack-only

task pack-only {

    mkdir $nuget_pack_dir
    cp "$nuspec_filename" "$nuget_pack_dir"

    mkdir "$nuget_pack_dir\lib"
    cp "$build_output_dir\PublishCoverity.exe" "$nuget_pack_dir\lib"

    $Spec = [xml](get-content "$nuget_pack_dir\$nuspec_filename")
    $Spec.package.metadata.version = ([string]$Spec.package.metadata.version).Replace("{Version}", $script:nugetVersion)
    $Spec.Save("$nuget_pack_dir\$nuspec_filename")

    exec { nuget pack "$nuget_pack_dir\$nuspec_filename" }
}

task postbuild -depends pack, archive, coverage-only, coveralls

task appveyor-install -depends GitVersion, RestoreNuGetPackages

task appveyor-build -depends RestoreNuGetPackages, build

task appveyor-test -depends AppVeyorEnvironmentSettings, postbuild, appveyor-checkCoverity, coverity
