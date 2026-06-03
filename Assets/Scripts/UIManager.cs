using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Daftar Panel")]
    public GameObject panelJudul;
    public GameObject panelProtokol;
    public GameObject panelEvaluasi;

    [Header("Teks Dinamis di Panel Evaluasi")]
    public TMP_Text teksVolumePemain;   // "Volume akhir beaker: 250.00 ml"
    public TMP_Text teksAkurasi;        // "Akurasi: 100%"
    public TMP_Text teksFeedback;       // pesan feedback

    [Header("Teks Detail Eksperimen (opsional)")]
    public TMP_Text teksLamaAduk;       // "Lama pengadukan: 12.4 detik"
    public TMP_Text teksSuhuMax;        // "Suhu tertinggi: 68.5 °C"
    public TMP_Text teksLajuTuang;      // "Laju tuang max: 3.2 ml/s"

    [Header("Pengaturan Penilaian")]
    public float kapasitasMaxBeaker = 250f;  // pembagi akurasi
    public float volumeAwalBeaker = 200f;
    public float toleransiAkurasi = 95f;     // % minimal agar "berhasil"
    public float minLamaAduk = 5f;           // detik minimal agar dianggap teraduk rata
    public float suhuBahaya = 80f;           // °C ambang berbahaya

    [Header("Pengaturan Scene")]
    public string namaSceneMainMenu = "MainMenu";

    // Data hasil simulasi
    private float volumePemain = 0f;
    private float dataLamaAduk = 0f;
    private float dataSuhuMax = 0f;
    private float dataLajuMax = 0f;
    private bool dataMenuangCepat = false;

    void Start()
    {
        if (panelJudul != null) panelJudul.SetActive(true);
        if (panelProtokol != null) panelProtokol.SetActive(false);
        if (panelEvaluasi != null) panelEvaluasi.SetActive(false);
    }

    // =========================================================
    //  NAVIGASI ANTAR PANEL
    // =========================================================
    public void LanjutKeProtokol()
    {
        if (panelJudul != null) panelJudul.SetActive(false);
        if (panelProtokol != null) panelProtokol.SetActive(true);
    }

    public void LanjutKeEvaluasi()
    {
        if (panelProtokol != null) panelProtokol.SetActive(false);
        if (panelEvaluasi != null) panelEvaluasi.SetActive(true);
        TampilkanHasil();
    }

    // =========================================================
    //  EVALUASI
    // =========================================================

    public void TampilkanEvaluasi(float volume)
    {
        volumePemain = volume;
        if (panelJudul != null) panelJudul.SetActive(false);
        if (panelProtokol != null) panelProtokol.SetActive(false);
        if (panelEvaluasi != null) panelEvaluasi.SetActive(true);
        TampilkanHasil();
    }

    public void TampilkanEvaluasi(float volume, float kapasitasMax)
    {
        kapasitasMaxBeaker = kapasitasMax;
        TampilkanEvaluasi(volume);
    }

    // Setel data eksperimen tambahan (dipanggil SEBELUM TampilkanEvaluasi)
    public void SetDataEksperimen(float lamaAduk, float suhuMax, float lajuMax, bool menuangCepat)
    {
        dataLamaAduk = lamaAduk;
        dataSuhuMax = suhuMax;
        dataLajuMax = lajuMax;
        dataMenuangCepat = menuangCepat;
    }

    public void SetVolumePemain(float volume) { volumePemain = volume; }

    private void TampilkanHasil()
    {
        float akurasi = Mathf.Clamp((volumePemain / kapasitasMaxBeaker) * 100f, 0f, 100f);

        if (teksVolumePemain != null)
            teksVolumePemain.text = volumePemain.ToString("0.00");

        if (teksAkurasi != null)
            teksAkurasi.text = akurasi.ToString("0");

        if (teksLamaAduk != null)
            teksLamaAduk.text = dataLamaAduk.ToString("0.0");

        if (teksSuhuMax != null)
            teksSuhuMax.text = dataSuhuMax.ToString("0.0");

        if (teksLajuTuang != null)
            teksLajuTuang.text = dataLajuMax.ToString("0.0");

        if (teksFeedback != null)
            teksFeedback.text = AmbilFeedback(akurasi);
    }

    private string AmbilFeedback(float akurasi)
    {
        // Prioritaskan peringatan keselamatan dulu
        if (dataMenuangCepat || dataSuhuMax >= suhuBahaya)
            return "suhu larutan terlalu tinggi karena asam dituang terlalu cepat. " +
                   "Tuangkan lebih perlahan dan aduk agar panas reaksi tersebar aman.";

        if (dataLamaAduk < minLamaAduk)
            return "Larutan kurang teraduk. Aktifkan pengaduk lebih lama agar campuran homogen dan suhu merata.";

        if (akurasi >= toleransiAkurasi)
            return "Penuangan berhasil. Hampir seluruh asam masuk dengan presisi baik, suhu terkendali, dan larutan teraduk rata.";

        if (akurasi >= 80f)
            return "Penuangan cukup baik, namun sebagian cairan tumpah. Tuangkan lebih perlahan agar tidak ada yang terbuang.";

        return "Banyak cairan tumpah saat menuang. Perhatikan posisi mulut gelas ukur tepat di atas beaker.";
    }

    // =========================================================
    //  TOMBOL DI PANEL EVALUASI
    // =========================================================
    public void UlangiSimulasi()
    {
        PlayerSpawn.modeUlangi = true;
        PlayerSpawn.spawnStage = 1;   // spawn di depan meja stage 1
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void Selesai()
    {
        Debug.Log("Praktikum Selesai - keluar aplikasi.");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void KembaliKeMainMenu()
    {
        SceneManager.LoadScene(namaSceneMainMenu);
    }
}