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

    // BARU: total lama pengadukan (detik)
    private float lamaPengadukan = 0f;

    void Start()
    {
        if (vortexEffect != null)
        {
            vortexEffect.SetActive(false);
        }
    }

    void Update()
    {
        // Akumulasi waktu selama mesin menyala
        if (isRunning)
        {
            lamaPengadukan += Time.deltaTime;
        }
    }

    public void ToggleStirrer()
    {
        IXRSelectInteractable objInSocket = socketInteractor.firstInteractableSelected;

        if (objInSocket != null && objInSocket.transform.name.Contains("Beaker_Glass"))
        {
            isRunning = !isRunning;

            if (isRunning)
            {
                if (vortexEffect != null) vortexEffect.SetActive(true);
                Debug.Log("Pengadukan Dimulai.");
            }
            else
            {
                if (vortexEffect != null) vortexEffect.SetActive(false);
                Debug.Log("Pengadukan Berhenti.");
            }
        }
        else
        {
            isRunning = false;
            if (vortexEffect != null) vortexEffect.SetActive(false);
            Debug.Log("TOMBOL DITEKAN, TAPI GELAS BELUM DITARUH ATAU OBJECT SALAH!");
        }
    }

    // -------------------------------------------------------
    // FUNGSI BARU (dipakai script lain)
    // -------------------------------------------------------
    public bool SedangMengaduk()
    {
        return isRunning;
    }

    public float GetLamaPengadukan()
    {
        return lamaPengadukan;
    }
}