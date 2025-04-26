using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameController : MonoBehaviour
{
    public GameObject[] mapPrefabs; // 0: Map1, 1: Map2
    private GameObject currentMap;

    public GameObject player;
    private int collectibleCount;
    private bool minigameRunning = false;
    public bool minigameSuccess = false;

    void Start()
    {
        StartMinigame();
    }

    void Update()
    {
        if (!minigameRunning) return;

        if (Input.GetMouseButtonDown(0)) // Ekrana t�klan�nca
        {
            EndMinigame(false);
        }

        if (collectibleCount <= 0) // T�m collectiblelar toplanm��sa
        {
            EndMinigame(true);
        }
    }

    public void StartMinigame()
    {
        int randomIndex = Random.Range(0, mapPrefabs.Length);
        currentMap = Instantiate(mapPrefabs[randomIndex], Vector3.zero, Quaternion.identity);

        collectibleCount = currentMap.GetComponentsInChildren<Collecible>().Length;
        player.SetActive(true); // Karakteri aktif et
        minigameRunning = true;

        // Skoru s�f�rlamak istersen buraya ekleyebilirsin
        // ScoreManager.instance.ResetScore(); gibi bir �ey
    }

    public void EndMinigame(bool success)
    {
        minigameSuccess = success;
        minigameRunning = false;

        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        player.SetActive(false);

        if (success)
        {
            Debug.Log("Minigame Ba�ar�yla Tamamland�!");
            // Burada oyununa ba�ar� puan� veya �d�l verebilirsin
        }
        else
        {
            Debug.Log("Minigame ba�ar�s�z!");
        }
    }

    public void CollectibleCollected()
    {
        collectibleCount--;
    }
}
