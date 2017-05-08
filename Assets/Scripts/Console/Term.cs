using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that has static functions for printing text in the DebugConsole.
/// </summary>
public class Term {
    private static DebugConsole Console;

    public static void SetDebugConsole(DebugConsole console) {
        Console = console;
    }

    public static bool IsVisible() {
        if (Console == null) {
            return false;
        } else {
            return Console.Visible;
        }
    }

    public static void Println(string text) {
        if (Console == null) {
            Debug.Log(text);
        } else {
            Console.Println(text);
        }
    }

    public static void Print(string text) {
        if (Console == null) {
            Debug.Log(text);
        } else {
            Console.Print(text);
        }
    }

    public static void AddCommand(string command, string description, DebugConsoleAction.Action action) {
        if (Console != null) {
            Console.AddCommand(command, description, action);
        }
    }
}
