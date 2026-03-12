using UnityEngine;
using UnityEngine.UI;

public class GameBootstrapper : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        var bootstrapper = new GameObject("[GameBootstrapper]");
        bootstrapper.AddComponent<GameBootstrapper>();
        Object.DontDestroyOnLoad(bootstrapper);
    }

    void Awake()
    {
        SetupManagers();
        SetupCamera();
        SetupUI();
    }

    private void SetupManagers()
    {
        gameObject.AddComponent<NetworkManager>();
        gameObject.AddComponent<NodeManager>();
        gameObject.AddComponent<ScanManager>();
        gameObject.AddComponent<ExploitManager>();
        gameObject.AddComponent<PropagationManager>();
        gameObject.AddComponent<UIManager>();
    }

    private void SetupCamera()
    {
        var mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 10f;
            mainCamera.transform.position = new Vector3(0, -5f, -10f);
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        }
    }

    private void SetupUI()
    {
        // Create Canvas
        var canvasObj = new GameObject("UICanvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create Button
        var buttonObj = new GameObject("ActionButton");
        buttonObj.transform.SetParent(canvasObj.transform, false);

        var buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 0.9f);

        var button = buttonObj.AddComponent<Button>();

        var rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0f);
        rectTransform.anchoredPosition = new Vector2(0, 20);
        rectTransform.sizeDelta = new Vector2(300, 60);

        // Create Button Text
        var textObj = new GameObject("ButtonText");
        textObj.transform.SetParent(buttonObj.transform, false);

        var text = textObj.AddComponent<Text>();
        text.text = "SCAN";
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (font != null)
            text.font = font;
        text.fontSize = 24;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Initialize UIManager
        var uiManager = ServiceLocator.Get<UIManager>();
        uiManager.Initialize(button, text);

        // ── Log Panel ─────────────────────────────────────────────────
        // Background image
        var logPanelObj = new GameObject("LogPanel");
        logPanelObj.transform.SetParent(canvasObj.transform, false);

        var logImage = logPanelObj.AddComponent<Image>();
        logImage.color = new Color(0f, 0f, 0f, 0.6f);

        var logPanelRect = logPanelObj.GetComponent<RectTransform>();
        logPanelRect.anchorMin = new Vector2(0f, 0f);
        logPanelRect.anchorMax = new Vector2(0f, 0f);
        logPanelRect.pivot    = new Vector2(0f, 0f);
        logPanelRect.anchoredPosition = new Vector2(20f, 20f);
        logPanelRect.sizeDelta = new Vector2(360f, 200f);

        // ScrollRect
        var scrollObj = new GameObject("Scroll");
        scrollObj.transform.SetParent(logPanelObj.transform, false);

        var scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical   = true;
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        var scrollRectTransform = scrollObj.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = Vector2.zero;
        scrollRectTransform.anchorMax = Vector2.one;
        scrollRectTransform.offsetMin = new Vector2(6f,  6f);
        scrollRectTransform.offsetMax = new Vector2(-6f, -6f);

        // Content (Text container)
        var contentObj = new GameObject("Content");
        contentObj.transform.SetParent(scrollObj.transform, false);

        var contentSizeFitter = contentObj.AddComponent<ContentSizeFitter>();
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var contentRect = contentObj.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot     = new Vector2(0f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = Vector2.zero;

        // Log Text
        var logTextObj = new GameObject("LogText");
        logTextObj.transform.SetParent(contentObj.transform, false);

        var logText = logTextObj.AddComponent<Text>();
        logText.font      = font ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        logText.fontSize  = 14;
        logText.color     = new Color(0.85f, 1f, 0.85f);
        logText.alignment = TextAnchor.UpperLeft;
        logText.text      = "";
        logText.supportRichText = true;
        logText.verticalOverflow   = VerticalWrapMode.Overflow;
        logText.horizontalOverflow = HorizontalWrapMode.Wrap;

        var logTextRect = logTextObj.GetComponent<RectTransform>();
        logTextRect.anchorMin = Vector2.zero;
        logTextRect.anchorMax = Vector2.one;
        logTextRect.offsetMin = Vector2.zero;
        logTextRect.offsetMax = Vector2.zero;

        scrollRect.content = contentRect;

        uiManager.InitializeLog(logText);
    }
}
