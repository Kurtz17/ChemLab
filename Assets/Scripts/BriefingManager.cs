using UnityEngine;
using UnityEngine.UI;

public class BriefingManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image slideImage;       // Image yang menampilkan gambar per halaman
    public Button nextButton;
    public Button backButton;
    public Button selesaiButton;

    [Header("Player Movement System")]
    public GameObject locomotionSystem;

    [Header("Slide Gambar (isi sesuai urutan halaman)")]
    public Sprite[] slides;        // drag brief-0, brief-1, brief-2, dst ke sini

    private int currentIndex = 0;

    void Start()
    {
        // Kunci pergerakan pemain saat briefing
        if (locomotionSystem != null)
            locomotionSystem.SetActive(false);

        // Hubungkan tombol
        nextButton.onClick.AddListener(NextSlide);
        backButton.onClick.AddListener(BackSlide);
        selesaiButton.onClick.AddListener(FinishBriefing);

        // Tampilkan halaman pertama
        UpdateUI();
    }

    void NextSlide()
    {
        if (currentIndex < slides.Length - 1)
        {
            currentIndex++;
            UpdateUI();
        }
    }

    void BackSlide()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateUI();
        }
    }

    void FinishBriefing()
    {
        // Buka kunci pergerakan pemain
        if (locomotionSystem != null)
            locomotionSystem.SetActive(true);

        // Hilangkan canvas briefing
        transform.parent.gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        // Ganti gambar sesuai halaman
        if (slides.Length > 0 && slideImage != null)
            slideImage.sprite = slides[currentIndex];

        // Tampilkan/sembunyikan tombol sesuai posisi halaman
        backButton.gameObject.SetActive(currentIndex > 0);
        nextButton.gameObject.SetActive(currentIndex < slides.Length - 1);
        selesaiButton.gameObject.SetActive(currentIndex == slides.Length - 1);
    }
}