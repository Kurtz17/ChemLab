using UnityEngine;

public class CustomPouring : MonoBehaviour
{
    [Header("Referensi Sistem")]
    public ParticleSystem aliranTuang; 
    public Renderer rendererH2SO4;   // Masukkan object Cairan_H2SO4
    public Renderer rendererBeaker;   // Masukkan object Cairan_Aquades

    [Header("Pengaturan")]
    public float sudutTuang = 80f;
    [Tooltip("Kecepatan habisnya air (0.0 sampai 1.0 per detik)")]
    public float kecepatanHabis = 0.5f; 

    // Level air internal (0.0 = kosong, 1.0 = penuh)
    private float fillH2SO4 = 1f;   // Gelas ukur mulai dari penuh
    private float fillBeaker = 0.7f; // Beaker mulai dari 40% (garis 200ml)

    private Material matH2SO4;
    private Material matBeaker;
    private bool sedangMenuang = false;

    // Evaluasi
    private float totalVolumeDituang = 0f;
    private float totalVolumeTerbuang = 0f;

    void Start()
    {
        // Ambil material asli agar bisa dimanipulasi
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
        }
    }

    private void KurangiCairanInternal()
    {
        // Kurangi air gelas ukur
        fillH2SO4 -= kecepatanHabis * Time.deltaTime;
        fillH2SO4 = Mathf.Clamp01(fillH2SO4);
        matH2SO4.SetFloat("_FillAmount", fillH2SO4);

        if (fillH2SO4 <= 0)
        {
            rendererH2SO4.gameObject.SetActive(false);
            HentikanTuang();
            Debug.Log($"EVALUASI -> Masuk Beaker: {totalVolumeDituang} | Tumpah ke Meja: {totalVolumeTerbuang}");
        }
    }

    public void ProsesTumpahan(GameObject hitObject)
    {
        if (hitObject.name.Contains("Beaker_Glass") || hitObject.name.Contains("Cairan_Aquades"))
        {
            TambahCairanBeaker();
        }
        else if (hitObject.name.Contains("Meja_Praktikum1") || hitObject.name.Contains("Lantai") || hitObject.name.Contains("Meja_Wastafel1"))
        {
            totalVolumeTerbuang += 0.1f;
            Debug.LogWarning("PENALTI! Cairan Kimia Tumpah ke: " + hitObject.name);
        }
    }

    private void TambahCairanBeaker()
    {
        // 1. KITA UBAH: Naikkan target fill ke 1.0f (Penuh 100%) agar tidak langsung mentok/return
        float targetFillBeaker = 1.0f;

        if (matBeaker == null || fillBeaker >= targetFillBeaker) return;

        // 2. KITA UBAH: Tambah volume secara fixed per-tabrakan partikel agar lebih responsif
        totalVolumeDituang += 0.05f;

        // 3. KITA UBAH: Naikkan level air beaker secara konstan (bukan pakai Time.deltaTime) 
        // karena OnParticleCollision dihitung per-tetes partikel yang masuk
        fillBeaker += 0.005f; 
        fillBeaker = Mathf.Clamp(fillBeaker, 0f, targetFillBeaker);
        
        matBeaker.SetFloat("_FillAmount", fillBeaker);

        // Pasang CCTV di Console buat mastiin angkanya beneran naik pas dituang
        Debug.Log($"🧪 SUKSES! Air Beaker naik. Level saat ini: {fillBeaker:F3}");
    }
}