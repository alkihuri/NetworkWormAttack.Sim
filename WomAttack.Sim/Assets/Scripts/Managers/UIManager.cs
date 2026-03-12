using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Button actionButton;
    private Text buttonText;

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

    private void OnButtonClick()
    {
        var networkManager = ServiceLocator.Get<NetworkManager>();
        networkManager.OnButtonClicked();
    }

    public void SetButtonText(string text)
    {
        if (buttonText != null)
        {
            buttonText.text = text;
        }
    }

    public void SetButtonInteractable(bool interactable)
    {
        if (actionButton != null)
        {
            actionButton.interactable = interactable;
        }
    }
}
