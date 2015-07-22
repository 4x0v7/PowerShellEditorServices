﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.PowerShell.EditorServices.Session;
using System.Management.Automation.Language;

namespace Microsoft.PowerShell.EditorServices.Language
{
    /// <summary>
    /// A way to define symbols on a higher level
    /// </summary>
    public enum SymbolType
    {
        /// <summary>
        /// The symbol type is unknown
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// The symbol is a vairable
        /// </summary>
        Variable,
        
        /// <summary>
        /// The symbol is a function
        /// </summary>
        Function,
        
        /// <summary>
        /// The symbol is a parameter
        /// </summary>
        Parameter
    }

    /// <summary>
    /// A class that holds the type, name, script extent, and source line of a symbol
    /// </summary>
    public class SymbolReference
    {
        #region Properties
        /// <summary>
        /// Gets the symbol's type
        /// </summary>
        public SymbolType SymbolType { get; private set; }

        /// <summary>
        /// Gets the name of the symbol
        /// </summary>
        public string SymbolName { get; private set; }

        /// <summary>
        /// Gets the script extent of the symbol
        /// </summary>
        public ScriptRegion ScriptRegion { get; private set; }

        /// <summary>
        /// Gets the contents of the line the given symbol is on
        /// </summary>
        public string SourceLine { get; internal set; }
        #endregion

        /// <summary>
        /// Constructs and instance of a SymbolReference
        /// </summary>
        /// <param name="symbolType">The higher level type of the symbol</param>
        /// <param name="scriptExtent">The script extent of the symbol</param>
        /// <param name="sourceLine">The line contents of the given symbol (defaults to empty string)</param>
        public SymbolReference(SymbolType symbolType, IScriptExtent scriptExtent, string sourceLine = "")
        {
            // TODO: Verify params
            this.SymbolType = symbolType;
            this.SymbolName = scriptExtent.Text;
            this.ScriptRegion = ScriptRegion.Create(scriptExtent);
            this.SourceLine = sourceLine;

            // TODO: Make sure end column number usage is correct
        }
    }
}
