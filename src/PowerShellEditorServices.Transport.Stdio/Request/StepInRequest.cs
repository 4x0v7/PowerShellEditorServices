﻿using Microsoft.PowerShell.EditorServices.Transport.Stdio.Message;
using Microsoft.PowerShell.EditorServices.Transport.Stdio.Response;
using Nito.AsyncEx;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.EditorServices.Transport.Stdio.Request
{
    [MessageTypeName("stepIn")]
    public class StepInRequest : RequestBase<object>
    {
        public override async Task ProcessMessage(
            EditorSession editorSession, 
            MessageWriter messageWriter)
        {
            editorSession.DebugService.StepIn();

            await messageWriter.WriteMessage(
                this.PrepareResponse(
                    new StepInResponse()));
        }
    }
}
