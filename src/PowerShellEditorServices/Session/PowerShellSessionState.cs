﻿
namespace Microsoft.PowerShell.EditorServices
{
    /// <summary>
    /// Enumerates the possible states for a PowerShellSession.
    /// </summary>
    public enum PowerShellSessionState
    {
        /// <summary>
        /// Indicates an unknown, potentially uninitialized state.
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// Indicates the state where the session is starting but 
        /// not yet fully initialized.
        /// </summary>
        NotStarted,

        /// <summary>
        /// Indicates that the session is ready to accept commands
        /// for execution.
        /// </summary>
        Ready,
        
        /// <summary>
        /// Indicates that the session is currently running a command.
        /// </summary>
        Running,

        /// <summary>
        /// Indicates that the session is aborting the current execution.
        /// </summary>
        Aborting,

        /// <summary>
        /// Indicates that the session is already disposed and cannot
        /// accept further execution requests.
        /// </summary>
        Disposed
    }
}
