using UnityEngine;
using TMPro;

public class CustomPouring : MonoBehaviour
{
    [Header("Referensi Sistem")]
    public ParticleSystem aliranTuang; 
    public Renderer rendererH2SO4;   
    public Renderer rendererBeaker;   

    [Header("Kalibrasi Volume Asli (Mililiter)")]
    public float volumeAwalGelasUkur = 50f;
    public float volumeAwalBeaker = 200f;
    public float batasMaxBeaker = 250f;

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
    [Tooltip("Pengali laju saat gelas baru miring sedikit (tuang pelan)")]
    public float pengaliLajuMin = 0.5f;
    [Tooltip("Pengali laju saat gelas miring penuh (tuang deras)")]
    public float pengaliLajuMax = 1.6f;

    [Header("Reaksi Suhu (Eksotermik - Pengenceran Asam)")]
    [Tooltip("Suhu ruangan / air suling di awal (°C)")]
    public float suhuAwal = 25f;
    [Tooltip("Kenaikan suhu per 1 ml asam yang masuk beaker (°C)")]
    public float kenaikanSuhuPerMl = 1.0f;
    [Tooltip("Batas laju aman (ml/detik). Di atas ini = menuang terlalu cepat")]
    public float lajuTuangAman = 4f;
    [Tooltip("Pengali lonjakan suhu jika menuang terlalu cepat")]
    public float faktorSpikeBahaya = 1.8f;
    [Tooltip("Penurunan suhu saat dibiarkan diam (°C/detik)")]
    public float pendinginanPasif = 1.0f;
    [Tooltip("Penurunan suhu saat diaduk (°C/detik)")]
    public float pendinginanSaatAduk = 5f;
    [Tooltip("Ambang suhu dianggap berbahaya (°C)")]
    public float suhuBahaya = 80f;

    [Header("Referensi Tambahan")]
    public StirrerController scriptPengaduk;  // untuk tahu sedang diaduk
    public TMP_Text teksSuhuLive;             // opsional: tampil suhu real-time (mis. di termometer)
    public TMP_Text teksLajuLive;             // opsional: tampil laju tuang real-time

    private float fillH2SO4;
    private float fillBeaker;
    private Material matH2SO4;
    private Material matBeaker;
    private bool sedangMenuang = false;

    private int partikelMasuk = 0;
    private int partikelTumpah = 0;

    // State suhu & laju
    private float suhuSaatIni;
    private float suhuMaxTercatat;
    private float lajuTuangSaatIni = 0f;   // ml/detik
    private float lajuTuangMax = 0f;
    private bool pernahMenuangTerlaluCepat = false;

    void Start()
    {
        fillH2SO4 = fillGelasUkurPenuh;
        fillBeaker = fillBeaker200ml;

        suhuSaatIni = suhuAwal;
        suhuMaxTercatat = suhuAwal;

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
        // --- Logika tuang ---
        if (matH2SO4 != null && fillH2SO4 > 0)
        {
            float kemiringan = Vector3.Angle(Vector3.up, transform.up);

            if (kemiringan >= sudutTuang)
            {
                MulaiTuang();
                KurangiCairanInternal(kemiringan);
            }
            else
            {
                HentikanTuang();
            }
        }
        else
        {
            if (sedangMenuang) HentikanTuang();
        }

        // --- Logika suhu: pendinginan terus berjalan tiap frame ---
        ProsesPendinginan();

        // --- Update tampilan live ---
        if (teksSuhuLive != null)
            teksSuhuLive.text = "Suhu: " + suhuSaatIni.ToString("0.0") + " °C";
        if (teksLajuLive != null)
            teksLajuLive.text = "Laju tuang: " + lajuTuangSaatIni.ToString("0.0") + " ml/s";
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
            lajuTuangSaatIni = 0f;   // berhenti menuang -> laju 0
            TampilkanLogEvaluasi(); 
        }
    }

    private void KurangiCairanInternal(float kemiringan)
    {
        // Laju tuang bergantung kemiringan: makin miring makin deras (lebih realistis)
        float kelebihanSudut = Mathf.Clamp01((kemiringan - sudutTuang) / (180f - sudutTuang));
        float pengali = Mathf.Lerp(pengaliLajuMin, pengaliLajuMax, kelebihanSudut);
        float kecepatanEfektif = kecepatanHabis * pengali;

        float deltaFill = kecepatanEfektif * Time.deltaTime;
        fillH2SO4 -= deltaFill;
        fillH2SO4 = Mathf.Clamp(fillH2SO4, 0f, fillGelasUkurPenuh);
        matH2SO4.SetFloat("_FillAmount", fillH2SO4);

        // Konversi fill -> mililiter yang keluar frame ini
        float mlFrame = (deltaFill / fillGelasUkurPenuh) * volumeAwalGelasUkur;

        // Laju tuang (ml/detik)
        if (Time.deltaTime > 0f) lajuTuangSaatIni = mlFrame / Time.deltaTime;
        if (lajuTuangSaatIni > lajuTuangMax) lajuTuangMax = lajuTuangSaatIni;

        // --- Reaksi eksotermik: suhu naik karena asam masuk air ---
        float kenaikan = mlFrame * kenaikanSuhuPerMl;

        // Menuang terlalu cepat -> lonjakan suhu lebih besar (bahaya)
        if (lajuTuangSaatIni > lajuTuangAman)
        {
            kenaikan *= faktorSpikeBahaya;
            pernahMenuangTerlaluCepat = true;
        }

        suhuSaatIni += kenaikan;
        if (suhuSaatIni > suhuMaxTercatat) suhuMaxTercatat = suhuSaatIni;

        UpdateVisualBeaker();

        if (fillH2SO4 <= 0)
        {
            rendererH2SO4.gameObject.SetActive(false);
            HentikanTuang();
        }
    }

    private void ProsesPendinginan()
    {
        bool sedangDiaduk = (scriptPengaduk != null && scriptPengaduk.SedangMengaduk());
        float lajuDingin = sedangDiaduk ? pendinginanSaatAduk : pendinginanPasif;

        // Suhu turun perlahan menuju suhu ruangan
        suhuSaatIni = Mathf.MoveTowards(suhuSaatIni, suhuAwal, lajuDingin * Time.deltaTime);
    }

    public void ProsesTumpahan(GameObject hitObject)
    {
        string namaBenda = hitObject.name.ToLower();

        if (namaBenda.Contains("cylinder") || namaBenda.Contains("sulfat") || namaBenda.Contains("alirantuang"))
            return; 

        if (namaBenda.Contains("beaker") || namaBenda.Contains("aquades") || namaBenda.Contains("vortex"))
            partikelMasuk++;
        else
            partikelTumpah++;
    }

    private void UpdateVisualBeaker()
    {
        int totalPartikel = partikelMasuk + partikelTumpah;
        if (totalPartikel == 0 || matBeaker == null) return;

        float persenKeluar = (fillGelasUkurPenuh - fillH2SO4) / fillGelasUkurPenuh; 
        float volumeKeluar = persenKeluar * volumeAwalGelasUkur;

        float rasioMasuk = (float)partikelMasuk / totalPartikel;
        float volumeMasukBeaker = volumeKeluar * rasioMasuk;

        float volumeBeakerSaatIni = volumeAwalBeaker + volumeMasukBeaker;

        float progressTambahan = (volumeBeakerSaatIni - volumeAwalBeaker) / (batasMaxBeaker - volumeAwalBeaker);
        fillBeaker = Mathf.Lerp(fillBeaker200ml, fillBeaker250ml, progressTambahan);
        fillBeaker = Mathf.Clamp(fillBeaker, fillBeaker200ml, fillBeaker250ml);
        matBeaker.SetFloat("_FillAmount", fillBeaker);
    }

    // -------------------------------------------------------
    // GETTER untuk SinkController / UIManager
    // -------------------------------------------------------
    public float GetVolumeAkhirBeaker()
    {
        int totalPartikel = partikelMasuk + partikelTumpah;
        if (totalPartikel == 0) return volumeAwalBeaker;

        float persenKeluar = (fillGelasUkurPenuh - fillH2SO4) / fillGelasUkurPenuh;
        float volumeKeluar = persenKeluar * volumeAwalGelasUkur;

        float rasioMasuk = (float)partikelMasuk / totalPartikel;
        float volumeMasukBeaker = volumeKeluar * rasioMasuk;

        return volumeAwalBeaker + volumeMasukBeaker; // total isi beaker
    }

    public float GetKapasitasMaxBeaker() { return batasMaxBeaker; }
    public float GetSuhuSaatIni()        { return suhuSaatIni; }
    public float GetSuhuMax()            { return suhuMaxTercatat; }
    public float GetLajuTuangMax()       { return lajuTuangMax; }
    public bool MenuangTerlaluCepat()    { return pernahMenuangTerlaluCepat; }
    public bool SuhuBerbahaya()          { return suhuMaxTercatat >= suhuBahaya; }

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

        Debug.Log($"📊 HASIL: {volumeKeBeaker:F1} ml masuk | Total: {totalDiBeaker:F1} ml | Tumpah: {volumeTumpahKeMeja:F1} ml | Suhu max: {suhuMaxTercatat:F1}°C | Laju max: {lajuTuangMax:F1} ml/s");
    }
}