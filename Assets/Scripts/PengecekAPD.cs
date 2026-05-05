using UnityEngine;

public class PengecekAPD : MonoBehaviour
{
    [Header("Masukkan Object APD dari Loker")]
    public GameObject jasLab;
    public GameObject sarungTangan;
    public GameObject kacamata;

    [Header("Masukkan Tembok Pemblokir Pintu")]
    public GameObject tembokPenahan;

    void Update()
    {
        // Mengecek apakah ketiga barang sudah hilang/mati (artinya sudah diklik/dipakai)
        if (jasLab.activeSelf == false && sarungTangan.activeSelf == false && kacamata.activeSelf == false)
        {
            // Jika ketiga APD sudah dipakai, hilangkan tembok penahan agar player bisa lewat
            if (tembokPenahan != null)
            {
                tembokPenahan.SetActive(false);
            }
        }
    }
}