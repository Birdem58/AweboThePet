using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Collecible : MonoBehaviour
{
    private MinigameController minigameController;

    private void Start()
    {
        minigameController = FindObjectOfType<MinigameController>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager.instance.AddScore(10);
            minigameController.CollectibleCollected();

            Debug.Log("Collected!");

            // Collectible'ý yok et
            Destroy(gameObject);
        }
    }
}
