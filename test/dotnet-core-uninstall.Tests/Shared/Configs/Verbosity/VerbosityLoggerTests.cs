using System;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Configs.Verbosity
{
    [TestClass]
    public class VerbosityLoggerTests
    {
        private static readonly VerbosityLevel DefaultVerbosityLevel = VerbosityLevel.Normal;
        private static readonly string DefaultMessage = string.Empty;

        [TestMethod]
        [DataRow(VerbosityLevel.Quiet)]
        [DataRow(VerbosityLevel.Minimal)]
        [DataRow(VerbosityLevel.Normal)]
        [DataRow(VerbosityLevel.Detailed)]
        [DataRow(VerbosityLevel.Diagnostic)]
        internal void TestConstructor(VerbosityLevel level)
        {
            var logger = new VerbosityLogger(level);
            logger.Level.Should().Be(level);
        }

        [TestMethod]
        [DataRow(VerbosityLevel.Diagnostic + 1)]
        [DataRow(VerbosityLevel.Diagnostic + 2)]
        [DataRow(VerbosityLevel.Diagnostic + 10)]
        internal void TestConstructorArgumentOutOfRangeException(VerbosityLevel level)
        {
            Action action = () => new VerbosityLogger(level);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        [DataRow(VerbosityLevel.Minimal)]
        [DataRow(VerbosityLevel.Normal)]
        [DataRow(VerbosityLevel.Detailed)]
        [DataRow(VerbosityLevel.Diagnostic)]
        internal void TestLog(VerbosityLevel level)
        {
            Action action = () => new VerbosityLogger(DefaultVerbosityLevel).Log(level, DefaultMessage);
            action.Should().NotThrow<Exception>();
        }

        [TestMethod]
        [DataRow(VerbosityLevel.Quiet)]
        [DataRow(VerbosityLevel.Diagnostic + 1)]
        [DataRow(VerbosityLevel.Diagnostic + 2)]
        [DataRow(VerbosityLevel.Diagnostic + 10)]
        internal void TestLogArgumentOutOfRangeException(VerbosityLevel level)
        {
            Action action = () => new VerbosityLogger(DefaultVerbosityLevel).Log(level, DefaultMessage);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
