﻿using Microsoft.PowerShell.EditorServices.Console;
using Microsoft.PowerShell.EditorServices.Session;
using Microsoft.PowerShell.EditorServices.Transport.Stdio.Message;
using Microsoft.PowerShell.EditorServices.Transport.Stdio.Model;
using Microsoft.PowerShell.EditorServices.Transport.Stdio.Response;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.EditorServices.Transport.Stdio.Request
{
    //    /** SetBreakpoints request; value of command field is "setBreakpoints".
    //        Sets multiple breakpoints for a single source and clears all previous breakpoints in that source.
    //        To clear all breakpoint for a source, specify an empty array.
    //        When a breakpoint is hit, a StoppedEvent (event type 'breakpoint') is generated.
    //    */
    [MessageTypeName("setBreakpoints")]
    public class SetBreakpointsRequest : RequestBase<SetBreakpointsRequestArguments>
    {
        public override async Task ProcessMessage(
            EditorSession editorSession, 
            MessageWriter messageWriter)
        {
            ScriptFile scriptFile =
                editorSession.Workspace.GetFile(
                    this.Arguments.Source.Path);

            BreakpointDetails[] breakpoints =
                await editorSession.DebugService.SetBreakpoints(
                    scriptFile,
                    this.Arguments.Lines);

            await messageWriter.WriteMessage(
                this.PrepareResponse(
                    SetBreakpointsResponse.Create(
                        breakpoints)));
        }
    }

    public class SetBreakpointsRequestArguments
    {
        public Source Source { get; set; }

        public int[] Lines { get; set; }
    }
}
