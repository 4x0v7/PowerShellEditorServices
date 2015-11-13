﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.PowerShell.EditorServices.Protocol.DebugAdapter;
using Microsoft.PowerShell.EditorServices.Protocol.MessageProtocol;
using System.Collections.Generic;

namespace Microsoft.PowerShell.EditorServices.Protocol.LanguageServer
{
    [MessageTypeName("definition")]
    public class DefinitionResponse : ResponseBase<FileSpan[]>
    {
        public static DefinitionResponse Create(SymbolReference result)
        {
            if (result != null)
            {
                //The protocol expects a filespan when there whould only be one definition
                List<FileSpan> declarResult = new List<FileSpan>();
                declarResult.Add(
                        new FileSpan()
                        {
                            Start = new Location
                            {
                                Line = result.ScriptRegion.StartLineNumber,
                                Offset = result.ScriptRegion.StartColumnNumber
                            },
                            End = new Location
                            {
                                Line = result.ScriptRegion.EndLineNumber,
                                Offset = result.ScriptRegion.EndColumnNumber
                            },
                            File = result.FilePath
                        });
                return new DefinitionResponse
                {
                    Body = declarResult.ToArray()
                };   
            }
            else 
            {
                return new DefinitionResponse
                {
                    Body = null
                };            
            }
        }
        public static DefinitionResponse Create()
        {
            return new DefinitionResponse
            {
                Body = null
            };   
        }
    }
}
