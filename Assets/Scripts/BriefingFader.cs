using UnityEngine;

public sealed class BriefingFader : MonoBehaviour
{
    [Header("Settings")]
    public Transform playerTransform; // Tarik 'Main Camera' dari XR Origin ke sini
    public CanvasGroup canvasGroup;
    public float fadeStartDistance = 2.0f; // Jarak mulai memudar
    public float fadeEndDistance = 0.5f;   // Jarak hingga benar-benar hilang

    void Update()
    {
        if (playerTransform == null || canvasGroup == null) return;

        // Hitung jarak antara pemain dan layar briefing
        float distance = Vector3.Distance(playerTransform.position, transform.position);

        // Map jarak ke nilai Alpha (0 hingga 1)
        if (distance <= fadeEndDistance) {
            canvasGroup.alpha = 0; // Hilang total
        }
        else if (distance >= fadeStartDistance) {
            canvasGroup.alpha = 1; // Terlihat penuh
        }
        else {
            // Interpolasi halus
            float range = fadeStartDistance - fadeEndDistance;
            float progress = (distance - fadeEndDistance) / range;
            canvasGroup.alpha = Mathf.Clamp01(progress);
        }
    }
}