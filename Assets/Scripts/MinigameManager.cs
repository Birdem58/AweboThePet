using UnityEngine;
using TMPro;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject minigameCanvas;
    public TMP_Text scoreText;

    [Header("Game Objects")]
    public GameObject MinigameEnv;
    public Transform obstacleSpawnPoint;
    public GameObject[] obstaclePrefabs;

    [Header("Game Settings")]
    public float obstacleSpeed = 5f;
    public float spawnInterval = 2f;
    public float jumpForce = 8f;

    private int currentScore = 0;
    private bool isMinigameActive = false;
    private Rigidbody2D aweboRb;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahne geçiþlerinde koru
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        aweboRb = MinigameEnv.GetComponent<Rigidbody2D>();
        InitializeMinigame();
    }

    void Update()
    {
        if (isMinigameActive)
        {
            HandleInput();
            UpdateScore();
        }
    }

    public void InitializeMinigame()
    {
        minigameCanvas.SetActive(false);
        MinigameEnv.SetActive(false);
    }

    public void StartMinigame()
    {
        // Ana oyunu duraklat
        PetManager.Instance.isGamePaused = true;

        // Mini oyunu aktif et
        isMinigameActive = true;
        minigameCanvas.SetActive(true);
        MinigameEnv.SetActive(true);
        MinigameEnv.transform.position = new Vector2(-6f, -1.5f);

        // Skor ve engelleri sýfýrla
        currentScore = 0;
        scoreText.text = "Score: 0";
        ClearObstacles();

        CancelInvoke();
        InvokeRepeating(nameof(SpawnObstacle), 1f, spawnInterval);

        Debug.Log("Mini oyun baþladý!"); // Log kontrolü
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Mathf.Abs(aweboRb.velocity.y) < 0.01f)
            {
                aweboRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    void SpawnObstacle()
    {
        GameObject obstacle = Instantiate(
            obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)],
            obstacleSpawnPoint.position,
            Quaternion.identity
        );
        obstacle.GetComponent<Rigidbody2D>().velocity = Vector2.left * obstacleSpeed;
    }

    void UpdateScore()
    {
        currentScore += Mathf.RoundToInt(Time.unscaledDeltaTime * 10f);
        scoreText.text = $"Score: {currentScore}";
    }

    public void EndMinigame(bool success)
    {
        isMinigameActive = false;
        CancelInvoke();
        ClearObstacles();

        if (success) PetManager.Instance.OnMinigameEnd(currentScore);
        ExitToMainGame();
    }

    void ExitToMainGame()
    {
        PetManager.Instance.isGamePaused = false;
        InitializeMinigame();
        Debug.Log("Ana oyuna dönüldü!");
    }

    void ClearObstacles()
    {
        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            Destroy(obstacle);
        }
    }

    public void OnAweboCollision()
    {
        EndMinigame(false);
    }
}