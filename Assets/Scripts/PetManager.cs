using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class PetManager : MonoBehaviour
{
    
    public static PetManager Instance { get; private set; }

    // Pet Statları
    [Header("Pet Stats")]
    [Range(0, 100)] public float health = 100f;
    [Range(0, 100)] public float happiness = 100f;
    [Range(0, 100)] public float hygiene = 100f;
    [Range(0, 100)] public float energy = 100f;
    [Header("Weight System")]
    [Range(0, 200)] public float weight = 50f;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public Vector2 moveRangeX = new Vector2(-5f, 5f); // X hareket sınırları
    public Vector2 moveRangeY = new Vector2(-3f, 3f); // Y hareket sınırları
    private Vector3 targetPosition;
    private bool isMoving = false;
    // Decay Hızları
    [Header("Decay Rates")]
    public float hungerDecayRate = 0.5f;
    public float happinessDecayRate = 0.3f;
    public float hygieneDecayRate = 0.2f;
    public float energyDecayRate = 0.4f;

    [Header("Sleep Settings")]
    public float sleepHappiness = 50f; 
    public float energyRecoveryRate = 15f;
    public bool isLightOff;
    public float warningInterval = 1f;       
    private Coroutine sleepWarningCoroutine;
    private bool isWarningActive = false;


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

    [Header("Animation Settings")]
    public Animator petAnimator;
    [Header("Fire Settings")]
    public GameObject[] firePrefabs; // Inspector'a alev prefab'larını sürükleyin
    public int maxFireCount = 5; // Maksimum alev sayısı
    public Vector2 xSpawnRange = new Vector2(-5f, 5f); // X spawn sınırları
    public Vector2 ySpawnRange = new Vector2(-3f, 3f); // Y spawn sınırları
    private List<GameObject> activeFires = new List<GameObject>();

    // Zaman ve Durumlar
    [Header("Game Timing")]
    public float dayDuration = 300f; // 5 DAKİKA = 24 SAAT
    public float initialHour = 9f; // Başlangıç saati (09:00)
    public float sleepTimeMultiplier = 3f; // Uykuda zaman hızı
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

        // Başlangıç saatini ayarla
        gameTime = (initialHour / 24f) * dayDuration;
    }

    void Start()
    {
        baseHygieneDecay = hygieneDecayRate; 
        skullSprite.SetActive(false); 
        StartCoroutine(PoopRoutine());
        StartCoroutine(RandomMovementRoutine());
        UpdateTimeDisplay();
    }

    void Update()
    {
        if (isGamePaused) return;
        if (petAnimator != null && !isWarningActive)
        {
            
            petAnimator.SetFloat("happiness", happiness);                  
          
            petAnimator.SetBool("awake", !isSleeping);
        }
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
        if (isSleeping || isGamePaused)
        {
            petAnimator.SetBool("isWalking", false);
            isMoving = false;
        }
        HandleSleepCycle();
        HandleSleepWarning();
        UpdateTimeDisplay();
    }

    #region Zaman Sistemleri
    void UpdateGameTime()
    {
        float timeMultiplier = isSleeping ? sleepTimeMultiplier : 1f;
        gameTime += Time.deltaTime * timeMultiplier;

        if (gameTime >= dayDuration) gameTime = 0;

        // Uyku kontrolü (22:00-06:00 arası veya enerji sıfır)
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

        if (energy <= 10 && !isSleeping)
        {
            StartSleeping();
        }
    }
    void HandleSleepCycle()
    {
        
        if (energy <= 10 && !isLightOff && !isSleeping)
        {
            happiness = Mathf.Clamp(happiness - (happinessDecayRate * 5 * Time.deltaTime), 0, 100);
        }

      
        if (isSleeping && energy >= 100f)
        {
            WakeUp();
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
    IEnumerator RandomMovementRoutine()
    {
        while (true)
        {
            if (!isSleeping && !isGamePaused && !isMoving)
            {
                // Rastgele hedef pozisyon belirle
                targetPosition = new Vector3(
                    Random.Range(moveRangeX.x, moveRangeX.y),
                    Random.Range(moveRangeY.x, moveRangeY.y),
                    aweboPetObject.transform.position.z
                );

                isMoving = true;
                petAnimator.SetBool("isWalking", true);

                // Hareketi gerçekleştir
                while (Vector3.Distance(aweboPetObject.transform.position, targetPosition) > 0.1f)
                {
                    if (isSleeping || isGamePaused) break; // Beklenmedik durumlarda durdur

                    aweboPetObject.transform.position = Vector3.MoveTowards(
                        aweboPetObject.transform.position,
                        targetPosition,
                        moveSpeed * Time.deltaTime
                    );
                    if (targetPosition.x > aweboPetObject.transform.position.x)
                    {
                        aweboPetObject.transform.localScale = new Vector3(1, 1, 1); // Sağa bak
                    }
                    else
                    {
                        aweboPetObject.transform.localScale = new Vector3(-1, 1, 1); // Sola bak
                    }
                    yield return null;
                }

                // Hareket tamamlandığında
                petAnimator.SetBool("isWalking", false);
                isMoving = false;

                // Rastgele bekleme süresi (2-5 saniye)
                yield return new WaitForSeconds(Random.Range(2f, 5f));
            }
            yield return null;
        }
    }
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
                Debug.Log("<color=red>Pet kaka yaptı!</color>");
            }
        }
    }
    void HandleSleepWarning()
    {
        bool needWarning = energy <= 10f && !isLightOff && !isSleeping;

        if (needWarning && sleepWarningCoroutine == null)
        {
            isWarningActive = true;
            sleepWarningCoroutine = StartCoroutine(SleepWarningBlink());
        }
        else if (!needWarning && sleepWarningCoroutine != null)
        {
            StopCoroutine(sleepWarningCoroutine);
            sleepWarningCoroutine = null;
            isWarningActive = false;
            // uyarı bitince kesin awake pozisyonuna dön
            petAnimator.SetBool("awake", true);
        }
    }

    IEnumerator SleepWarningBlink()
    {
        while (true)
        {
            // “uyku zamanı geldi” uyarısı: sleep animasyonuna gir
            petAnimator.SetBool("awake", false);
            yield return new WaitForSeconds(warningInterval);

            // sonra hemen awake’e çık
            petAnimator.SetBool("awake", true);
            yield return new WaitForSeconds(warningInterval);
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
        // Ceza = Aktif ve 5 saniyeyi geçmiş poop sayısı * penaltyPerPoop
        int penalizedPoops = activePoops.Count(poop => poop.penaltyApplied);
        hygieneDecayRate = baseHygieneDecay + (penalizedPoops * penaltyPerPoop);
    }


    IEnumerator SleepRoutine()
    {
        Debug.Log("<color=blue>Uyku modu aktif!</color>");
        while (isSleeping)
        {
            energy = Mathf.Clamp(energy + energyRecoveryRate * Time.deltaTime, 0, 100);
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
            float healthBeforeClamp = health; // Clamp öncesi değer

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

            // 1. Health ve Happiness'ı 0-100 arasında sınırla
            health = Mathf.Clamp(health, 0, 100);
            happiness = Mathf.Clamp(happiness, 0, 100);

            // 2. Health 100'ü geçerse kilo artışı (Clamp öncesi kontrol)
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
            Debug.Log("<color=magenta>Oyun oynandı!</color>");
        }
    }

    public void StartSleeping()
    {
        
        if (!isSleeping && isLightOff)
        {
            if (sleepWarningCoroutine != null)
            {
                StopCoroutine(sleepWarningCoroutine);
                sleepWarningCoroutine = null;
                isWarningActive = false;
                petAnimator.SetBool("awake", true);
            }

            isSleeping = true;
            StopAllCoroutines();        
            StartCoroutine(SleepRoutine());
        }
    }

    public void WakeUp()
    {
        isSleeping = false;
        StartCoroutine(PoopRoutine());
        Debug.Log("<color=yellow>Uyandırıldı!</color>");
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