using System.Collections;
using UnityEngine;

// Menangani spawn pemain saat Ulangi ditekan.
// Mendukung dua titik spawn berbeda: stage 1 (depan meja 1) dan stage 2 (depan meja 2).
// Spawn awal dari Briefing Room dibiarkan apa adanya.
public class PlayerSpawn : MonoBehaviour
{
    // static -> bertahan saat reload scene
    public static bool modeUlangi = false;
    public static int spawnStage = 1;   // 1 = depan meja stage 1, 2 = depan meja stage 2

    [Header("Referensi")]
    [Tooltip("Drag XR Origin (XR Rig) ke sini")]
    public Transform xrOrigin;

    [Tooltip("Titik spawn Ulangi stage 1 (depan meja praktikum 1)")]
    public Transform spawnDepanMeja1;

    [Tooltip("Titik spawn Ulangi stage 2 (depan meja titrasi)")]
    public Transform spawnDepanMeja2;

    void Start()
    {
        if (!modeUlangi) return;  // bukan mode ulangi -> biarkan spawn normal
        StartCoroutine(PindahkanPemain());
    }

    private IEnumerator PindahkanPemain()
    {
        yield return null;
        yield return null;
        yield return new WaitForEndOfFrame();

        // Pilih titik spawn sesuai stage
        Transform target = (spawnStage == 2) ? spawnDepanMeja2 : spawnDepanMeja1;

        if (xrOrigin == null || target == null)
        {
            Debug.LogWarning("PlayerSpawn: xrOrigin atau titik spawn stage " + spawnStage + " belum di-assign!");
            modeUlangi = false;
            yield break;
        }

        // Kompensasi offset kamera headset VR
        Camera xrCamera = xrOrigin.GetComponentInChildren<Camera>();
        if (xrCamera != null)
        {
            Vector3 cameraOffset = xrCamera.transform.position - xrOrigin.position;
            cameraOffset.y = 0f;
            Vector3 targetOriginPos = target.position - cameraOffset;
            targetOriginPos.y = target.position.y;
            xrOrigin.position = targetOriginPos;
            xrOrigin.rotation = target.rotation;
        }
        else
        {
            xrOrigin.SetPositionAndRotation(target.position, target.rotation);
        }

        Debug.Log("PlayerSpawn: pemain di-spawn di depan meja stage " + spawnStage);
        modeUlangi = false;
    }
}