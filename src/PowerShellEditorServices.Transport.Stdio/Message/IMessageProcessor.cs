﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.PowerShell.EditorServices.Session;

namespace Microsoft.PowerShell.EditorServices.Transport.Stdio.Message
{
    /// <summary>
    /// Provides an interface for classes that can process an incoming
    /// message of a given type.
    /// </summary>
    public interface IMessageProcessor
    {
        /// <summary>
        /// Performs some action
        /// </summary>
        /// <param name="editorSession"></param>
        /// <param name="messageWriter"></param>
        void ProcessMessage(
            EditorSession editorSession,
            MessageWriter messageWriter);
    }
}
