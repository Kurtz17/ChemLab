using UnityEngine;

public class PourController : MonoBehaviour
{
    [Header("Referensi Cairan")]
    public Transform cairanH2SO4; 
    public Transform cairanBeaker; 

    [Header("Pengaturan Tuang")]
    public float sudutTuang = 80f; 
    [Tooltip("Kecepatan penuangan (0.5f berarti butuh sekitar 2 detik untuk habis)")]
    public float kecepatanTuang = 0.5f;

    [Header("Kalibrasi Visual Beaker (250ml)")]
    [Tooltip("Masukkan angka Scale Y Cairan Beaker saat pas di garis 250ml!")]
    public float targetSkalaBeakerY; 
    
    [Tooltip("Masukkan angka POSITION Y Cairan Beaker saat dasar air menempel di dasar gelas (250ml)!")]
    public float targetPosisiBeakerY; // KITA AKAN ISI ANGKA AJAIB BARU DI SINI

    private bool diAtasBeaker = false;
    private bool sudahDituang = false;

    // Variabel untuk menyimpan data awal sebelum dituang
    private float skalaAwalH2SO4Y;
    private float skalaAwalBeakerY;
    private Vector3 posAwalH2SO4;
    private Vector3 posAwalBeaker;

    // Progress penuangan (0.0 sampai 1.0 alias 0% sampai 100%)
    private float progressTuang = 0f;

    void Start()
    {
        // Simpan data awal sebelum dituang
        if (cairanH2SO4 != null) 
        {
            skalaAwalH2SO4Y = cairanH2SO4.localScale.y;
            posAwalH2SO4 = cairanH2SO4.localPosition;
        }
        
        if (cairanBeaker != null) 
        {
            skalaAwalBeakerY = cairanBeaker.localScale.y;
            posAwalBeaker = cairanBeaker.localPosition;
        }
    }

    void Update()
    {
        if (sudahDituang || cairanH2SO4 == null || cairanBeaker == null) return;

        if (diAtasBeaker)
        {
            float kemiringan = Vector3.Angle(Vector3.up, transform.up);
            if (kemiringan >= sudutTuang)
            {
                ProsesTuangCairan();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "ZonaTuang_Beaker") diAtasBeaker = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "ZonaTuang_Beaker") diAtasBeaker = false;
    }

    private void ProsesTuangCairan()
    {
        // Menambah progress penuangan berdasarkan waktu
        progressTuang += kecepatanTuang * Time.deltaTime;
        
        // Pastikan mentok di angka 1 (100%)
        progressTuang = Mathf.Clamp01(progressTuang);

        // --- 1. ANIMASI H2SO4 MENYUSUT (DI GELAS UKUR) ---
        float h2so4ScaleY = Mathf.Lerp(skalaAwalH2SO4Y, 0f, progressTuang);
        cairanH2SO4.localScale = new Vector3(cairanH2SO4.localScale.x, h2so4ScaleY, cairanH2SO4.localScale.z);
        
        // Turunkan posisinya sedikit agar air tetap menempel di dasar gelas ukur saat menyusut
        float susutH2SO4 = skalaAwalH2SO4Y - h2so4ScaleY;
        cairanH2SO4.localPosition = new Vector3(posAwalH2SO4.x, posAwalH2SO4.y - (susutH2SO4 / 2f), posAwalH2SO4.z);


        // --- 2. ANIMASI BEAKER BERTAMBAH (DI BEAKER) ---
        
        // A. LERP SKALA (Ukurannya membesar)
        float beakerScaleY = Mathf.Lerp(skalaAwalBeakerY, targetSkalaBeakerY, progressTuang);
        cairanBeaker.localScale = new Vector3(cairanBeaker.localScale.x, beakerScaleY, cairanBeaker.localScale.z);
        
        // B. LERP POSISI (Kita naikkan agar pantat air tidak menembus gelas) -> INI PERBAIKANNYA
        float beakerPosY = Mathf.Lerp(posAwalBeaker.y, targetPosisiBeakerY, progressTuang);
        cairanBeaker.localPosition = new Vector3(posAwalBeaker.x, posAwalBeaker.y, beakerPosY);


        // --- 3. CEK JIKA SUDAH SELESAI ---
        if (progressTuang >= 1f)
        {
            cairanH2SO4.gameObject.SetActive(false);
            sudahDituang = true;
            Debug.Log("Penuangan Selesai: Volume campuran pas 250ml dan tidak menembus gelas!");
        }
    }
}