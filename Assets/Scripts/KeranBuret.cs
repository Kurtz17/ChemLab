using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeranBuret : MonoBehaviour
{
    [Header("Pengaturan Buret")]
    public float volumePerTetes = 0.05f; 

    [Header("Referensi Sistem")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketBuret;
    public ParticleSystem efekTetesan; // Slot untuk Particle System air

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable simpleInteractable;

    void Start()
    {
        simpleInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        
        if (simpleInteractable != null)
        {
            // Mendaftarkan tombol Trigger (Klik Kiri)
            simpleInteractable.activated.AddListener(TeteskanCairanActivate);
            
            // Mendaftarkan tombol Grip (Tombol G)
            simpleInteractable.selectEntered.AddListener(TeteskanCairanSelect);
        }
    }

    // Fungsi perantara untuk tombol Trigger
    private void TeteskanCairanActivate(ActivateEventArgs arg)
    {
        ProsesTetesan();
    }

    // Fungsi perantara untuk tombol Grip
    private void TeteskanCairanSelect(SelectEnterEventArgs arg)
    {
        ProsesTetesan();
    }

    // Logika utama agar kodenya tidak berulang
    private void ProsesTetesan()
    {
        Debug.Log("🚰 Keran diputar! 1 Tetes keluar.");

        // Mainkan efek partikel air
        if (efekTetesan != null)
        {
            efekTetesan.Play();
        }

        if (socketBuret != null && socketBuret.hasSelection)
        {
            GameObject bendaDiBawah = socketBuret.interactablesSelected[0].transform.gameObject;
            ErlenmeyerTitrasi erlenmeyer = bendaDiBawah.GetComponent<ErlenmeyerTitrasi>();

            if (erlenmeyer != null)
            {
                erlenmeyer.TambahVolumeNaOH(volumePerTetes);
            }
        }
        else
        {
            Debug.Log("⚠️ Tetesan jatuh terbuang ke meja!");
        }
    }
}