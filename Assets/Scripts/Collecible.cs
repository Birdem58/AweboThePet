using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Collecible : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Burada skor art�rma i�lemi yap�labilir
            Debug.Log("Collected!");

            // Collectible'� yok et
            Destroy(gameObject);
        }
    }
}
