using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameController : MonoBehaviour
{
    public GameObject[] mapPrefabs; // 0: Map1, 1: Map2
    private GameObject currentMap;

    public AudioSource musicSource;
    public AudioClip map1Music;
    public AudioClip map2Music;
    public AudioClip collectSound;
    public GameObject scoreManager;

    public GameObject miniGameUI;
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
        currentMap = Instantiate(mapPrefabs[randomIndex], new Vector3(0,-77,0), Quaternion.identity);

        // M�zik se�imi
        if (randomIndex == 0)
        {
            musicSource.clip = map1Music;
        }
        else if (randomIndex == 1)
        {
            musicSource.clip = map2Music;
        }

        musicSource.loop = true;
        musicSource.Play();
        miniGameUI.SetActive(true);
        scoreManager.SetActive(true);
        collectibleCount = currentMap.GetComponentsInChildren<Collecible>().Length;
        player.SetActive(true); // Karakteri aktif et
        minigameRunning = true;
        PetManager.Instance.ToggleCameraAndUi(false);

        // Skoru s�f�rlamak istersen buraya ekleyebilirsin
        // ScoreManager.instance.ResetScore(); gibi bir �ey
    }

    public void EndMinigame(bool success)
    {
        minigameSuccess = success;
        minigameRunning = false;


        StartCoroutine(EndMiniGameRoutine());

        

        if (success)
        {
            Debug.Log("Minigame Ba�ar�yla Tamamland�!");
            PetManager.Instance.happiness += 10f; 
            // Burada oyununa ba�ar� puan� veya �d�l verebilirsin
        }
        else
        {
            Debug.Log("Minigame ba�ar�s�z!");
        }

        
    }

    private IEnumerator EndMiniGameRoutine()
    {
        yield return new WaitForSeconds(1f);
        // M�zi�i durdur
        musicSource.Stop();
        SceneManager.UnloadSceneAsync("minigame");
        PetManager.Instance.ToggleCameraAndUi(true);
        PetManager.Instance.isGamePaused = false;
        player.SetActive(false);

        // Haritay� yok et
        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        
    }

    public void CollectibleCollected()
    {
        collectibleCount--;
        musicSource.PlayOneShot(collectSound);
    }
}
