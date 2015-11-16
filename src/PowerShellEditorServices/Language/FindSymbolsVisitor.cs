﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System.Collections.Generic;
using System.Management.Automation.Language;

namespace Microsoft.PowerShell.EditorServices
{
    /// <summary>
    /// The visitor used to find all the symbols (function and class defs) in the AST.
    /// </summary>
    internal class FindSymbolsVisitor : AstVisitor2
    {
        public List<SymbolReference> SymbolReferences { get; private set; }

        public FindSymbolsVisitor()
        {
            this.SymbolReferences = new List<SymbolReference>();
        }

        /// <summary>
        /// Adds each function defintion as a 
        /// </summary>
        /// <param name="functionDefinitionAst">A functionDefinitionAst object in the script's AST</param>
        /// <returns>A decision to stop searching if the right symbol was found, 
        /// or a decision to continue if it wasn't found</returns>
        public override AstVisitAction VisitFunctionDefinition(FunctionDefinitionAst functionDefinitionAst)
        {
            IScriptExtent nameExtent = new ScriptExtent() {
                Text = functionDefinitionAst.Name,
                StartLineNumber = functionDefinitionAst.Extent.StartLineNumber,
                EndLineNumber = functionDefinitionAst.Extent.EndLineNumber,
                StartColumnNumber = functionDefinitionAst.Extent.StartColumnNumber,
                EndColumnNumber = functionDefinitionAst.Extent.EndColumnNumber
            };

            SymbolType symbolType = 
                functionDefinitionAst.IsWorkflow ? 
                    SymbolType.Workflow : SymbolType.Function;

            this.SymbolReferences.Add(
                new SymbolReference(
                    symbolType,
                    nameExtent));

            return AstVisitAction.Continue;
        }

        /// <summary>
        ///  Checks to see if this variable expression is the symbol we are looking for.
        /// </summary>
        /// <param name="variableExpressionAst">A VariableExpressionAst object in the script's AST</param>
        /// <returns>A descion to stop searching if the right symbol was found, 
        /// or a decision to continue if it wasn't found</returns>
        public override AstVisitAction VisitVariableExpression(VariableExpressionAst variableExpressionAst)
        {
            if (!IsAssignedAtScriptScope(variableExpressionAst))
            {
                return AstVisitAction.Continue;
            }

            this.SymbolReferences.Add(
                new SymbolReference(
                    SymbolType.Variable,
                    variableExpressionAst.Extent));

            return AstVisitAction.Continue;
        }

        public override AstVisitAction VisitConfigurationDefinition(ConfigurationDefinitionAst configurationDefinitionAst)
        {
            IScriptExtent nameExtent = new ScriptExtent() {
                Text = configurationDefinitionAst.InstanceName.Extent.Text,
                StartLineNumber = configurationDefinitionAst.Extent.StartLineNumber,
                EndLineNumber = configurationDefinitionAst.Extent.EndLineNumber,
                StartColumnNumber = configurationDefinitionAst.Extent.StartColumnNumber,
                EndColumnNumber = configurationDefinitionAst.Extent.EndColumnNumber
            };

            this.SymbolReferences.Add(
                new SymbolReference(
                    SymbolType.Configuration,
                    nameExtent));

            return AstVisitAction.Continue;
        }

        private bool IsAssignedAtScriptScope(VariableExpressionAst variableExpressionAst)
        {
            Ast parent = variableExpressionAst.Parent;
            if (!(parent is AssignmentStatementAst))
            {
                return false;
            }

            parent = parent.Parent;
            if (parent == null || parent.Parent == null || parent.Parent.Parent == null)
            {
                return true;
            }

            return false;
        }
    }
}
