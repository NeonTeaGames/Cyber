using System.Collections.Generic;

/// <summary>
/// Class that defines an action that commands in the
/// <see cref="DebugConsole"/> might use.
/// </summary>
public class DebugConsoleAction {
    /// <summary>
    /// A delegate for all of the actions that commands do.
    /// </summary>
    public delegate void Action(List<string> command);
    /// <summary>
    /// A description that will be shown when using the "help (command)" command in the console.
    /// </summary>
    public readonly string Description;

    private Action ActionFunc;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugConsoleAction"/> class.
    /// </summary>
    /// <param name="description">Description.</param>
    /// <param name="actionFunc">Action func.</param>
    public DebugConsoleAction(string description, Action actionFunc) {
        this.Description = description;
        this.ActionFunc = actionFunc;
    }

    /// <summary>
    /// Executes the <see cref="ActionFunc"/>.
    /// </summary>
    /// <param name="command">Command.</param>
    public void Call(List<string> command) {
        ActionFunc(command);
    }
}
