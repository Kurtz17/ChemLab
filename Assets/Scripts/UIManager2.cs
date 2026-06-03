using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager2 : MonoBehaviour
{
    [Header("Daftar Panel")]
    public GameObject panelJudul;
    public GameObject panelProtokol;
    public GameObject panelEvaluasi;

    [Header("Teks Dinamis di Panel Evaluasi")]
    public TMP_Text teksJumlahTetesPP;   // "Tetes PP masuk: 3 / 3"
    public TMP_Text teksVolumeNaOH;      // "Volume NaOH: 0.45 ml"
    public TMP_Text teksHasilWarna;      // "Perubahan warna: Berhasil / Gagal"
    public TMP_Text teksFeedback;        // pesan feedback

    [Header("Pengaturan Penilaian")]
    public int targetTetesPP = 3;
    public float targetMinNaOH = 0.15f;  // ml minimal NaOH (3 tetes x 0.05ml)
    public float targetMaxNaOH = 0.30f;  // ml batas lebih NaOH

    [Header("Pengaturan Scene")]
    public string namaSceneMainMenu = "MainMenu";

    // Data hasil simulasi (diisi oleh SinkControllerLevel2 sebelum tampil)
    private int dataJumlahTetesPP = 0;
    private float dataVolumeNaOH = 0f;
    private bool dataBerubahWarna = false;

    void Start()
    {
        if (panelJudul != null) panelJudul.SetActive(true);
        if (panelProtokol != null) panelProtokol.SetActive(false);
        if (panelEvaluasi != null) panelEvaluasi.SetActive(false);
    }

    // =========================================================
    //  NAVIGASI
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

    // Dipanggil SinkControllerLevel2 saat cuci selesai
    public void TampilkanEvaluasi(int tetesPP, float volNaOH, bool berubahWarna)
    {
        dataJumlahTetesPP = tetesPP;
        dataVolumeNaOH = volNaOH;
        dataBerubahWarna = berubahWarna;

        if (panelJudul != null) panelJudul.SetActive(false);
        if (panelProtokol != null) panelProtokol.SetActive(false);
        if (panelEvaluasi != null) panelEvaluasi.SetActive(true);

        TampilkanHasil();
    }

    private void TampilkanHasil()
    {
        if (teksJumlahTetesPP != null)
            teksJumlahTetesPP.text = dataJumlahTetesPP.ToString();

        if (teksVolumeNaOH != null)
            teksVolumeNaOH.text = dataVolumeNaOH.ToString("0.00");

        if (teksHasilWarna != null)
            teksHasilWarna.text = dataBerubahWarna ? "Berhasil" : "Belum terjadi";

        if (teksFeedback != null)
            teksFeedback.text = AmbilFeedback();
    }

    private string AmbilFeedback()
    {
        // Cek tiap kondisi secara berurutan dari yang paling kritis
        if (dataJumlahTetesPP < targetTetesPP)
            return "Indikator PP kurang dari 3 tetes. Pastikan meneteskan PP sebanyak 3 tetes ke dalam erlenmeyer sebelum titrasi dimulai.";

        if (dataVolumeNaOH < targetMinNaOH)
            return "Volume NaOH terlalu sedikit. Teteskan NaOH dari buret minimal 3 kali hingga larutan mulai berubah warna.";

        if (!dataBerubahWarna)
            return "Larutan belum berubah warna. Goyangkan erlenmeyer setelah meneteskan NaOH agar reaksi terjadi dan warna berubah menjadi merah muda.";

        if (dataVolumeNaOH > targetMaxNaOH)
            return "Titrasi melewati titik ekivalen. Volume NaOH terlalu banyak sehingga larutan berubah ke magenta. Teteskan lebih perlahan agar berhenti di merah muda.";

        return "Titrasi berhasil! Larutan berubah menjadi merah muda pada titik ekivalen. Volume NaOH dan jumlah tetes PP sesuai prosedur.";
    }

    // =========================================================
    //  TOMBOL DI PANEL EVALUASI
    // =========================================================
    public void UlangiSimulasi()
    {
        PlayerSpawn.modeUlangi = true;
        PlayerSpawn.spawnStage = 2;   // spawn di depan meja stage 2
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void Selesai()
    {
        Debug.Log("Praktikum Titrasi Selesai - keluar aplikasi.");
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