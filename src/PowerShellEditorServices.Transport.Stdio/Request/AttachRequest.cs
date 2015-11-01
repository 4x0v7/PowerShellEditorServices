//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.PowerShell.EditorServices.Transport.Stdio.Message;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.EditorServices.Transport.Stdio.Request
{
    [MessageTypeName("attach")]
    public class AttachRequest : RequestBase<AttachRequestArguments>
    {
        public override Task ProcessMessage(
            EditorSession editorSession, 
            MessageWriter messageWriter)
        {
            throw new System.NotImplementedException();
        }
    }

    public class AttachRequestArguments
    {
        public string Address { get; set; }

        public int Port { get; set; }
    }
}

