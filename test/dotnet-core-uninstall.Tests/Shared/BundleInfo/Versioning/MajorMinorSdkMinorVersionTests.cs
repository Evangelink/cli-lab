using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;
using Microsoft.DotNet.Tools.Uninstall.Tests.TestUtils;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.BundleInfo.Versioning
{
    [TestClass]
    public class MajorMinorSdkMinorVersionTests
    {
        [TestMethod]
        [DataRow(1, 0, 0)]
        [DataRow(1, 1, 0)]
        [DataRow(2, 0, 1)]
        [DataRow(2, 1, 7)]
        [DataRow(2, 2, 3)]
        [DataRow(3, 0, 1)]
        [DataRow(12, 345, 7890)]
        public void TestConstructorInts(int major, int minor, int sdkMinor)
        {
            var majorMinor = new MajorMinorSdkMinorVersion(major, minor, sdkMinor);

            majorMinor.Major.Should().Be(major);
            majorMinor.Minor.Should().Be(minor);
            majorMinor.SdkMinor.Should().Be(sdkMinor);
        }

        [TestMethod]
        [DataRow(-1, 0, 0)]
        [DataRow(1, -1, 1)]
        [DataRow(2, 0, -1)]
        [DataRow(-12, -345, 7890)]
        [DataRow(-12, 345, -7890)]
        [DataRow(12, -345, -7890)]
        [DataRow(-12, -345, -7890)]
        public void TestConstructorIntsArgumentOutOfRangeException(int major, int minor, int sdkMinor)
        {
            Action action = () => new MajorMinorSdkMinorVersion(major, minor, sdkMinor);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        public static IEnumerable<object[]> GetDataForTestEquality()
        {
            yield return new object[]
            {
                new MajorMinorSdkMinorVersion(1, 0, 1),
                new MajorMinorSdkMinorVersion(1, 0, 1)
            };

            yield return new object[]
            {
                new MajorMinorSdkMinorVersion(12, 345, 6789),
                new MajorMinorSdkMinorVersion(12, 345, 6789)
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetDataForTestEquality))]
        internal void TestEquality(MajorMinorSdkMinorVersion majorMinor1, MajorMinorSdkMinorVersion majorMinor2)
        {
            EqualityComparisonTestUtils<MajorMinorSdkMinorVersion>.TestEquality(majorMinor1, majorMinor2);
        }

        public static IEnumerable<object[]> GetDataForTestInequality()
        {
            yield return new object[]
            {
                new MajorMinorSdkMinorVersion(1, 0, 1),
                new MajorMinorSdkMinorVersion(2, 0, 1)
            };

            yield return new object[]
            {
                new MajorMinorSdkMinorVersion(2, 1, 7),
                new MajorMinorSdkMinorVersion(2, 2, 3)
            };

            yield return new object[]
            {
                new MajorMinorSdkMinorVersion(12, 345, 678),
                new MajorMinorSdkMinorVersion(12, 345, 6789)
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetDataForTestInequality))]
        internal void TestInequality(MajorMinorSdkMinorVersion lower, MajorMinorSdkMinorVersion higher)
        {
            EqualityComparisonTestUtils<MajorMinorSdkMinorVersion>.TestInequality(lower, higher);
        }

        public static IEnumerable<object[]> GetDataForTestInequalityNull()
        {
            yield return new object[]
            {
                new MajorMinorSdkMinorVersion(1, 0, 1)
            };

            yield return new object[]
            {
                new MajorMinorSdkMinorVersion(12, 345, 6789)
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetDataForTestInequalityNull))]
        internal void TestInequalityNull(MajorMinorSdkMinorVersion majorMinor)
        {
            EqualityComparisonTestUtils<MajorMinorSdkMinorVersion>.TestInequalityNull(majorMinor);
        }
    }
}
