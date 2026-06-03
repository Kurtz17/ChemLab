using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LokerBriefing : MonoBehaviour
{
    [Header("Referensi Audio")]
    public AudioSource audioSource;
    public AudioClip suaraLoker; // Gunakan file audio 8 detikmu

    [Header("Pengaturan Durasi")]
    [Tooltip("Mau berapa detik suara ini dimainkan sebelum dipotong?")]
    public float durasiPotongSuara = 2f; 

    private Coroutine audioCoroutine;
    private float cooldownTerakhir = 0f;
    private const float COOLDOWN_WAKTU = 0.3f; // Pengaman suara double dalam satu klik

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable simpleInteractable;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Start()
    {
        // Deteksi klik/trigger
        simpleInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (simpleInteractable != null)
        {
            simpleInteractable.activated.AddListener(TriggerSuaraXR);
            simpleInteractable.selectEntered.AddListener(TriggerSuaraXR);
        }

        // Deteksi genggaman tangan VR
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(TriggerSuaraXR);
        }
    }

    private void TriggerSuaraXR(BaseInteractionEventArgs arg)
    {
        // Cek apakah interaksi ini tidak terlalu cepat (mencegah bug double sound dalam 1 frame)
        if (Time.time - cooldownTerakhir > COOLDOWN_WAKTU)
        {
            cooldownTerakhir = Time.time;
            MainkanSuaraLoker();
        }
    }

    public void MainkanSuaraLoker()
    {
        if (audioSource != null && suaraLoker != null)
        {
            // FITUR INTERRUPT: Jika suara sebelumnya masih bunyi, cut langsung dan reset dari awal
            if (audioCoroutine != null)
            {
                StopCoroutine(audioCoroutine);
            }
            
            // Jalankan hitung mundur pemotongan suara yang baru
            audioCoroutine = StartCoroutine(PutarSuaraDenganBatasanWaktu());
        }
    }

    private IEnumerator PutarSuaraDenganBatasanWaktu()
    {
        // 1. Paksa stop suara yang sedang berjalan (jika ada)
        audioSource.Stop(); 

        // 2. Set file lagu dan mulai dari detik ke-0
        audioSource.clip = suaraLoker;
        audioSource.time = 0f; 
        audioSource.Play();
        Debug.Log("🔓 Loker berinteraksi: Suara dimainkan dari detik ke-0.");

        // 3. Tunggu selama 2 detik
        yield return new WaitForSeconds(durasiPotongSuara);

        // 4. Matikan paksa suaranya setelah 2 detik habis
        audioSource.Stop();
        audioCoroutine = null;
        Debug.Log("🔒 Suara loker dipotong otomatis.");
    }
}