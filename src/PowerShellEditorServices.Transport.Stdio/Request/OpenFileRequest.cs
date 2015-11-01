﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.PowerShell.EditorServices.Transport.Stdio.Message;
using Nito.AsyncEx;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.EditorServices.Transport.Stdio.Request
{
    [MessageTypeName("open")]
    public class OpenFileRequest : RequestBase<FileRequestArguments>
    {
        public static OpenFileRequest Create(string filePath)
        {
            return new OpenFileRequest
            {
                Arguments = new FileRequestArguments
                {
                    File = filePath
                }
            };
        }

        public override Task ProcessMessage(
            EditorSession editorSession,
            MessageWriter messageWriter)
        {
            // Open the file in the current session
            editorSession.Workspace.GetFile(this.Arguments.File);

            return TaskConstants.Completed;
        }
    }
}
