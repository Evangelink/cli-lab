using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Commands;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Commands
{
    [TestClass]
    public class CommandBundleFilterTests
    {
        private static readonly string[] versions = { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200", "5.0.100", "6.0.100", "7.0.100", "8.0.100", "9.0.100", "10.0.100", "11.11.11" };
        private Dictionary<string, BundleArch> versionsWithArch = new Dictionary<string, BundleArch>
        {
            { "3.0.0", BundleArch.X64 },
            { "3.0.0-preview", BundleArch.X64 },
            { "1.0.1", BundleArch.X64 },
            { "1.0.0", BundleArch.X64 },
            { "3.0.1", BundleArch.X86 },
            { "3.0.2", BundleArch.X86 },
            { "3.0.2-preview1", BundleArch.X86 },
            { "3.0.2-preview2", BundleArch.X86 },
            { "3.1.0", BundleArch.X86 },
            { "2.1.1", BundleArch.X86 },
        };

        [TestMethod]
        [OSCondition(OperatingSystems.Windows)]
        [DataRow("remove --all --sdk", new string[] { "1.0.0", "1.0.1" })]
        [DataRow("dry-run --all --sdk", new string[] { "1.0.0", "1.0.1" })]
        [DataRow("whatif --all --sdk", new string[] { "1.0.0", "1.0.1" })]
        [DataRow("remove --all-below 5.0.0 --sdk --force", new string[] { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200" })]
        [DataRow("remove --sdk 1.0.1", new string[] { "1.0.1" })]
        [DataRow("remove --sdk 1.0.0", new string[] { "1.0.0" })]
        [DataRow("remove --sdk 1.0.1 2.1.0 1.0.1", new string[] { "2.1.0", "1.0.1", "1.0.1" })]
        [DataRow("remove --sdk 1.0.0 1.0.1 1.1.0 2.1.0 2.1.500 2.1.600 2.2.100 2.2.200",
            new string[] { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200" })]
        public void TestRequiredUninstallableWhenExplicitlyAddedWindows(string command, string[] expectedUninstallable)
        {
            var bundles = new List<Bundle>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), "sdk", v));
                bundles.Add(new Bundle<RuntimeVersion>(new RuntimeVersion(v), new BundleArch(), "runtime", v));
            }
            TestRequiredUninstallableWhenExplicitlyAdded(bundles, command, expectedUninstallable, new string[0]);
        }

        [TestMethod]
        [OSCondition(OperatingSystems.Windows)]
        [DataRow("whatif --all --sdk --x64", new string[] { "3.0.0", "3.0.0-preview", "1.0.0" })]
        [DataRow("whatif --all --sdk --x86", new string[] { "3.0.1", "3.0.2", "3.0.2-preview1", "3.0.2-preview2" })]
        public void TestRequiredUninstallableWithOptionsWindows(string command, string[] expectedUninstallableSdk)
        {
            var bundles = new List<Bundle>();
            foreach (var pair in versionsWithArch)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(pair.Key), pair.Value, "sdk", pair.Key));
                bundles.Add(new Bundle<RuntimeVersion>(new RuntimeVersion(pair.Key), pair.Value, "runtime", pair.Key));
            }
            TestRequiredUninstallableWhenExplicitlyAdded(bundles, command, expectedUninstallableSdk, new string[0]);
        }

        [TestMethod]
        [OSCondition(OperatingSystems.MacOSX)]
        [DataRow("remove --all-below 5.0.0 --sdk", new string[] { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200" }, new string[] { })]
        [DataRow("remove --all-below 5.0.0 --sdk --force", new string[] { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200" }, new string[] { })]
        [DataRow("remove --all-below 5.0.0 --runtime", new string[] { }, new string[] { "1.0.0", "2.1.0", "2.1.500", "2.2.100" })]
        [DataRow("remove --all-below 5.0.0 --runtime --force", new string[] { }, new string[] { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200" })]
        [DataRow("remove --sdk 1.0.0 1.0.1 1.1.0 2.1.0 2.1.500 2.1.600 2.2.100 2.2.200", new string[] { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200" }, new string[] { })]
        [DataRow("remove --runtime 1.0.0 1.0.1 1.1.0 2.1.0 2.1.500 2.1.600 2.2.100 2.2.200", new string[] { }, new string[] { "1.0.0", "1.0.1", "1.1.0", "2.1.0", "2.1.500", "2.1.600", "2.2.100", "2.2.200" })]
        public void TestRequiredUninstallableWhenExplicitlyAddedMac(string command, string[] expectedUninstallableSdk, string[] expectedUninstallableRuntime)
        {
            var bundles = new List<Bundle>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), "sdk", v));
                bundles.Add(new Bundle<RuntimeVersion>(new RuntimeVersion(v), new BundleArch(), "runtime", v));
            }
            TestRequiredUninstallableWhenExplicitlyAdded(bundles, command, expectedUninstallableSdk, expectedUninstallableRuntime);
        }

        [TestMethod]
        [OSCondition(OperatingSystems.MacOSX)]
        [DataRow("remove --all-previews --sdk", new string[] { "3.0.0-preview", "3.0.2-preview1", "3.0.2-preview2" })]
        [DataRow("remove --all-lower-patches --sdk", new string[] { "1.0.0", "3.0.1", "3.0.0", "3.0.0-preview", "3.0.2-preview1", "3.0.2-preview2" })]
        public void TestRequiredUninstallableWithOptionsMac(string command, string[] expectedUninstallableSdk)
        {
            var bundles = new List<Bundle>();
            foreach (var pair in versionsWithArch)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(pair.Key), pair.Value, "sdk", pair.Key));
                bundles.Add(new Bundle<RuntimeVersion>(new RuntimeVersion(pair.Key), pair.Value, "runtime", pair.Key));
            }
            TestRequiredUninstallableWhenExplicitlyAdded(bundles, command, expectedUninstallableSdk, new string[0]);
        }

        internal void TestRequiredUninstallableWhenExplicitlyAdded(IEnumerable<Bundle> bundles, string command, string[] expectedUninstallableSdk, string[] expectedUninstallableRuntime)
        {
            using var scope = new AssertionScope();
            scope.AddReportable("bundles", () => String.Join(Environment.NewLine, bundles.Select(b => b.ToDebugString())));
            scope.AddReportable("command", () => command);
            scope.AddReportable("expectedUninstallableSdk", () => String.Join(Environment.NewLine, expectedUninstallableSdk));
            scope.AddReportable("expectedUninstallableRuntime", () => String.Join(Environment.NewLine, expectedUninstallableRuntime));
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);
            var uninstallableBundles = CommandBundleFilter.GetFilteredBundles(bundles, parseResult);
            scope.AddReportable("uninstallableBundles", () => String.Join(Environment.NewLine, uninstallableBundles.Select(b => b.ToDebugString())));

            var uninstallableSdks = uninstallableBundles.Where(b => b.Version is SdkVersion).Select(b => b.DisplayName);
            var requiredSdks = bundles.Except(uninstallableBundles).Where(b => b.Version is SdkVersion).Select(b => b.DisplayName);
            var uninstallableRuntimes = uninstallableBundles.Where(b => b.Version is RuntimeVersion).Select(b => b.DisplayName);
            var requiredRuntimes = bundles.Except(uninstallableBundles).Where(b => b.Version is RuntimeVersion).Select(b => b.DisplayName);

            bundles = bundles.Where(bundle => command.Contains(bundle.UninstallCommand)); // Only check bundles of type specified (SDK, Runtime, etc)
            (uninstallableSdks.Count() + requiredSdks.Count()).Should().Be(bundles.Count());
            uninstallableSdks.ToHashSet().Should().BeEquivalentTo(expectedUninstallableSdk.ToHashSet());
            requiredSdks.Should().BeEquivalentTo(bundles.Select(bundle => bundle.DisplayName).Where(v => !expectedUninstallableSdk.Contains(v)));

            (uninstallableRuntimes.Count() + requiredRuntimes.Count()).Should().Be(bundles.Count());
            uninstallableRuntimes.ToHashSet().Should().BeEquivalentTo(expectedUninstallableRuntime.ToHashSet());
            requiredRuntimes.Should().BeEquivalentTo(bundles.Select(bundle => bundle.DisplayName).Where(v => !expectedUninstallableRuntime.Contains(v)));

            requiredRuntimes.Should().NotBeEmpty();
        }

        [TestMethod]
        [DataRow("remove {0} 10.0.100")]
        [DataRow("remove {0} 11.11.11")]
        [DataRow("remove {0} --all --force")]
        [DataRow("remove {0} 1.0.0 1.0.1 1.1.0 2.1.0 2.1.500 2.1.600 2.2.100 2.2.200 5.0.100 7.0.100 11.11.11")]
        public void TestUpperLimitAlwaysRequired(string command)
        {
            var sdkBundles = new List<Bundle<SdkVersion>>();
            foreach (string v in versions)
            {
                sdkBundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, v));
            }
            CheckUpperLimitAlwaysRequired(string.Format(command, "--sdk"), sdkBundles);

            var runtimeBundles = new List<Bundle<RuntimeVersion>>();
            foreach (string v in versions)
            {
                runtimeBundles.Add(new Bundle<RuntimeVersion>(new RuntimeVersion(v), new BundleArch(), string.Empty, v));
            }
            CheckUpperLimitAlwaysRequired(string.Format(command, "--runtime"), runtimeBundles);

            if (RuntimeInfo.RunningOnWindows)
            {
                // Hosting bundles are only on windows
                var otherBundles = new List<Bundle>();
                foreach (string v in versions)
                {
                    otherBundles.Add(new Bundle<HostingBundleVersion>(new HostingBundleVersion(v), new BundleArch(), string.Empty, v));
                }
                CheckUpperLimitAlwaysRequired(string.Format(command, "--hosting-bundle"), otherBundles);
            }
        }

        internal void CheckUpperLimitAlwaysRequired(string command, IEnumerable<Bundle> bundles)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);
            Action filteringAction = () => CommandBundleFilter.GetFilteredBundles(bundles, parseResult);
            filteringAction.Should().Throw<UninstallationNotAllowedException>("Expected command '{0}' to fail when the following bundles were installed: {1}", command, String.Join(", ", bundles.Select(b => b.DisplayName).ToList()));
        }

        [TestMethod]
        public async Task TestHelpOutputContainsExplanationParagraph()
        {
            foreach (var command in new string[] { "dry-run -h", "whatif -h", "remove -h" })
            {
                var console = new TestConsole();
                _ = await CommandLineConfigs.UninstallCommandParser.InvokeAsync(command, console);

                console.Out.ToString().Should().Contain(RuntimeInfo.RunningOnWindows ? LocalizableStrings.HelpExplanationParagraphWindows :
                    LocalizableStrings.HelpExplanationParagraphMac);
            }
        }

    }
}
