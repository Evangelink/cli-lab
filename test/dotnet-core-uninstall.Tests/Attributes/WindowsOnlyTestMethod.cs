using Microsoft.DotNet.Tools.Uninstall.Shared.Utils;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Attributes
{
    internal sealed class WindowsOnlyTestMethod : TestMethodAttribute
    {
        public WindowsOnlyTestMethod()
        {
            if (!RuntimeInfo.RunningOnWindows)
            {
                IgnoreMessage = "Ignored on non-Windows platforms";
            }
        }
    }
}
