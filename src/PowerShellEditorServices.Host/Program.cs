﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.PowerShell.EditorServices.Transport.Stdio;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.PowerShell.EditorServices.Host
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // In the future, a more robust argument parser will be added here
            bool waitForDebugger =
                args.Any(
                    arg => 
                        string.Equals(
                            arg,
                            "/waitForDebugger",
                            StringComparison.InvariantCultureIgnoreCase));

            // Should we wait for the debugger before starting?
            if (waitForDebugger)
            {
                while (!Debugger.IsAttached)
                {
                    Thread.Sleep(500);
                }
            }

            // TODO: Select host, console host, and transport based on command line arguments

            IHost host = new StdioHost();
            host.Start();
        }
    }
}
