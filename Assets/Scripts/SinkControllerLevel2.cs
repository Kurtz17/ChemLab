using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SinkControllerLevel2 : MonoBehaviour
{
    [Header("Referensi Alat (Level 2)")]
    public GameObject erlenmeyer;
    public GameObject pipet;
    public GameObject botolReagenPP; // Opsional ikut dicuci/direset posisinya

    [Header("Titik Selesai Cuci (Teleport)")]
    public Transform titikBersihErlenmeyer;
    public Transform titikBersihPipet;
    public Transform titikBersihBotolPP;

    [Header("Efek Air Keran")]
    public GameObject waterEffect; // Masukkan efek partikel/objek air ke sini

    // Status sensor
    private bool erlenmeyerDiDalam = false;
    private bool pipetDiDalam = false;
    private bool sedangDicuci = false;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable simpleInteractable;

    void Start()
    {
        if (waterEffect != null) waterEffect.SetActive(false);

        // Mengambil komponen pencet dari tuas keran
        simpleInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (simpleInteractable != null)
        {
            simpleInteractable.activated.AddListener(TriggerCuci);
            simpleInteractable.selectEntered.AddListener(TriggerCuci);
        }
    }

    // Menggunakan IsChildOf seperti Level 1 agar sensor lebih akurat
    private void OnTriggerEnter(Collider other)
    {
        if (erlenmeyer != null && other.transform.IsChildOf(erlenmeyer.transform)) 
        {
            erlenmeyerDiDalam = true;
            Debug.Log("Sensor: Erlenmeyer MASUK!");
        }
        
        if (pipet != null && other.transform.IsChildOf(pipet.transform)) 
        {
            pipetDiDalam = true;
            Debug.Log("Sensor: Pipet Tetes MASUK!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (erlenmeyer != null && other.transform.IsChildOf(erlenmeyer.transform)) 
        {
            erlenmeyerDiDalam = false;
            Debug.Log("Sensor: Erlenmeyer KELUAR!");
        }
        
        if (pipet != null && other.transform.IsChildOf(pipet.transform)) 
        {
            pipetDiDalam = false;
            Debug.Log("Sensor: Pipet Tetes KELUAR!");
        }
    }

    // Fungsi perantara untuk tombol VR
    private void TriggerCuci(BaseInteractionEventArgs arg)
    {
        MulaiCuciAlat();
    }

    public void MulaiCuciAlat()
    {
        if (erlenmeyerDiDalam && pipetDiDalam && !sedangDicuci)
        {
            StartCoroutine(ProsesCuciCoroutine());
        }
        else if (sedangDicuci)
        {
            Debug.Log("Sabar, sedang dicuci...");
        }
        else
        {
            Debug.Log($"Gagal Cuci. Status saat ini -> Erlenmeyer: {erlenmeyerDiDalam} | Pipet: {pipetDiDalam}");
        }
    }

    private IEnumerator ProsesCuciCoroutine()
    {
        sedangDicuci = true;
        
        if (waterEffect != null) waterEffect.SetActive(true);
        Debug.Log("🚿 Proses cuci Titrasi dimulai...");

        yield return new WaitForSeconds(3f); // Cuci selama 3 detik

        if (waterEffect != null) waterEffect.SetActive(false);

        // RESET LOGIKA CAIRAN & WARNA (Memanggil script masing-masing)
        if (erlenmeyer != null)
        {
            ErlenmeyerTitrasi scriptE = erlenmeyer.GetComponent<ErlenmeyerTitrasi>();
            if (scriptE != null) scriptE.ResetPraktikum();
        }

        if (pipet != null)
        {
            PipetTetes scriptP = pipet.GetComponent<PipetTetes>();
            if (scriptP != null) scriptP.ResetPipet();
        }

        // TELEPORTASI KE MEJA AWAL (Menggunakan logika mantap dari Level 1)
        ResetFisikaDanTeleport(erlenmeyer, titikBersihErlenmeyer);
        ResetFisikaDanTeleport(pipet, titikBersihPipet);
        ResetFisikaDanTeleport(botolReagenPP, titikBersihBotolPP);

        sedangDicuci = false;
        Debug.Log("✨ Praktikum Titrasi Selesai! Alat sudah di meja.");

        // NANTI: Panggil UI Manager untuk Evaluasi Titrasi di sini
    }

    private void ResetFisikaDanTeleport(GameObject obj, Transform targetTransform)
    {
        if (obj == null || targetTransform == null) return;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; 
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            obj.transform.position = targetTransform.position;
            obj.transform.rotation = targetTransform.rotation; 

            StartCoroutine(ReenablePhysics(rb));
        }
    }

    private IEnumerator ReenablePhysics(Rigidbody rb)
    {
        yield return new WaitForEndOfFrame();
        if(rb != null) rb.isKinematic = false;
    }
}