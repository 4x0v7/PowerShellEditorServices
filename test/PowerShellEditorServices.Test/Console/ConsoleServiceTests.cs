﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.PowerShell.EditorServices.Console;
using Microsoft.PowerShell.EditorServices.Session;
using System;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.PowerShell.EditorServices.Test.Console
{
    public class ConsoleServiceTests : IDisposable
    {
        private Workspace workspace;
        private ScriptFile debugScriptFile;
        private TestConsoleHost consoleHost;
        private ConsoleService consoleService;
        private PowerShellSession powerShellSession;

        public ConsoleServiceTests()
        {
            this.consoleHost = new TestConsoleHost();
        }

        public void Dispose()
        {
        }

        [Fact]
        public async Task ReceivesChoicePrompt()
        {
            string choiceScript =
                @"
                $caption = ""Test Choice"";
                $message = ""Make a selection"";
                $choiceA = new-Object System.Management.Automation.Host.ChoiceDescription ""&A"",""A"";
                $choiceB = new-Object System.Management.Automation.Host.ChoiceDescription ""&B"",""B"";
                $choices = [System.Management.Automation.Host.ChoiceDescription[]]($choiceA,$choiceB);
                $response = $host.ui.PromptForChoice($caption, $message, $choices, 1)
                $response";

            await this.powerShellSession.ExecuteScript(choiceScript);

            // TODO: Verify prompt info

            // Verify prompt result written to output
            //Assert.Equal(
            //    "1" + Environment.NewLine,
            //    this.consoleHost.GetOutputForType(OutputType.Normal));
        }
    }
}
