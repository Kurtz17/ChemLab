using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PindahSceneFade : MonoBehaviour
{
    [Header("Pengaturan Pindah Scene")]
    public string namaSceneTujuan = "Lab"; 
    
    [Header("Pengaturan Efek Fade")]
    public Image layarHitam; 
    public float kecepatanFade = 1.5f;

    void Start()
    {
        // Bikin layar memudar dari gelap ke terang pas scene baru mulai
        if (layarHitam != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Mengecek apakah yang nabrak tembok itu benar-benar Player
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeOutLaluPindah());
        }
    }

    IEnumerator FadeIn()
    {
        layarHitam.gameObject.SetActive(true);
        layarHitam.color = new Color(0, 0, 0, 1); // Hitam pekat
        
        while (layarHitam.color.a > 0)
        {
            float alphaBaru = layarHitam.color.a - (Time.deltaTime * kecepatanFade);
            layarHitam.color = new Color(0, 0, 0, alphaBaru);
            yield return null;
        }
        layarHitam.gameObject.SetActive(false); // Matiin layar hitam kalau pemandangan udah jelas
    }

    IEnumerator FadeOutLaluPindah()
    {
        layarHitam.gameObject.SetActive(true);
        layarHitam.color = new Color(0, 0, 0, 0); // Transparan
        
        while (layarHitam.color.a < 1)
        {
            float alphaBaru = layarHitam.color.a + (Time.deltaTime * kecepatanFade);
            layarHitam.color = new Color(0, 0, 0, alphaBaru);
            yield return null;
        }

        // Pindah scene setelah layar gelap total
        SceneManager.LoadScene(namaSceneTujuan);
    }
}