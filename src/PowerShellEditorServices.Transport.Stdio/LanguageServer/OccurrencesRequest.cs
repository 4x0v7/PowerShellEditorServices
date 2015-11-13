﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.PowerShell.EditorServices.Protocol.MessageProtocol;
using Nito.AsyncEx;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.EditorServices.Protocol.LanguageServer
{
    [MessageTypeName("occurrences")]
    public class OccurrencesRequest : FileRequest<FileLocationRequestArgs>
    {
        public override async Task ProcessMessage(
            EditorSession editorSession,
            MessageWriter messageWriter)
        {
            ScriptFile scriptFile = this.GetScriptFile(editorSession);

            FindOccurrencesResult occurrencesResult =
                editorSession.LanguageService.FindOccurrencesInFile(
                    scriptFile,
                    this.Arguments.Line,
                    this.Arguments.Offset);

            OccurrencesResponse occurrencesResponce = 
                OccurrencesResponse.Create(occurrencesResult, this.Arguments.File);

            await messageWriter.WriteMessage(
                this.PrepareResponse(
                    occurrencesResponce));
        }
    }
}
