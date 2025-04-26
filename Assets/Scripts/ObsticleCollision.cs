using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MinigameManager.Instance.OnAweboCollision();
        }
    }
}