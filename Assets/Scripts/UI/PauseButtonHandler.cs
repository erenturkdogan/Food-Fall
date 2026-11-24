using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// PauseButton'a eklenir. OnClick yerine kendisi singleton'ı çağırır.
/// Sahne içi referans bozulmasından etkilenmez.
/// </summary>
[RequireComponent(typeof(Button))]
public class PauseButtonHandler : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (PauseManager.Instance != null)
                PauseManager.Instance.TogglePause();
            else
                Debug.LogError("PauseManager.Instance null!");
        });
    }
}
