using UnityEngine;
using System.Collections;

public class PetManager : MonoBehaviour
{
    // Singleton
    public static PetManager Instance { get; private set; }

    // Pet Statlarý
    [Header("Pet Stats")]
    [Range(0, 100)] public float hunger = 100f;
    [Range(0, 100)] public float happiness = 100f;
    [Range(0, 100)] public float hygiene = 100f;
    [Range(0, 100)] public float energy = 100f;

    // Decay Hýzlarý
    [Header("Decay Rates")]
    public float hungerDecayRate = 0.5f;
    public float happinessDecayRate = 0.3f;
    public float hygieneDecayRate = 0.2f;
    public float energyDecayRate = 0.4f;

    // Kaka Sistemi
    [Header("Poop Settings")]
    public float minPoopInterval = 4f;
    public float maxPoopInterval = 10f;
    private bool hasPooped = false;

    // Zaman ve Durumlar
    [Header("Game States")]
    public float dayDuration = 900f; // 15 dakika = 24 saat
    private float gameTime = 0f;
    public bool isSleeping = false;
    public bool isGamePaused = false; // Mini oyun veya diðer durumlar için pause

    // Referanslar
    [Header("References")]
    public MinigameManager minigameManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(PoopRoutine());
    }

    void Update()
    {
        if (isGamePaused) return; // Oyun duraklatýldýysa hiçbir þey yapma

        if (!isSleeping)
        {
            UpdateNeeds();
            UpdateGameTime();
        }
    }

    #region Core Systems
    void UpdateNeeds()
    {
        // Ýhtiyaçlar sadece oyun duraklatýlmadýðýnda ve uyumadýðýnda azalýr
        hunger = Mathf.Clamp(hunger - hungerDecayRate * Time.deltaTime, 0, 100);
        happiness = Mathf.Clamp(happiness - happinessDecayRate * Time.deltaTime, 0, 100);
        hygiene = Mathf.Clamp(hygiene - (hasPooped ? 2f : hygieneDecayRate) * Time.deltaTime, 0, 100);
        energy = Mathf.Clamp(energy - energyDecayRate * Time.deltaTime, 0, 100);

        if (energy <= 0) StartSleeping();
    }

    void UpdateGameTime()
    {
        gameTime += Time.deltaTime;
        if (gameTime >= dayDuration) gameTime = 0;

        // Gece kontrolü (22:00-06:00 arasý uyku)
        float currentHour = (gameTime / dayDuration) * 24f;
        if (currentHour >= 22f || currentHour <= 6f) StartSleeping();
    }
    #endregion

    #region Routines
    IEnumerator PoopRoutine()
    {
        while (true)
        {
            float randomInterval = Random.Range(minPoopInterval, maxPoopInterval);
            yield return new WaitForSeconds(randomInterval);

            if (!isSleeping && !isGamePaused) // Duraklatýlmamýþsa kaka yap
            {
                hasPooped = true;
                Debug.Log("<color=red>Pet kaka yaptý!</color>");
            }
        }
    }

    IEnumerator SleepRoutine()
    {
        Debug.Log("<color=blue>Uyku modu aktif!</color>");
        while (isSleeping)
        {
            energy = Mathf.Clamp(energy + 15f * Time.deltaTime, 0, 100);
            yield return null;
        }
    }
    #endregion

    #region Player Actions
    public void Feed()
    {
        if (!isGamePaused && !isSleeping)
        {
            hunger = Mathf.Clamp(hunger + 30f, 0, 100);
            Debug.Log("<color=green>Yemek verildi!</color>");
        }
    }

    public void Clean()
    {
        if (!isGamePaused && hasPooped)
        {
            hygiene = 100f;
            hasPooped = false;
            Debug.Log("<color=cyan>Temizlendi!</color>");
        }
    }

    public void Play()
    {
        if (!isGamePaused && !isSleeping)
        {
            // Mini oyunu baþlat ve oyunu duraklat
            isGamePaused = true;
            minigameManager.StartMinigame();
        }
    }

    public void StartSleeping()
    {
        if (!isSleeping)
        {
            isSleeping = true;
            StopAllCoroutines();
            StartCoroutine(SleepRoutine());
        }
    }

    public void WakeUp()
    {
        isSleeping = false;
        StartCoroutine(PoopRoutine());
        Debug.Log("<color=yellow>Uyandýrýldý!</color>");
    }
    #endregion

    #region Minigame Callbacks
    // Mini oyun bittiðinde çaðrýlýr (MinigameManager'dan tetiklenir)
    public void OnMinigameEnd(int score)
    {
        isGamePaused = false; // Oyunu devam ettir
        happiness = Mathf.Clamp(happiness + (score / 10f), 0, 100);
        energy = Mathf.Clamp(energy - 20f, 0, 100);
    }
    #endregion
}