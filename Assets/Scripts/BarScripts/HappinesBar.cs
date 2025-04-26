using UnityEngine;
using UnityEngine.UI;

public class HappinessBar : MonoBehaviour
{
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider == null) Debug.LogError("Slider componenti bulunamad�!");
    }

    void Update()
    {
        if (PetManager.Instance != null)
        {
            slider.value = PetManager.Instance.happiness;
        }
    }
}