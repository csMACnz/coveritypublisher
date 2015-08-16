namespace csmacnz.CoverityPublisher.Integration.Tests
{
    public static class BuildMetrics
    {
        public static string GetValidContents()
        {
            return BuildContents(failuresCount:0, successesCount:1);
        }
        public static string GetContentsWithFailures()
        {
            return BuildContents(failuresCount: 2, successesCount: 1);
        }

        private static string BuildContents(int failuresCount, int successesCount)
        {
            return string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE coverity SYSTEM ""config.dtd"">
<coverity>

<metrics>
    <metric>
        <name>time</name>
        <value>39</value>
    </metric>
    <metric>
        <name>args</name>
        <value>C:\tools\cov-analysis-win64-7.6.0\bin\cov-build.exe --dir cov-int</value>
    </metric>
    <metric>
        <name>host</name>
        <value>MyMachine</value>
    </metric>
    <metric>
        <name>short-platform</name>
        <value>win64</value>
    </metric>
    <metric>
        <name>platform</name>
        <value>Windows 8 (Unknown Edition number 101), 64-bit (build 9200)</value>
    </metric>
    <metric>
        <name>ident</name>
        <value>7.6.0 (build 9b77a50df0 p-harmony-push-21098.563)</value>
    </metric>
    <metric>
        <name>user</name>
        <value>MyUser</value>
    </metric>
    <metric>
        <name>cwd</name>
        <value>C:\Dev\Project</value>
    </metric>
    <metric>
        <name>config</name>
        <value>C:\tools\cov-analysis-win64-7.6.0\config\coverity_config.xml</value>
    </metric>
    <metric>
        <name>intermediatedir</name>
        <value>c:\Dev\Project\cov-int</value>
    </metric>
    <metric>
        <name>outputdir</name>
        <value>c:\Dev\Project\cov-int\output</value>
    </metric>
    <metric>
        <name>emitdir</name>
        <value>c:\Dev\Project\cov-int\emit</value>
    </metric>
    <metric>
        <name>force</name>
        <value>no</value>
    </metric>
    <metric>
        <name>cygwin</name>
        <value>no</value>
    </metric>
    <metric>
        <name>failures</name>
        <value>{0}</value>
    </metric>
    <metric>
        <name>successes</name>
        <value>{1}</value>
    </metric>
    <metric>
        <name>uptodate</name>
        <value>0</value>
    </metric>
    <metric>
        <name>buildcmd</name>
        <value>msbuild.exe /t:Clean;Build /p:Configuration=Release C:\Dev\Project\src\MySolution.sln</value>
    </metric>
</metrics>
</coverity>", failuresCount, successesCount);
        }

        public static string FileName { get { return "BUILD.metrics.xml"; } }
    }
}