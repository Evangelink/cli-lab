using System;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Tests.TestUtils;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo.Versioning
{
    [TestClass]
    public class MajorMinorVersionTests
    {
        [TestMethod]
        [DataRow("1.0", 1, 0)]
        [DataRow("1.1", 1, 1)]
        [DataRow("2.0", 2, 0)]
        [DataRow("2.1", 2, 1)]
        [DataRow("2.2", 2, 2)]
        [DataRow("3.0", 3, 0)]
        [DataRow("12.345", 12, 345)]
        [DataRow("0012.00345", 12, 345)]
        public void TestFromInput(string input, int major, int minor)
        {
            var majorMinor = MajorMinorVersion.FromInput(input);
            TestProperties(majorMinor, major, minor);

            MajorMinorVersion.TryFromInput(input, out majorMinor)
                .Should().BeTrue();
            TestProperties(majorMinor, major, minor);
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
        public void TestFromInputReject(string input)
        {
            Action action = () => MajorMinorVersion.FromInput(input);
            action.Should().Throw<InvalidInputVersionException>(string.Format(LocalizableStrings.InvalidInputVersionExceptionMessageFormat, input));

            MajorMinorVersion.TryFromInput(input, out var majorMinor)
                .Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, 0)]
        [DataRow(1, 1)]
        [DataRow(2, 0)]
        [DataRow(2, 1)]
        [DataRow(2, 2)]
        [DataRow(3, 0)]
        [DataRow(12, 345)]
        public void TestConstructor(int major, int minor)
        {
            var majorMinor = new MajorMinorVersion(major, minor);

            TestProperties(majorMinor, major, minor);
        }

        [TestMethod]
        [DataRow(-1, 0)]
        [DataRow(1, -1)]
        [DataRow(-12, -345)]
        public void TestConstructorIntsArgumentOutOfRangeException(int major, int minor)
        {
            Action action = () => new MajorMinorVersion(major, minor);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        private void TestProperties(BeforePatch majorMinor, int major, int minor)
        {
            majorMinor.Major.Should().Be(major);
            majorMinor.Minor.Should().Be(minor);
        }

        [TestMethod]
        [DataRow("1.0", "1.0")]
        [DataRow("1.1", "1.1")]
        [DataRow("2.0", "2.0")]
        [DataRow("2.1", "2.1")]
        [DataRow("2.2", "2.2")]
        [DataRow("3.0", "3.0")]
        [DataRow("12.345", "0012.00345")]
        public void TestEquality(string input1, string input2)
        {
            var majorMinor1 = MajorMinorVersion.FromInput(input1);
            var majorMinor2 = MajorMinorVersion.FromInput(input2);

            EqualityComparisonTestUtils<MajorMinorVersion>.TestEquality(majorMinor1, majorMinor2);
        }

        [TestMethod]
        [DataRow("1.0", "2.0")]
        [DataRow("2.1", "2.2")]
        [DataRow("1.2", "2.1")]
        [DataRow("6.66", "23.33")]
        public void TestInequality(string lower, string higher)
        {
            var lowerMajorMinor = MajorMinorVersion.FromInput(lower);
            var higherMajorMinor = MajorMinorVersion.FromInput(higher);

            EqualityComparisonTestUtils<MajorMinorVersion>.TestInequality(lowerMajorMinor, higherMajorMinor);
        }

        [TestMethod]
        [DataRow("1.1")]
        [DataRow("12.345")]
        public void TestInequalityNull(string input)
        {
            var majorMinor = MajorMinorVersion.FromInput(input);

            EqualityComparisonTestUtils<MajorMinorVersion>.TestInequalityNull(majorMinor);
        }
    }
}
