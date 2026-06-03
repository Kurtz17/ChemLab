using UnityEngine;

public class OutfitEquip : MonoBehaviour
{
    [Header("Referensi Audio")]
    public AudioClip suaraPakaiEquipment; 

    [Header("Volume Suara")]
    [Range(0f, 1f)]
    public float volumeSuara = 1f; 

    // Namanya diubah jadi lebih umum agar cocok untuk kacamata, masker, gloves, dll.
    public void MainkanSuaraDanGunakanAlat()
    {
        if (suaraPakaiEquipment != null)
        {
            Vector3 posisiTelingaPlayer = Camera.main != null ? Camera.main.transform.position : transform.position;
            AudioSource.PlayClipAtPoint(suaraPakaiEquipment, posisiTelingaPlayer, volumeSuara);
            Debug.Log($"👕 SFX {gameObject.name} berhasil dipicu.");
        }

        gameObject.SetActive(false);
    }
}