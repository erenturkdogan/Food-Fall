using UnityEngine;

/// <summary>
/// PausePanel içindeki butonlardan çağrılır.
/// </summary>
public class PauseMenuButtons : MonoBehaviour
{
    public void OnResume()
    {
        if (PauseManager.Instance != null) PauseManager.Instance.Resume();
    }

    public void OnRestart()
    {
        if (PauseManager.Instance != null) PauseManager.Instance.Restart();
    }

    public void OnMainMenu()
    {
        if (GameManager.Instance != null) GameManager.Instance.GoToMainMenu();
    }
}
