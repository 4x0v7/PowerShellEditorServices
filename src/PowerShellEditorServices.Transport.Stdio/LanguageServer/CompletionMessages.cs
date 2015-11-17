﻿using Microsoft.PowerShell.EditorServices.Protocol.MessageProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.EditorServices.Protocol.LanguageServer
{
    public class CompletionRequest
    {
        public static readonly
            RequestType<TextDocumentPosition, CompletionItem[], object> Type =
            RequestType<TextDocumentPosition, CompletionItem[], object>.Create("textDocument/completion");
    }

    public class CompletionResolveRequest
    {
        public static readonly
            RequestType<CompletionItem, CompletionItem, object> Type =
            RequestType<CompletionItem, CompletionItem, object>.Create("completionItem/resolve");
    }

    public enum CompletionItemKind
    {
        	Text = 1,
            Method = 2,
            Function = 3,
            Constructor = 4,
            Field = 5,
            Variable = 6,
            Class = 7,
            Interface = 8,
            Module = 9,
            Property = 10,
            Unit = 11,
            Value = 12,
            Enum = 13,
            Keyword = 14,
            Snippet = 15,
            Color = 16,
            File = 17,
            Reference = 18
    }

    public class TextEdit 
    {
        public Range Range { get; set; }

        public string NewText { get; set; }
    }

    public class CompletionItem
    {
        public string Label { get; set; }

        public CompletionItemKind? Kind { get; set; }

        public string Detail { get; set; }

        /// <summary>
        /// Gets or sets the documentation string for the completion item.
        /// </summary>
        public string Documentation { get; set; }

        public string SortText { get; set; }

        public string FilterText { get; set; }

        public string InsertText { get; set; }

        public TextEdit TextEdit { get; set; }
    }
}
