﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace Microsoft.PowerShell.EditorServices.Extensions
{
    /// <summary>
    /// Provides a PowerShell-facing API which allows scripts to
    /// interact with the editor's window.
    /// </summary>
    public class EditorWindow
    {
        #region Private Fields

        private IEditorOperations editorOperations;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the EditorWindow class.
        /// </summary>
        /// <param name="editorOperations">An IEditorOperations implementation which handles operations in the host editor.</param>
        internal EditorWindow(IEditorOperations editorOperations)
        {
            this.editorOperations = editorOperations;
        }

        #endregion 

        #region Public Methods

        /// <summary>
        /// Shows an informational message to the user.
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        public void ShowInformationMessage(string message)
        {
            this.editorOperations.ShowInformationMessage(message).Wait();
        }

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        public void ShowErrorMessage(string message)
        {
            this.editorOperations.ShowErrorMessage(message).Wait();
        }

        /// <summary>
        /// Shows a warning message to the user.
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        public void ShowWarningMessage(string message)
        {
            this.editorOperations.ShowWarningMessage(message).Wait();
        }

        /// <summary>
        /// Sets the status bar message in the editor UI (if applicable).
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        public void SetStatusBarMessage(string message)
        {
            this.editorOperations.SetStatusBarMessage(message).Wait();
        }

        #endregion 
    }
}
