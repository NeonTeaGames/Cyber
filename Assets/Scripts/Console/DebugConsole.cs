using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour {
    private static readonly Regex CommandPartRegex = new Regex("([^ \"]+ )|(\"[^\"]+\")");

    public GameObject Panel;
    public InputField InputField;
    public Text TextField;
    public bool Visible = false;

    private Dictionary<string, DebugConsoleAction> Actions = new Dictionary<string, DebugConsoleAction>();

    public void CallCommand() {
        if (InputField.text.Length == 0) {
            return;
        }
        Println(InputField.text);
        List<string> Arguments = new List<string>();
        MatchCollection Matches = CommandPartRegex.Matches(InputField.text + " ");
        for (int i = 0; i < Matches.Count; i++) {
            Arguments.Add(Matches[i].Value.Replace('"', ' ').Trim());
        }
        foreach (string Action in Actions.Keys) {
            string[] Parts = Action.Split(' ');
            if (Arguments.Count == Parts.Length && Arguments[0].Equals(Parts[0])) {
                Arguments.RemoveAt(0);
                Actions[Action].Call(Arguments);
                break;
            }
        }
        InputField.text = "";
        InputField.ActivateInputField();
    }

    /// <summary>
    /// Adds a command to be used in the console.
    /// </summary>
    /// <param name="command">The command template that should be used.
    /// eg. "print (text)" or "add (number) (number)"</param>
    /// <param name="description">Description.</param>
    /// <param name="action">Action.</param>
    public void AddCommand(string command, string description, DebugConsoleAction.Action action) {
        string PrettyDescription = command;
        foreach (string Line in description.Split('\n')) {
            PrettyDescription += "\n  " + Line;
        }
        Actions[command] = new DebugConsoleAction(PrettyDescription, action);
    }

    public void Println(string text) {
        Print(text + "\n");
    }

    // TODO: Handle removing history when it gets very long. Very long console logs might cause problems when displaying new prints.
    public void Print(string text) {
        TextField.text += text;
    }

    private void Start() {
        AddCommand("help", "Lists all commands.", (args) => {
            Println("Commands:");
            foreach (string Action in Actions.Keys) {
                Println("- " + Action);
            }
        });

        AddCommand("help (command)", "Describes the given command.", (args) => {
            // Check complete versions of the names (so you can do eg. help "help (command)")
            foreach (string Action in Actions.Keys) {
                if (Action.Equals(args[0])) {
                    Println(Actions[Action].Description);
                    return;
                }
            }
            // Check just names
            foreach (string Action in Actions.Keys) {
                string[] Parts = Action.Split(' ');
                if (Parts[0].Equals(args[0])) {
                    Println(Actions[Action].Description);
                    return;
                }
            }
            Println("That command doesn't exist.");
        });

        AddCommand("print (text)", "Prints the given text.", (args) => {
            Println(args[0]);
        });

        Term.SetDebugConsole(this);
    }

    private void Update() {
        if (Input.GetButtonDown("Console Toggle")) {
            Visible = !Visible;
        }
        RectTransform Rect = Panel.GetComponent<RectTransform>();
        Vector2 OffsetMin = Rect.offsetMin;
        if (Visible) {
            if (OffsetMin.y > 1) {
                OffsetMin.y = Mathf.Lerp(OffsetMin.y, 0, 5f * Time.deltaTime);
            }
            if (!InputField.isFocused) {
                InputField.ActivateInputField();
            }
        } else {
            if (1000 - OffsetMin.y > 1) {
                OffsetMin.y = Mathf.Lerp(OffsetMin.y, 1000, 1f * Time.deltaTime);
            }
            if (InputField.isFocused) {
                InputField.DeactivateInputField();
            }
        }
        Rect.offsetMin = OffsetMin;
    }
}
