using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Button actionButton;
    private Text buttonText;

    private Text logText;
    private ScrollRect logScrollRect;
    private const int MaxLines = 50;
    private readonly System.Collections.Generic.Queue<string> logLines =
        new System.Collections.Generic.Queue<string>();

    void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void Initialize(Button button, Text text)
    {
        actionButton = button;
        buttonText = text;
        actionButton.onClick.AddListener(OnButtonClick);
        SetButtonInteractable(false);
    }

    public void InitializeLog(Text text, ScrollRect scrollRect)
    {
        logText = text;
        logScrollRect = scrollRect;
    }

    public void AppendLog(string message)
    {
        if (logLines.Count >= MaxLines)
            logLines.Dequeue();

        logLines.Enqueue(message);

        if (logText != null)
            logText.text = string.Join("\n", logLines);

        ScrollToBottom();
    }

    public void ClearLog()
    {
        logLines.Clear();
        if (logText != null)
            logText.text = "";
    }

    private void ScrollToBottom()
    {
        if (logScrollRect == null) return;
        Canvas.ForceUpdateCanvases();
        logScrollRect.verticalNormalizedPosition = 0f;
    }

    private void OnButtonClick()
    {
        var networkManager = ServiceLocator.Get<NetworkManager>();
        networkManager.OnButtonClicked();
    }

    public void SetButtonText(string text)
    {
        if (buttonText != null)
            buttonText.text = text;
    }

    public void SetButtonInteractable(bool interactable)
    {
        if (actionButton != null)
            actionButton.interactable = interactable;
    }
}
