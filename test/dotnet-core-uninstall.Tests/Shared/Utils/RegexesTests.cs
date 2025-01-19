using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Utils
{
    [TestClass]
    public class RegexesTests
    {
        private static readonly string TestCachePathPrefex = @"C:\ProgramData\Package Cache\{01234567-89b-cdef-0123-456789abcdef}";

        [TestMethod]
        [DataRow("1.0")]
        [DataRow("1.1")]
        [DataRow("2.0")]
        [DataRow("2.1")]
        [DataRow("2.2")]
        [DataRow("3.0")]
        [DataRow("12.345")]
        [DataRow("0012.00345")]
        public void TestBundleMajorMinorRegexAccept(string input)
        {
            TestRegexAccept(Regexes.BundleMajorMinorRegex, input);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("2")]
        [DataRow("2.")]
        [DataRow("2.2.")]
        [DataRow("2.2.2")]
        [DataRow("2.2.202")]
        [DataRow("a.0")]
        [DataRow("0.a")]
        [DataRow("2.2-preview")]
        [DataRow("2.2-preview-011768")]
        [DataRow("2.2-preview-011768-15")]
        [DataRow("3.0.0-preview5-27626-15")]
        [DataRow("3.0.100-preview5-011568")]
        [DataRow("Hello2.2World")]
        [DataRow("Hello 2.2 World")]
        [DataRow("2. 2")]
        [DataRow("2 .2")]
        [DataRow("2 . 2")]
        public void TestBundleMajorMinorRegexReject(string input)
        {
            TestRegexReject(Regexes.BundleMajorMinorRegex, input);
        }

        [TestMethod]
        [DataRow(@"\dotnet-sdk-2.1.4-win-x64.exe")]
        [DataRow(@"\dotnet-sdk-2.1.700-win-x64.exe")]
        [DataRow(@"\dotnet-sdk-2.1.300-rc1-008673-win-x86.exe")]
        [DataRow(@"\dotnet-sdk-3.0.100-preview5-0011568-win-x86.exe")]
        [DataRow(@"\dotnet-runtime-2.1.0-rc1-win-x64.exe")]
        [DataRow(@"\dotnet-runtime-2.2.5-win-x64.exe")]
        [DataRow(@"\dotnet-runtime-3.0.0-preview5-27626-15-win-x86.exe")]
        [DataRow(@"\dotnet-dev-win-x64.1.1.14.exe")]
        [DataRow(@"\dotnet-win-x64.1.0.11.exe")]
        [DataRow(@"\AspNetCore.2.0.9.RuntimePackageStore_x64.exe")]
        [DataRow(@"\aspnetcore-runtime-2.2.6-win-x86.exe")]
        [DataRow(@"\aspnetcore-runtime-2.1.0-preview1-final-win-x64.exe")]
        [DataRow(@"\aspnetcore-runtime-2.1.0-rc1-final-win-x64.exe")]
        [DataRow(@"\aspnetcore-runtime-2.2.0-preview3-35497-win-x64.exe")]
        [DataRow(@"\aspnetcore-runtime-3.0.0-preview-18579-0056-win-x64.exe")]
        [DataRow(@"\aspnetcore-runtime-3.0.0-preview6.19307.2-win-x64.exe")]
        [DataRow(@"\DotNetCore.1.0.9_1.1.6-WindowsHosting.exe")]
        [DataRow(@"\DotNetCore.1.0.10_1.1.7-WindowsHosting.exe")]
        [DataRow(@"\dotnetcore.1.0.14_1.1.11-windowshosting.exe")]
        [DataRow(@"\DotNetCore.2.0.3-WindowsHosting.exe")]
        [DataRow(@"\dotnet-hosting-2.1.0-preview1-final-win.exe")]
        [DataRow(@"\dotnet-hosting-2.1.0-rc1-final-win.exe")]
        [DataRow(@"\dotnet-hosting-2.1.12-win.exe")]
        [DataRow(@"\dotnet-hosting-2.2.0-preview2-35157-win.exe")]
        [DataRow(@"\dotnet-hosting-2.2.6-win.exe")]
        [DataRow(@"\dotnet-hosting-3.0.0-preview-18579-0056-win.exe")]
        [DataRow(@"\dotnet-hosting-3.0.0-preview5-19227-01-win.exe")]
        [DataRow(@"\dotnet-hosting-3.0.0-preview6.19307.2-win.exe")]
        public void TestBundleCachePathRegexAccept(string suffix)
        {
            TestRegexAccept(Regexes.BundleCachePathRegex, $"{TestCachePathPrefex}{suffix}");
        }

        [TestMethod]
        [DataRow(@"\dotnet-sdk-2.1.300-preview-win-x64.exe")]
        [DataRow(@"\dotnet-sdk-2.1.300-preview1-win-x86.exe")]
        [DataRow(@"\dotnet-sdk-2.0.300-preview1-008174-01-win-x64.exe")]
        [DataRow(@"\dotnet-sdk-2.1.300-rc-win-x64.exe")]
        [DataRow(@"\dotnet-sdk-2.1.300-rc1-win-x86.exe")]
        [DataRow(@"\dotnet-sdk-2.1.300-rc1-002111-01-win-x64.exe")]
        [DataRow(@"\dotnet-sdk-2.1.300-rc1-final-win-x64.exe")]
        [DataRow(@"\dotnet-runtime-2.0.0-preview-win-x86.exe")]
        [DataRow(@"\dotnet-runtime-2.0.0-preview1-win-x64.exe")]
        [DataRow(@"\dotnet-runtime-2.0.0-preview1-002111-win-x64.exe")]
        [DataRow(@"\dotnet-runtime-2.1.0-rc-win-x86.exe")]
        [DataRow(@"\dotnet-runtime-2.1.0-rc1-002111-win-x64.exe")]
        [DataRow(@"\dotnet-runtime-2.1.0-rc1-002111-01-win-x64.exe")]
        [DataRow(@"\dotnet-runtime-2.1.0-rc1-final-win-x86.exe")]
        [DataRow(@"\dotnet-dev-win-x64-1.1.14.exe")]
        [DataRow(@"\dotnet-win-x64-1.0.11.exe")]
        [DataRow(@"\dotnet-dev-win-x64.3.0.0-preview5-27626-15.exe")]
        [DataRow(@"\dotnet-win-x64.2.1.300-preview1-008174.exe")]
        [DataRow(@"\dotnet-sdk-2.1.700.exe")]
        [DataRow(@"\dotnet-sdk-2.1.700-x64.exe")]
        [DataRow(@"\dotnet-sdk-2.1.700-win.exe")]
        [DataRow(@"\dotnet-sdk-2.1.700-windows-x64.exe")]
        [DataRow(@"\dotnet-sdk-2.1.4-win-x66.exe")]
        [DataRow(@"\dotnet-sdk-2.1.4-win-x64.exeHelloWorld")]
        [DataRow(@"\dotnet-sdk-2.1.4-win-x64.exe Hello World")]
        [DataRow(@"\AspNetCore.2.0.9.RuntimePackageStore.exe")]
        [DataRow(@"\AspNetCore.2.0.9.RuntimePackageStore_x66.exe")]
        [DataRow(@"\AspNetCore.2.0.9.RuntimePackageStore.x64.exe")]
        [DataRow(@"\AspNetCore.2.0.9.RuntimePackageStore-x64.exe")]
        [DataRow(@"\aspnetcore-runtime-2.2.6-win.exe")]
        [DataRow(@"\aspnetcore-runtime-2.2.6-win-x66.exe")]
        [DataRow(@"\aspnetcore-runtime-2.1.0-preview1-win-x64.exe")]
        [DataRow(@"\aspnetcore-runtime-2.1.0-rc1-win-x64.exe")]
        [DataRow(@"\DotNetCore.1.0.9_1.1.6-windowshosting.exe")]
        [DataRow(@"\dotnetcore.1.0.9_1.1.6-WindowsHosting.exe")]
        [DataRow(@"\DotNetCore-1.0.9-1.1.6-WindowsHosting.exe")]
        [DataRow(@"\DotNetCore_1.0.9_1.1.6_WindowsHosting.exe")]
        [DataRow(@"\DotNetCore.1.0.9.1.1.6.WindowsHosting.exe")]
        [DataRow(@"\dotnetcore.1.0.14_1.1.11-WindowsHosting.exe")]
        [DataRow(@"\DotNetCore.1.0.14_1.1.11-windowshosting.exe")]
        [DataRow(@"\DotNetCore.2.0.3-windowshosting.exe")]
        [DataRow(@"\dotnetcore.2.0.3-WindowsHosting.exe")]
        [DataRow(@"\DotNetCore-2.0.3-WindowsHosting.exe")]
        [DataRow(@"\DotNetCore_2.0.3_WindowsHosting.exe")]
        [DataRow(@"\dotnet-hosting-2.1.0-preview1-win.exe")]
        [DataRow(@"\dotnet-hosting-2.1.0-preview1-final.exe")]
        [DataRow(@"\dotnet-hosting-2.1.0-preview1-final-win-x64.exe")]
        [DataRow(@"\DotNet-Hosting-2.1.0-Preview1-final-win.exe")]
        [DataRow(@"\dotnet-hosting-2.1.0-rc1-win.exe")]
        [DataRow(@"\dotnet-hosting-2.1.0-rc1-final.exe")]
        [DataRow(@"\dotnet-hosting-2.1.0-rc1-final-win-x64.exe")]
        [DataRow(@"\DotNet-Hosting-2.1.0-RC1-final-win.exe")]
        [DataRow(@"\dotnet-hosting-2.1.0-release-candidate-1-final-win.exe")]
        [DataRow(@"\dotnet-hosting-2.1.12-win-x64.exe")]
        [DataRow(@"\dotnet-hosting-2.2.0-preview2-win.exe")]
        [DataRow(@"\dotnet-hosting-3.0.0-preview6.19307-win.exe")]
        [DataRow(@"\dotnet-hosting-3.0.0-preview6-19307.2-win.exe")]
        public void TestBundleCachePathRegexReject(string suffix)
        {
            TestRegexReject(Regexes.BundleCachePathRegex, $"{TestCachePathPrefex}{suffix}");
        }

        private void TestRegexAccept(Regex regex, string input)
        {
            regex.IsMatch(input).Should().BeTrue();
        }

        private void TestRegexReject(Regex regex, string input)
        {
            regex.IsMatch(input).Should().BeFalse();
        }
    }
}
