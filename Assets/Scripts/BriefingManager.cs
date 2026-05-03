using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BriefingManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI instructionText;
    public Button nextButton;
    public Button backButton;
    public Button selesaiButton;

    [Header("Player Movement System")]
    // Ini adalah objek Locomotion System yang mengatur bisa/tidaknya player bergerak
    public GameObject locomotionSystem; 

    [Header("Isi Modul Instruksi")]
    [TextArea(3, 5)]
    public string[] instructions;

    private int currentIndex = 0;

    void Start()
    {
        // 1. Kunci pergerakan pemain saat game dimulai
        if (locomotionSystem != null) 
        {
            locomotionSystem.SetActive(false);
        }

        // 2. Hubungkan tombol dengan fungsinya
        nextButton.onClick.AddListener(NextInstruction);
        backButton.onClick.AddListener(BackInstruction);
        selesaiButton.onClick.AddListener(FinishBriefing);

        // 3. Tampilkan halaman pertama
        UpdateUI();
    }

    void NextInstruction()
    {
        if (currentIndex < instructions.Length - 1)
        {
            currentIndex++;
            UpdateUI();
        }
    }

    void BackInstruction()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateUI();
        }
    }

    void FinishBriefing()
    {
        // Buka kembali kunci pergerakan pemain agar bisa jalan
        if (locomotionSystem != null) 
        {
            locomotionSystem.SetActive(true);
        }

        // Hilangkan canvas briefing ini dari layar
        transform.parent.gameObject.SetActive(false); 
    }

    void UpdateUI()
    {
        // Ganti teks sesuai halaman saat ini
        if (instructions.Length > 0)
        {
            instructionText.text = instructions[currentIndex];
        }

        // Sembunyikan/tampilkan tombol sesuai halaman
        backButton.gameObject.SetActive(currentIndex > 0);
        nextButton.gameObject.SetActive(currentIndex < instructions.Length - 1);
        selesaiButton.gameObject.SetActive(currentIndex == instructions.Length - 1);
    }
}