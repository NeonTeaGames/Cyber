using System.Collections.Generic;

public class DebugConsoleAction {
    public delegate void Action(List<string> command);
    public readonly string Description;

    private Action ActionFunc;

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
