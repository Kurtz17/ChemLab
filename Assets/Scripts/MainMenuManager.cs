using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ada untuk fitur pindah scene

public class MainMenuManager : MonoBehaviour
{
    [Header("Pengaturan Pemain")]
    // Variabel untuk menahan pergerakan pemain di scene menu
    public GameObject sistemGerakPemain; 

    void Start()
    {
        // Mengunci pergerakan VR saat scene ini baru mulai
        if (sistemGerakPemain != null)
        {
            sistemGerakPemain.SetActive(false);
        }
    }

    public void KlikPlay()
    {
        // Memuat scene baru. 
        // PASTIKAN ejaan "briefingroom" sama persis dengan nama file scenemu (huruf besar/kecil ngaruh!)
        SceneManager.LoadScene("BriefingRoom"); 
    }

    public void KlikExit()
    {
        Debug.Log("Game Ditutup!");
        Application.Quit();
    }
}