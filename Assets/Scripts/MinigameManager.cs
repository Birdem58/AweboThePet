using UnityEngine;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    // Singleton
    public static MinigameManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject minigameCanvas;
    public Text scoreText;
    public Text gameOverText;
    public Button retryButton;

    [Header("Game Objects")]
    public GameObject aweboPlayer;
    public Transform obstacleSpawnPoint;
    public GameObject[] obstaclePrefabs;

    [Header("Game Settings")]
    public float obstacleSpeed = 5f;
    public float spawnInterval = 2f;
    public float jumpForce = 8f;

    // Oyun durumu
    private int currentScore = 0;
    private bool isMinigameActive = false;
    private Rigidbody2D aweboRb;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        aweboRb = aweboPlayer.GetComponent<Rigidbody2D>();
        retryButton.onClick.AddListener(RestartMinigame);
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

    #region Minigame Setup
    public void InitializeMinigame()
    {
        minigameCanvas.SetActive(false);
        aweboPlayer.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
    }

    public void StartMinigame()
    {
        // Ana oyunu duraklat
        PetManager.Instance.isGamePaused = true;

        // Mini oyunu baþlat
        isMinigameActive = true;
        minigameCanvas.SetActive(true);
        aweboPlayer.SetActive(true);
        aweboPlayer.transform.position = new Vector2(-6f, -1.5f);

        currentScore = 0;
        scoreText.text = "Score: 0";

        CancelInvoke();
        InvokeRepeating("SpawnObstacle", 1f, spawnInterval);
    }
    #endregion

    #region Gameplay Mechanics
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
    #endregion

    #region Game State Management
    public void EndMinigame(bool success)
    {
        isMinigameActive = false;
        CancelInvoke();

        // UI feedback
        gameOverText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        gameOverText.text = success ? "Baþarýlý!" : "Kaybettin!";

        // Pet'e skoru ilet
        if (success) PetManager.Instance.OnMinigameEnd(currentScore);
    }

    public void RestartMinigame()
    {
        // Tüm engelleri temizle
        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            Destroy(obstacle);
        }

        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        StartMinigame();
    }

    public void ExitToMainGame()
    {
        // Ana oyuna dön
        PetManager.Instance.isGamePaused = false;
        InitializeMinigame();
    }
    #endregion

    #region Collision Handling
    // Bu fonksiyon engellerin trigger'larýndan çaðrýlacak
    public void OnAweboCollision()
    {
        EndMinigame(false);
    }
    #endregion
}