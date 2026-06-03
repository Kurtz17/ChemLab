using UnityEngine;
public class BGMController : MonoBehaviour
{
    private static BGMController instance;

    void Awake()
    {
        // Mengecek apakah sudah ada BGM yang menyala dari scene sebelumnya
        if (instance == null)
        {
            // Kalau belum ada, jadikan objek ini sebagai BGM utama
            instance = this;
            
            // Tiket VIP: Jangan hancurkan objek ini saat pindah scene!
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // Kalau ternyata sudah ada BGM yang terbawa dari scene sebelumnya,
            // Hancurkan BGM yang baru ini agar suaranya tidak double/balapan!
            Destroy(gameObject);
        }
    }
}