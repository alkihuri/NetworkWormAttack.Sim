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
    }
}
