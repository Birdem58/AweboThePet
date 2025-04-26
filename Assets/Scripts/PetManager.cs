using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class PetManager : MonoBehaviour
{
    
    public static PetManager Instance { get; private set; }

    // Pet Statlarý
    [Header("Pet Stats")]
    [Range(0, 100)] public float health = 100f;
    [Range(0, 100)] public float happiness = 100f;
    [Range(0, 100)] public float hygiene = 100f;
    [Range(0, 100)] public float energy = 100f;
    [Header("Weight System")]
    [Range(0, 200)] public float weight = 50f;
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
    public GameObject[] shitPrefabs;
    public GameObject aweboPetObject; 
    public float poopSpawnRadius = 1f;
    public float penaltyPerPoop = 2f; 
    private HashSet<ShitController> activePoops = new HashSet<ShitController>();
        private float baseHygieneDecay;

    public GameObject skullSprite;

    [Header("Fire Settings")]
    public GameObject[] firePrefabs; // Inspector'a alev prefab'larýný sürükleyin
    public int maxFireCount = 5; // Maksimum alev sayýsý
    public Vector2 xSpawnRange = new Vector2(-5f, 5f); // X spawn sýnýrlarý
    public Vector2 ySpawnRange = new Vector2(-3f, 3f); // Y spawn sýnýrlarý
    private List<GameObject> activeFires = new List<GameObject>();

    // Zaman ve Durumlar
    [Header("Game Timing")]
    public float dayDuration = 300f; // 5 DAKÝKA = 24 SAAT
    public float initialHour = 9f; // Baþlangýç saati (09:00)
    public float sleepTimeMultiplier = 3f; // Uykuda zaman hýzý
    private float gameTime;
    public bool isSleeping = false;
    public bool isGamePaused = false;

    // UI
    [Header("UI Settings")]
    public TMP_Text timeText;
    public bool showDebugTime = true;

    // Referanslar
    [Header("References")]
    public MinigameManager minigameManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Baþlangýç saatini ayarla
        gameTime = (initialHour / 24f) * dayDuration;
    }

    void Start()
    {
        baseHygieneDecay = hygieneDecayRate; 
        skullSprite.SetActive(false); 
        StartCoroutine(PoopRoutine());
        UpdateTimeDisplay();
    }

    void Update()
    {
        if (isGamePaused) return;

        if (!isSleeping)
        {
            UpdateNeeds();
            UpdateGameTime();
            if (happiness <= 35f && activeFires.Count < maxFireCount && !isSleeping)
            {
                SpawnFire();
            }
            skullSprite.SetActive(hygiene <= 25f);
        }
        
        UpdateTimeDisplay();
    }

    #region Zaman Sistemleri
    void UpdateGameTime()
    {
        float timeMultiplier = isSleeping ? sleepTimeMultiplier : 1f;
        gameTime += Time.deltaTime * timeMultiplier;

        if (gameTime >= dayDuration) gameTime = 0;

        // Uyku kontrolü (22:00-06:00 arasý veya enerji sýfýr)
        float currentHour = (gameTime / dayDuration) * 24f;
        if ((currentHour >= 22f || currentHour <= 6f) && !isSleeping)
        {
            StartSleeping();
        }
    }

    void UpdateTimeDisplay()
    {
        float totalSeconds = (gameTime / dayDuration) * 86400f;
        System.DateTime time = System.DateTime.Today.AddSeconds(totalSeconds);
        string timeString = time.ToString("HH:mm");

        if (timeText != null) timeText.text = timeString;
        if (showDebugTime) Debug.Log("Oyun Saati: " + timeString);
    }

    public void SetGameTime(int hour, int minute)
    {
        float totalSeconds = (hour * 3600) + (minute * 60);
        gameTime = (totalSeconds / 86400f) * dayDuration;
    }
    #endregion

    #region Core Sistemler
    void UpdateNeeds()
    {
        health = Mathf.Clamp(health - hungerDecayRate * Time.deltaTime, 0, 100);
        happiness = Mathf.Clamp(happiness - happinessDecayRate * Time.deltaTime, 0, 100);
        hygiene = Mathf.Clamp(hygiene - hygieneDecayRate * Time.deltaTime, 0, 100);
        energy = Mathf.Clamp(energy - energyDecayRate * Time.deltaTime, 0, 100);

        if (energy <= 0 && !isSleeping)
        {
            StartSleeping();
        }
    }

    void SpawnFire()
    {
        Vector3 spawnPos = new Vector3(
            Random.Range(xSpawnRange.x, xSpawnRange.y),
            Random.Range(ySpawnRange.x, ySpawnRange.y),
            -2f // Sabit Z düzlemi
        );

        GameObject selectedFire = firePrefabs[Random.Range(0, firePrefabs.Length)];
        GameObject fire = Instantiate(selectedFire, spawnPos, Quaternion.identity);
        activeFires.Add(fire);
    }

    public void ExtinguishFire(GameObject fire, float happinessGain = 5f)
    {
        if (activeFires.Contains(fire))
        {
            happiness = Mathf.Clamp(happiness + happinessGain, 0, 100);
            activeFires.Remove(fire);
            Destroy(fire);
        }
    }
    #endregion

    #region Routinler
    IEnumerator PoopRoutine()
    {
        while (true)
        {
            float randomInterval = Random.Range(minPoopInterval, maxPoopInterval);
            yield return new WaitForSeconds(randomInterval);

            if (!isSleeping && !isGamePaused)
            {
                // Kaka spawnla ve durumu güncelle
                SpawnShit();
                hasPooped = true;
                Debug.Log("<color=red>Pet kaka yaptý!</color>");
            }
        }
    }

    void SpawnShit()
    {
        if (shitPrefabs.Length == 0 || aweboPetObject == null) return;

        Transform petTransform = aweboPetObject.transform;

      
        float randomRadius = Random.Range(1f, 3f);
        Vector2 randomCircle = Random.insideUnitCircle.normalized * randomRadius;


        float randomY = Random.Range(0f, 1.5f);
        Vector3 spawnPosition = new Vector3(
            petTransform.position.x + randomCircle.x,
            petTransform.position.y + randomY,
            -2f 
        );

        GameObject selectedShit = shitPrefabs[Random.Range(0, shitPrefabs.Length)];
        Instantiate(selectedShit, spawnPosition, Quaternion.identity);
    }

    public void RegisterPoop(ShitController poop)
    {
        activePoops.Add(poop);
        UpdateHygieneDecay();
    }

    public void RemovePoop(ShitController poop)
    {
        activePoops.Remove(poop);
        UpdateHygieneDecay();
    }

    public void AddHygienePenalty()
    {
        UpdateHygieneDecay();
    }

    void UpdateHygieneDecay()
    {
        // Ceza = Aktif ve 5 saniyeyi geçmiþ poop sayýsý * penaltyPerPoop
        int penalizedPoops = activePoops.Count(poop => poop.penaltyApplied);
        hygieneDecayRate = baseHygieneDecay + (penalizedPoops * penaltyPerPoop);
    }


    IEnumerator SleepRoutine()
    {
        Debug.Log("<color=blue>Uyku modu aktif!</color>");
        while (isSleeping)
        {
            energy = Mathf.Clamp(energy + 15f * Time.deltaTime, 0, 100);
            yield return null;
        }
        Debug.Log("<color=yellow>Uyku modu pasif!</color>");
    }
    #endregion

    #region Player Aksiyonlar
    public void Feed(FoodType foodType)
    {
        if (!isGamePaused && !isSleeping)
        {
            float healthBeforeClamp = health; // Clamp öncesi deðer

            switch (foodType)
            {
                case FoodType.Meal:
                    health += 30f;
                    break;

                case FoodType.Snack:
                    health += 20f;
                    happiness += 15f;
                    break;
            }

            // 1. Health ve Happiness'ý 0-100 arasýnda sýnýrla
            health = Mathf.Clamp(health, 0, 100);
            happiness = Mathf.Clamp(happiness, 0, 100);

            // 2. Health 100'ü geçerse kilo artýþý (Clamp öncesi kontrol)
            if (healthBeforeClamp > 100f)
            {
                float excess = healthBeforeClamp - 100f;
                weight = Mathf.Clamp(weight + (excess * 0.5f), 0, 200);
                Debug.Log($"<color=red>Fazla yemek! Kilo +{excess * 0.5f}</color>");
            }

            Debug.Log($"<color=green>{foodType} verildi!</color>");
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
            isGamePaused = true;
            minigameManager.StartMinigame();
            Debug.Log("<color=magenta>Oyun oynandý!</color>");
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
    public void OnMinigameEnd(int score)
    {
        isGamePaused = false;
        happiness = Mathf.Clamp(happiness + (score / 10f), 0, 100);
        energy = Mathf.Clamp(energy - 20f, 0, 100);
    }
    #endregion
    public enum FoodType { Meal, Snack }
}