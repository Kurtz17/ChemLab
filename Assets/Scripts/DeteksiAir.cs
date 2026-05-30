using UnityEngine;

public class DeteksiAir : MonoBehaviour
{
    [Tooltip("Tarik object Gelas Ukur yang punya script CustomPouring ke sini")]
    public CustomPouring scriptGelasUkur;

    // Fungsi ini akan otomatis dipanggil oleh Unity jika Send Collision Messages dicentang
    void OnParticleCollision(GameObject hitObject)
    {
        if (scriptGelasUkur != null)
        {
            // Lapor ke script induk bahwa ada air yang menabrak sesuatu
            scriptGelasUkur.ProsesTumpahan(hitObject);
        }
    }
}