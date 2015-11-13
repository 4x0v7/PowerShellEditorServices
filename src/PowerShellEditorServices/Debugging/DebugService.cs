﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.PowerShell.EditorServices.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.EditorServices
{
    /// <summary>
    /// Provides a high-level service for interacting with the
    /// PowerShell debugger in the context of a PowerShellSession.
    /// </summary>
    public class DebugService
    {
        #region Fields

        private PowerShellSession powerShellSession;

        // TODO: This needs to be managed per nested session
        private Dictionary<string, List<Breakpoint>> breakpointsPerFile = 
            new Dictionary<string, List<Breakpoint>>();

        private int nextVariableId;
        private List<VariableDetails> currentVariables;
        private StackFrameDetails[] callStackFrames;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DebugService class and uses
        /// the given PowerShellSession for all future operations.
        /// </summary>
        /// <param name="powerShellSession">
        /// The PowerShellSession to use for all debugging operations.
        /// </param>
        public DebugService(PowerShellSession powerShellSession)
        {
            Validate.IsNotNull("powerShellSession", powerShellSession);

            this.powerShellSession = powerShellSession;
            this.powerShellSession.DebuggerStop += this.OnDebuggerStop;
            this.powerShellSession.BreakpointUpdated += this.OnBreakpointUpdated;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the list of breakpoints for the current debugging session.
        /// </summary>
        /// <param name="scriptFile">The ScriptFile in which breakpoints will be set.</param>
        /// <param name="lineNumbers">The line numbers at which breakpoints will be set.</param>
        /// <param name="clearExisting">If true, causes all existing breakpoints to be cleared before setting new ones.</param>
        /// <returns>An awaitable Task that will provide details about the breakpoints that were set.</returns>
        public async Task<BreakpointDetails[]> SetBreakpoints(
            ScriptFile scriptFile, 
            int[] lineNumbers, 
            bool clearExisting = true)
        {
            IEnumerable<Breakpoint> resultBreakpoints = null;

            if (clearExisting)
            {
                await this.ClearBreakpointsInFile(scriptFile);
            }

            if (lineNumbers.Length > 0)
            {
                PSCommand psCommand = new PSCommand();
                psCommand.AddCommand("Set-PSBreakpoint");
                psCommand.AddParameter("Script", scriptFile.FilePath);
                psCommand.AddParameter("Line", lineNumbers.Length > 0 ? lineNumbers : null);

                resultBreakpoints =
                    await this.powerShellSession.ExecuteCommand<Breakpoint>(
                        psCommand);

                return
                    resultBreakpoints
                        .Select(BreakpointDetails.Create)
                        .ToArray();
            }

            return new BreakpointDetails[0];
        }

        /// <summary>
        /// Sends a "continue" action to the debugger when stopped.
        /// </summary>
        public void Continue()
        {
            this.powerShellSession.ResumeDebugger(
                DebuggerResumeAction.Continue);
        }

        /// <summary>
        /// Sends a "step over" action to the debugger when stopped. 
        /// </summary>
        public void StepOver()
        {
            this.powerShellSession.ResumeDebugger(
                DebuggerResumeAction.StepOver);
        }

        /// <summary>
        /// Sends a "step in" action to the debugger when stopped.
        /// </summary>
        public void StepIn()
        {
            this.powerShellSession.ResumeDebugger(
                DebuggerResumeAction.StepInto);
        }

        /// <summary>
        /// Sends a "step out" action to the debugger when stopped.
        /// </summary>
        public void StepOut()
        {
            this.powerShellSession.ResumeDebugger(
                DebuggerResumeAction.StepOut);
        }

        /// <summary>
        /// Causes the debugger to break execution wherever it currently
        /// is at the time.  This is equivalent to clicking "Pause" in a 
        /// debugger UI.
        /// </summary>
        public void Break()
        {
            // Break execution in the debugger
            this.powerShellSession.BreakExecution();
        }

        /// <summary>
        /// Aborts execution of the debugger while it is running, even while
        /// it is stopped.  Equivalent to calling PowerShellSession.AbortExecution.
        /// </summary>
        public void Abort()
        {
            this.powerShellSession.AbortExecution();
        }

        /// <summary>
        /// Gets the list of variables that are children of the scope or variable
        /// that is identified by the given referenced ID.
        /// </summary>
        /// <param name="variableReferenceId"></param>
        /// <returns>An array of VariableDetails instances which describe the requested variables.</returns>
        public VariableDetails[] GetVariables(int variableReferenceId)
        {
            VariableDetails[] childVariables = null;

            if (variableReferenceId >= VariableDetails.FirstVariableId)
            {
                int correctedId =
                    (variableReferenceId - VariableDetails.FirstVariableId);

                VariableDetails parentVariable = 
                    this.currentVariables[correctedId];

                if (parentVariable.IsExpandable)
                {
                    childVariables = parentVariable.GetChildren();

                    foreach (var child in childVariables)
                    {
                        this.currentVariables.Add(child);
                        child.Id = this.nextVariableId;
                        this.nextVariableId++;
                    }
                }
                else
                {
                    childVariables = new VariableDetails[0];
                }
            }
            else
            {
                // TODO: Get variables for the desired scope ID
                childVariables = this.currentVariables.ToArray();
            }

            return childVariables;
        }

        /// <summary>
        /// Evaluates a variable expression in the context of the stopped
        /// debugger.  This method decomposes the variable expression to
        /// walk the cached variable data for the specified stack frame.
        /// </summary>
        /// <param name="variableExpression">The variable expression string to evaluate.</param>
        /// <param name="stackFrameId">The ID of the stack frame in which the expression should be evaluated.</param>
        /// <returns>A VariableDetails object containing the result.</returns>
        public VariableDetails GetVariableFromExpression(string variableExpression, int stackFrameId)
        {
            // Break up the variable path
            string[] variablePathParts = variableExpression.Split('.');

            VariableDetails resolvedVariable = null;
            IEnumerable<VariableDetails> variableList = this.currentVariables;

            foreach (var variableName in variablePathParts)
            {
                if (variableList == null)
                {
                    // If there are no children left to search, break out early
                    return null;
                }

                resolvedVariable =
                    variableList.FirstOrDefault(
                        v =>
                            string.Equals(
                                v.Name,
                                variableExpression,
                                StringComparison.InvariantCultureIgnoreCase));

                if (resolvedVariable != null &&
                    resolvedVariable.IsExpandable)
                {
                    // Continue by searching in this variable's children
                    variableList = this.GetVariables(resolvedVariable.Id);
                }
            }

            return resolvedVariable;
        }

        /// <summary>
        /// Evaluates an expression in the context of the stopped
        /// debugger.  This method will execute the specified expression
        /// PowerShellSession.
        /// </summary>
        /// <param name="expressionString">The expression string to execute.</param>
        /// <param name="stackFrameId">The ID of the stack frame in which the expression should be executed.</param>
        /// <returns>A VariableDetails object containing the result.</returns>
        public async Task<VariableDetails> EvaluateExpression(string expressionString, int stackFrameId)
        {
            var results = 
                await this.powerShellSession.ExecuteScriptString(
                    expressionString);

            // Since this method should only be getting invoked in the debugger,
            // we can assume that Out-String will be getting used to format results
            // of command executions into string output.

            return new VariableDetails(
                expressionString, 
                string.Join(Environment.NewLine, results));
        }

        /// <summary>
        /// Gets the list of stack frames at the point where the
        /// debugger sf stopped.
        /// </summary>
        /// <returns>
        /// An array of StackFrameDetails instances that contain the stack trace.
        /// </returns>
        public StackFrameDetails[] GetStackFrames()
        {
            return this.callStackFrames;
        }

        /// <summary>
        /// Gets the list of variable scopes for the stack frame that
        /// is identified by the given ID.
        /// </summary>
        /// <param name="stackFrameId">The ID of the stack frame at which variable scopes should be retrieved.</param>
        /// <returns>The list of VariableScope instances which describe the available variable scopes.</returns>
        public VariableScope[] GetVariableScopes(int stackFrameId)
        {
            // TODO: Return different scopes based on PowerShell scoping mechanics
            return new VariableScope[]
            {
                new VariableScope(1, "Locals")
            };
        }

        #endregion

        #region Private Methods

        private async Task ClearBreakpointsInFile(ScriptFile scriptFile)
        {
            List<Breakpoint> breakpoints = null;

            // Get the list of breakpoints for this file
            if (this.breakpointsPerFile.TryGetValue(scriptFile.Id, out breakpoints))
            {
                if (breakpoints.Count > 0)
                {
                    PSCommand psCommand = new PSCommand();
                    psCommand.AddCommand("Remove-PSBreakpoint");
                    psCommand.AddParameter("Breakpoint", breakpoints.ToArray());

                    await this.powerShellSession.ExecuteCommand<object>(psCommand);

                    // Clear the existing breakpoints list for the file
                    breakpoints.Clear();
                }
            }
        }

        private async Task FetchVariables()
        {
            this.nextVariableId = VariableDetails.FirstVariableId;
            this.currentVariables = new List<VariableDetails>();

            PSCommand psCommand = new PSCommand();
            psCommand.AddCommand("Get-Variable");
            psCommand.AddParameter("Scope", "Local");

            var results = await this.powerShellSession.ExecuteCommand<PSVariable>(psCommand);

            foreach (var variable in results)
            {
                var details = new VariableDetails(variable);
                details.Id = this.nextVariableId;
                this.currentVariables.Add(details);

                this.nextVariableId++;
            }
        }

        private async Task FetchStackFrames()
        {
            PSCommand psCommand = new PSCommand();
            psCommand.AddCommand("Get-PSCallStack");

            var results = await this.powerShellSession.ExecuteCommand<CallStackFrame>(psCommand);

            this.callStackFrames =
                results
                    .Select(StackFrameDetails.Create)
                    .ToArray();
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the debugger stops execution at a breakpoint or when paused.
        /// </summary>
        public event EventHandler<DebuggerStopEventArgs> DebuggerStopped;

        private async void OnDebuggerStop(object sender, DebuggerStopEventArgs e)
        {
            // Get the call stack and local variables
            await this.FetchStackFrames();
            await this.FetchVariables();

            // Notify the host that the debugger is stopped
            if (this.DebuggerStopped != null)
            {
                this.DebuggerStopped(sender, e);
            }
        }

        /// <summary>
        /// Raised when a breakpoint is added/removed/updated in the debugger.
        /// </summary>
        public event EventHandler<BreakpointUpdatedEventArgs> BreakpointUpdated;

        private void OnBreakpointUpdated(object sender, BreakpointUpdatedEventArgs e)
        {
            List<Breakpoint> breakpoints = null;

            // Normalize the script filename for proper indexing
            string normalizedScriptName = e.Breakpoint.Script.ToLower();

            // Get the list of breakpoints for this file
            if (!this.breakpointsPerFile.TryGetValue(normalizedScriptName, out breakpoints))
            {
                breakpoints = new List<Breakpoint>();
                this.breakpointsPerFile.Add(
                    normalizedScriptName,
                    breakpoints);
            }

            // Add or remove the breakpoint based on the update type
            if (e.UpdateType == BreakpointUpdateType.Set)
            {
                breakpoints.Add(e.Breakpoint);
            }
            else if(e.UpdateType == BreakpointUpdateType.Removed)
            {
                breakpoints.Remove(e.Breakpoint);
            }
            else
            {
                // TODO: Do I need to switch out instances for updated breakpoints?
            }

            if (this.BreakpointUpdated != null)
            {
                this.BreakpointUpdated(sender, e);
            }
        }

        #endregion
    }
}
