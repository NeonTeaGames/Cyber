using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber.Console {
    
    /// <summary>
    /// A class that has static functions for printing text in the DebugConsole.
    /// </summary>
    public class Term {
        
        private static DebugConsole Console;

        /// <summary>
        /// Sets the <see cref="DebugConsole"/> singleton that will be used in other static <see cref="Term"/> functions.
        /// </summary>
        /// <param name="console">Console.</param>
        public static void SetDebugConsole(DebugConsole console) {
            Console = console;
        }

        /// <summary>
        /// Returns whether or not the DebugConsole is currently on the screen, ready to be used.
        /// </summary>
        /// <returns><c>true</c> if is visible; otherwise, <c>false</c>.</returns>
        public static bool IsVisible() {
            if (Console == null) {
                return false;
            } else {
                return Console.Visible;
            }
        }

        /// <summary>
        /// See <see cref="DebugConsole.Println"/>.
        /// </summary>
        /// <param name="text">Text.</param>
        public static void Println(string text) {
            if (Console == null) {
                Debug.Log(text);
            } else {
                Console.Println(text);
            }
        }

        /// <summary>
        /// See <see cref="DebugConsole.Print"/>.
        /// </summary>
        /// <param name="text">Text.</param>
        public static void Print(string text) {
            if (Console == null) {
                Debug.Log(text);
            } else {
                Console.Print(text);
            }
        }

        /// <summary>
        /// See <see cref="DebugConsole.AddCommand"/>
        /// </summary>
        /// <param name="command">Command.</param>
        /// <param name="description">Description.</param>
        /// <param name="action">Action.</param>
        public static void AddCommand(string command, string description, DebugConsoleAction.Action action) {
            if (Console != null) {
                Console.AddCommand(command, description, action);
            }
        }
    }
}
