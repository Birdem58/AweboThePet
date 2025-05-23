using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider == null) Debug.LogError("Slider componenti bulunamadı!");
    }

    void Update()
    {
        if (PetManager.Instance != null)
        {
            slider.value = PetManager.Instance.health; // Health = Hunger olarak varsayıldı
        }
    }
}