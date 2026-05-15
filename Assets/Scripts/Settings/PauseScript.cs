using UnityEngine;

public class PauseScript : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;

    public void OpenPausePanel()
    {
        pausePanel.SetActive(true);
    }

    public void ClosePausePanel()
    {
        pausePanel.SetActive(false);
    }

    public void ExitGame()
    {
        MessagePrebafScript.Show(
            "Are you sure you want to exit the game?",
            QuitApplication);
    }

    static void QuitApplication()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
