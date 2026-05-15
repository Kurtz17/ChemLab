using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;   // Namespace baru XR Toolkit
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Namespace baru XR Toolkit

public class StirrerController : MonoBehaviour
{
    [Header("Referensi Komponen VR")]
    public XRSocketInteractor socketInteractor; // Socket tempat ditaruhnya gelas

    [Header("Referensi Efek Visual (Cairan)")]
    public GameObject vortexEffect; // Particle system 'VortexEffect' tadi

    private bool isRunning = false; // Status apakah mesin sedang menyala

    void Start()
    {
        // Pastikan efek pusaran mati di awal
        if (vortexEffect != null)
        {
            vortexEffect.SetActive(false);
        }
    }

    // --- LOGIKA UTAMA (Akan dipanggil saat tombol ditekan) ---

    public void ToggleStirrer()
    {
        // Mengecek apakah ada object yang sedang nempel di socket menggunakan syntax TERBARU
        IXRSelectInteractable objInSocket = socketInteractor.firstInteractableSelected;

        // JIKA ADA OBJECT DI SOCKET, DAN JIKA OBJECT TERSEBUT PUNYA NAMA 'beaker'
        if (objInSocket != null && objInSocket.transform.name.Contains("Beaker_Glass"))
        {
            isRunning = !isRunning; // Tukar status mesin (nyala->mati atau mati->nyala)

            if (isRunning)
            {
                // HIDUPKAN pusaran air
                if (vortexEffect != null) vortexEffect.SetActive(true);
                Debug.Log("Pengadukan Dimulai.");
            }
            else
            {
                // MATIKAN pusaran air
                if (vortexEffect != null) vortexEffect.SetActive(false);
                Debug.Log("Pengadukan Berhenti.");
            }
        }
        else
        {
            // Jika gelas belum ditaruh, mesin tidak bisa menyala
            isRunning = false;
            if (vortexEffect != null) vortexEffect.SetActive(false);
            Debug.Log("TOMBOL DITEKAN, TAPI GELAS BELUM DITARUH ATAU OBJECT SALAH!");
        }
    }
}