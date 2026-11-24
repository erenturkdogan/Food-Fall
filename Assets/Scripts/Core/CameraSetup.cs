using UnityEngine;

/// <summary>
/// Oyun kamerasını her cihazda tutarlı göstermek için
/// hedef aspect ratio'ya göre orthographic size ayarlar.
/// Main Camera'ya eklenir.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraSetup : MonoBehaviour
{
    [Tooltip("Tasarım hedef en-boy oranı (9/16 portrait)")]
    public float targetAspect = 9f / 16f;

    void Awake()
    {
        Camera cam = GetComponent<Camera>();
        float currentAspect = (float)Screen.width / Screen.height;

        // Hedeften daha geniş ekranlarda (tablet vs.) letterbox
        if (currentAspect > targetAspect)
        {
            float scaleHeight = currentAspect / targetAspect;
            cam.orthographicSize *= scaleHeight;
        }
        // Daha dar ekranlarda size küçültülür — içerik sığar
    }
}
