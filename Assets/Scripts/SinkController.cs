using System.Collections;
using UnityEngine;

public class SinkController : MonoBehaviour
{
    [Header("Referensi Gelas")]
    public GameObject beaker;
    public GameObject gelasUkur;

    [Header("Referensi Cairan (Untuk dikosongkan)")]
    public GameObject cairanBeaker;
    public GameObject cairanH2SO4;

    [Header("Titik Selesai Cuci")]
    public Transform titikBersihBeaker;
    public Transform titikBersihGelasUkur;

    [Header("Efek Air Keran")]
    public GameObject waterEffect;

    // -------------------------------------------------------
    // SAMBUNGAN KE EVALUASI (drag di Inspector)
    // -------------------------------------------------------
    [Header("Referensi Evaluasi")]
    public UIManager uiManager;             // drag LabBoardCanvas ke sini
    public CustomPouring scriptPenuangan;   // drag Gelas Ukur ke sini
    public StirrerController scriptPengaduk; // drag alat pengaduk ke sini

    // Status sensor
    private bool beakerDiDalam = false;
    private bool gelasUkurDiDalam = false;
    private bool sedangDicuci = false;

    void Start()
    {
        if (waterEffect != null) waterEffect.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.IsChildOf(beaker.transform)) 
        {
            beakerDiDalam = true;
            Debug.Log("Sensor: Beaker MASUK!");
        }
        
        if (other.transform.IsChildOf(gelasUkur.transform)) 
        {
            gelasUkurDiDalam = true;
            Debug.Log("Sensor: Gelas Ukur MASUK!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.IsChildOf(beaker.transform)) 
        {
            beakerDiDalam = false;
            Debug.Log("Sensor: Beaker KELUAR!");
        }
        
        if (other.transform.IsChildOf(gelasUkur.transform)) 
        {
            gelasUkurDiDalam = false;
            Debug.Log("Sensor: Gelas Ukur KELUAR!");
        }
    }

    public void MulaiCuciGelas()
    {
        if (beakerDiDalam && gelasUkurDiDalam && !sedangDicuci)
        {
            StartCoroutine(ProsesCuciCoroutine());
        }
        else if (sedangDicuci)
        {
            Debug.Log("Sabar, sedang dicuci...");
        }
        else
        {
            Debug.Log($"Gagal Cuci. Status saat ini -> Beaker: {beakerDiDalam} | Gelas Ukur: {gelasUkurDiDalam}");
        }
    }

    private IEnumerator ProsesCuciCoroutine()
    {
        sedangDicuci = true;
        
        if (waterEffect != null) waterEffect.SetActive(true);
        Debug.Log("Proses cuci dimulai...");

        yield return new WaitForSeconds(2f);

        if (waterEffect != null) waterEffect.SetActive(false);

        // KOSONGKAN GELAS
        if (cairanBeaker != null) cairanBeaker.SetActive(false);
        if (cairanH2SO4 != null) cairanH2SO4.SetActive(false);

        // TELEPORTASI
        ResetFisikaDanTeleport(beaker, titikBersihBeaker);
        ResetFisikaDanTeleport(gelasUkur, titikBersihGelasUkur);

        sedangDicuci = false;
        Debug.Log("Praktikum Selesai!");

        // -------------------------------------------------------
        // TAMPILKAN EVALUASI dengan volume asli dari penuangan
        // -------------------------------------------------------
        if (uiManager != null && scriptPenuangan != null)
        {
            float volumeAkhir = scriptPenuangan.GetVolumeAkhirBeaker();
            float kapasitasMax = scriptPenuangan.GetKapasitasMaxBeaker();
            float suhuMax = scriptPenuangan.GetSuhuMax();
            float lajuMax = scriptPenuangan.GetLajuTuangMax();
            bool menuangCepat = scriptPenuangan.MenuangTerlaluCepat();
            float lamaAduk = (scriptPengaduk != null) ? scriptPengaduk.GetLamaPengadukan() : 0f;

            // Kirim data tambahan dulu, baru tampilkan evaluasi
            uiManager.SetDataEksperimen(lamaAduk, suhuMax, lajuMax, menuangCepat);
            uiManager.TampilkanEvaluasi(volumeAkhir, kapasitasMax);
        }
        else
        {
            Debug.LogWarning("SinkController: uiManager atau scriptPenuangan belum di-assign di Inspector!");
        }
    }

    private void ResetFisikaDanTeleport(GameObject obj, Transform targetTransform)
    {
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