using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger {  // for logcat debugging
    private static string DEBUG_PREFIX = "HAS# ";

    public static event System.Action<string> OnLog;
    public static event System.Action<string> OnErrorLog;

    public static void Log(string message) {
        OnLog?.Invoke(DEBUG_PREFIX + message);
    }

    public static void LogError(string message) {
        OnErrorLog?.Invoke(DEBUG_PREFIX + message);
    }
}
