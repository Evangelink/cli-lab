using System.Collections.Generic;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Commands;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo.Versioning;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Commands
{
    [TestClass]
    public class UninstallCommandExecTests
    {
        internal enum Results
        {
            Success,
            Reject, 
            Error
        }

        [TestMethod]
        [DataRow("Y", Results.Success)]
        [DataRow("YES", Results.Success)]
        [DataRow("yes", Results.Success)]
        [DataRow("n", Results.Reject)]
        [DataRow("", Results.Error)]
        [DataRow("foo", Results.Error)]
        internal void UserInputIsInterpretedCorrectly(string userResponse, Results expectedResult)
        {
            var bundles = new Dictionary<Bundle, string>() { { new Bundle<SdkVersion>(new SdkVersion(), new BundleArch(), string.Empty, string.Empty), "Required" } };
            try
            {
                var res = UninstallCommandExec.AskWithWarningsForRequiredBundles(bundles, userResponse);
                res.Should().Be(expectedResult.Equals(Results.Success));
            }
            catch
            {
                expectedResult.Should().Be(Results.Error);
            }

            try
            {
                var res = UninstallCommandExec.AskItAndReturnUserAnswer(bundles, userResponse);
                res.Should().Be(expectedResult.Equals(Results.Success));
            }
            catch
            {
                expectedResult.Should().Be(Results.Error);
            }
        }
    }
}
