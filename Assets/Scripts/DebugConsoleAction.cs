using System.Collections.Generic;

public class DebugConsoleAction {
    public delegate void Action(List<string> args);

    private string Description;
    private Action ActionFunc;

    public DebugConsoleAction(string description, Action actionFunc) {
        this.Description = description;
        this.ActionFunc = actionFunc;
    }

    public void Call(List<string> args) {
        ActionFunc(args);
    }
}
