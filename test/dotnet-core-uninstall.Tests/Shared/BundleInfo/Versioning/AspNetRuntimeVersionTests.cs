using System;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo.Versioning
{
    [TestClass]
    public class AspNetRuntimeVersionTests
    {
        [TestMethod]
        [DataRow("2.2.5", 2, 2, 5, false)]
        [DataRow("0.2.5", 0, 2, 5, false)]
        [DataRow("2.1.0-rc1-final", 2, 1, 0, true)]
        [DataRow("2.1.0-preview2-final", 2, 1, 0, true)]
        [DataRow("3.0.0-preview-18579-0056", 3, 0, 0, true)]
        [DataRow("3.0.0-preview6.19307.2", 3, 0, 0, true)]
        public void TestConstructor(string input, int major, int minor, int patch, bool isPrerelease)
        {
            TestProperties(new AspNetRuntimeVersion(input), major, minor, patch, isPrerelease, input);
            TestProperties(BundleVersion.FromInput<AspNetRuntimeVersion>(input), major, minor, patch, isPrerelease, input);
        }

        private static void TestProperties(AspNetRuntimeVersion version, int major, int minor, int patch, bool isPrerelease, string toStringExpected)
        {
            version.Major.Should().Be(major);
            version.Minor.Should().Be(minor);
            version.Patch.Should().Be(patch);
            version.IsPrerelease.Should().Be(isPrerelease);
            version.MajorMinor.Should().Be(new MajorMinorVersion(major, minor));

            version.Type.Should().Be(BundleType.AspNetRuntime);
            version.BeforePatch.Should().Be(new MajorMinorVersion(major, minor));

            version.ToString().Should().Be(toStringExpected);
            version.ToStringWithAsterisk().Should().Be(toStringExpected);
        }

        [TestMethod]
        [DataRow("2.2.5", "2.2.5")]
        [DataRow("2.1.0-preview2-final", "2.1.0-preview2-final")]
        public void TestEquality(string input1, string input2)
        {
            var version1 = new AspNetRuntimeVersion(input1);
            var version2 = new AspNetRuntimeVersion(input2);

            TestUtils.EqualityComparisonTestUtils<AspNetRuntimeVersion>.TestEquality(version1, version2);
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
            var lowerVersion = new AspNetRuntimeVersion(lower);
            var higherVersion = new AspNetRuntimeVersion(higher);

            TestUtils.EqualityComparisonTestUtils<AspNetRuntimeVersion>.TestInequality(lowerVersion, higherVersion);
        }

        [TestMethod]
        [DataRow("2.2.5")]
        [DataRow("3.0.0-preview5-27626-15")]
        public void TestInequalityNull(string input)
        {
            var version = new AspNetRuntimeVersion(input);

            TestUtils.EqualityComparisonTestUtils<AspNetRuntimeVersion>.TestInequalityNull(version);
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
            Action action = () => new AspNetRuntimeVersion(input);

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
        public void TestFromInputReject(string input)
        {
            Action action = () => new AspNetRuntimeVersion(input);

            action.Should().Throw<InvalidInputVersionException>(string.Format(LocalizableStrings.InvalidInputVersionExceptionMessageFormat, input));
        }
    }
}
