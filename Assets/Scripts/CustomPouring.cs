using UnityEngine;

public class CustomPouring : MonoBehaviour
{
    [Header("Referensi Sistem")]
    public ParticleSystem aliranTuang; 
    public Renderer rendererH2SO4;   
    public Renderer rendererBeaker;   

    [Header("Kalibrasi Volume Asli (Mililiter)")]
    public float volumeAwalGelasUkur = 50f;  // Isi Gelas Ukur (100% shader = 50ml)
    public float volumeAwalBeaker = 200f;    // Isi awal Beaker
    public float batasMaxBeaker = 250f;      // Kapasitas target mentok di Beaker

    [Header("Kalibrasi Visual 3D (Fill Amount Shader)")]
    [Tooltip("Angka shader saat Beaker di garis 200ml")]
    public float fillBeaker200ml = 0.7f;
    [Tooltip("Angka shader saat Beaker di garis 250ml (Penuh)")]
    public float fillBeaker250ml = 1.0f;
    [Tooltip("Angka shader saat Gelas Ukur Penuh (50ml)")]
    public float fillGelasUkurPenuh = 1.0f;

    [Header("Pengaturan Tuang")]
    public float sudutTuang = 80f;
    public float kecepatanHabis = 0.2f; 

    private float fillH2SO4;
    private float fillBeaker;

    private Material matH2SO4;
    private Material matBeaker;
    private bool sedangMenuang = false;

    // Penghitung jumlah partikel fisik untuk rasio
    private int partikelMasuk = 0;
    private int partikelTumpah = 0;

    void Start()
    {
        fillH2SO4 = fillGelasUkurPenuh;
        fillBeaker = fillBeaker200ml;

        if (rendererH2SO4 != null)
        {
            matH2SO4 = rendererH2SO4.material;
            matH2SO4.SetFloat("_FillAmount", fillH2SO4);
        }
        
        if (rendererBeaker != null)
        {
            matBeaker = rendererBeaker.material;
            matBeaker.SetFloat("_FillAmount", fillBeaker);
        }
    }

    void Update()
    {
        if (matH2SO4 == null || fillH2SO4 <= 0) 
        {
            if (sedangMenuang) HentikanTuang();
            return;
        }

        float kemiringan = Vector3.Angle(Vector3.up, transform.up);

        if (kemiringan >= sudutTuang)
        {
            MulaiTuang();
            KurangiCairanInternal();
        }
        else
        {
            HentikanTuang();
        }
    }

    private void MulaiTuang()
    {
        if (!sedangMenuang)
        {
            sedangMenuang = true;
            aliranTuang.Play();
        }
    }

    private void HentikanTuang()
    {
        if (sedangMenuang)
        {
            sedangMenuang = false;
            aliranTuang.Stop();
            TampilkanLogEvaluasi(); 
        }
    }

    private void KurangiCairanInternal()
    {
        // Kurangi visual Gelas Ukur
        fillH2SO4 -= kecepatanHabis * Time.deltaTime;
        fillH2SO4 = Mathf.Clamp(fillH2SO4, 0f, fillGelasUkurPenuh);
        matH2SO4.SetFloat("_FillAmount", fillH2SO4);

        UpdateVisualBeaker();

        if (fillH2SO4 <= 0)
        {
            rendererH2SO4.gameObject.SetActive(false);
            HentikanTuang();
        }
    }

    public void ProsesTumpahan(GameObject hitObject)
    {
        string namaBenda = hitObject.name.ToLower();

        if (namaBenda.Contains("cylinder") || namaBenda.Contains("sulfat") || namaBenda.Contains("alirantuang"))
        {
            return; 
        }

        if (namaBenda.Contains("beaker") || namaBenda.Contains("aquades") || namaBenda.Contains("vortex"))
        {
            partikelMasuk++;
        }
        else
        {
            partikelTumpah++;
        }
    }

    private void UpdateVisualBeaker()
    {
        int totalPartikel = partikelMasuk + partikelTumpah;
        if (totalPartikel == 0 || matBeaker == null) return;

        // 1. Hitung berapa ML cairan yang sudah keluar dari Gelas Ukur (Proporsional dari 50ml)
        float persenKeluar = (fillGelasUkurPenuh - fillH2SO4) / fillGelasUkurPenuh; 
        float volumeKeluar = persenKeluar * volumeAwalGelasUkur;

        // 2. Tentukan berapa ML yang sukses masuk Beaker
        float rasioMasuk = (float)partikelMasuk / totalPartikel;
        float volumeMasukBeaker = volumeKeluar * rasioMasuk;

        // 3. Total ML di dalam Beaker saat ini (Mulai dari 200ml + yang baru masuk)
        float volumeBeakerSaatIni = volumeAwalBeaker + volumeMasukBeaker;

        // 4. MAPPING KE SHADER: Ubah penambahan 50ml menjadi kenaikan shader dari 0.7 ke 1.0
        float progressTambahan = (volumeBeakerSaatIni - volumeAwalBeaker) / (batasMaxBeaker - volumeAwalBeaker);
        fillBeaker = Mathf.Lerp(fillBeaker200ml, fillBeaker250ml, progressTambahan);
        
        // Kunci maksimal agar tidak luber melewati 1.0
        fillBeaker = Mathf.Clamp(fillBeaker, fillBeaker200ml, fillBeaker250ml);
        matBeaker.SetFloat("_FillAmount", fillBeaker);
    }

    private void TampilkanLogEvaluasi()
    {
        int totalPartikel = partikelMasuk + partikelTumpah;
        if (totalPartikel == 0) return;

        float persenKeluar = (fillGelasUkurPenuh - fillH2SO4) / fillGelasUkurPenuh; 
        float volumeKeluar = persenKeluar * volumeAwalGelasUkur;
        
        float rasioMasuk = (float)partikelMasuk / totalPartikel;
        float rasioTumpah = (float)partikelTumpah / totalPartikel;

        float volumeKeBeaker = volumeKeluar * rasioMasuk;
        float volumeTumpahKeMeja = volumeKeluar * rasioTumpah;

        float totalDiBeaker = volumeAwalBeaker + volumeKeBeaker;

        Debug.Log($"📊 HASIL AKHIR: {volumeKeBeaker:F1} ml berhasil ditambah. Total isi Beaker: {totalDiBeaker:F1} ml | Terbuang: {volumeTumpahKeMeja:F1} ml");
    }
}