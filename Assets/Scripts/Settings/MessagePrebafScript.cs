using System;
using TMPro;
using UnityEngine;

public class MessagePrebafScript : MonoBehaviour
{
    static MessagePrebafScript _instance;

    Action _onYes;
    Action _onNo;

    [SerializeField] private TextMeshProUGUI messageText;

    void Awake() => _instance = this;

    void OnEnable() => _instance = this;

    /// <summary>Показать окно. onYes вызывается после нажатия «Да», onNo — после «Нет» (необязательно).</summary>
    public static void Show(string text, Action onYes, Action onNo = null)
    {
        if (_instance == null)
        {
            Debug.LogError("MessagePrebafScript: на сцене нет объекта с этим скриптом.");
            return;
        }

        _instance.ShowInternal(text, onYes, onNo);
    }

    void ShowInternal(string text, Action onYes, Action onNo)
    {
        _onYes = onYes;
        _onNo = onNo;

        if (messageText != null)
            messageText.text = text;

        gameObject.SetActive(true);
    }

    public void YesButton()
    {
        Action callback = _onYes;
        ClearCallbacks();
        gameObject.SetActive(false);
        callback?.Invoke();
    }

    public void NoButton()
    {
        Action callback = _onNo;
        ClearCallbacks();
        gameObject.SetActive(false);
        callback?.Invoke();
    }

    void ClearCallbacks()
    {
        _onYes = null;
        _onNo = null;
    }
}
