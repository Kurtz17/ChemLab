using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PipetTetes : MonoBehaviour
{
    [Header("Status Cairan")]
    public bool isTerisi = false;
    public int sisaTetesan = 0;

    [Header("Efek Visual")]
    public ParticleSystem efekTetesan;

    [Header("Efek Suara")]
    public AudioSource audioSource; // Komponen speaker di pipet
    public AudioClip suaraSedot;    // Suara saat mengambil indikator PP (sluurp)
    public AudioClip suaraTetes;    // Suara saat menetaskan indikator PP (plop)

    private bool sedangDiDalamBotolPP = false;
    private ErlenmeyerTitrasi targetErlenmeyer; 
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        if (grabInteractable != null)
        {
            grabInteractable.activated.AddListener(AksiPipet);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string namaBenda = other.gameObject.name.ToLower();
        string namaInduk = other.transform.root.name.ToLower();

        if (namaBenda.Contains("pp") || namaInduk.Contains("pp"))
        {
            sedangDiDalamBotolPP = true;
        }

        if (namaBenda.Contains("erlenmeyer") || namaInduk.Contains("erlenmeyer"))
        {
            targetErlenmeyer = other.transform.root.GetComponentInChildren<ErlenmeyerTitrasi>();
            Debug.Log("🔍 Ujung pipet siap menetes di atas Erlenmeyer!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string namaBenda = other.gameObject.name.ToLower();
        string namaInduk = other.transform.root.name.ToLower();

        if (namaBenda.Contains("pp") || namaInduk.Contains("pp"))
        {
            sedangDiDalamBotolPP = false;
        }

        if (namaBenda.Contains("erlenmeyer") || namaInduk.Contains("erlenmeyer"))
        {
            targetErlenmeyer = null; 
        }
    }

    public void AksiPipet(ActivateEventArgs arg)
    {
        // LOGIKA 1: Kalau pipet KOSONG (Waktunya Sedot)
        if (!isTerisi)
        {
            if (sedangDiDalamBotolPP)
            {
                isTerisi = true;
                sisaTetesan = 3;
                Debug.Log("💧 Indikator PP berhasil disedot! Pipet terisi 3 tetes.");
                
                // Mainkan suara sedot
                if (audioSource != null && suaraSedot != null)
                {
                    audioSource.PlayOneShot(suaraSedot);
                }
            }
            else
            {
                Debug.Log("❌ Gagal sedot! Ujung pipet belum masuk ke Botol PP.");
            }
        }
        // LOGIKA 2: Kalau pipet BERISI (Waktunya Tetes)
        else 
        {
            if (sisaTetesan > 0)
            {
                sisaTetesan--;
                Debug.Log($"💧 Meneteskan 1 tetes! Sisa di pipet: {sisaTetesan}");

                // Mainkan efek partikel air
                if (efekTetesan != null)
                {
                    efekTetesan.Stop(); // Pengaman agar partikel bisa diputar ulang dengan cepat
                    efekTetesan.Play();
                }

                // Mainkan suara tetesan
                if (audioSource != null && suaraTetes != null)
                {
                    audioSource.PlayOneShot(suaraTetes);
                }
                
                // Kalau posisinya pas di atas Erlenmeyer, masukkan airnya!
                if (targetErlenmeyer != null)
                {
                    targetErlenmeyer.TambahTetesPP();
                }
                else
                {
                    Debug.Log("⚠️ Tetesan terbuang ke meja karena tidak diarahkan ke Erlenmeyer.");
                }

                // Kalau air habis, kosongkan pipet
                if (sisaTetesan <= 0)
                {
                    isTerisi = false;
                    Debug.Log("⚪ Pipet sudah kosong kembali.");
                }
            }
        }
    }

    // Fungsi untuk mengosongkan Pipet Tetes
    public void ResetPipet()
    {
        isTerisi = false;
        sisaTetesan = 0;
    }
}