using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class DebugLogButton : MonoBehaviour
{
    private List<LogEntry> logEntries = new List<LogEntry>();
    private bool showDebug = false;
    private Vector2 scrollPosition;
    private bool showLogs = true;
    private bool showWarnings = true;
    private bool showErrors = true;
    private bool showExceptions = true;
    public GUISkin customSkin;

    // Texture for the semi-transparent background
    private Texture2D backgroundTexture;

    private struct LogEntry
    {
        public string formattedMessage;
        public LogType type;
    }

    void Start()
    {
        // Create a 1x1 texture for the background
        backgroundTexture = new Texture2D(1, 1);
        // Set it to semi-transparent dark grey (adjustable)
        backgroundTexture.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.1f, 0.5f));
        backgroundTexture.Apply();
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string color = type switch
        {
            LogType.Error => "<color=red>",
            LogType.Warning => "<color=yellow>",
            LogType.Exception => "<color=green>",
            _ => "<color=white>" // LogType.Log and LogType.Assert
        };
        string formattedMessage = $"{color}[{type}] {logString}\n{stackTrace}</color>\n\n";
        logEntries.Add(new LogEntry { formattedMessage = formattedMessage, type = type });
    }

    void OnGUI()
    {
        if (customSkin != null) GUI.skin = customSkin;

        // Toggle button to show/hide the debug panel
        if (GUI.Button(new Rect(10, 10, 100, 30), "Show Debug"))
        {
            showDebug = !showDebug;
        }

        if (showDebug)
        {
            GUI.BeginGroup(new Rect(10, 50, Screen.width - 20, Screen.height - 60));

            // Clear button and filter toggles
            if (GUI.Button(new Rect(0, 0, 100, 30), "Clear"))
            {
                logEntries.Clear();
            }
            showLogs = GUI.Toggle(new Rect(110, 0, 80, 30), showLogs, "Logs");
            showWarnings = GUI.Toggle(new Rect(200, 0, 80, 30), showWarnings, "Warnings");
            showErrors = GUI.Toggle(new Rect(290, 0, 80, 30), showErrors, "Errors");
            showExceptions = GUI.Toggle(new Rect(380, 0, 80, 30), showExceptions, "Exceptions");

            // Build the filtered log messages
            StringBuilder sb = new StringBuilder();
            int totalLines = 0;
            foreach (var entry in logEntries)
            {
                if (ShouldDisplay(entry.type))
                {
                    sb.Append(entry.formattedMessage);
                    totalLines += entry.formattedMessage.Split('\n').Length;
                }
            }
            string logMessages = sb.ToString();
            float contentHeight = totalLines * 15;

            // Define the log view area
            Rect logViewRect = new Rect(0, 40, Screen.width - 20, Screen.height - 100);

            // Draw the semi-transparent dark grey background
            GUI.DrawTexture(logViewRect, backgroundTexture);

            // Scroll view for the logs
            scrollPosition = GUI.BeginScrollView(
                logViewRect,
                scrollPosition,
                new Rect(0, 0, Screen.width - 40, contentHeight)
            );

            // Display the logs with rich text for colors
            GUI.Label(
                new Rect(0, 0, Screen.width - 40, contentHeight),
                logMessages,
                new GUIStyle(GUI.skin.label) { richText = true }
            );

            GUI.EndScrollView();
            GUI.EndGroup();
        }
    }

    private bool ShouldDisplay(LogType type)
    {
        return (type == LogType.Log || type == LogType.Assert) && showLogs ||
               type == LogType.Warning && showWarnings ||
               type == LogType.Error && showErrors ||
               type == LogType.Exception && showExceptions;
    }
}