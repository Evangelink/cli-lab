using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using FluentAssertions;
using Microsoft.DotNet.Tools.Uninstall.Shared.BundleInfo;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs;
using Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity;
using Microsoft.DotNet.Tools.Uninstall.Shared.Exceptions;
using Microsoft.DotNet.Tools.Uninstall.Tests.Attributes;

namespace Microsoft.DotNet.Tools.Uninstall.Tests.Shared.Configs
{
    [TestClass]
    public class CommandLineConfigsTests
    {
        [TestMethod]
        [DataRow("list", new string[] { })]
        [DataRow("list --sdk", new string[] { "sdk" })]
        [DataRow("list --runtime", new string[] { "runtime" })]
        [DataRow("list --sdk --runtime", new string[] { "sdk", "runtime" })]
        [DataRow("list -v d", new string[] { "verbosity" })]
        [DataRow("list --verbosity diag", new string[] { "verbosity" })]
        [DataRow("list --sdk -v q", new string[] { "verbosity", "sdk" })]
        [DataRow("list --runtime --verbosity minimal", new string[] { "verbosity", "runtime" })]
        [DataRow("list --sdk --runtime -v normal", new string[] { "verbosity", "sdk", "runtime" })]
        public void TestListCommandAccept(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);
            parseResult.CommandResult.Command.Name.Should().Be("list");
            parseResult.CommandResult.Tokens.Should().BeEmpty();

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            CommandLineConfigs.ListAuxOptions
                .Where(option => parseResult.FindResultFor(option) != null)
                .Select(option => option.Name)
                .Should().BeEquivalentTo(expectedAuxOptions);
        }

        [WindowsOnlyTestMethod]
        [DataRow("list --aspnet-runtime", new string[] { "aspnet-runtime" })]
        [DataRow("list -v n --aspnet-runtime", new string[] { "verbosity", "aspnet-runtime" })]
        [DataRow("list --sdk --verbosity diag --aspnet-runtime", new string[] { "verbosity", "sdk", "aspnet-runtime" })]
        [DataRow("list --runtime --hosting-bundle", new string[] { "runtime", "hosting-bundle" })]
        public void TestListCommandAcceptWindows(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.CommandResult.Command.Name.Should().Be("list");
            parseResult.CommandResult.Tokens.Should().BeEmpty();

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            CommandLineConfigs.ListAuxOptions
                .Where(option => parseResult.FindResultFor(option) != null)
                .Select(option => option.Name)
                .Should().BeEquivalentTo(expectedAuxOptions);
        }

        [TestMethod]
        [DataRow("list --all")]
        [DataRow("list --all-lower-patches")]
        [DataRow("list --all-but-latest")]
        [DataRow("list --all-but 2.2.300")]
        [DataRow("list --all-below 2.2.300")]
        [DataRow("list --all-previews")]
        [DataRow("list --all-previews-but-latest")]
        [DataRow("list --major-minor 2.2")]
        [DataRow("list 2.2")]
        [DataRow("list 2.2.300")]
        [DataRow("list --all --sdk")]
        [DataRow("list --all-but 2.2.5 --runtime")]
        [DataRow("list --major-minor 2.2 --sdk --runtime")]
        [DataRow("list -v")]
        [DataRow("list --verbosity")]
        [DataRow("list --all --sdk -v")]
        [DataRow("list --all-but 2.2.5 --verbosity --runtime")]
        [DataRow("list --major-minor 2.2 --sdk -v --runtime")]
        [DataRow("list --hosting-bundle --major-minor 1.1 --sdk")]
        [DataRow("list --version")]
        [DataRow("list -v q --version")]
        [DataRow("list --sdk --version")]
        [DataRow("list --sdk --runtime --version")]
        [DataRow("list --aspnet-runtime --version --hosting-bundle")]
        public void TestListCommandReject(string command)
        {
            CommandLineConfigs.UninstallRootCommand.Parse(command).Errors
                .Should().NotBeEmpty();
        }

        [TestMethod]
        [DataRow("--all")]
        [DataRow("--all-lower-patches")]
        [DataRow("--all-but-latest")]
        [DataRow("--all-but", "2.2.300", new[] { "2.2.300" })]
        [DataRow("--all-but", "2.2.300 3.0.100", new[] { "2.2.300", "3.0.100" })]
        [DataRow("--all-below", "2.2.300", "2.2.300")]
        [DataRow("--all-previews")]
        [DataRow("--all-previews-but-latest")]
        [DataRow("--major-minor", "2.2", "2.2")]
        [DataRow("", "2.2.300", new[] { "2.2.300" })]
        [DataRow("", "2.2.300 3.0.100", new[] { "2.2.300", "3.0.100" })]
        [DataRow("", "--unknown-option", new[] { "--unknown-option" })]
        [DataRow("", "--unknown-option argument", new[] { "--unknown-option", "argument" })]
        public void TestRemoveCommandAccept(string option, string argValue = "", object expected = null)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"remove {option} {argValue}");

            if (!option.Equals(string.Empty))
            {
                parseResult.CommandResult.OptionResult(option).Should().NotBeNull();
                parseResult.ValueForOption(option).Should().BeEquivalentTo(expected);
            }
            else
            {
                parseResult.CommandResult.Tokens.Select(t => t.Value).As<object>()
                    .Should().BeEquivalentTo(expected);
            }

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();
        }

        [TestMethod]
        [DataRow("--all")]
        [DataRow("--all-lower-patches")]
        [DataRow("--all-but-latest")]
        [DataRow("--all-but", "2.2.300", new[] { "2.2.300" })]
        [DataRow("--all-but", "2.2.300 3.0.100", new[] { "2.2.300", "3.0.100" })]
        [DataRow("--all-below", "2.2.300", "2.2.300")]
        [DataRow("--all-previews")]
        [DataRow("--all-previews-but-latest")]
        [DataRow("--major-minor", "2.2", "2.2")]
        [DataRow("", "2.2.300", new[] { "2.2.300" })]
        [DataRow("", "2.2.300 3.0.100", new[] { "2.2.300", "3.0.100" })]
        [DataRow("", "--unknown-option", new[] { "--unknown-option" })]
        [DataRow("", "--unknown-option argument", new[] { "--unknown-option", "argument" })]
        public void TestDryRunCommandAccept(string option, string argValue = "", object expected = null)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"dry-run {option} {argValue}");

            if (!option.Equals(string.Empty))
            {
                parseResult.CommandResult.OptionResult(option).Should().NotBeNull();
                parseResult.ValueForOption(option).Should().BeEquivalentTo(expected);
            }
            else
            {
                parseResult.CommandResult.Tokens.Select(t => t.Value).As<object>()
                    .Should().BeEquivalentTo(expected);
            }

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();
        }

        [TestMethod]
        [DataRow("--all")]
        [DataRow("--all-lower-patches")]
        [DataRow("--all-but-latest")]
        [DataRow("--all-but", "2.2.300", new[] { "2.2.300" })]
        [DataRow("--all-but", "2.2.300 3.0.100", new[] { "2.2.300", "3.0.100" })]
        [DataRow("--all-below", "2.2.300", "2.2.300")]
        [DataRow("--all-previews")]
        [DataRow("--all-previews-but-latest")]
        [DataRow("--major-minor", "2.2", "2.2")]
        [DataRow("", "2.2.300", new[] { "2.2.300" })]
        [DataRow("", "2.2.300 3.0.100", new[] { "2.2.300", "3.0.100" })]
        [DataRow("", "--unknown-option", new[] { "--unknown-option" })]
        [DataRow("", "--unknown-option argument", new[] { "--unknown-option", "argument" })]
        public void TestWhatIfCommandAccept(string option, string argValue = "", object expected = null)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse($"whatif {option} {argValue}");

            if (!option.Equals(string.Empty))
            {
                parseResult.CommandResult.OptionResult(option).Should().NotBeNull();
                parseResult.ValueForOption(option).Should().BeEquivalentTo(expected);
            }
            else
            {
                parseResult.CommandResult.Tokens.Select(t => t.Value).As<object>()
                    .Should().BeEquivalentTo(expected);
            }

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();
        }

        [TestMethod]
        [DataRow("--all --sdk", new string[] { "sdk" })]
        [DataRow("--all --sdk --force", new string[] { "sdk", "force" })]
        [DataRow("--all-below 2.2.300 --runtime", new string[] { "runtime" })]
        [DataRow("--all-but 2.1.5 2.1.7 3.0.0-preview-10086 --sdk --runtime", new string[] { "sdk", "runtime" })]
        [DataRow("2.1.300 3.0.100-preview-276262-01 --verbosity diagnostic", new string[] { "verbosity" })]
        [DataRow("--all -v quiet --sdk", new string[] { "verbosity", "sdk" })]
        [DataRow("--major-minor 2.3 --verbosity m --runtime", new string[] { "verbosity", "runtime" })]
        [DataRow("--all-but 2.1.5 2.1.7 3.0.0-preview-10086 --sdk -v n --runtime", new string[] { "verbosity", "sdk", "runtime" })]
        [DataRow("--all-below 2.2.300 --runtime --yes", new string[] { "runtime", "yes" })]
        [DataRow("--all-but 2.1.5 2.1.7 3.0.0-preview-10086 --sdk -y --runtime", new string[] { "sdk", "runtime", "yes" })]
        public void TestRemoveCommandAcceptAux(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse("remove " + command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            CommandLineConfigs.RemoveAuxOptions
                .Where(option => parseResult.FindResultFor(option) != null)
                .Select(option => option.Name)
                .Should().BeEquivalentTo(expectedAuxOptions);
        }

        [TestMethod]
        [DataRow("--all --sdk", new string[] { "sdk" })]
        [DataRow("--all-below 2.2.300 --runtime", new string[] { "runtime" })]
        [DataRow("--all-but 2.1.5 2.1.7 3.0.0-preview-10086 --sdk --runtime", new string[] { "sdk", "runtime" })]
        [DataRow("2.1.300 3.0.100-preview-276262-01 --verbosity diagnostic", new string[] { "verbosity" })]
        [DataRow("--all -v quiet --sdk", new string[] { "verbosity", "sdk" })]
        [DataRow("--major-minor 2.3 --verbosity m --runtime", new string[] { "verbosity", "runtime" })]
        [DataRow("--all-but 2.1.5 2.1.7 3.0.0-preview-10086 --sdk -v n --runtime", new string[] { "verbosity", "sdk", "runtime" })]
        public void TestDryRunCommandAcceptAux(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse("dry-run " + command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            CommandLineConfigs.RemoveAuxOptions
                        .Where(option => parseResult.FindResultFor(option) != null)
                .Select(option => option.Name)
                .Should().BeEquivalentTo(expectedAuxOptions);
        }

        [TestMethod]
        [DataRow("--all --sdk", new string[] { "sdk" })]
        [DataRow("--all-below 2.2.300 --runtime", new string[] { "runtime" })]
        [DataRow("--all-but 2.1.5 2.1.7 3.0.0-preview-10086 --sdk --runtime", new string[] { "sdk", "runtime" })]
        [DataRow("2.1.300 3.0.100-preview-276262-01 --verbosity diagnostic", new string[] { "verbosity" })]
        [DataRow("--all -v quiet --sdk", new string[] { "verbosity", "sdk" })]
        [DataRow("--major-minor 2.3 --verbosity m --runtime", new string[] { "verbosity", "runtime" })]
        [DataRow("--all-but 2.1.5 2.1.7 3.0.0-preview-10086 --sdk -v n --runtime", new string[] { "verbosity", "sdk", "runtime" })]
        public void TestWhatIfCommandAcceptAux(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse("whatif " + command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            CommandLineConfigs.RemoveAuxOptions
                .Where(option => parseResult.FindResultFor(option) != null)
                .Select(option => option.Name)
                .Should().BeEquivalentTo(expectedAuxOptions);
        }

        [WindowsOnlyTestMethod]
        [DataRow("remove --all --sdk --aspnet-runtime", new string[] { "sdk", "aspnet-runtime" })]
        [DataRow("remove --major-minor 1.1 --hosting-bundle -v q", new string[] { "hosting-bundle", "verbosity" })]
        public void TestOptionsAcceptAuxWindows(string command, string[] expectedAuxOptions)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            CommandLineConfigs.RemoveAuxOptions
                .Where(option => parseResult.FindResultFor(option) != null)
                .Select(option => option.Name)
                .Should().BeEquivalentTo(expectedAuxOptions);
        }

        [TestMethod]
        [DataRow("")]
        public void TestOptionsReject(string command)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            (parseResult.Errors.Count != 0 ||
                parseResult.UnparsedTokens.Count != 0 ||
                parseResult.UnmatchedTokens.Count != 0 ||
                parseResult.CommandResult.Tokens.Count != 0)
            .Should().BeTrue();
        }

        [TestMethod]
        [DataRow("--all-but")]
        [DataRow("--all-below")]
        [DataRow("--dry-run")]
        [DataRow("--major-minor")]
        [DataRow("--all-but --sdk")]
        [DataRow("--all-below --runtime")]
        [DataRow("--major-minor --verbosity q --sdk --runtime")]
        [DataRow("--verbosity")]
        [DataRow("-v")]
        [DataRow("--all-but 2.2.300 -v")]
        [DataRow("--all-below 1.23.456 -v")]
        [DataRow("--major-minor 3.0 --verbosity")]
        [DataRow("--all-but 2.1.5 2.1.7 3.0.0 --sdk --verbosity")]
        [DataRow("--dry-run --all-but")]
        [DataRow("--yes --verbosity")]
        [DataRow("--major-minor -y")]
        public void TestRemoveCommandReject(string command)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse("remove " + command);

            (parseResult.Errors.Count != 0 ||
                parseResult.UnparsedTokens.Count != 0 ||
                parseResult.UnmatchedTokens.Count != 0 ||
                parseResult.CommandResult.Tokens.Count != 0)
            .Should().BeTrue();
        }

        [TestMethod]
        [DataRow("--all-but")]
        [DataRow("--yes")]
        [DataRow("--all-below")]
        [DataRow("--dry-run")]
        [DataRow("--major-minor")]
        [DataRow("--all-but --sdk")]
        [DataRow("--all-below --runtime")]
        [DataRow("--major-minor --verbosity q --sdk --runtime")]
        [DataRow("--verbosity")]
        [DataRow("-v")]
        [DataRow("--all-but 2.2.300 -v")]
        [DataRow("--all-below 1.23.456 -v")]
        [DataRow("--major-minor 3.0 --verbosity")]
        [DataRow("--all-but 2.1.5 2.1.7 3.0.0 --sdk --verbosity")]
        [DataRow("--dry-run --all-but")]
        [DataRow("--yes --verbosity")]
        public void TestDryRunCommandReject(string command)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse("dry-run " + command);

            (parseResult.Errors.Count != 0 ||
                parseResult.UnparsedTokens.Count != 0 ||
                parseResult.UnmatchedTokens.Count != 0 ||
                parseResult.CommandResult.Tokens.Count != 0)
            .Should().BeTrue();
        }

        [TestMethod]
        [DataRow("--all-but")]
        [DataRow("--yes")]
        [DataRow("--all-below")]
        [DataRow("--dry-run")]
        [DataRow("--major-minor")]
        [DataRow("--all-but --sdk")]
        [DataRow("--all-below --runtime")]
        [DataRow("--major-minor --verbosity q --sdk --runtime")]
        [DataRow("--verbosity")]
        [DataRow("-v")]
        [DataRow("--all-but 2.2.300 -v")]
        [DataRow("--all-below 1.23.456 -v")]
        [DataRow("--major-minor 3.0 --verbosity")]
        [DataRow("--all-but 2.1.5 2.1.7 3.0.0 --sdk --verbosity")]
        [DataRow("--dry-run --all-but")]
        [DataRow("--yes --verbosity")]
        public void TestWhatIfCommandReject(string command)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse("whatif " + command);

            (parseResult.Errors.Count != 0 ||
                parseResult.UnparsedTokens.Count != 0 ||
                parseResult.UnmatchedTokens.Count != 0 ||
                parseResult.CommandResult.Tokens.Count != 0)
            .Should().BeTrue();
        }

        [MacOsOnlyTestMethod]
        [DataRow("remove --all --aspnet-runtime")]
        [DataRow("remove --major-minor 1.1 --hosting-bundle")]
        [DataRow("remove --all-but 2.2.300 --sdk --aspnet-runtime -v q")]
        [DataRow("remove --all-below 2.2 --verbosity normal --sdk --hosting-bundle")]
        [DataRow("remove --major-minor 1.1 --hosting-bundle --dry-run")]
        [DataRow("remove --all-but 2.2.300 --sdk --aspnet-runtime --yes -v q")]
        [DataRow("remove --all-below 2.2 --verbosity normal -y --sdk --hosting-bundle")]
        public void TestOptionsRejectMacOs(string command)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            (parseResult.Errors.Count != 0 ||
                parseResult.UnparsedTokens.Count != 0 ||
                parseResult.UnmatchedTokens.Count != 0 ||
                parseResult.CommandResult.Tokens.Count != 0)
            .Should().BeTrue();
        }

        [TestMethod]
        [DataRow("--all")]
        [DataRow("--all-lower-patches")]
        [DataRow("--all-but-latest")]
        [DataRow("--all-but", "2.2.300")]
        [DataRow("--all-but", "2.2.300 3.0.100")]
        [DataRow("--all-below", "2.2.300")]
        [DataRow("--all-previews")]
        [DataRow("--all-previews-but-latest")]
        [DataRow("--major-minor", "2.2")]
        [DataRow("", "2.2.300")]
        [DataRow("", "2.2.300 3.0.100")]
        [DataRow("", "--unknown-option")]
        [DataRow("", "--unknown-option argument")]
        public void TestGetUninstallRemoveOptionAccept(string option, string argValue = "")
        {
            var commandResult = CommandLineConfigs.UninstallRootCommand.Parse($"remove {option} {argValue}").CommandResult;

            if (option.Equals(string.Empty))
            {
                commandResult.GetUninstallMainOption().Should().BeNull();
            }
            else
            {
                commandResult.GetUninstallMainOption().Name
                    .Should().Be(commandResult.OptionResult(option).Option.Name);
            }
        }

        [TestMethod]
        [DataRow("--all")]
        [DataRow("--all-lower-patches")]
        [DataRow("--all-but-latest")]
        [DataRow("--all-but", "2.2.300")]
        [DataRow("--all-but", "2.2.300 3.0.100")]
        [DataRow("--all-below", "2.2.300")]
        [DataRow("--all-previews")]
        [DataRow("--all-previews-but-latest")]
        [DataRow("--major-minor", "2.2")]
        [DataRow("", "2.2.300")]
        [DataRow("", "2.2.300 3.0.100")]
        [DataRow("", "--unknown-option")]
        [DataRow("", "--unknown-option argument")]
        public void TestGetUninstallDryRunOptionAccept(string option, string argValue = "")
        {
            var commandResult = CommandLineConfigs.UninstallRootCommand.Parse($"dry-run {option} {argValue}").CommandResult;

            if (option.Equals(string.Empty))
            {
                commandResult.GetUninstallMainOption().Should().BeNull();
            }
            else
            {
                commandResult.GetUninstallMainOption().Name
                    .Should().Be(commandResult.OptionResult(option).Option.Name);
            }
        }

        [TestMethod]
        [DataRow("--all")]
        [DataRow("--all-lower-patches")]
        [DataRow("--all-but-latest")]
        [DataRow("--all-but", "2.2.300")]
        [DataRow("--all-but", "2.2.300 3.0.100")]
        [DataRow("--all-below", "2.2.300")]
        [DataRow("--all-previews")]
        [DataRow("--all-previews-but-latest")]
        [DataRow("--major-minor", "2.2")]
        [DataRow("", "2.2.300")]
        [DataRow("", "2.2.300 3.0.100")]
        [DataRow("", "--unknown-option")]
        [DataRow("", "--unknown-option argument")]
        public void TestGetUninstallWhatIfOptionAccept(string option, string argValue = "")
        {
            var commandResult = CommandLineConfigs.UninstallRootCommand.Parse($"whatif {option} {argValue}").CommandResult;

            if (option.Equals(string.Empty))
            {
                commandResult.GetUninstallMainOption().Should().BeNull();
            }
            else
            {
                commandResult.GetUninstallMainOption().Name
                    .Should().Be(commandResult.OptionResult(option).Option.Name);
            }
        }

        public static IEnumerable<object[]> GetDataForTestGetUninstallMainOptionOptionsConflictException()
        {
            yield return new object[]
            {
                (Command: "remove", Option: "--all", ArgValue: ""),
                (Command: "remove", Option: "--all-lower-patches", ArgValue: "")
            };

            yield return new object[]
            {
                (Command: "dry-run", Option: "--all", ArgValue: ""),
                (Command: "dry-run", Option: "--all-below", ArgValue: "2.2.300")
            };

            yield return new object[]
            {
                (Command: "whatif", Option: "--all", ArgValue: ""),
                (Command: "whatif", Option: "--all-but", ArgValue: "2.2.300 2.1.700")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700"),
                (Command: "remove", Option: "--major-minor", ArgValue: "2.1")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700"),
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 3.0.100")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700"),
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 3.0.100 --unknown-option")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-below", ArgValue: "--unknown-option"),
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 3.0.100")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-below", ArgValue: "2.2.300"),
                (Command: "remove", Option: "--major-minor", ArgValue: "--unknown-option")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-lower-patches", ArgValue: ""),
                (Command: "remove", Option: "--all", ArgValue: "")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-below", ArgValue: "2.2.300"),
                (Command: "remove", Option: "--all", ArgValue: "")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 2.1.700"),
                (Command: "remove", Option: "--all", ArgValue: "")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--major-minor", ArgValue: "2.1"),
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 3.0.100"),
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 3.0.100 --unknown-option"),
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 3.0.100"),
                (Command: "remove", Option: "--all-below", ArgValue: "--unknown-option")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--major-minor", ArgValue: "--unknown-option"),
                (Command: "remove", Option: "--all-below", ArgValue: "2.2.300")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all", ArgValue: ""),
                (Command: "remove", Option: "--all-lower-patches", ArgValue: ""),
                (Command: "remove", Option: "--all-below", ArgValue: "2.2.300")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all", ArgValue: ""),
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700"),
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 --unknown-option")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all", ArgValue: ""),
                (Command: "remove", Option: "--all-but", ArgValue: "2.1.700 3.0.100"),
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 --unknown-option")
            };

            yield return new object[]
            {
                (Command: "remove", Option: "--all", ArgValue: ""),
                (Command: "remove", Option: "--all-below", ArgValue: "2.1.700"),
                (Command: "remove", Option: "--all-but", ArgValue: "2.2.300 --unknown-option"),
                (Command: "remove", Option: "--all-lower-patches", ArgValue: ""),
                (Command: "remove", Option: "--major-minor", ArgValue: "2.2"),
                (Command: "remove", Option: "--all-previews-but-latest", ArgValue: "")
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetDataForTestGetUninstallMainOptionOptionsConflictException))]
        public void TestGetUninstallMainOptionOptionsConflictException(params (string Command, string Option, string ArgValue)[] options)
        {
            var command = string.Join(" ", options.Select(option => $"{option.Command} {option.Option} {option.ArgValue}"));
            var optionNames = string.Join(", ", options.Select(option => $"--{option.Option}"));

            Action action = () => CommandLineConfigs.UninstallRootCommand.Parse(command)
                .CommandResult.GetUninstallMainOption();

            action.Should().Throw<OptionsConflictException>(string.Format(LocalizableStrings.OptionsConflictExceptionMessageFormat, optionNames));
        }

        [TestMethod]
        [DataRow("remove", "--all", "2.2.300")]
        [DataRow("remove", "--all", "--unknown-option")]
        [DataRow("dry-run", "--all", "2.2.300")]
        [DataRow("whatif", "--all", "2.2.300")]
        public void TestGetUninstallMainOptionMoreThanZeroVersionSpecifiedException(string command, string option, string commandArgValue)
        {
            Action action1 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{command} {option} {commandArgValue}")
                .CommandResult.GetUninstallMainOption();

            Action action2 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{command} {commandArgValue} {option}")
                .CommandResult.GetUninstallMainOption();

            action1.Should().Throw<MoreThanZeroVersionSpecifiedException>(string.Format(LocalizableStrings.MoreThanZeroVersionSpecifiedExceptionMessageFormat, option));
            action2.Should().Throw<MoreThanZeroVersionSpecifiedException>(string.Format(LocalizableStrings.MoreThanZeroVersionSpecifiedExceptionMessageFormat, option));
        }

        [TestMethod]
        [DataRow("remove", "--all-below", "2.1.700", "2.2.300")]
        [DataRow("whatif", "--all-below", "2.1.700", "2.2.300")]
        [DataRow("dry-run", "--all-below", "2.1.700", "2.2.300")]
        [DataRow("remove", "--all-below", "--unknown-option-1", "--unknown-option-2")]
        public void TestGetUninstallMainOptionMoreThanOneVersionSpecifiedException(string command, string option, string commandArgValue, string optionArgValue)
        {
            Action action1 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{command} {option} {optionArgValue} {commandArgValue}")
                .CommandResult.GetUninstallMainOption();

            Action action2 = () => CommandLineConfigs.UninstallRootCommand.Parse($"{command} {commandArgValue} {option} {optionArgValue}")
                .CommandResult.GetUninstallMainOption();

            action1.Should().Throw<MoreThanOneVersionSpecifiedException>(string.Format(LocalizableStrings.MoreThanOneVersionSpecifiedExceptionMessageFormat, option));
            action2.Should().Throw<MoreThanOneVersionSpecifiedException>(string.Format(LocalizableStrings.MoreThanOneVersionSpecifiedExceptionMessageFormat, option));
        }

        [TestMethod]
        [DataRow("remove", "--all-but", "2.2.300")]
        [DataRow("whatif", "--all-but", "2.2.300")]
        [DataRow("dry-run", "--all-but", "2.2.300")]
        [DataRow("remove", "--all-but", "2.2.300 2.1.700")]
        [DataRow("remove", "--all-but", "--unknown-option 2.1.700")]
        [DataRow("remove", "--all-but", "2.1.700 --unknown-option")]
        [DataRow("remove", "--all-but", "--unknown-option-1 --unknown-option-2")]
        [DataRow("remove", "--all-but", "2.2.300", "2.1.700")]
        [DataRow("remove", "--all-but", "2.2.300 2.1.700", "2.1.202 2.2.233")]
        [DataRow("remove", "--all-but", "--unknown-option 2.1.700", "--unknown-option-2")]
        [DataRow("remove", "--all-but", "2.1.700 --unknown-option", "--unknown-option-2 2.3.333")]
        [DataRow("remove", "--all-but", "--unknown-option-1 --unknown-option-2", "3.0.100")]
        public void TestGetUninstallMainOptionVersionBeforeOptionException(string command, string option, string commandArgValue, string optionArgValue = "")
        {
            Action action = () => CommandLineConfigs.UninstallRootCommand.Parse($"{command} {commandArgValue} {option} {optionArgValue}")
                .CommandResult.GetUninstallMainOption();

            action.Should().Throw<VersionBeforeOptionException>(string.Format(LocalizableStrings.VersionBeforeOptionExceptionMessageFormat, option));
        }

        [TestMethod]
        [DataRow("remove --sdk", BundleType.Sdk)]
        [DataRow("remove --runtime", BundleType.Runtime)]
        [DataRow("remove --sdk --runtime", BundleType.Sdk | BundleType.Runtime)]
        [DataRow("remove --sdk --verbosity minimal", BundleType.Sdk)]
        [DataRow("remove -v normal --runtime", BundleType.Runtime)]
        [DataRow("remove --sdk --verbosity diag --runtime", BundleType.Sdk | BundleType.Runtime)]
        [DataRow("remove --sdk --all-but 2.2.300 2.1.700", BundleType.Sdk)]
        [DataRow("remove --runtime --all-below 3.0.1-preview-10086", BundleType.Runtime)]
        [DataRow("remove --sdk --runtime --all-previews", BundleType.Sdk | BundleType.Runtime)]
        [DataRow("remove --runtime --all", BundleType.Runtime)]
        [DataRow("remove --yes --sdk --all-below 2.0", BundleType.Sdk)]
        [DataRow("remove --sdk --runtime -y", BundleType.Sdk | BundleType.Runtime)]
        [DataRow("dry-run --sdk --runtime", BundleType.Sdk | BundleType.Runtime)]
        [DataRow("whatif --sdk --runtime", BundleType.Sdk | BundleType.Runtime)]
        internal void TestGetTypeSelectionRemoveCommand(string command, BundleType expected)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            parseResult.GetTypeSelection()
                .Should().Be(expected);
        }

        [WindowsOnlyTestMethod]
        [DataRow("remove", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime | BundleType.HostingBundle)]
        [DataRow("remove -v q", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime | BundleType.HostingBundle)]
        [DataRow("remove --all", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime | BundleType.HostingBundle)]
        [DataRow("remove --aspnet-runtime", BundleType.AspNetRuntime)]
        [DataRow("remove --sdk --aspnet-runtime --all-but 2.2.3", BundleType.Sdk | BundleType.AspNetRuntime)]
        [DataRow("remove --sdk --runtime --aspnet-runtime", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime)]
        [DataRow("remove --hosting-bundle --aspnet-runtime", BundleType.AspNetRuntime | BundleType.HostingBundle)]
        [DataRow("remove --hosting-bundle --sdk --all", BundleType.Sdk | BundleType.HostingBundle)]
        [DataRow("remove --yes", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime | BundleType.HostingBundle)]
        [DataRow("remove -y", BundleType.Sdk | BundleType.Runtime | BundleType.AspNetRuntime | BundleType.HostingBundle)]
        internal void TestGetTypeSelectionRootCommandWindows(string command, BundleType expected)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            parseResult.GetTypeSelection()
                .Should().Be(expected);
        }

        [MacOsOnlyTestMethod]
        [DataRow("remove", BundleType.Sdk | BundleType.Runtime)]
        [DataRow("remove -v q", BundleType.Sdk | BundleType.Runtime)]
        [DataRow("remove --all", BundleType.Sdk | BundleType.Runtime)]
        [DataRow("remove --yes", BundleType.Sdk | BundleType.Runtime)]
        [DataRow("remove -y", BundleType.Sdk | BundleType.Runtime)]
        internal void TestGetTypeSelectionRootCommandMacOs(string command, BundleType expected)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            parseResult.GetTypeSelection()
                .Should().Be(expected);
        }

        [TestMethod]
        [DataRow("remove 2.2.300 --sdk", VerbosityLevel.Normal)]
        [DataRow("remove --all -v q --sdk", VerbosityLevel.Quiet)]
        [DataRow("remove --all-below 2.2 --verbosity minimal --sdk", VerbosityLevel.Minimal)]
        [DataRow("remove --all-but 2.2.300 2.1.700 -v normal --runtime", VerbosityLevel.Normal)]
        [DataRow("remove --runtime --all-previews --verbosity d", VerbosityLevel.Detailed)]
        [DataRow("remove -v diag --runtime --major-minor 2.2", VerbosityLevel.Diagnostic)]
        [DataRow("remove -v diagnostic --major-minor 2.2 --runtime", VerbosityLevel.Diagnostic)]
        [DataRow("remove 1.1.11 -v normal --hosting-bundle", VerbosityLevel.Normal)]
        [DataRow("list", VerbosityLevel.Normal)]
        [DataRow("list -v q", VerbosityLevel.Quiet)]
        [DataRow("list --verbosity minimal", VerbosityLevel.Minimal)]
        [DataRow("list -v normal", VerbosityLevel.Normal)]
        [DataRow("list --verbosity d", VerbosityLevel.Detailed)]
        [DataRow("list -v diag", VerbosityLevel.Diagnostic)]
        [DataRow("list -v diagnostic", VerbosityLevel.Diagnostic)]
        [DataRow("list -v d", VerbosityLevel.Detailed)]
        [DataRow("remove --runtime --all-below 2.0 --yes -v q", VerbosityLevel.Quiet)]
        [DataRow("remove --runtime --all-previews --y -v diag", VerbosityLevel.Diagnostic)]
        internal void TestGetVerbosityLevel(string command, VerbosityLevel expected)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);

            parseResult.Errors.Should().BeEmpty();
            parseResult.UnparsedTokens.Should().BeEmpty();
            parseResult.UnmatchedTokens.Should().BeEmpty();

            parseResult.CommandResult.GetVerbosityLevel()
                .Should().Be(expected);
        }

        [TestMethod]
        [DataRow("remove 2.2.300 --sdk -v qu")]
        [DataRow("remove --all --sdk --verbosity mini")]
        [DataRow("remove --major-minor 2.1 -v unknown")]
        [DataRow("list -v qu")]
        [DataRow("list --verbosity mini")]
        [DataRow("list -v unknown")]
        public void TestGetVerbosityLevelVerbosityLevelInvalidException(string command)
        {
            var parseResult = CommandLineConfigs.UninstallRootCommand.Parse(command);
            Action action = () => parseResult.CommandResult.GetVerbosityLevel();

            action.Should().Throw<VerbosityLevelInvalidException>(LocalizableStrings.VerbosityLevelInvalidExceptionMessage);
        }
    }
}
