using UnityEngine;

/// <summary>
/// Static helper — call SimLog.Write("message") from anywhere to show simulation status in the UI log panel.
/// </summary>
public static class SimLog
{
    public static void Write(string message)
    {
        var ui = ServiceLocator.Get<UIManager>();
        if (ui != null)
            ui.AppendLog(message);
        else
            Debug.Log($"[SimLog] {message}");
    }

    public static void Clear()
    {
        var ui = ServiceLocator.Get<UIManager>();
        ui?.ClearLog();
    }
}

