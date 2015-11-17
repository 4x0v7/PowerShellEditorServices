﻿using Microsoft.PowerShell.EditorServices.Protocol.MessageProtocol;

namespace Microsoft.PowerShell.EditorServices.Protocol.LanguageServer
{
    public enum DocumentHighlightKind 
    {
        Text = 1,
        Read = 2,
        Write = 3
    }

    public class DocumentHighlight 
    {
	    public Range Range { get; set; }

        public DocumentHighlightKind Kind { get; set; }
    }

    public class DocumentHighlightRequest
    {
        public static readonly
            RequestType<TextDocumentPosition, DocumentHighlight[], object> Type =
            RequestType<TextDocumentPosition, DocumentHighlight[], object>.Create("textDocument/documentHighlight");
    }
}
