using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugConsole : MonoBehaviour {
    private Dictionary<string, DebugConsoleAction> Actions = new Dictionary<string, DebugConsoleAction>();

    /// <summary>
    /// Adds a command to be used in the console.
    /// </summary>
    /// <param name="command">The command template that should be used.
    /// eg. "print (text)" or "add (number) (number)"</param>
    /// <param name="description">Description.</param>
    /// <param name="action">Action.</param>
    public void AddCommand(string command, string description, DebugConsoleAction.Action action) {
        Actions[command] = new DebugConsoleAction(description, action);
    }
}
