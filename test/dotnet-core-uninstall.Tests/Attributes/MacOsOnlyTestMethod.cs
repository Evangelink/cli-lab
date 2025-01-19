using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Attributes
{
    internal sealed class MacOsOnlyTestMethod : TestMethodAttribute
    {
        public MacOsOnlyTestMethod()
        {
            if (!RuntimeInfo.RunningOnOSX)
            {
                IgnoreMessage = "Ignored on non-macOS platforms";
            }
        }
    }
}
