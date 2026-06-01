using System.Collections;
using UnityEngine;

// Hanya menangani kasus "Ulangi":
// - Spawn awal dari Briefing Room  -> DIBIARKAN apa adanya (sudah benar)
// - Tekan tombol "Ulangi"          -> pindahkan pemain ke DEPAN MEJA
public class PlayerSpawn : MonoBehaviour
{
    public static bool modeUlangi = false;

    [Header("Referensi")]
    [Tooltip("Drag GameObject 'XR Origin (XR Rig)' ke sini")]
    public Transform xrOrigin;

    [Tooltip("Titik spawn saat menekan Ulangi (depan meja)")]
    public Transform spawnDepanMeja;

    void Start()
    {
        if (!modeUlangi) return;          // bukan mode ulangi -> biarkan normal
        StartCoroutine(PindahkanPemain());
    }

    private IEnumerator PindahkanPemain()
    {
        // Tunggu beberapa frame supaya sistem tracking XR selesai inisialisasi
        // (kalau langsung di Start, posisi sering ditimpa balik oleh XR)
        yield return null;
        yield return null;
        yield return new WaitForEndOfFrame();

        if (xrOrigin != null && spawnDepanMeja != null)
        {
            // Matikan dulu CharacterController kalau ada, biar tidak menahan teleport
            CharacterController cc = xrOrigin.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            xrOrigin.SetPositionAndRotation(spawnDepanMeja.position, spawnDepanMeja.rotation);

            if (cc != null) cc.enabled = true;

            Debug.Log("PlayerSpawn: pemain dipindah ke depan meja.");
        }
        else
        {
            Debug.LogWarning("PlayerSpawn: xrOrigin atau spawnDepanMeja belum di-assign!");
        }

        modeUlangi = false; // reset agar reload berikutnya normal
    }
}