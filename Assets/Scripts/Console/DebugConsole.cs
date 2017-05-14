using Cyber.Networking.Clientside;
using Cyber.Networking.Serverside;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Cyber.Util;

namespace Cyber.Console {
    
    /// <summary>
    /// Controls an input and an output to implement a console interface to call 
    /// arbitrary commands which can be set from anywhere in the program through
    /// <see cref="Term"/>.
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

        /// <summary>
        /// The length of a row in the console in characters.
        /// </summary>
        public int RowLength = 80;

        /// <summary>
        /// How many lines are included in the <see cref="TextField"/> when 
        /// the total amount of lines exceeds this.
        /// </summary>
        public int LinesRendered = 15;

        private Dictionary<string, DebugConsoleAction> Actions = new Dictionary<string, DebugConsoleAction>();
        private List<string> Lines = new List<string>();
        private List<string> Commands = new List<string>();
        private int LastCommandIndex = 0;
        private string LastUnexecutedCommand = "";
        private int ScrollOffset = 0;

        /// <summary>
        /// Creates a new <see cref="DebugConsole"/>, and sets the <see cref="Term"/>'s singleton.
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
            // Log this command
            Println(InputField.text);
            Commands.Add(InputField.text);
            LastCommandIndex = Commands.Count;

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

            // Clear the input field
            InputField.text = "";
            InputField.ActivateInputField();
        }

        /// <summary>
        /// Adds a command to be used in the console.
        /// </summary>
        /// <param name="command">The command template that should be used.
        /// eg. "print (text)" or "add (number) (number)".</param>
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

        /// <summary>
        /// Prints text into the Console. Wraps text at <see cref="RowLength"/>.
        /// </summary>
        /// <remarks>
        /// If the linecount exceeds <see cref="MaxLinesUntilCleanup"/>, the
        /// console is cleared to the point that it only has
        /// <see cref="LineCountSavedFromCleanup"/> lines.
        /// </remarks>
        /// <param name="text">Text.</param>
        public void Print(string text) {
            // Wrap lines and log them to Lines
            int Index = 0;
            int EscapeIndex = 0;
            do {
                int NewLineIndex = text.IndexOf("\n", Index) + 1;
                if (NewLineIndex == 0 || NewLineIndex > Index + RowLength) {
                    NewLineIndex = Index + Mathf.Min(text.Length - Index, RowLength);
                }
                string Line = text.Substring(Index, NewLineIndex - Index).Replace("\n", "");
                Index = NewLineIndex;
                Lines.Add(Line);
                TextField.text += Line + "\n";
                EscapeIndex++;
            } while (Index < text.Length && EscapeIndex < 10);
            UpdateTextField();
        }

        private void UpdateTextField() {
            string NewLog = "";
            int LineAmt = Mathf.Min(LinesRendered, Lines.Count);
            for (int i = LineAmt; i > 0; i--) {
                int Index = Lines.Count - (i + ScrollOffset);
                if (Index >= 0 && Index < Lines.Count) {
                    NewLog += Lines[Index] + "\n";
                }
            }
            TextField.text = NewLog;
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

            AddCommand("shutdown", "Shuts the game down.", (args) => {
                if (Client.IsRunning()) {
                    Client.Shutdown();
                }
                if (Server.IsRunning()) {
                    Server.Shutdown();
                }
                Application.Quit();
            });

            Println("Use the \"help\" command for a list of commands.");

            // Set an accurate row length (if the panel is set)
            if (Panel != null && Panel.GetComponent<RectTransform>() != null) {
                float CharacterWidth = FontUtil.GetCharacterWidth(TextField.font, 
                    TextField.fontSize, TextField.fontStyle);
                float PanelWidth = Panel.GetComponent<RectTransform>().rect.width;
                RowLength = (int) (PanelWidth / CharacterWidth);
            }
        }

        private void Update() {
            // Inputs
            if (Input.GetButtonDown("Console Toggle")) {
                Visible = !Visible;
            }
            if (Input.GetButtonDown("Enter Command")) {
                CallCommand();
            }
            if (Input.GetButtonDown("Previous Command")) {
                if (LastCommandIndex - 1 >= 0) {
                    if (LastCommandIndex == Commands.Count) {
                        // The last command is the last one that was executed
                        // Save the currently written command so it can be returned to
                        LastUnexecutedCommand = InputField.text;
                    }
                    InputField.text = Commands[--LastCommandIndex];
                }
            }
            if (Input.GetButtonDown("Next Command")) {
                if (LastCommandIndex + 1 < Commands.Count) {
                    InputField.text = Commands[++LastCommandIndex];
                } else if (LastCommandIndex + 1 == Commands.Count) {
                    LastCommandIndex++;
                    InputField.text = LastUnexecutedCommand;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && ScrollOffset + 1 < Lines.Count) {
                ScrollOffset++;
                UpdateTextField();
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && ScrollOffset - 1 >= 0) {
                ScrollOffset--;
                UpdateTextField();
            }

            // Slide up/down animation
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
}
