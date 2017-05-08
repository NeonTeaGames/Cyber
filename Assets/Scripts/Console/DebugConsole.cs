using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls an input and an output to implement a console interface to call 
/// arbitrary commands which can be set from anywhere in the program.
/// <seealso cref="Term"/>
/// </summary>
public class DebugConsole : MonoBehaviour {
    private static readonly Regex CommandPartRegex = new Regex("([^ \"]+ )|(\"[^\"]+\")");

    /// <summary>
    /// The parent of the <see cref="InputField"/> and <see cref="TextField"/>.
    /// </summary>
    public GameObject Panel;
    /// <summary>
    /// The input for the console.
    /// </summary>
    public InputField InputField;
    /// <summary>
    /// The output for the console.
    /// </summary>
    public Text TextField;
    /// <summary>
    /// Controls the visibility of the console. For how this is controlled in-game, see <see cref="Update"/>.
    /// </summary>
    public bool Visible = false;

    private Dictionary<string, DebugConsoleAction> Actions = new Dictionary<string, DebugConsoleAction>();

    /// <summary>
    /// Creates a new <see cref="DebugConsole"/>, and sets the <see cref="Term"/>'s singleton.
    /// <seealso cref="Term.SetDebugConsole"/>
    /// </summary>
    public DebugConsole() {
        Term.SetDebugConsole(this);
    }

    /// <summary>
    /// Tries to call the command in the <see cref="InputField"/>. 
    /// </summary>
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

    /// <summary>
    /// Prints text into the DebugConsole and adds a newline.
    /// </summary>
    /// <param name="text">Text.</param>
    public void Println(string text) {
        Print(text + "\n");
    }

    // TODO: Handle removing history when it gets very long. Very long console logs might cause problems when displaying new prints.
    /// <summary>
    /// Prints text into the Console.
    /// </summary>
    /// <param name="text">Text.</param>
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
