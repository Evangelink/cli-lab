using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.VSVersioning;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.VSVersioning
{
    [TestClass]
    public class VSVersionTests
    {
        [TestMethod]
        [OSCondition(OperatingSystems.Windows)]
        [DataRow(new string[] { }, new bool[] { })]
        [DataRow(new string[] { "1.0.0" }, new bool[] { false })]
        [DataRow(new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false })]
        [DataRow(new string[] { "2.1.0", "1.0.1" }, new bool[] { false, false })]
        [DataRow(new string[] { "1.0.0", "1.0.1", "1.1.0" }, new bool[] { true, true, false })]
        [DataRow(new string[] { "1.0.0", "1.0.1", "2.0.0" }, new bool[] { true, false, false })]
        [DataRow(new string[] { "1.0.0", "1.0.1", "1.0.2" }, new bool[] { true, true, false })]
        [DataRow(new string[] { "2.1.500", "2.1.600" }, new bool[] { false, false })]
        [DataRow(new string[] { "2.1.500", "2.1.400", "2.1.600" }, new bool[] { false, true, false })]
        [DataRow(new string[] { "2.2.100", "2.2.200" }, new bool[] { false, false })]
        [DataRow(new string[] { "2.2.100", "2.2.200", "2.2.300" }, new bool[] { false, true, false })]
        [DataRow(new string[] { "3.0.100", "3.1.201", "5.0.100" }, new bool[] { true, false, false })]
        [DataRow(new string[] { "6.0.100", "6.0.101", "7.0.100" }, new bool[] { true, false, false })]
        [DataRow(new string[] { "7.0.100", "7.0.101", "8.0.100" }, new bool[] { true, false, false })]
        [DataRow(new string[] { "8.0.100", "8.0.101", "9.0.100" }, new bool[] { true, false, false })]
        [DataRow(new string[] { "10.0.100", "10.0.101", "11.100.100" }, new bool[] { false, false, false })]
        public void TestGetUninstallableWindows(string[] versions, bool[] allowed)
        {
            var bundles = new List<Bundle>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, string.Empty));
            }

            var uninstallable = VisualStudioSafeVersionsExtractor.GetUninstallableBundles(bundles);

            CheckAllowed(bundles, uninstallable, allowed, null);
        }

        [TestMethod]
        [OSCondition(OperatingSystems.MacOSX)]
        [DataRow(new string[] { }, new bool[] { }, new string[] { }, new bool[] { })]
        [DataRow(new string[] { "1.0.0" }, new bool[] { false }, new string[] { }, new bool[] { })]
        [DataRow(new string[] { }, new bool[] { }, new string[] { "1.0.0" }, new bool[] { false })]
        [DataRow(new string[] { "1.0.0" }, new bool[] { false }, new string[] { "1.0.0" }, new bool[] { false })]
        [DataRow(new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false }, new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false })]
        [DataRow(new string[] { "2.1.0", "1.0.1" }, new bool[] { false, true }, new string[] { "1.0.0", "1.1.0" }, new bool[] { false, false })]
        [DataRow(new string[] { "3.0.0", "7.0.100" }, new bool[] { true, false }, new string[] { "1.0.0", "1.1.0", "1.0.1", "1.0.2", "1.1.3" }, new bool[] { true, true, true, false, false })]
        [DataRow(new string[] { "3.0.0", "5.0.100" }, new bool[] { true, false }, new string[] { "1.0.0", "1.1.0", "1.0.1", "5.0.100" }, new bool[] { true, false, false, false })]
        [DataRow(new string[] { "5.0.100", "5.0.101", "11.100.100" }, new bool[] { true, false, false }, new string[] { "5.0.100", "11.0.0" }, new bool[] { false, false })]
        [DataRow(new string[] { "5.0.100", "6.0.100", "6.0.101" }, new bool[] { true, true, false }, new string[] { "5.0.100" }, new bool[] { false })]
        public void TestGetUninstallableMac(string[] sdkVersions, bool[] sdkAllowed, string[] runtimeVersions, bool[] runtimeAllowed)
        {
            var bundles = new List<Bundle>();
            foreach (string v in sdkVersions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, string.Empty));
            }
            foreach (string v in runtimeVersions)
            {
                bundles.Add(new Bundle<RuntimeVersion>(new RuntimeVersion(v), new BundleArch(), string.Empty, string.Empty));
            }

            var uninstallable = VisualStudioSafeVersionsExtractor.GetUninstallableBundles(bundles);

            CheckAllowed(bundles, uninstallable, sdkAllowed, runtimeAllowed);
        }

        [TestMethod]
        [OSCondition(OperatingSystems.Windows)]
        [DataRow(new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false })]
        [DataRow(new string[] { "1.0.0", "1.0.1", "1.1.0" }, new bool[] { true, true, false })]
        [DataRow(new string[] { "1.0.0", "1.0.1", "2.0.0" }, new bool[] { true, false, false })]
        [DataRow(new string[] { "1.0.0", "1.0.1", "1.0.2" }, new bool[] { true, true, false })]
        [DataRow(new string[] { "2.1.500", "2.1.400", "2.1.600" }, new bool[] { false, true, false })]
        [DataRow(new string[] { "2.2.100", "2.2.200", "2.2.300" }, new bool[] { false, true, false })]
        [DataRow(new string[] { "5.0.100", "5.0.101", "10.0.1" }, new bool[] { true, false, false })]
        [DataRow(new string[] { "6.0.100", "7.0.100", "7.0.101" }, new bool[] { false, true, false })]
        [DataRow(new string[] { "9.0.100", "9.0.101", "10.100.100" }, new bool[] { true, false, false })]
        [DataRow(new string[] { "10.0.100", "10.0.101", "11.100.100" }, new bool[] { false, false, false })]
        public void TestGetUninstallableNonSdkVersionsWindows(string[] versions, bool[] allowed)
        {
            var bundles = new List<Bundle>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, v));
            }
            TestGetUninstallableNonSdkVersions(bundles, allowed, null);
        }

        [TestMethod]
        [OSCondition(OperatingSystems.MacOSX)]
        [DataRow(new string[] { "1.0.0" }, new bool[] { false }, new string[] { "1.0.0" }, new bool[] { false })]
        [DataRow(new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false }, new string[] { "1.0.0", "1.0.1" }, new bool[] { true, false })]
        [DataRow(new string[] { "2.1.0", "1.0.1" }, new bool[] { false, true }, new string[] { "2.0.0", "1.1.0" }, new bool[] { false, false })]
        [DataRow(new string[] { "3.0.100", "5.0.100" }, new bool[] { true, false }, new string[] { "1.0.0", "1.1.0", "1.0.1", "1.0.2", "1.1.3" }, new bool[] { true, true, true, false, false })]
        [DataRow(new string[] { "3.0.100", "5.0.100" }, new bool[] { true, false }, new string[] { "1.0.0", "1.1.0", "1.0.1", "5.0.100" }, new bool[] { true, false, false, false })]
        [DataRow(new string[] { "6.0.100", "6.0.101", "10.100.100" }, new bool[] { true, false, false }, new string[] { "6.0.100", "10.0.0" }, new bool[] { false, false })]
        public void TestGetUninstallableNonSdkVersionsMac(string[] sdkVersions, bool[] sdkAllowed, string[] runtimeVersions, bool[] runtimeAllowed)
        {
            var bundles = new List<Bundle>();
            foreach (string v in sdkVersions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, v));
            }
            foreach (string v in runtimeVersions)
            {
                bundles.Add(new Bundle<RuntimeVersion>(new RuntimeVersion(v), new BundleArch(), string.Empty, v));
            }
            TestGetUninstallableNonSdkVersions(bundles, sdkAllowed, runtimeAllowed);
        }

        internal void TestGetUninstallableNonSdkVersions(IEnumerable<Bundle> bundles, bool[] sdkAllowed, bool[] runtimeAllowed)
        {
            bundles = bundles.Concat(new List<Bundle>
            {
                new Bundle<AspNetRuntimeVersion>(new AspNetRuntimeVersion("1.0.0"), new BundleArch(), string.Empty, "AspNetVersion"),
                new Bundle<AspNetRuntimeVersion>(new AspNetRuntimeVersion("11.0.0"), new BundleArch(), string.Empty, "AspNetVersion"),
                new Bundle<HostingBundleVersion>(new HostingBundleVersion("1.0.0"), new BundleArch(), string.Empty, "HostingBundleVersion"),
                new Bundle<HostingBundleVersion>(new HostingBundleVersion("11.0.0"), new BundleArch(), string.Empty, "HostingBundleVersion")
            });

            var uninstallable = VisualStudioSafeVersionsExtractor.GetUninstallableBundles(bundles);

            // Check that we still have all of the non-sdk bundles
            uninstallable.Where(b => b.Version is AspNetRuntimeVersion).Should().HaveCount(1);
            uninstallable.Where(b => b.Version is HostingBundleVersion).Should().HaveCount(1);

            CheckAllowed(bundles, uninstallable, sdkAllowed, runtimeAllowed);
        }

        private void CheckAllowed(IEnumerable<Bundle> allBundles, IEnumerable<Bundle> uninstallableBundles, bool[] sdkAllowed, bool[] runtimeAllowed)
        {
            using var scope = new AssertionScope();
            scope.AddReportable("allBundles", () => String.Join(Environment.NewLine, allBundles.Select(b => b.ToDebugString())));
            scope.AddReportable("uninstallableBundles", () => String.Join(Environment.NewLine, uninstallableBundles.Select(b => b.ToDebugString())));
            var sdkBundles = allBundles.Where(bundle => bundle.Version is SdkVersion).ToArray();
            var runtimeBundles = allBundles.Where(bundle => bundle.Version is RuntimeVersion).ToArray();
            var otherBundles = allBundles.Except(sdkBundles).Except(runtimeBundles);
            for (int i = 0; i < sdkBundles.Count(); i++)
            {
                if (sdkAllowed[i])
                {
                    uninstallableBundles.Should().Contain(sdkBundles[i]);
                }
                else
                {
                    uninstallableBundles.Should().NotContain(sdkBundles[i]);
                }
            }

            for (int i = 0; i < runtimeBundles.Count(); i++)
            {
                if (runtimeAllowed[i])
                {
                    uninstallableBundles.Should().Contain(runtimeBundles[i]);
                }
                else
                {
                    uninstallableBundles.Should().NotContain(runtimeBundles[i]);
                }
            }
            // Check others are uninstallable unless their version is above the upper limit
            foreach (Bundle bundle in otherBundles)
            {
                if (bundle.Version.SemVer > VisualStudioSafeVersionsExtractor.UpperLimit)
                {
                    uninstallableBundles.Should().NotContain(bundle);
                }
                else
                {
                    uninstallableBundles.Should().Contain(bundle);
                }
            }
        }

        [TestMethod]
        [OSCondition(OperatingSystems.Windows)]
        [DataRow(new string[] { }, new string[] { })]
        [DataRow(new string[] { "1.0.1", "1.0.0" }, new string[] { "", "None" })]
        [DataRow(new string[] { "2.3.0", "2.1.800", "2.1.300" }, new string[] { "None", " 2019", " 2017" })]
        [DataRow(new string[] { "2.1.500", "2.1.400", "2.1.600" }, new string[] { " 2017", "None", " 2019" })]
        [DataRow(new string[] { "2.1.500", "10.0.1", "10.0.0" }, new string[] { " 2017", "UpperLimit", "UpperLimit" })]
        public void TestGetListCommandUninstallableStringsWindows(string[] versions, string[] expectedStrings)
        {
            expectedStrings = expectedStrings.Select(s => s.Equals("UpperLimit") ? VisualStudioSafeVersionsExtractor.UpperLimit.ToNormalizedString() : s).ToArray();

            var bundles = new List<Bundle>();
            foreach (string v in versions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, v));
            }

            TestGetListCommandUninstallableStrings(bundles, ExpandExpectationShortHand(expectedStrings), new string[0]);
        }

        [TestMethod]
        [OSCondition(OperatingSystems.MacOSX)]
        [DataRow(new string[] { }, new string[] { }, new string[] { }, new string[] { })]
        [DataRow(new string[] { }, new string[] { }, new string[] { "1.0.0" }, new string[] { "Runtime" })]
        [DataRow(new string[] { "1.0.0" }, new string[] { "SDK" }, new string[] { }, new string[] { })]
        [DataRow(new string[] { "1.0.0" }, new string[] { "SDK" }, new string[] { "1.0.0" }, new string[] { "Runtime" })]
        [DataRow(new string[] { "1.0.0", "1.0.1" }, new string[] { "None", "SDK" }, new string[] { "1.0.0", "1.0.1" }, new string[] { "None", "Runtime" })]
        [DataRow(new string[] { "2.1.0", "1.0.1" }, new string[] { "SDK", "None" }, new string[] { "2.0.0", "1.1.0" }, new string[] { "Runtime", "Runtime" })]
        [DataRow(new string[] { "3.0.100", "5.0.100" }, new string[] { "None", "SDK" }, new string[] { "1.0.0", "1.1.0", "1.0.1", "1.0.2", "1.1.3" }, new string[] { "None", "None", "None", "Runtime", "Runtime" })]
        [DataRow(new string[] { "3.0.100", "5.0.100" }, new string[] { "None", "SDK" }, new string[] { "1.0.0", "1.1.0", "1.0.1", "10.0.100" }, new string[] { "None", "Runtime", "Runtime", "UpperLimit" })]
        [DataRow(new string[] { "5.0.100", "5.0.101", "11.100.100" }, new string[] { "None", "SDK", "UpperLimit" }, new string[] { "5.0.100", "11.0.0" }, new string[] { "Runtime", "UpperLimit" })]
        public void TestGetListCommandUninstallableStringsMac(string[] sdkVersions, string[] sdkExpected, string[] runtimeVersions, string[] runtimeExpected)
        {
            sdkExpected = sdkExpected.Select(s => s.Equals("UpperLimit") ? VisualStudioSafeVersionsExtractor.UpperLimit.ToNormalizedString() : s).ToArray();
            runtimeExpected = runtimeExpected.Select(s => s.Equals("UpperLimit") ? VisualStudioSafeVersionsExtractor.UpperLimit.ToNormalizedString() : s).ToArray();

            var bundles = new List<Bundle>();
            foreach (string v in sdkVersions)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion(v), new BundleArch(), string.Empty, v));
            }
            foreach (string v in runtimeVersions)
            {
                bundles.Add(new Bundle<RuntimeVersion>(new RuntimeVersion(v), new BundleArch(), string.Empty, v));
            }

            TestGetListCommandUninstallableStrings(bundles, ExpandExpectationShortHand(sdkExpected), ExpandExpectationShortHand(runtimeExpected));
        }

        internal void TestGetListCommandUninstallableStrings(IEnumerable<Bundle> bundles, string[] sdkExpected, string[] runtimeExpected)
        {
            bundles = bundles.Concat(new List<Bundle>
            {
                new Bundle<AspNetRuntimeVersion>(new AspNetRuntimeVersion("1.0.0"), new BundleArch(), string.Empty, "AspNetVersion"),
                new Bundle<AspNetRuntimeVersion>(new AspNetRuntimeVersion("11.0.0"), new BundleArch(), string.Empty, "AspNetVersion"),
                new Bundle<HostingBundleVersion>(new HostingBundleVersion("1.0.0"), new BundleArch(), string.Empty, "HostingBundleVersion"),
                new Bundle<HostingBundleVersion>(new HostingBundleVersion("11.0.0"), new BundleArch(), string.Empty, "HostingBundleVersion")
            });

            var strings = VisualStudioSafeVersionsExtractor.GetReasonRequiredStrings(bundles);
            strings.Count().Should().Be(bundles.Count());

            var sdkBundles = strings.Where(pair => pair.Key.Version is SdkVersion).ToArray();
            var sdkStrings = sdkBundles.Select(pair => pair.Value);
            var runtimeBundles = strings.Where(pair => pair.Key.Version is RuntimeVersion).ToArray();
            var runtimeStrings = runtimeBundles.Select(pair => pair.Value);
            var otherBundles = strings.Except(sdkBundles).Except(runtimeBundles);

            sdkStrings.Should().BeEquivalentTo(sdkExpected);
            runtimeStrings.Should().BeEquivalentTo(runtimeExpected);

            otherBundles.Should().HaveCount(4);
            // All bundles above the upper limit are required
            otherBundles.Where(pair => pair.Key.Version.SemVer >= VisualStudioSafeVersionsExtractor.UpperLimit)
                .ToList().ForEach(str => str.Value.Should().Be(string.Format(LocalizableStrings.UpperLimitRequirement, VisualStudioSafeVersionsExtractor.UpperLimit)));
            // Non-sdk bundles are always uninstallable below the upper limit
            otherBundles.Where(pair => pair.Key.Version.SemVer < VisualStudioSafeVersionsExtractor.UpperLimit)
                .ToList().ForEach(str => str.Value.Should().Be(string.Empty));
        }

        private string[] ExpandExpectationShortHand(string[] input)
        {
            var shortHandToFullExpectedString = new Dictionary<string, string>
            {
                { "None", string.Empty },
                { VisualStudioSafeVersionsExtractor.UpperLimit.ToString(), string.Format(LocalizableStrings.UpperLimitRequirement, VisualStudioSafeVersionsExtractor.UpperLimit) },
                { string.Empty, string.Format(LocalizableStrings.WindowsRequirementExplanationString, string.Empty)},
                { " 2017", string.Format(LocalizableStrings.WindowsRequirementExplanationString, " 2017")},
                { " 2019", string.Format(LocalizableStrings.WindowsRequirementExplanationString, " 2019")},
                { "SDK", LocalizableStrings.MacSDKRequirementExplanationString},
                { "Runtime", LocalizableStrings.MacRuntimeRequirementExplanationString}
            };
            var output = new string[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = shortHandToFullExpectedString[input[i]];
            }

            return output;
        }

        [TestMethod]
        [OSCondition(OperatingSystems.Windows)]
        public void TestUninstallableStringsCorrectManySDKs()
        {
            var bundles = new List<Bundle>
            {
                new Bundle<SdkVersion>(new SdkVersion("3.0.100-preview-0"), BundleArch.X64, string.Empty, "3.0.100"),
                new Bundle<RuntimeVersion>(new RuntimeVersion("2.0.0"), BundleArch.X64, string.Empty, "2.0.0"),
            };

            for (int i = 0; i < 5; i++)
            {
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion("2.0." + i), BundleArch.X64, string.Empty, "2.0." + i));
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion("2.0." + i + "-preview-0"), BundleArch.X64, string.Empty, "2.0." + i + "-preview-0"));
                bundles.Add(new Bundle<SdkVersion>(new SdkVersion("2.0." + i + "-preview-1"), BundleArch.X64, string.Empty, "2.0." + i + "-preview-1"));
            }

            var strings = VisualStudioSafeVersionsExtractor.GetReasonRequiredStrings(bundles);
            strings.Count.Should().Be(bundles.Count);

            var expectedProtected = new string[] { "3.0.100", "2.0.4" };
            AssertRequirementStringsCorrect(bundles, strings, expectedProtected);
        }

        [TestMethod]
        [OSCondition(OperatingSystems.Windows)]
        public void TestUninstallableStringsCorrectAcrossRequirementDivisions()
        {
            var bundles = new List<Bundle>
            {
                new Bundle<SdkVersion>(new SdkVersion("2.0.0"), BundleArch.X64, string.Empty, "2.0.0"),
                new Bundle<SdkVersion>(new SdkVersion("2.0.0-preview-0"), BundleArch.X64, string.Empty, "2.0.0-preview-0"),
                new Bundle<SdkVersion>(new SdkVersion("2.0.0-preview-1"), BundleArch.X64, string.Empty, "2.0.0-preview-1")
            };

            var strings = VisualStudioSafeVersionsExtractor.GetReasonRequiredStrings(bundles);
            var expectedProtected = new string[] { "2.0.0" };
            AssertRequirementStringsCorrect(bundles, strings, expectedProtected);
        }

        private void AssertRequirementStringsCorrect(List<Bundle> bundles, Dictionary<Bundle, string> bundleStringPairs, string[] expectedProtected)
        {
            bundleStringPairs.Count.Should().Be(bundles.Count);

            var expectedUninstallable = bundles.Select(bundle => bundle.DisplayName)
                .Except(expectedProtected);

            bundleStringPairs.Where(pair => pair.Key.Version is SdkVersion)
                .Where(pair => string.IsNullOrEmpty(pair.Value))
                .Select(pair => pair.Key.DisplayName)
                .Should().BeEquivalentTo(expectedUninstallable);

            bundleStringPairs.Where(pair => !string.IsNullOrEmpty(pair.Value))
                .Select(pair => pair.Key.DisplayName)
                .Should().BeEquivalentTo(expectedProtected);
        }
    }
}
