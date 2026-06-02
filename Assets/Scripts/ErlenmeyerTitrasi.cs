using UnityEngine;

public class ErlenmeyerTitrasi : MonoBehaviour
{
    [Header("Status Persiapan (PP)")]
    public int jumlahTetesanPP = 0;
    public bool indikatorSiap = false;

    [Header("Status Titrasi (NaOH)")]
    public float totalVolumeNaOH = 0f;
    public float targetVolumePas = 0.15f; 
    public float targetVolumeLebih = 0.30f; 

    [Header("Visual & Fisika")]
    public MeshRenderer cairanRenderer; 
    
    // Nilai ini mungkin perlu kamu perbesar (misal 2.0 atau 3.0) karena hitungan manual biasanya angkanya lebih besar
    public float batasKecepatanGoyang = 2.0f; 

    [Header("Pengaturan Warna")]
    public Color warnaBening = new Color(1f, 1f, 1f, 0.1f);
    public Color warnaMerahMuda = new Color(1f, 0.5f, 0.8f, 0.8f); 
    public Color warnaMagenta = new Color(1f, 0f, 1f, 0.9f); 

    private Material materialCairan;
    private Color warnaSekarang;
    
    // Variabel baru untuk Speedometer Manual
    private Vector3 posisiSebelumnya;

    void Start()
    {
        if (cairanRenderer != null)
        {
            materialCairan = cairanRenderer.material;
            warnaSekarang = warnaBening;
            SetWarnaCairan(warnaSekarang);
        }
        
        // Simpan posisi awal
        posisiSebelumnya = transform.position;
    }

    void Update()
    {
        if (materialCairan == null) return;

        // 1. SPEEDOMETER MANUAL: Jarak tempuh / Waktu tempuh
        float kecepatanLinear = (transform.position - posisiSebelumnya).magnitude / Time.deltaTime;
        
        // 2. Simpan posisi saat ini untuk dihitung di frame berikutnya
        posisiSebelumnya = transform.position;

        // CCTV Kecepatan (Akan nge-print angka kalau benda digerakkan)
        if (kecepatanLinear > 0.5f)
        {
            Debug.Log($"🌪️ Kecepatan Tangan: {kecepatanLinear:F2}");
        }

        // 3. Cek Syarat Goyang & Warna
        if (kecepatanLinear > batasKecepatanGoyang && indikatorSiap && totalVolumeNaOH > 0)
        {
            ProsesPerubahanWarna();
        }
    }

    public void TambahTetesPP()
    {
        if (indikatorSiap) return;
        
        jumlahTetesanPP++;
        Debug.Log($"🧪 PP masuk: {jumlahTetesanPP}/3");
        
        if (jumlahTetesanPP >= 3)
        {
            indikatorSiap = true;
            Debug.Log("✅ Indikator PP Siap!");
        }
    }

    public void TambahVolumeNaOH(float volumeMasuk)
    {
        if (!indikatorSiap) return;
        
        totalVolumeNaOH += volumeMasuk;
        Debug.Log($"💧 Volume NaOH saat ini: {totalVolumeNaOH:F2} ml");
    }

    private void ProsesPerubahanWarna()
    {
        Color targetWarna = warnaBening;

        if (totalVolumeNaOH >= targetVolumeLebih)
        {
            targetWarna = warnaMagenta; 
        }
        else if (totalVolumeNaOH >= targetVolumePas)
        {
            targetWarna = warnaMerahMuda; 
        }

        // Kita naikkan kecepatannya jadi 5.0f biar langsung "ngegas" berubahnya!
        warnaSekarang = Color.Lerp(warnaSekarang, targetWarna, Time.deltaTime * 1.5f);
        SetWarnaCairan(warnaSekarang);

        // CCTV Warna: Untuk memastikan fungsi ini benar-benar terpanggil
        Debug.Log($"🎨 Mencoba ubah warna! R:{warnaSekarang.r:F2} G:{warnaSekarang.g:F2} B:{warnaSekarang.b:F2}");
    }

    private void SetWarnaCairan(Color warnaBaru)
    {
        // Tembak kedua nama properti yang paling umum di Shader Graph
        materialCairan.SetColor("_WarnaAir", warnaBaru); 
    }
    // Fungsi untuk mengosongkan Erlenmeyer ke posisi semula
    public void ResetPraktikum()
    {
        jumlahTetesanPP = 0;
        totalVolumeNaOH = 0f;
        indikatorSiap = false;
        warnaSekarang = warnaBening;
        SetWarnaCairan(warnaBening);
        Debug.Log("♻️ Erlenmeyer berhasil dicuci dan di-reset!");
    }
}