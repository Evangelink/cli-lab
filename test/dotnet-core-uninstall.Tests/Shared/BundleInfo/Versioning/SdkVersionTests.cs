using System;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo.Versioning
{
    [TestClass]
    public class SdkVersionTests
    {
        [TestMethod]
        [DataRow("2.2.300", 2, 2, 3, 0, false)]
        [DataRow("0.2.300", 0, 2, 3, 0, false)]
        [DataRow("2.0.300", 2, 0, 3, 0, false)]
        [DataRow("2.2.202", 2, 2, 2, 2, false)]
        [DataRow("3.0.100-preview5-011568", 3, 0, 1, 0, true)]
        [DataRow("2.0.0-rc", 2, 0, 0, 0, true)]
        [DataRow("2.0.2-rc1-abcdef", 2, 0, 0, 2, true)]
        public void TestConstructor(string input, int major, int minor, int sdkMinor, int patch, bool isPrerelease)
        {
            TestProperties(new SdkVersion(input), major, minor, sdkMinor, patch, isPrerelease, input);
            TestProperties(BundleVersion.FromInput<SdkVersion>(input), major, minor, sdkMinor, patch, isPrerelease, input);
        }

        private static void TestProperties(SdkVersion version, int major, int minor, int sdkMinor, int patch, bool isPrerelease, string toStringExpected)
        {
            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.SdkMinor.Should().Be(sdkMinor);
            version.Patch.Should().Be(patch);
            version.IsPrerelease.Should().Be(isPrerelease);
            version.MajorMinor.Should().Be(new MajorMinorVersion(major, minor));

            version.Type.Should().Be(BundleType.Sdk);
            version.BeforePatch.Should().Be(new MajorMinorSdkMinorVersion(major, minor, sdkMinor));

            version.ToString().Should().Be(toStringExpected);
            version.ToStringWithAsterisk().Should().Be(toStringExpected);
        }

        [TestMethod]
        [DataRow("2.2.300", "2.2.300")]
        [DataRow("3.0.100-preview5-011568", "3.0.100-preview5-011568")]
        public void TestEquality(string input1, string input2)
        {
            var version1 = new SdkVersion(input1);
            var version2 = new SdkVersion(input2);

            TestUtils.EqualityComparisonTestUtils<SdkVersion>.TestEquality(version1, version2);
        }

        [TestMethod]
        [DataRow("1.2.300", "2.2.300")]
        [DataRow("2.1.300", "2.2.300")]
        [DataRow("2.2.200", "2.2.300")]
        [DataRow("2.2.302", "2.2.342")]
        [DataRow("3.0.100-preview-009812", "3.0.100-preview5-011568")]
        [DataRow("3.0.100-preview5-011568", "3.0.100-rc1-008673")]
        [DataRow("3.0.100-preview5-011568", "3.0.100")]
        [DataRow("3.0.100-rc1-008673", "3.0.100")]
        public void TestInequality(string lower, string higher)
        {
            var lowerVersion = new SdkVersion(lower);
            var higherVersion = new SdkVersion(higher);

            TestUtils.EqualityComparisonTestUtils<SdkVersion>.TestInequality(lowerVersion, higherVersion);
        }

        [TestMethod]
        [DataRow("2.2.300")]
        [DataRow("3.0.100-preview5-001568")]
        public void TestInequalityNull(string input)
        {
            var version = new SdkVersion(input);

            TestUtils.EqualityComparisonTestUtils<SdkVersion>.TestInequalityNull(version);
        }

        [TestMethod]
        [DataRow("1.0.0-preview2-003121")]
        [DataRow("1.0.4")]
        [DataRow("1.1.14")]
        [DataRow("1.0.0-preview2.1-003177")]
        [DataRow("2.0.0-preview1-005977")]
        [DataRow("2.0.0")]
        [DataRow("2.1.100")]
        [DataRow("2.1.105")]
        [DataRow("2.1.202")]
        [DataRow("2.1.300-preview1-008174")]
        [DataRow("2.1.300-rc1-008673")]
        [DataRow("2.1.302")]
        [DataRow("2.2.300")]
        [DataRow("3.0.100-preview-009812")]
        [DataRow("3.0.100-preview6-012264")]
        [DataRow("2.0.0-preview")]
        [DataRow("2.0.0-preview1")]
        [DataRow("2.0.0-preview1-008174-01")]
        [DataRow("2.1.300-preview")]
        [DataRow("2.1.300-preview1")]
        [DataRow("2.0.300-preview1-008174-01")]
        [DataRow("2.1.300-rc")]
        [DataRow("2.1.300-rc1")]
        [DataRow("2.1.300-rc1-002111-01")]
        [DataRow("2.1.300-rc1-final")]
        [DataRow("2.0.100-preview1-abcdef")]
        public void TestFromInputAccept(string input)
        {
            Action action = () => new SdkVersion(input);

            action.Should().NotThrow();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("-2.2.300")]
        [DataRow("-1.2.300")]
        [DataRow("2.2.-300")]
        [DataRow("3.-1.100-preview5-011568")]
        [DataRow("3.1.-100-preview5-011568")]
        [DataRow("3.1.-1-1-preview5-011568")]
        [DataRow("1.0")]
        [DataRow("1.0.")]
        [DataRow("12.345")]
        [DataRow("0012.00345")]
        [DataRow("2.2.5.002111")]
        [DataRow("2.2.500.002111")]
        [DataRow("a.0.100")]
        [DataRow("0.a.302")]
        [DataRow("0.0.abc")]
        [DataRow("Hello2.2.300World")]
        [DataRow("Hello 2.2.300 World")]
        public void TestFromInputReject(string input)
        {
            Action action = () => new SdkVersion(input);

            action.Should().Throw<InvalidInputVersionException>(string.Format(LocalizableStrings.InvalidInputVersionExceptionMessageFormat, input));
        }
    }
}
