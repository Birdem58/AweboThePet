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
            // Burada skor artýrma iþlemi yapýlabilir
            Debug.Log("Collected!");

            // Collectible'ý yok et
            Destroy(gameObject);
        }
    }
}
