using System;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo.Versioning
{
    [TestClass]
    public class HostingBundleVersionTests
    {
        [TestMethod]
        [DataRow("2.2.5", "test footnote", 2, 2, 5, false, true, "2.2.5 (*)")]
        [DataRow("0.2.5", null, 0, 2, 5, false, false, "0.2.5")]
        [DataRow("2.1.0-rc1-final", null, 2, 1, 0, true, false, "2.1.0-rc1-final")]
        [DataRow("2.1.0-preview2-final", null, 2, 1, 0, true, false, "2.1.0-preview2-final")]
        [DataRow("3.0.0-preview-18579-0056", "test footnote", 3, 0, 0, true, true, "3.0.0-preview-18579-0056 (*)")]
        [DataRow("3.0.0-preview6.19307.2", "test footnote", 3, 0, 0, true, true, "3.0.0-preview6.19307.2 (*)")]
        public void TestConstructor(string input, string footnote, int major, int minor, int patch, bool isPrerelease, bool hasFootnote, string toStringWithAsterisk)
        {
            TestProperties(new HostingBundleVersion(input, footnote), footnote, major, minor, patch, isPrerelease, hasFootnote, input, toStringWithAsterisk);
            TestProperties(BundleVersion.FromInput<HostingBundleVersion>(input, footnote), footnote, major, minor, patch, isPrerelease, hasFootnote, input, toStringWithAsterisk);
        }

        private static void TestProperties(HostingBundleVersion version, string footnote, int major, int minor, int patch, bool isPrerelease, bool hasFootnote, string toStringExpected, string toStringWithAsteriskExpected)
        {
            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.Patch.Should().Be(patch);
            version.IsPrerelease.Should().Be(isPrerelease);
            version.MajorMinor.Should().Be(new MajorMinorVersion(major, minor));
            version.Footnote.Should().Be(footnote);

            version.Type.Should().Be(BundleType.HostingBundle);
            version.BeforePatch.Should().Be(new MajorMinorVersion(major, minor));
            version.HasFootnote.Should().Be(hasFootnote);

            version.ToString().Should().Be(toStringExpected);
            version.ToStringWithAsterisk().Should().Be(toStringWithAsteriskExpected);
        }

        [TestMethod]
        [DataRow("2.2.5", "2.2.5")]
        [DataRow("2.1.0-preview2-final", "2.1.0-preview2-final")]
        public void TestEquality(string input1, string input2)
        {
            var version1 = new HostingBundleVersion(input1);
            var version2 = new HostingBundleVersion(input2);

            TestUtils.EqualityComparisonTestUtils<HostingBundleVersion>.TestEquality(version1, version2);
        }

        [TestMethod]
        [DataRow("1.2.5", "2.2.5")]
        [DataRow("2.1.5", "2.2.5")]
        [DataRow("2.2.4", "2.2.5")]
        [DataRow("3.0.0-preview-99999-01", "3.0.0-preview5-27626-15")]
        [DataRow("3.0.0-preview5-27122-01", "3.0.0-preview5-27626-15")]
        [DataRow("3.0.0-preview5-27626-15", "3.0.0-rc1-final")]
        [DataRow("3.0.0-preview5-27626-15", "3.0.0")]
        [DataRow("3.0.0-preview-18579-0056", "3.0.0-preview6.19307.2")]
        [DataRow("3.0.0-preview5-19227-01", "3.0.0-preview6.19307.2")]
        [DataRow("3.0.0-rc1-final", "3.0.0")]
        public void TestInequality(string lower, string higher)
        {
            var lowerVersion = new HostingBundleVersion(lower);
            var higherVersion = new HostingBundleVersion(higher);

            TestUtils.EqualityComparisonTestUtils<HostingBundleVersion>.TestInequality(lowerVersion, higherVersion);
        }

        [TestMethod]
        [DataRow("2.2.5")]
        [DataRow("3.0.0-preview5-27626-15")]
        public void TestInequalityNull(string input)
        {
            var version = new HostingBundleVersion(input);

            TestUtils.EqualityComparisonTestUtils<HostingBundleVersion>.TestInequalityNull(version);
        }

        [TestMethod]
        [DataRow("1.0.0")]
        [DataRow("1.0.16")]
        [DataRow("2.0.0-preview1-002111-00")]
        [DataRow("2.1.0-rc1")]
        [DataRow("2.2.5")]
        [DataRow("3.0.0-preview-27122-01")]
        [DataRow("3.0.0-preview5-27626-15")]
        [DataRow("2.0.0-preview")]
        [DataRow("2.0.0-preview1")]
        [DataRow("2.0.0-preview1-002111")]
        [DataRow("2.1.0-rc")]
        [DataRow("2.1.0-rc1-002111")]
        [DataRow("2.1.0-rc1-002111-01")]
        [DataRow("2.1.0-rc1-final")]
        [DataRow("2.0.0-preview1-abcdef-01")]
        [DataRow("2.0.0-preview1-002111-ab")]
        [DataRow("3.0.0-preview.27122.1")]
        [DataRow("3.0.0-preview5.27626.15")]
        public void TestFromInputAccept(string input)
        {
            Action action = () => new HostingBundleVersion(input);

            action.Should().NotThrow();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("-2.2.5")]
        [DataRow("-1.2.5")]
        [DataRow("3.-1.0")]
        [DataRow("2.2.-5")]
        [DataRow("3.0.-1-preview5-27626-15")]
        [DataRow("1.0")]
        [DataRow("1.0.")]
        [DataRow("12.345")]
        [DataRow("0012.00345")]
        [DataRow("2.2.5.002111")]
        [DataRow("a.0.0")]
        [DataRow("0.a.0")]
        [DataRow("0.0.a")]
        [DataRow("Hello2.2.5World")]
        [DataRow("Hello 2.2.5 World")]
        [DataRow("1.1.13(*)")]
        [DataRow("3.0.0-preview6.19307.2(*)")]
        public void TestFromInputReject(string input)
        {
            Action action = () => new HostingBundleVersion(input);

            action.Should().Throw<InvalidInputVersionException>(string.Format(LocalizableStrings.InvalidInputVersionExceptionMessageFormat, input));
        }
    }
}
