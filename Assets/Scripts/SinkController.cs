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
        // Debug Log Detektif: Biar kita tahu wastafel nyentuh apa
        // Debug.Log("Wastafel disentuh oleh: " + other.name);

        // Gunakan IsChildOf: Mengecek apakah yang menyentuh ini adalah bagian dari Beaker/Gelas Ukur
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
            // Debug tambahan biar kelihatan mana yang belum terbaca
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
    }

    private void ResetFisikaDanTeleport(GameObject obj, Transform targetTransform)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; 
            rb.linearVelocity = Vector3.zero; // Unity 6 update
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