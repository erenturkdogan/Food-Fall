using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Eklendiği butona otomatik tıklama sesi ekler.
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonClick();
        });
    }
}
